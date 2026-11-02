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

    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM [monitor].[Company] WHERE Company = @company)
        INSERT INTO [monitor].[Company] (Company, [Name], TradeName) VALUES (@company, @name, @tradeName);

    EXEC [auditing].[sp_Add_Company_App] @company = @company, @idParent = 140;

    COMMIT TRANSACTION;
END
GO
