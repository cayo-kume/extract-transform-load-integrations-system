CREATE OR ALTER PROCEDURE [auditing].[sp_Log_Start] (
    @jobTitle VARCHAR(80),
    @parentExecutionGUID UNIQUEIDENTIFIER,
    @idJob INT OUTPUT,
    @execution UNIQUEIDENTIFIER OUTPUT,
    @idType INT = 1 -- auditing.Types: 1 = Procedure
) AS
BEGIN
    SET NOCOUNT ON;
    SET ANSI_WARNINGS OFF
    SET ARITHABORT OFF;

    SET @idJob = NULL;

	SELECT
        @idJob = IdJob
	FROM
        [auditing].[Jobs]
	WHERE
        Title = @jobTitle
        AND IdType = @idType;

    IF @idJob IS NULL
    BEGIN
        DECLARE @msg NVARCHAR(400) = CONCAT('Job ', @jobTitle, ' (IdType ', @idType, ') não cadastrado em auditing.Jobs.');
        THROW 50001, @msg, 1;
    END

    SET @execution = NEWID();

	INSERT INTO [auditing].[Logs] (IdJob, IdType, StartDate, Execution, ExecutionParent) VALUES
    (@idJob, @idType, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time', @execution, @parentExecutionGUID);
END
GO

CREATE OR ALTER PROCEDURE [auditing].[sp_Log_Message] (
    @execution UNIQUEIDENTIFIER,
    @message VARCHAR(MAX),
    @logLevel INT
) AS
BEGIN
	SET XACT_ABORT OFF
	SET @message = LEFT(@message,800)

    INSERT INTO [auditing].[Messages] (IdJob, IdType, Execution, [Message], IsError)
    SELECT
        IdJob,
        IdType,
        Execution,
        @message,
        CASE WHEN @logLevel = 5 THEN 1 ELSE 0 END
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
	    INSERT INTO [auditing].[Messages] (IdJob, IdType, Execution, [Message], IsError)
	        SELECT
                IdJob,
                IdType,
                Execution,
		        LEFT(CONCAT('Concluída com Erro. ' , @err_msg), 800),
		        1
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
            INSERT INTO [auditing].[Messages] (IdJob, IdType, Execution, [Message])
            SELECT
                IdJob,
                IdType,
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