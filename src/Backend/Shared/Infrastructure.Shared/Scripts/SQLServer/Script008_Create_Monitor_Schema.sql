/****** Schema monitor: empresas e personalização por empresa do painel (src/Backend/Shared/docs/PROPOSTA-DOC-COMPANY-AUDITING.md, 2.2) ******/
--fica fora de auditing: auditing é o registro do que rodou; monitor é configuração de apresentação e de acesso
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'monitor')
BEGIN
    EXEC('CREATE SCHEMA monitor')
END
GO

-- empresas clientes (CNPJ da holding): cadastradas pela tela ou pelo .ps1 de teste, nunca pelo DbUp
-- (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.1); credenciais ficam nas tabelas *Parameters de cada módulo
CREATE TABLE [monitor].[Company](
    [Company] [varchar](20) NOT NULL,
    [Name] [nvarchar](150) NOT NULL,
    [TradeName] [nvarchar](150) NULL,
    [IsActive] [bit] NOT NULL CONSTRAINT [DF_Company_IsActive] DEFAULT (1),
    [CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Company_CreatedAt] DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    [UpdatedAt] [datetime] NOT NULL CONSTRAINT [DF_Company_UpdatedAt] DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    -- só dígitos: o mesmo doc_company das tabelas *Parameters
    CONSTRAINT [CK_Company_Company] CHECK (LEN([Company]) > 0 AND [Company] NOT LIKE '%[^0-9]%'),
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED
(
    [Company] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- todo job de auditing.Jobs é de uma empresa cadastrada
ALTER TABLE [auditing].[Jobs] ADD  CONSTRAINT [FK_Jobs_Company] FOREIGN KEY([Company])
REFERENCES [monitor].[Company] ([Company])
GO

-- de/para dos CNPJs que aparecem em doc_company nas tabelas *Parameters para a empresa do painel (holding). As tabelas
-- *Parameters não mudam: os serviços filtram por JOIN monitor.HoldingCompanies ON Company = doc_company WHERE
-- Holding = @company. Um CNPJ é de uma holding só. A própria holding só entra aqui se algum módulo tiver parâmetros com
-- o CNPJ dela (NEWBLOOMERS: só as marcas Misha e Open Era; cliente de um CNPJ só: X -> X). Sem FK nas *Parameters:
-- cada módulo tem a sua tabela, e o .ps1 de teste avisa sobre CNPJ de parâmetro sem holding.
CREATE TABLE [monitor].[HoldingCompanies](
    [Company] [varchar](20) NOT NULL,
    [Holding] [varchar](20) NOT NULL,
    [Name] [nvarchar](150) NULL,
    [CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_HoldingCompanies_CreatedAt] DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    CONSTRAINT [CK_HoldingCompanies_Company] CHECK (LEN([Company]) > 0 AND [Company] NOT LIKE '%[^0-9]%'),
 CONSTRAINT [PK_HoldingCompanies] PRIMARY KEY CLUSTERED
(
    [Company] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [monitor].[HoldingCompanies] ADD  CONSTRAINT [FK_HoldingCompanies_Company] FOREIGN KEY([Holding])
REFERENCES [monitor].[Company] ([Company])
GO

CREATE NONCLUSTERED INDEX [IX_HoldingCompanies_Holding] ON [monitor].[HoldingCompanies] ([Holding] ASC)
WITH (STATISTICS_NORECOMPUTE = OFF) ON [PRIMARY]
GO

-- imagem por módulo e empresa; Company = '*' é a imagem padrão do módulo
CREATE TABLE [monitor].[AppImage](
    [IdParent] [int] NOT NULL,
    [Company] [varchar](20) NOT NULL,
    [ContentType] [varchar](100) NOT NULL,
    [Data] [varbinary](max) NOT NULL,
    [UpdatedAt] [datetime] NOT NULL CONSTRAINT [DF_AppImage_UpdatedAt] DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
 CONSTRAINT [PK_AppImage] PRIMARY KEY CLUSTERED
(
    [IdParent] ASC,
    [Company] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [monitor].[AppImage] ADD  CONSTRAINT [FK_AppImage_Apps] FOREIGN KEY([IdParent])
REFERENCES [auditing].[Apps] ([IdParent])
GO

-- parâmetros de apresentação e acesso que não existem em nenhuma tabela *Parameters
CREATE TABLE [monitor].[AppSetting](
    [IdParent] [int] NOT NULL,
    [Company] [varchar](20) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Value] [nvarchar](max) NULL,
    [IsSecret] [bit] NOT NULL CONSTRAINT [DF_AppSetting_IsSecret] DEFAULT (0),
    [UpdatedAt] [datetime] NOT NULL CONSTRAINT [DF_AppSetting_UpdatedAt] DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
 CONSTRAINT [PK_AppSetting] PRIMARY KEY CLUSTERED
(
    [IdParent] ASC,
    [Company] ASC,
    [Name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [monitor].[AppSetting] ADD  CONSTRAINT [FK_AppSetting_Apps] FOREIGN KEY([IdParent])
REFERENCES [auditing].[Apps] ([IdParent])
GO

-- falha reconhecida pelo painel (src/Frontend/WebApp/docs/PLANO-WEBAPP.md, 11.5.1): um reconhecimento por job da empresa,
-- válido enquanto a última execução for Execution (nula: job atrasado que nunca rodou) e até ExpiresAt. Reconhecer de novo
-- substitui o anterior. A saúde do job não muda: o painel só tira o job de "Falhas Ativas".
CREATE TABLE [monitor].[JobAcknowledgement](
    [IdJob] [int] NOT NULL,
    [IdType] [int] NOT NULL,
    [Company] [varchar](20) NOT NULL,
    [Execution] [uniqueidentifier] NULL,
    [Note] [nvarchar](500) NULL,
    [User] [nvarchar](100) NOT NULL,
    [At] [datetime] NOT NULL CONSTRAINT [DF_JobAcknowledgement_At] DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    [ExpiresAt] [datetime] NOT NULL,
 CONSTRAINT [PK_JobAcknowledgement] PRIMARY KEY CLUSTERED
(
    [IdJob] ASC,
    [IdType] ASC,
    [Company] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [monitor].[JobAcknowledgement] ADD  CONSTRAINT [FK_JobAcknowledgement_Jobs] FOREIGN KEY([IdJob], [IdType], [Company])
REFERENCES [auditing].[Jobs] ([IdJob], [IdType], [Company])
GO

-- cadastra a empresa (ou só confere, se já existir) e cria os jobs dos hosts dela (IdParent 140, Script007): toda
-- empresa tem o seu próprio 14000. Os jobs nascem inativos, como em auditing.sp_Add_Company_App (Script002).
CREATE OR ALTER PROCEDURE [auditing].[sp_Add_Company] (
    @company VARCHAR(20),
    @name NVARCHAR(150),
    @tradeName NVARCHAR(150) = NULL
) AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- antes da transação: o CNPJ de uma marca de outra holding não vira empresa do painel
    DECLARE @holding VARCHAR(20) = (SELECT Holding FROM [monitor].[HoldingCompanies] WHERE Company = @company);
    IF @holding IS NOT NULL AND @holding <> @company
    BEGIN
        DECLARE @msg NVARCHAR(200) = CONCAT(N'O CNPJ ', @company, N' já é da holding ', @holding, N' em monitor.HoldingCompanies.');
        THROW 50003, @msg, 1;
    END

    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM [monitor].[Company] WHERE Company = @company)
        INSERT INTO [monitor].[Company] (Company, [Name], TradeName) VALUES (@company, @name, @tradeName);

    EXEC [auditing].[sp_Add_Company_App] @company = @company, @idParent = 140;

    COMMIT TRANSACTION;
END
GO

-- liga um CNPJ de parâmetros (doc_company) à empresa do painel. Chamada pela tela (ou pelo .ps1 de teste) ao gravar os
-- parâmetros de um módulo: a holding só entra como Company quando ela mesma tem parâmetros. Rodar de novo para o mesmo
-- par não faz nada; um CNPJ que já é de outra holding, ou que é outra empresa do painel, é recusado.
CREATE OR ALTER PROCEDURE [auditing].[sp_Add_Holding_Company] (
    @company VARCHAR(20),
    @docCompany VARCHAR(20),
    @name NVARCHAR(150) = NULL
) AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @msg NVARCHAR(200);

    IF NOT EXISTS (SELECT 1 FROM [monitor].[Company] WHERE Company = @company)
    BEGIN
        SET @msg = CONCAT(N'A empresa ', @company, N' não está em monitor.Company.');
        THROW 50002, @msg, 1;
    END

    DECLARE @holding VARCHAR(20) = (SELECT Holding FROM [monitor].[HoldingCompanies] WHERE Company = @docCompany);
    IF @holding IS NOT NULL AND @holding <> @company
    BEGIN
        SET @msg = CONCAT(N'O CNPJ ', @docCompany, N' já é da holding ', @holding, N' em monitor.HoldingCompanies.');
        THROW 50003, @msg, 1;
    END

    IF @docCompany <> @company AND EXISTS (SELECT 1 FROM [monitor].[Company] WHERE Company = @docCompany)
    BEGIN
        SET @msg = CONCAT(N'O CNPJ ', @docCompany, N' é uma empresa do painel e não pode ser marca de ', @company, N'.');
        THROW 50004, @msg, 1;
    END

    IF @holding IS NULL
        INSERT INTO [monitor].[HoldingCompanies] (Company, Holding, [Name]) VALUES (@docCompany, @company, @name);
END
GO
