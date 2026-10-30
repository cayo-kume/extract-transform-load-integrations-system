INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (documentos fiscais: 8583 - 8594)*/
(8083, 8583, 'sp_LinxXMLDocumentos_Sync', 'Procedure de sincronização entre as tabelas LinxXMLDocumentos raw e trusted', 3, NULL, 'default', 1),
(8084, 8584, 'sp_LinxNFeEvento_Sync', 'Procedure de sincronização entre as tabelas LinxNFeEvento raw e trusted', 3, NULL, 'default', 1),
(8085, 8585, 'sp_LinxNfse_Sync', 'Procedure de sincronização entre as tabelas LinxNfse raw e trusted', 3, NULL, 'default', 1),
(8086, 8586, 'sp_LinxEntradaNFe_Sync', 'Procedure de sincronização entre as tabelas LinxEntradaNFe raw e trusted', 3, NULL, 'default', 1),
(8087, 8587, 'sp_LinxEntradaNFeProdutos_Sync', 'Procedure de sincronização entre as tabelas LinxEntradaNFeProdutos raw e trusted', 3, NULL, 'default', 1),
(8088, 8588, 'sp_LinxConhecimentoFretes_Sync', 'Procedure de sincronização entre as tabelas LinxConhecimentoFretes raw e trusted', 3, NULL, 'default', 1),
(8089, 8589, 'sp_LinxFreteTipos_Sync', 'Procedure de sincronização entre as tabelas LinxFreteTipos raw e trusted', 3, NULL, 'default', 1),
(8090, 8590, 'sp_LinxReducoesZ_Sync', 'Procedure de sincronização entre as tabelas LinxReducoesZ raw e trusted', 3, NULL, 'default', 1),
(8091, 8591, 'sp_LinxECF_Sync', 'Procedure de sincronização entre as tabelas LinxECF raw e trusted', 3, NULL, 'default', 1),
(8092, 8592, 'sp_LinxECFFormasPagamento_Sync', 'Procedure de sincronização entre as tabelas LinxECFFormasPagamento raw e trusted', 3, NULL, 'default', 1),
(8093, 8593, 'sp_LinxNfceEstacao_Sync', 'Procedure de sincronização entre as tabelas LinxNfceEstacao raw e trusted', 3, NULL, 'default', 1),
(8094, 8594, 'sp_LinxSeries_Sync', 'Procedure de sincronização entre as tabelas LinxSeries raw e trusted', 3, NULL, 'default', 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8083, 'LinxXMLDocumentos', 'Integração de dados da tabela LinxXMLDocumentos, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8084, 'LinxNFeEvento', 'Integração de dados da tabela LinxNFeEvento, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8085, 'LinxNfse', 'Integração de dados da tabela LinxNfse, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8086, 'LinxEntradaNFe', 'Integração de dados da tabela LinxEntradaNFe, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8087, 'LinxEntradaNFeProdutos', 'Integração de dados da tabela LinxEntradaNFeProdutos, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8088, 'LinxConhecimentoFretes', 'Integração de dados da tabela LinxConhecimentoFretes, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8089, 'LinxFreteTipos', 'Integração de dados da tabela LinxFreteTipos, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8090, 'LinxReducoesZ', 'Integração de dados da tabela LinxReducoesZ, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8091, 'LinxECF', 'Integração de dados da tabela LinxECF, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8092, 'LinxECFFormasPagamento', 'Integração de dados da tabela LinxECFFormasPagamento, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8093, 'LinxNfceEstacao', 'Integração de dados da tabela LinxNfceEstacao, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8094, 'LinxSeries', 'Integração de dados da tabela LinxSeries, a cada 3 minutos', 3, NULL, 'default', 2),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(80, 8208, 'LinxXMLDocumentosByDocumento', 'Integração de LinxXMLDocumentos por documento (GET LinxXMLDocumentos/{docCompany}/{cnpjEmp}/{documento})', 0, NULL, 'default', 2);
