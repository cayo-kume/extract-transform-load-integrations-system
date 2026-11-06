/****** Schema monitor: personalização por holding do painel (src/Backend/Shared/.docs/PROPOSTA-DOC-COMPANY-AUDITING.md, 2.2) ******/
--fica fora de auditing: auditing é o registro do que rodou e o cadastro (holdings, empresas e jobs); monitor é
--configuração de apresentação e de acesso. As holdings e as empresas ficam em auditing.Holders e auditing.Companies
--(.docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md)
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'monitor')
BEGIN
    EXEC('CREATE SCHEMA monitor')
END
GO

-- imagem por módulo e holding; Holder = '*' é a imagem padrão do módulo
CREATE TABLE [monitor].[AppImage](
    [IdParent] [int] NOT NULL,
    [Holder] [varchar](20) NOT NULL,
    [ContentType] [varchar](100) NOT NULL,
    [Data] [varbinary](max) NOT NULL,
    [UpdatedAt] [datetime] NOT NULL CONSTRAINT [DF_AppImage_UpdatedAt] DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
 CONSTRAINT [PK_AppImage] PRIMARY KEY CLUSTERED
(
    [IdParent] ASC,
    [Holder] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [monitor].[AppImage] ADD  CONSTRAINT [FK_AppImage_Apps] FOREIGN KEY([IdParent])
REFERENCES [auditing].[Apps] ([IdParent])
GO

-- parâmetros de apresentação e acesso por módulo e holding que não existem em nenhuma tabela *Parameters
CREATE TABLE [monitor].[AppSetting](
    [IdParent] [int] NOT NULL,
    [Holder] [varchar](20) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Value] [nvarchar](max) NULL,
    [IsSecret] [bit] NOT NULL CONSTRAINT [DF_AppSetting_IsSecret] DEFAULT (0),
    [UpdatedAt] [datetime] NOT NULL CONSTRAINT [DF_AppSetting_UpdatedAt] DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
 CONSTRAINT [PK_AppSetting] PRIMARY KEY CLUSTERED
(
    [IdParent] ASC,
    [Holder] ASC,
    [Name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [monitor].[AppSetting] ADD  CONSTRAINT [FK_AppSetting_Apps] FOREIGN KEY([IdParent])
REFERENCES [auditing].[Apps] ([IdParent])
GO

-- falha reconhecida pelo painel (src/Frontend/WebApp/.docs/PLANO-WEBAPP.md, 11.5.1): um reconhecimento por job da empresa,
-- válido enquanto a última execução for Execution (nula: job atrasado que nunca rodou) e até ExpiresAt. Reconhecer de novo
-- substitui o anterior. A saúde do job não muda: o painel só tira o job de "Falhas Ativas".
CREATE TABLE [monitor].[JobAcknowledgement](
    [IdJob] [int] NOT NULL,
    [IdType] [int] NOT NULL,
    [Holder] [varchar](20) NOT NULL,
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
    [Holder] ASC,
    [Company] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [monitor].[JobAcknowledgement] ADD  CONSTRAINT [FK_JobAcknowledgement_Jobs] FOREIGN KEY([IdJob], [IdType], [Holder], [Company])
REFERENCES [auditing].[Jobs] ([IdJob], [IdType], [Holder], [Company])
GO

-- usuários do painel (WebApp): login por usuário ou e-mail; a senha só como hash (PBKDF2, PasswordHasher do ASP.NET Core).
-- O primeiro usuário de uma holding nasce com ela, na tela de cadastro; os outros, em Cadastros › Cadastrar Usuário.
-- Profile é o perfil de acesso dentro da holding: Administrador (tudo da holding, dá qualquer perfil), Gestor (muda o que é
-- da empresa dele) ou Visualizador (só vê). Uma holding nunca vê outra, qualquer que seja o perfil. IsOwner marca o dono do
-- aplicativo, o único que vê todas as holdings, o Executar Querys e os Hosts de Agenda; não se muda pelo painel. O primeiro
-- usuário do banco nasce Administrador e dono; o de cada holding nova, Gestor
CREATE TABLE [monitor].[Users](
    [IdUser] [int] IDENTITY(1,1) NOT NULL,
    [Company] [varchar](20) NOT NULL,
    [Name] [nvarchar](150) NOT NULL,
    [UserName] [nvarchar](100) NOT NULL,
    [Email] [nvarchar](200) NOT NULL,
    [PasswordHash] [nvarchar](500) NOT NULL,
    [Profile] [varchar](20) NOT NULL CONSTRAINT [DF_Users_Profile] DEFAULT ('Visualizador'),
    [IsOwner] [bit] NOT NULL CONSTRAINT [DF_Users_IsOwner] DEFAULT (0),
    [CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Users_CreatedAt] DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED
(
    [IdUser] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Users_UserName] UNIQUE ([UserName] ASC),
 CONSTRAINT [UQ_Users_Email] UNIQUE ([Email] ASC),
 CONSTRAINT [CK_Users_Profile] CHECK ([Profile] IN ('Administrador', 'Gestor', 'Visualizador'))
) ON [PRIMARY]
GO

ALTER TABLE [monitor].[Users] ADD  CONSTRAINT [FK_Users_Companies] FOREIGN KEY([Company])
REFERENCES [auditing].[Companies] ([Company])
GO
