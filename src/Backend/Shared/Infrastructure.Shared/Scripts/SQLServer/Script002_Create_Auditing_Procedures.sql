CREATE OR ALTER PROCEDURE [auditing].[sp_Log_Start] (
    @jobTitle VARCHAR(80),
    @parentExecutionGUID UNIQUEIDENTIFIER,
    @idJob INT OUTPUT,
    @execution UNIQUEIDENTIFIER OUTPUT,
    @idType INT = 1, -- auditing.Types: 1 = Procedure
    -- empresa da execução (auditing.Companies): o job tem uma linha por empresa em auditing.Jobs, e a holding sai da
    -- empresa (docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md). Sem padrão útil: nula, a execução é recusada (50005)
    @company VARCHAR(20) = NULL
) AS
BEGIN
    SET NOCOUNT ON;
    SET ANSI_WARNINGS OFF
    SET ARITHABORT OFF;

    DECLARE @msg NVARCHAR(400);
    DECLARE @holder VARCHAR(20);

    IF NULLIF(@company, '') IS NULL
    BEGIN
        SET @msg = CONCAT('Job ', @jobTitle, ' (IdType ', @idType, ') sem empresa: informe @company.');
        THROW 50005, @msg, 1;
    END

    SELECT @holder = Holder FROM [auditing].[Companies] WHERE Company = @company;

    IF @holder IS NULL
    BEGIN
        SET @msg = CONCAT('Empresa ', @company, ' não cadastrada em auditing.Companies.');
        THROW 50002, @msg, 1;
    END

    SET @idJob = NULL;

	SELECT
        @idJob = j.IdJob
	FROM
        [auditing].[Jobs] j
        JOIN [auditing].[Catalog] c ON c.IdJob = j.IdJob AND c.IdType = j.IdType
	WHERE
        c.Title = @jobTitle
        AND j.IdType = @idType
        AND j.Holder = @holder
        AND j.Company = @company;

    IF @idJob IS NULL
    BEGIN
        SET @msg = CONCAT('Job ', @jobTitle, ' (IdType ', @idType, ') não cadastrado em auditing.Jobs para a empresa ', @company, '.');
        THROW 50001, @msg, 1;
    END

    SET @execution = NEWID();

	INSERT INTO [auditing].[Logs] (IdJob, IdType, Holder, Company, StartDate, Execution, ExecutionParent) VALUES
    (@idJob, @idType, @holder, @company, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time', @execution, @parentExecutionGUID);
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

    INSERT INTO [auditing].[Messages] (IdJob, IdType, Holder, Company, Execution, [Message], [Level])
    SELECT
        IdJob,
        IdType,
        Holder,
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
	    INSERT INTO [auditing].[Messages] (IdJob, IdType, Holder, Company, Execution, [Message], [Level])
	        SELECT
                IdJob,
                IdType,
                Holder,
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
            INSERT INTO [auditing].[Messages] (IdJob, IdType, Holder, Company, Execution, [Message])
            SELECT
                IdJob,
                IdType,
                Holder,
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

-- jobs de um módulo no catálogo: os de auditing.Catalog com IdParent = módulo (auditing.Apps) e, abaixo deles, as
-- procedures (IdType = 1), cujo IdParent é o IdJob do job que as chama (ex.: 3500 sp_MovideskTickets_Sync, IdParent 3000).
-- A tela do WebApp contrata um módulo para a empresa com INSERT direto em auditing.Jobs a partir desta função, com a
-- agenda sugerida (docs/AGENDA-SUGERIDA-JOBS.csv), e remove com UPDATE de IsActive
-- (docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md)
CREATE OR ALTER FUNCTION [auditing].[fn_CatalogTree] (
    @idParent INT
) RETURNS TABLE AS
RETURN
(
    WITH tree AS (
        SELECT c.IdJob, c.IdType
        FROM [auditing].[Catalog] c
        WHERE c.IdParent = @idParent

        UNION ALL

        SELECT c.IdJob, c.IdType
        FROM [auditing].[Catalog] c
        JOIN tree t ON c.IdParent = t.IdJob
        WHERE c.IdType = 1
    )
    SELECT DISTINCT IdJob, IdType FROM tree
);
GO
