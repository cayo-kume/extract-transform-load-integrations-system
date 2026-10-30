INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (configurações tributárias: 8579 - 8582)*/
(8079, 8579, 'sp_LinxConfiguracoesTributarias_Sync', 'Procedure de sincronização entre as tabelas LinxConfiguracoesTributarias raw e trusted', 1440, 1),
(8080, 8580, 'sp_LinxConfiguracoesTributariasDetalhes_Sync', 'Procedure de sincronização entre as tabelas LinxConfiguracoesTributariasDetalhes raw e trusted', 1440, 1),
(8081, 8581, 'sp_LinxConfiguracoesTributariasDetalhesSimplificado_Sync', 'Procedure de sincronização entre as tabelas LinxConfiguracoesTributariasDetalhesSimplificado raw e trusted', 1440, 1),
(8082, 8582, 'sp_LinxConfiguracoesTributariasEmpresas_Sync', 'Procedure de sincronização entre as tabelas LinxConfiguracoesTributariasEmpresas raw e trusted', 1440, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8079, 'LinxConfiguracoesTributarias', 'Integração de dados da tabela LinxConfiguracoesTributarias, uma vez por dia', 1440, 2),
(80, 8080, 'LinxConfiguracoesTributariasDetalhes', 'Integração de dados da tabela LinxConfiguracoesTributariasDetalhes, uma vez por dia', 1440, 2),
(80, 8081, 'LinxConfiguracoesTributariasDetalhesSimplificado', 'Integração de dados da tabela LinxConfiguracoesTributariasDetalhesSimplificado, uma vez por dia', 1440, 2),
(80, 8082, 'LinxConfiguracoesTributariasEmpresas', 'Integração de dados da tabela LinxConfiguracoesTributariasEmpresas, uma vez por dia', 1440, 2);
