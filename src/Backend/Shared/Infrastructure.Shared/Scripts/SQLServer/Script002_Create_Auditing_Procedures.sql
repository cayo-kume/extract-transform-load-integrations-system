CREATE PROCEDURE [auditing].[sp_Log_Start] (
    @jobTitle VARCHAR(80),
    @parentExecutionGUID UNIQUEIDENTIFIER, 
    @idJob INT OUTPUT, 
    @execution UNIQUEIDENTIFIER OUTPUT
) AS  
BEGIN
    SET NOCOUNT ON;
    SET ANSI_WARNINGS OFF
    SET ARITHABORT OFF;

	SELECT 
        @idjob = idjob, 
        @execution = newid()
	FROM 
        [auditing].[Jobs]
	WHERE 
        title = @jobTitle

	INSERT INTO [auditing].[Logs] (IdJob, StartDate, Execution, ExecutionParent) VALUES
    (@idjob, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time', @execution, @parentExecutionGUID)  

    SELECT 
        CAST(@idJob AS INT) AS idJob, 
        CAST(@execution AS UNIQUEIDENTIFIER) AS Execution;
END
GO

CREATE PROCEDURE [auditing].[sp_Log_Message] (
    @execution UNIQUEIDENTIFIER, 
    @message VARCHAR(MAX),
    @logLevel INT
) AS
BEGIN
	SET XACT_ABORT OFF
	SET @message = LEFT(@message,800)
	
    INSERT INTO [auditing].[Messages] (Execution, [Message], IsError) VALUES 
    (@execution, @message, CASE WHEN @logLevel = 5 THEN 1 ELSE 0 END)
END 
GO

CREATE PROCEDURE [auditing].[sp_Log_Error] (
    @Execution uniqueidentifier, 
    @err_msg varchar(max)
) AS
BEGIN
    BEGIN TRY
	    INSERT INTO [auditing].[Messages] (Execution, [Message], IsError) 
	        SELECT 
                @Execution, 
		        CONCAT('Concluída com Erro. ' , @err_msg), 
		        1

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

CREATE PROCEDURE [auditing].[sp_Log_End] (
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
            INSERT INTO [auditing].[Messages] (execution, [message]) 
            VALUES (@Execution, 'Concluída com sucesso. ' + @resultado);

        UPDATE [auditing].[Logs] SET 
            EndDate = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'
        WHERE 
            Execution = @Execution;
    END TRY

    BEGIN CATCH	
        THROW;
    END CATCH
END