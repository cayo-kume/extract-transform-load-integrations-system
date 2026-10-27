INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (atributos de produto: 8534 - 8543)*/
(8034, 8534, 'sp_LinxSetores_Sync', 'Procedure de sincronização entre as tabelas LinxSetores raw e trusted', 1440, 1),
(8035, 8535, 'sp_LinxLinhas_Sync', 'Procedure de sincronização entre as tabelas LinxLinhas raw e trusted', 1440, 1),
(8036, 8536, 'sp_LinxMarcas_Sync', 'Procedure de sincronização entre as tabelas LinxMarcas raw e trusted', 1440, 1),
(8037, 8537, 'sp_LinxColecoes_Sync', 'Procedure de sincronização entre as tabelas LinxColecoes raw e trusted', 1440, 1),
(8038, 8538, 'sp_LinxTamanhos_Sync', 'Procedure de sincronização entre as tabelas LinxTamanhos raw e trusted', 1440, 1),
(8039, 8539, 'sp_LinxCores_Sync', 'Procedure de sincronização entre as tabelas LinxCores raw e trusted', 1440, 1),
(8040, 8540, 'sp_LinxEspessuras_Sync', 'Procedure de sincronização entre as tabelas LinxEspessuras raw e trusted', 1440, 1),
(8041, 8541, 'sp_LinxUnidades_Sync', 'Procedure de sincronização entre as tabelas LinxUnidades raw e trusted', 1440, 1),
(8042, 8542, 'sp_LinxClassificacoes_Sync', 'Procedure de sincronização entre as tabelas LinxClassificacoes raw e trusted', 1440, 1),
(8043, 8543, 'sp_LinxSubClasses_Sync', 'Procedure de sincronização entre as tabelas LinxSubClasses raw e trusted', 1440, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8034, 'LinxSetores', 'Integração de dados da tabela LinxSetores, uma vez por dia', 1440, 2),
(80, 8035, 'LinxLinhas', 'Integração de dados da tabela LinxLinhas, uma vez por dia', 1440, 2),
(80, 8036, 'LinxMarcas', 'Integração de dados da tabela LinxMarcas, uma vez por dia', 1440, 2),
(80, 8037, 'LinxColecoes', 'Integração de dados da tabela LinxColecoes, uma vez por dia', 1440, 2),
(80, 8038, 'LinxTamanhos', 'Integração de dados da tabela LinxTamanhos, uma vez por dia', 1440, 2),
(80, 8039, 'LinxCores', 'Integração de dados da tabela LinxCores, uma vez por dia', 1440, 2),
(80, 8040, 'LinxEspessuras', 'Integração de dados da tabela LinxEspessuras, uma vez por dia', 1440, 2),
(80, 8041, 'LinxUnidades', 'Integração de dados da tabela LinxUnidades, uma vez por dia', 1440, 2),
(80, 8042, 'LinxClassificacoes', 'Integração de dados da tabela LinxClassificacoes, uma vez por dia', 1440, 2),
(80, 8043, 'LinxSubClasses', 'Integração de dados da tabela LinxSubClasses, uma vez por dia', 1440, 2);
