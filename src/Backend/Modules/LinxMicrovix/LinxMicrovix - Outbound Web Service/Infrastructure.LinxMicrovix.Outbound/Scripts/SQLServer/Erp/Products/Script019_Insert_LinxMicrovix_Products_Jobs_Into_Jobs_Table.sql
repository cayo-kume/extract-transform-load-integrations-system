INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (produtos: 8524 - 8533)*/
(8024, 8524, 'sp_LinxProdutos_Sync', 'Procedure de sincronização entre as tabelas LinxProdutos raw e trusted', 120, NULL, 'default', 1),
(8025, 8525, 'sp_LinxProdutosAssociados_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosAssociados raw e trusted', 120, NULL, 'default', 1),
(8026, 8526, 'sp_LinxProdutosCamposAdicionais_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosCamposAdicionais raw e trusted', 120, NULL, 'default', 1),
(8027, 8527, 'sp_LinxProdutosCodBar_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosCodBar raw e trusted', 120, NULL, 'default', 1),
(8028, 8528, 'sp_LinxProdutosFornec_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosFornec raw e trusted', 120, NULL, 'default', 1),
(8029, 8529, 'sp_LinxProdutosInformacoes_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosInformacoes raw e trusted', 120, NULL, 'default', 1),
(8030, 8530, 'sp_LinxProdutosImagensURL_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosImagensURL raw e trusted', 120, NULL, 'default', 1),
(8031, 8531, 'sp_LinxProdutosLotes_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosLotes raw e trusted', 120, NULL, 'default', 1),
(8032, 8532, 'sp_LinxLotesProdutos_Sync', 'Procedure de sincronização entre as tabelas LinxLotesProdutos raw e trusted', 120, NULL, 'default', 1),
(8033, 8533, 'sp_LinxProdutosSerial_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosSerial raw e trusted', 120, NULL, 'default', 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8024, 'LinxProdutos', 'Integração de dados da tabela LinxProdutos, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8025, 'LinxProdutosAssociados', 'Integração de dados da tabela LinxProdutosAssociados, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8026, 'LinxProdutosCamposAdicionais', 'Integração de dados da tabela LinxProdutosCamposAdicionais, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8027, 'LinxProdutosCodBar', 'Integração de dados da tabela LinxProdutosCodBar, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8028, 'LinxProdutosFornec', 'Integração de dados da tabela LinxProdutosFornec, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8029, 'LinxProdutosInformacoes', 'Integração de dados da tabela LinxProdutosInformacoes, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8030, 'LinxProdutosImagensURL', 'Integração de dados da tabela LinxProdutosImagensURL, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8031, 'LinxProdutosLotes', 'Integração de dados da tabela LinxProdutosLotes, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8032, 'LinxLotesProdutos', 'Integração de dados da tabela LinxLotesProdutos, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8033, 'LinxProdutosSerial', 'Integração de dados da tabela LinxProdutosSerial, a cada 2 horas', 120, NULL, 'default', 2),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(80, 8198, 'LinxProdutosCodBarByCodProduto', 'Integração de LinxProdutosCodBar por codProduto (GET LinxProdutosCodBar/{docCompany}/{codProduto})', 0, NULL, 'default', 2),
(80, 8204, 'LinxProdutosCamposAdicionaisByCodProduto', 'Integração de LinxProdutosCamposAdicionais por codProduto (GET LinxProdutosCamposAdicionais/{docCompany}/{cnpjEmp}/{codProduto})', 0, NULL, 'default', 2),
(80, 8205, 'LinxProdutosByCodProduto', 'Integração de LinxProdutos por codProduto (GET LinxProdutos/{docCompany}/{codProduto})', 0, NULL, 'default', 2);
