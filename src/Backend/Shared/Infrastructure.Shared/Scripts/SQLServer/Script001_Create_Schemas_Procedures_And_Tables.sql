/****** Alter Charset Database ******/
ALTER DATABASE Company
COLLATE Latin1_General_100_CI_AS_SC_UTF8;
GO

/****** Create Schemas ******/
CREATE SCHEMA auditing;
GO

CREATE SCHEMA general;
GO

CREATE SCHEMA untreated;
GO

/****** Create Data Tables Types ******/
CREATE TYPE dbo.MergeOutput AS TABLE ([operacao] VARCHAR(50) NULL, [chave] varchar(max) null);

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
	[IdType] [int] NULL
 CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED 
(
	[IdJob] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [auditing].[Logs](
	[IdExecution] [bigint] IDENTITY(1,1) NOT NULL,
	[IdJob] [smallint] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Execution] [uniqueidentifier] NOT NULL,
	[ExecutionParent] [uniqueidentifier] NULL,
 CONSTRAINT [PK_JobExecutionsLogs_1] PRIMARY KEY CLUSTERED 
(
	[Execution] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF, DATA_COMPRESSION = ROW) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [auditing].[Messages](
	[IdExecutionMessage] [bigint] IDENTITY(1,1) NOT NULL,
	[Execution] [uniqueidentifier] NULL,
	[IsError] [bit] NULL,
	[Message] [nvarchar](800) NULL,
	[SqlCommand] [nvarchar](max) NULL,
	[ExceptionMessage] [nvarchar](max) NULL,
	[ExceptionStackTrace] [nvarchar](max) NULL
 CONSTRAINT [PK_JobExecutionsMessages] PRIMARY KEY CLUSTERED 
(
	[IdExecutionMessage] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF, DATA_COMPRESSION = ROW) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [auditing].[Records](
	[IdRecord] [bigint] IDENTITY(1,1) NOT NULL,
	[FieldKeyValue] [nvarchar](400) NULL,
	[RecordText] [varchar](max) NULL,
	[Execution] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Records] PRIMARY KEY CLUSTERED 
(
	[IdRecord] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [auditing].[Records]  WITH NOCHECK ADD  CONSTRAINT [FK_Records_JobExecutionsLogs] FOREIGN KEY([Execution])
REFERENCES [auditing].[Logs] ([Execution])
GO

ALTER TABLE [auditing].[Records] CHECK CONSTRAINT [FK_Records_JobExecutionsLogs]
GO

CREATE TABLE [auditing].[Types](
	[IdType] [int] IDENTITY(1,1) NOT NULL,
	[JobType] [nvarchar](50) NULL,
	[Description] [nvarchar](250) NULL,
 CONSTRAINT [PK_JobsTypes] PRIMARY KEY CLUSTERED 
(
	[IdType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--NÃO CRIAR RELAÇÃO ENTRE APPS E JOBS, POIS UM JOB PODE SER UM APP PARA UMA PROCEDURE DE SINCRONIZAÇÃO, E SE A RELAÇÃO EXISTIR TODO JOB TERIA QUE SER UM APP E NÃO É ISSO QUE EU DESEJO