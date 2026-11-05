/****** Aplicações removidas da Visão Geral do WebApp (src/Frontend/WebApp/README.md, "Visão Geral") ******/
-- O ✕ do card esconde a aplicação da Visão Geral da empresa, para todos os usuários dela: a contratação, a credencial, a
-- agenda e o histórico continuam como estão, e o "Mostrar" do Adicionar aplicação apaga a linha.
CREATE TABLE [monitor].[HiddenApps](
    [Company] [varchar](20) NOT NULL,
    [IdParent] [int] NOT NULL,
    [HiddenAt] [datetime] NOT NULL CONSTRAINT [DF_HiddenApps_HiddenAt] DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
 CONSTRAINT [PK_HiddenApps] PRIMARY KEY CLUSTERED
(
    [Company] ASC,
    [IdParent] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [monitor].[HiddenApps] ADD CONSTRAINT [FK_HiddenApps_Companies] FOREIGN KEY([Company])
REFERENCES [auditing].[Companies] ([Company])
GO

ALTER TABLE [monitor].[HiddenApps] ADD CONSTRAINT [FK_HiddenApps_Apps] FOREIGN KEY([IdParent])
REFERENCES [auditing].[Apps] ([IdParent])
GO

-- o etl_monitor que já existe (banco migrado antes deste script) ganha a permissão aqui; o criado depois, pela
-- dbo.sp_Create_Monitor_User (Script009)
IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'etl_monitor')
    GRANT INSERT, DELETE ON [monitor].[HiddenApps] TO [etl_monitor];
GO
