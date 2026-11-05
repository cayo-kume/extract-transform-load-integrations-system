/****** Usuários de banco do WebApp (src/Frontend/WebApp/docs/PLANO-WEBAPP.md, passo 10) ******/
-- A senha não fica em script nem no git: o DbUp só cria as procedures, e quem tem ALTER ANY LOGIN (o sa em
-- desenvolvimento) roda cada uma com a senha, à mão ou pelo migrator.csx do Shared com as variáveis de ambiente
-- ETL_MONITOR_PASSWORD e ETL_QUERY_PASSWORD. Rodar de novo não troca a senha (isso é ALTER LOGIN, fora daqui) e só
-- completa o que faltar: um módulo migrado depois ganha as permissões de *Parameters, e um banco criado depois ganha o
-- etl_query. No Azure SQL Database (EngineEdition 5) não há login no servidor: o usuário é contido no banco.

-- etl_monitor: leitura de auditing e monitor, a agenda e a ativação dos jobs, o reconhecimento de falhas, as imagens e o
-- cadastro de holdings, empresas, jobs e usuários do painel (INSERT direto e as tabelas *Parameters que já existem no banco)
CREATE OR ALTER PROCEDURE [dbo].[sp_Create_Monitor_User] (
    @password NVARCHAR(128) = NULL
) AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @contained BIT = IIF(CAST(SERVERPROPERTY('EngineEdition') AS INT) = 5, 1, 0);
    DECLARE @sql NVARCHAR(MAX);

    IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'etl_monitor')
    BEGIN
        IF @contained = 0 AND NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'etl_monitor')
        BEGIN
            IF @password IS NULL
                THROW 50010, N'O login etl_monitor não existe: informe @password para criá-lo.', 1;

            SET @sql = N'CREATE LOGIN [etl_monitor] WITH PASSWORD = ' + QUOTENAME(@password, '''') + N', CHECK_POLICY = ON;';
            EXEC (@sql);
        END

        IF @contained = 1
        BEGIN
            IF @password IS NULL
                THROW 50010, N'O usuário etl_monitor não existe: informe @password para criá-lo.', 1;

            SET @sql = N'CREATE USER [etl_monitor] WITH PASSWORD = ' + QUOTENAME(@password, '''') + N';';
            EXEC (@sql);
        END
        ELSE
            CREATE USER [etl_monitor] FOR LOGIN [etl_monitor];
    END

    GRANT SELECT ON SCHEMA::[auditing] TO [etl_monitor];
    GRANT SELECT ON SCHEMA::[monitor] TO [etl_monitor];

    -- cadastro de holdings e empresas pela tela (INSERT direto, docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md)
    GRANT INSERT, UPDATE ON [auditing].[Holders] TO [etl_monitor];
    GRANT INSERT, UPDATE ON [auditing].[Companies] TO [etl_monitor];

    -- jobs da empresa: a tela contrata um módulo com INSERT a partir de auditing.fn_CatalogTree e muda a agenda e a
    -- ativação só nessas colunas
    GRANT INSERT ON [auditing].[Jobs] TO [etl_monitor];
    GRANT UPDATE ON [auditing].[Jobs] ([IsActive], [IntervalTimeInMinutes], [RunningDailyAt], [Queue]) TO [etl_monitor];

    GRANT INSERT, UPDATE, DELETE ON [monitor].[JobAcknowledgement] TO [etl_monitor];
    GRANT INSERT, UPDATE, DELETE ON [monitor].[AppImage] TO [etl_monitor];
    GRANT INSERT, UPDATE ON [monitor].[Users] TO [etl_monitor];

    -- parâmetros dos módulos (tela de cadastro): uma tabela *Parameters por módulo migrado neste banco
    SET @sql = N'';
    SELECT @sql += N'GRANT SELECT, INSERT, UPDATE ON ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + N' TO [etl_monitor];' + CHAR(10)
    FROM sys.tables t
    JOIN sys.schemas s ON s.schema_id = t.schema_id
    WHERE t.name LIKE N'%Parameters' AND s.name NOT IN (N'auditing', N'monitor', N'dbo');

    EXEC (@sql);
END
GO

-- etl_query: a tela Executar Querys, em todos os bancos ONLINE do servidor (sem os de sistema): ler, escrever, DDL e
-- EXECUTE, sem db_owner e sem permissão de servidor. Banco criado depois: rodar de novo, com @database para só ele.
CREATE OR ALTER PROCEDURE [dbo].[sp_Grant_Query_User] (
    @password NVARCHAR(128) = NULL,
    @database SYSNAME = NULL
) AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @contained BIT = IIF(CAST(SERVERPROPERTY('EngineEdition') AS INT) = 5, 1, 0);
    DECLARE @sql NVARCHAR(MAX);
    DECLARE @grants NVARCHAR(MAX) = N'
        IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = ''etl_query'')
            CREATE USER [etl_query] FOR LOGIN [etl_query];
        ALTER ROLE [db_datareader] ADD MEMBER [etl_query];
        ALTER ROLE [db_datawriter] ADD MEMBER [etl_query];
        ALTER ROLE [db_ddladmin] ADD MEMBER [etl_query];
        GRANT EXECUTE TO [etl_query];';

    IF @contained = 1
    BEGIN
        -- Azure SQL Database: um banco por conexão, usuário contido só neste
        IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'etl_query')
        BEGIN
            IF @password IS NULL
                THROW 50010, N'O usuário etl_query não existe: informe @password para criá-lo.', 1;

            SET @sql = N'CREATE USER [etl_query] WITH PASSWORD = ' + QUOTENAME(@password, '''') + N';';
            EXEC (@sql);
        END

        EXEC (@grants);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'etl_query')
    BEGIN
        IF @password IS NULL
            THROW 50010, N'O login etl_query não existe: informe @password para criá-lo.', 1;

        SET @sql = N'CREATE LOGIN [etl_query] WITH PASSWORD = ' + QUOTENAME(@password, '''') + N', CHECK_POLICY = ON;';
        EXEC (@sql);
    END

    IF @database IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = @database AND database_id > 4)
    BEGIN
        DECLARE @msg NVARCHAR(300) = CONCAT(N'O banco ', @database, N' não existe ou é de sistema.');
        THROW 50011, @msg, 1;
    END

    DECLARE @current SYSNAME;
    DECLARE databases CURSOR LOCAL FAST_FORWARD FOR
        SELECT name FROM sys.databases
        WHERE database_id > 4 AND state_desc = 'ONLINE' AND is_read_only = 0 AND (@database IS NULL OR name = @database)
        ORDER BY name;

    OPEN databases;
    FETCH NEXT FROM databases INTO @current;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = N'EXEC ' + QUOTENAME(@current) + N'.sys.sp_executesql @grants;';
        EXEC sys.sp_executesql @sql, N'@grants NVARCHAR(MAX)', @grants = @grants;

        FETCH NEXT FROM databases INTO @current;
    END

    CLOSE databases;
    DEALLOCATE databases;
END
GO
