/****** Schema monitor: personalização por empresa do painel (src/Backend/Shared/docs/PROPOSTA-DOC-COMPANY-AUDITING.md, 2.2) ******/
--fica fora de auditing: auditing é o registro do que rodou; monitor é configuração de apresentação e de acesso
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'monitor')
BEGIN
    EXEC('CREATE SCHEMA monitor')
END
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

-- parâmetros de apresentação e acesso que não existem em nenhuma tabela *Parameters (decisão em aberto, proposta 7.2 item 1)
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
