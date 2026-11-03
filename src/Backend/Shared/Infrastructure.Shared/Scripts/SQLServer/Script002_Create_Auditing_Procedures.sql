CREATE OR ALTER PROCEDURE [auditing].[sp_Log_Start] (
    @jobTitle VARCHAR(80),
    @parentExecutionGUID UNIQUEIDENTIFIER,
    @idJob INT OUTPUT,
    @execution UNIQUEIDENTIFIER OUTPUT,
    @idType INT = 1, -- auditing.Types: 1 = Procedure
    -- empresa do painel (holding) da execução: o job tem uma linha por empresa em auditing.Jobs
    -- (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.3). Sem padrão útil: nula, a execução é recusada (50005)
    @company VARCHAR(20) = NULL
) AS
BEGIN
    SET NOCOUNT ON;
    SET ANSI_WARNINGS OFF
    SET ARITHABORT OFF;

    DECLARE @msg NVARCHAR(400);

    IF NULLIF(@company, '') IS NULL
    BEGIN
        SET @msg = CONCAT('Job ', @jobTitle, ' (IdType ', @idType, ') sem empresa: informe @company.');
        THROW 50005, @msg, 1;
    END

    SET @idJob = NULL;

	SELECT
        @idJob = IdJob
	FROM
        [auditing].[Jobs]
	WHERE
        Title = @jobTitle
        AND IdType = @idType
        AND Company = @company;

    IF @idJob IS NULL
    BEGIN
        SET @msg = CONCAT('Job ', @jobTitle, ' (IdType ', @idType, ') não cadastrado em auditing.Jobs para a empresa ', @company, '.');
        THROW 50001, @msg, 1;
    END

    SET @execution = NEWID();

	INSERT INTO [auditing].[Logs] (IdJob, IdType, Company, StartDate, Execution, ExecutionParent) VALUES
    (@idJob, @idType, @company, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time', @execution, @parentExecutionGUID);
END
GO

CREATE OR ALTER PROCEDURE [auditing].[sp_Log_Message] (
    @execution UNIQUEIDENTIFIER,
    @message VARCHAR(MAX),
    -- 1 informação, 2 aviso, 3 erro (auditing.Messages.Level)
    @logLevel INT
) AS
BEGIN
	SET XACT_ABORT OFF
	SET @message = LEFT(@message,800)

    INSERT INTO [auditing].[Messages] (IdJob, IdType, Company, Execution, [Message], [Level])
    SELECT
        IdJob,
        IdType,
        Company,
        Execution,
        @message,
        @logLevel
    FROM
        [auditing].[Logs]
    WHERE
        Execution = @execution;
END
GO

CREATE OR ALTER PROCEDURE [auditing].[sp_Log_Error] (
    @Execution uniqueidentifier,
    @err_msg varchar(max)
) AS
BEGIN
    BEGIN TRY
	    INSERT INTO [auditing].[Messages] (IdJob, IdType, Company, Execution, [Message], [Level])
	        SELECT
                IdJob,
                IdType,
                Company,
                Execution,
		        LEFT(CONCAT('Concluída com Erro. ' , @err_msg), 800),
		        3
            FROM
                [auditing].[Logs]
            WHERE
                Execution = @Execution;

	    UPDATE [auditing].[Logs] SET
            EndDate = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'
	    WHERE
            Execution = @Execution
    END TRY

    BEGIN CATCH
	   THROW;
    END CATCH
END

GO

CREATE OR ALTER PROCEDURE [auditing].[sp_Log_End] (
    @Execution UNIQUEIDENTIFIER,
    @MergeOutputSTR NVARCHAR(MAX)
) AS
BEGIN
    SET NOCOUNT ON;
    SET CONCAT_NULL_YIELDS_NULL ON;

    DECLARE @resultado VARCHAR(300) = '';
    DECLARE @MergeOutputXML XML;

    BEGIN TRY
        SET @MergeOutputXML = CAST(@MergeOutputSTR AS XML);

        DECLARE @MergeOutput TABLE (operacao VARCHAR(50), chave VARCHAR(MAX));

        INSERT INTO @MergeOutput (operacao, chave)
            SELECT
                T.c.value('(operacao)[1]', 'VARCHAR(50)'),
                T.c.value('(chave)[1]', 'VARCHAR(MAX)')
            FROM
                @MergeOutputXML.nodes('/root/item') AS T(c);

        IF (SELECT COUNT(1) FROM @MergeOutput) = 0
            SET @resultado = 'Sucesso';
        ELSE
            SELECT
                @resultado = CONCAT(@resultado, ' ', operacao, ':', COUNT(*))
            FROM
                @MergeOutput
            GROUP BY
                operacao;

        IF (@resultado <> 'Sucesso')
            INSERT INTO [auditing].[Messages] (IdJob, IdType, Company, Execution, [Message])
            SELECT
                IdJob,
                IdType,
                Company,
                Execution,
                'Concluída com sucesso. ' + @resultado
            FROM
                [auditing].[Logs]
            WHERE
                Execution = @Execution;

        UPDATE [auditing].[Logs] SET
            EndDate = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'
        WHERE
            Execution = @Execution;
    END TRY

    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- jobs de um módulo no catálogo: os de auditing.JobCatalog com IdParent = módulo (auditing.Apps) e, abaixo deles, as
-- procedures (IdType = 1), cujo IdParent é o IdJob do job que as chama (ex.: 3500 sp_MovideskTickets_Sync, IdParent 3000)
CREATE OR ALTER FUNCTION [auditing].[fn_JobCatalogTree] (
    @idParent INT
) RETURNS TABLE AS
RETURN
(
    WITH tree AS (
        SELECT c.IdJob, c.IdType
        FROM [auditing].[JobCatalog] c
        WHERE c.IdParent = @idParent

        UNION ALL

        SELECT c.IdJob, c.IdType
        FROM [auditing].[JobCatalog] c
        JOIN tree t ON c.IdParent = t.IdJob
        WHERE c.IdType = 1
    )
    SELECT DISTINCT IdJob, IdType FROM tree
);
GO

-- contrata um módulo para a empresa: copia os jobs do módulo (auditing.fn_JobCatalogTree, com as procedures) de
-- auditing.JobCatalog para auditing.Jobs, inativos e sem duplicar (rodar de novo não muda nada). Ativar é UPDATE em
-- auditing.Jobs.IsActive, pela tela. (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.1)
CREATE OR ALTER PROCEDURE [auditing].[sp_Add_Company_App] (
    @company VARCHAR(20),
    @idParent INT
) AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [monitor].[Company] WHERE Company = @company)
    BEGIN
        DECLARE @msg NVARCHAR(400) = CONCAT('Empresa ', @company, ' não cadastrada em monitor.Company.');
        THROW 50002, @msg, 1;
    END

    INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IsActive, IdType, Company)
    SELECT
        c.IdParent,
        c.IdJob,
        c.Title,
        c.[Description],
        c.IntervalTimeInMinutes,
        c.RunningDailyAt,
        c.Queue,
        0,
        c.IdType,
        @company
    FROM
        [auditing].[fn_JobCatalogTree](@idParent) t
        JOIN [auditing].[JobCatalog] c ON c.IdJob = t.IdJob AND c.IdType = t.IdType
    WHERE
        NOT EXISTS (
            SELECT 1
            FROM [auditing].[Jobs] j
            WHERE j.IdJob = c.IdJob AND j.IdType = c.IdType AND j.Company = @company
        );

    SELECT @@ROWCOUNT AS JobsCreated;
END
GO

-- remove o módulo da empresa: desativa os jobs dela, com as procedures. Não apaga, porque auditing.Logs referencia
-- auditing.Jobs.
CREATE OR ALTER PROCEDURE [auditing].[sp_Remove_Company_App] (
    @company VARCHAR(20),
    @idParent INT
) AS
BEGIN
    SET NOCOUNT ON;

    UPDATE j SET
        IsActive = 0
    FROM
        [auditing].[Jobs] j
        JOIN [auditing].[fn_JobCatalogTree](@idParent) t ON t.IdJob = j.IdJob AND t.IdType = j.IdType
    WHERE
        j.Company = @company;
END
GO
