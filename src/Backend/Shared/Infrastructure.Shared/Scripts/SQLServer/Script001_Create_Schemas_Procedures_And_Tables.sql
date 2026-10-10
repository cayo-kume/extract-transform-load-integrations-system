/****** Alter Charset Database ******/
ALTER DATABASE Company
COLLATE Latin1_General_100_CI_AS_SC_UTF8;
GO

/****** Create Schemas ******/
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'auditing')
BEGIN
    -- O comando CREATE SCHEMA não aceita variáveis e deve ser executado isoladamente ou via EXEC
    EXEC('CREATE SCHEMA auditing')
END

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'general')
BEGIN
    EXEC('CREATE SCHEMA general')
END

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'untreated')
BEGIN
    EXEC('CREATE SCHEMA untreated')
END

/****** Create Data Tables Types ******/
IF NOT EXISTS (SELECT * FROM sys.table_types WHERE name = 'MergeOutput' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TYPE dbo.MergeOutput AS TABLE (
        [operacao] VARCHAR(50) NULL, 
        [chave] VARCHAR(MAX) NULL
    );
END

/****** Create Data Tables ******/
CREATE TABLE [auditing].[Apps](
	[IdParent] [int] NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Description] [nvarchar](250) NULL,
 CONSTRAINT [PK_Apps] PRIMARY KEY CLUSTERED 
(
	[IdParent] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [auditing].[Jobs](
	[IdParent] [int] NULL,
	[IdJob] [int] NOT NULL,
	[Title] [nvarchar](80) NULL,
	[Description] [nvarchar](800) NULL,
	[IntervalTimeInMinutes] [int] NOT NULL,
	[IdType] [int] NOT NULL
 CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED 
(
	[IdJob] ASC,
	[IdType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [auditing].[Logs](
	[IdExecution] [bigint] IDENTITY(1,1) NOT NULL,
	[IdJob] [int] NOT NULL,
	[IdType] [int] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Execution] [uniqueidentifier] NOT NULL,
	[ExecutionParent] [uniqueidentifier] NULL,
 CONSTRAINT [PK_JobExecutionsLogs_1] PRIMARY KEY CLUSTERED 
(
	[IdJob] ASC,
	[IdType] ASC,
	[Execution] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF, DATA_COMPRESSION = ROW) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [auditing].[Messages](
	[IdExecutionMessage] [bigint] IDENTITY(1,1) NOT NULL,
	[IdJob] [int] NOT NULL,
	[IdType] [int] NOT NULL,
	[Execution] [uniqueidentifier] NOT NULL,
	[IsError] [bit] NULL,
	[Message] [nvarchar](800) NULL,
	[SqlCommand] [nvarchar](max) NULL,
	[ExceptionMessage] [nvarchar](max) NULL,
	[ExceptionStackTrace] [nvarchar](max) NULL
 CONSTRAINT [PK_JobExecutionsMessages] PRIMARY KEY CLUSTERED 
(
	[IdExecutionMessage] ASC,
	[IdJob] ASC,
	[IdType] ASC,
	[Execution] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF, DATA_COMPRESSION = ROW) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [auditing].[Records](
	[IdRecord] [bigint] IDENTITY(1,1) NOT NULL,
	[IdJob] [int] NOT NULL,
	[IdType] [int] NOT NULL,
	[FieldKeyValue] [nvarchar](400) NULL,
	[RecordText] [varchar](max) NULL,
	[Execution] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Records] PRIMARY KEY CLUSTERED 
(
	[IdRecord] ASC,
	[IdJob] ASC,
	[IdType] ASC,
	[Execution] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [auditing].[Types](
	[IdType] [int] NOT NULL,
	[JobType] [nvarchar](50) NULL,
	[Description] [nvarchar](250) NULL,
 CONSTRAINT [PK_JobsTypes] PRIMARY KEY CLUSTERED 
(
	[IdType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--NÃO CRIAR RELAÇÃO ENTRE APPS E JOBS, POIS UM JOB PODE SER UM APP PARA UMA PROCEDURE DE SINCRONIZAÇÃO, E SE A RELAÇÃO EXISTIR TODO JOB TERIA QUE SER UM APP E NÃO É ISSO QUE EU DESEJO

ALTER TABLE [auditing].[Records] ADD  CONSTRAINT [FK_Records] FOREIGN KEY([IdJob], [IdType], [Execution])
REFERENCES [auditing].[Logs] ([IdJob], [IdType], [Execution])
GO

ALTER TABLE [auditing].[Messages] ADD  CONSTRAINT [FK_Messages] FOREIGN KEY([IdJob], [IdType], [Execution])
REFERENCES [auditing].[Logs] ([IdJob], [IdType], [Execution])
GO

ALTER TABLE [auditing].[Logs] ADD  CONSTRAINT [FK_Logs] FOREIGN KEY([IdJob],[IdType])
REFERENCES [auditing].[Jobs] ([IdJob],[IdType])
GO

ALTER TABLE [auditing].[Jobs] ADD  CONSTRAINT [FK_Jobs] FOREIGN KEY([IdType])
REFERENCES [auditing].[Types] ([IdType])
GO