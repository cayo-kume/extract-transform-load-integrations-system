/****** Objeto:  StoredProcedure [dbo].[sp_TryApplock]    Data do Script: 26/07/2026 07:36:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE  PROCEDURE [dbo].[sp_TryApplock]
    @Resource		 NVARCHAR(255),
    @LockTimeoutBase INT = 5000, --padrão 5 segundos
    @MaxTries		 INT = 5,
    @LockAcquired	 BIT OUTPUT
AS
BEGIN
    DECLARE  @res	INT
			,@tries INT = 0
			,@LockTimeout int

	SET @LockAcquired = 0

    WHILE @tries < @MaxTries AND @LockAcquired = 0
    BEGIN
		set @LockTimeout = @LockTimeoutBase * (@tries + 1)
        EXEC @res = sp_getapplock
				 @Resource	  = @Resource,
				 @LockMode	  = 'Exclusive',
				 @LockOwner	  = 'Transaction',
				 @LockTimeout = @LockTimeout

        IF @res >= 0
            SET @LockAcquired = 1
        ELSE
            SET @tries += 1
    END
END
GO

/****** Objeto:  StoredProcedure [dbo].[p_etl_table_maintenance]    Data do Script: 26/07/2026 07:39:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_Etl_Tables_Maintenance]
AS
BEGIN
    --LPereira - Trunca todas as tabelas zeradas no schema Untreted.
    -- Como estas tabelas são "Heap", esta é toda a manutenção necessária.
    SET NOCOUNT ON;

    DECLARE @sql NVARCHAR(MAX) = N'';

    SELECT @sql += 'TRUNCATE TABLE [' + s.name + '].[' + t.name + '];' + CHAR(13)
    FROM sys.tables t
    JOIN sys.schemas s ON t.schema_id = s.schema_id
    LEFT JOIN sys.dm_db_partition_stats ps ON t.object_id = ps.object_id AND ps.index_id IN (0,1)
    WHERE s.name = 'untreated'
    GROUP BY s.name, t.name
    HAVING SUM(ISNULL(ps.row_count, 0)) = 0;

    IF @sql <> ''
        EXEC sp_executesql @sql;
END
GO

/****** Objeto:  StoredProcedure [dbo].[p_index_maintenance]    Data do Script: 26/07/2026 07:39:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [dbo].[sp_Index_Maintenance] (@ParentExecutionGUID uniqueidentifier = null) as
begin
/*
Autor:   LPereira
Data:    24/06/2025
Obs:     
*/
    declare  @execution	UniqueIdentifier
            ,@output	dbo.MergeOutput
            ,@idjob		smallint
            ,@res       int
            ,@msg       varchar(max)

    --Início
    exec auditing.sp_Log_Start 'sp_index_maintenance', @ParentExecutionGUID, @idjob, @execution output

    --ÍNDICES
    DECLARE @sql     NVARCHAR(MAX)
          , @type    NVARCHAR(10)
          , @ix_name NVARCHAR(max)
    --Caso o índice tenha um percentual de desfragmentação maior que 30, reconstró o índice
    DECLARE c1 CURSOR LOCAL FAST_FORWARD FOR
    SELECT distinct top 10 
            i.name,
           CASE WHEN avg_fragmentation_in_percent >= 30 
                 THEN 'REBUILD'
                 ELSE 'REORGANIZE'
           END,
           concat(
            'ALTER INDEX [', i.name, '] ON [', OBJECT_SCHEMA_NAME(ips.object_id), '].[', OBJECT_NAME(ips.object_id), ']',
            CASE WHEN avg_fragmentation_in_percent >= 30 
                 THEN ' REBUILD PARTITION = ALL WITH (DATA_COMPRESSION = ROW, ONLINE = ON, RESUMABLE = ON);'
                 ELSE ' REORGANIZE;'
            END
            )
    FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'SAMPLED') AS ips
    JOIN sys.indexes i 
      ON ips.object_id = i.object_id AND ips.index_id = i.index_id
    WHERE (ips.page_count > 500
      AND avg_fragmentation_in_percent >= 10)

    OPEN c1;

    FETCH NEXT FROM c1 INTO @ix_name,@type,@sql;
    

    WHILE @@FETCH_STATUS = 0
    BEGIN
        --PRINT 'Executando: ' + @sql;
        BEGIN TRY
            EXEC sp_executesql @sql;
            
            set @msg = 'ix: ' + @ix_name
            EXEC auditing.sp_Log_Message @execution, @msg,5;

            insert into @output values(@type,@ix_name)

        END TRY
        BEGIN CATCH
            --PRINT 'Erro: ' + ERROR_MESSAGE();

            set @msg = CONCAT_WS(' ','ix: ',@ix_name,'Erro: ', ERROR_MESSAGE())
            exec auditing.sp_Log_Message @execution, @msg,5;

            insert into @output values('Erro',@ix_name)
        END CATCH;

        FETCH NEXT FROM c1 INTO @ix_name,@type,@sql;
    END

    CLOSE c1;
    DEALLOCATE c1;
    exec auditing.sp_Log_End @execution, @output;
end
GO

/****** Objeto:  StoredProcedure [dbo].[p_server_maintenance]    Data do Script: 26/07/2026 07:39:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [dbo].[sp_Database_Maintenance] @ParentExecutionGUID uniqueidentifier = null as
begin
    declare  @execution	UniqueIdentifier
            ,@output	dbo.MergeOutput
            ,@idjob		smallint
            ,@res       int

    begin try

    --Início
    exec auditing.sp_Log_Start 'sp_statistics_maintenance', @ParentExecutionGUID, @idjob, @execution output
    
    exec [dbo].[sp_Delete_Auditing_Logs]
    exec dbo.p_index_maintenance
    exec dbo.p_statistics_maintenance

    end try
    begin catch
        throw
    end catch
       
    --Fim
    exec auditing.sp_Log_End @execution, @output;


    
    --Excluir depois da migração.
    DECLARE @today DATE = GETDATE() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'
    insert into migracao.migracao_linx_totvs
    --OpenEra: JK / Campinas / Batel / E-Commerce
    select cnpj, '27/07/2026', null obs from (values('42538267000349'),('42538267000420'),('42538267000500'),('42538267000268')) tab(cnpj) where @today = '27/07/2026' union all
    -- Misha: Iguatemi / Morumbi / JK / Anália
    select cnpj, '28/07/2026', null from (values('38367316000431'),('38367316000350'),('38367316000270'),('38367316001160')) tab(cnpj) where @today = '28/07/2026' union all
    -- Misha: Batel / Leblon / Higienópolis / E-Commerce
    select cnpj, '29/07/2026', null from (values('38367316000946'),('38367316000601'),('38367316000784'),('38367316000199')) tab(cnpj) where @today = '29/07/2026'

end

GO

/****** Objeto:  StoredProcedure [dbo].[p_statistics_maintenance]    Data do Script: 26/07/2026 07:39:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [dbo].[sp_Statistics_Maintenance] (@ParentExecutionGUID uniqueidentifier = null) as
begin
    declare  @execution	    UniqueIdentifier
            ,@output	    dbo.MergeOutput
            ,@idjob		    smallint
            ,@res           int
            ,@error_message varchar(300)
            ,@SQL nvarchar(1000)

    --Início
    exec auditing.sp_Log_Start 'p_statistics_maintenance', @ParentExecutionGUID, @idjob, @execution output

    begin try

        --ESTATÍSTICAS
        --EXEC sp_updatestats;

        ----Fatos   
        --UPDATE STATISTICS dw.Fato_Canal_Movimentacao         WITH FULLSCAN;
        --UPDATE STATISTICS dw.Fato_Canal_Movimentacao_Captado WITH FULLSCAN;
        --UPDATE STATISTICS dw.Fato_Flash_Vendas               WITH FULLSCAN;

        --Cursor dinânimco para verificar se precisa de full scan ou atualização comum de estatísticas
        declare cur_stats cursor local fast_forward for
        select concat(  'update statistics ', 
                        quotename(schema_name(t.schema_id)), '.', 
                        quotename(t.name),
                        iif(left(t.name,4) = 'Fato' or coalesce(sp.modification_counter, 0) > (COALESCE(sp.rows, 0) * 0.05), ' with fullscan', '')) 
        from sys.tables t
        cross apply sys.dm_db_stats_properties(t.object_id, 1) sp
        where coalesce(sp.modification_counter, 0) > 0 
        --and (coalesce(sp.modification_counter, 0) > (coalesce(sp.rows, 0) * 0.01));

        open cur_stats

        fetch next from cur_stats into @sql

        while @@fetch_status = 0
        begin
            --print @sql
            exec sp_executesql @sql
            fetch next from cur_stats into @sql
        end;

        close cur_stats
        deallocate cur_stats


    end try
    begin catch
        select @error_message = left(ERROR_MESSAGE(),300)
        exec [auditing].[sp_Log_Error] @execution, @error_message 
    end catch
     
    exec auditing.sp_Log_nd @execution, @output;
end

GO

/****** Objeto:  StoredProcedure [dbo].[sp_Delete_Auditing_Logs]  Data do Script: 26/07/2026 07:39:33 ******/
CREATE PROCEDURE [dbo].[sp_Delete_Auditing_Logs] (
    @limitDate DATE = NULL, 
    @batchSize INT = 100000
) AS
BEGIN
    SET NOCOUNT ON
    SET ANSI_WARNINGS OFF

    SET @limitDate = ISNULL (@limitDate, DATEADD (DAY, -15, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'))

    DECLARE @total BIGINT = 0;
    DECLARE @rowcount BIGINT = 0;
    DECLARE @estimatedTotalRecords BIGINT;
    
    PRINT 'Início: ' + FORMAT(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time', 'yyyy-MM-dd HH:mm:ss');

    --Delete Messages
    DELETE 
        m
    FROM 
        [auditing].[Messages] m
    JOIN 
        [auditing].[Logs] l ON m.Execution = l.Execution
    WHERE 
        l.StartDate < @LimitDate;

    SET @rowcount = @@rowcount
    
    IF @rowcount = 0    
        GOTO AVISO

    PRINT 'Excluídos ' + CAST(@rowcount AS VARCHAR) + ' registros de Mensagens';

    --Delete Records
    SELECT 
        @estimatedTotalRecords = SUM(ROWS)
    FROM 
        sys.partitions
    WHERE 
        OBJECT_ID = OBJECT_ID('auditing.Records')
    AND 
        INDEX_ID = 1;

    WHILE 1 = 1
    BEGIN
        DELETE 
            TOP (@batchSize) r
        FROM 
            [auditing].[Records] r
        JOIN 
            [auditing].[Logs] l ON r.Execution = l.Execution
        WHERE
            l.StartDate < @LimitDate;

        SET @rowcount = @@ROWCOUNT;

        SET @total += @rowcount;

        PRINT 'Excluídos ' + CAST(@total AS VARCHAR) + '/' + CAST(@estimatedTotalRecords AS VARCHAR) + ' registros';
        
        IF @rowcount < @batchSize or @rowcount = 0 
            BREAK;

        WAITFOR DELAY '00:00:03';
    END

    PRINT 'Excluídos ' + CAST(@total AS VARCHAR) + ' registros de Records';

    --Delete logs
    SELECT 
        @estimatedTotalRecords = SUM(ROWS)
    FROM 
        sys.partitions
    WHERE 
        OBJECT_ID = OBJECT_ID('auditing.logs')
    AND 
        INDEX_ID = 1;
    
    SET @total = 0

    WHILE 1 = 1
    BEGIN
        DELETE TOP 
            (@batchSize)
        FROM 
            [auditing].[Logs]
        WHERE 
            StartDate < @LimitDate;

        SET @rowcount = @@ROWCOUNT;
        SET @total += @rowcount;

        PRINT 'Excluídos ' + CAST(@total AS VARCHAR) + '/' + CAST(@estimatedTotalRecords AS VARCHAR) + ' registros';
        
        IF @rowcount = 0  or @rowcount = 0 
            BREAK;

        WAITFOR DELAY '00:00:02';
    END

    PRINT 'Excluídos ' + CAST(@total AS VARCHAR) + ' registros de Logs';
    GOTO FIM

    AVISO:
        PRINT 'Não há registros a excluir';

    FIM:
        PRINT 'Fim: ' + FORMAT(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time', 'yyyy-MM-dd HH:mm:ss');
END

GO