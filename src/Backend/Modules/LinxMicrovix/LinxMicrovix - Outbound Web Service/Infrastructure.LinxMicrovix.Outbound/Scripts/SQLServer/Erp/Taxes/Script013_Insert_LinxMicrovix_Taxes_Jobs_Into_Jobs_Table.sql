INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (tabelas fiscais: 8563 - 8578)*/
(8063, 8563, 'sp_LinxNaturezaOperacao_Sync', 'Procedure de sincronização entre as tabelas LinxNaturezaOperacao raw e trusted', 1440, 1),
(8064, 8564, 'sp_LinxCfopFiscal_Sync', 'Procedure de sincronização entre as tabelas LinxCfopFiscal raw e trusted', 1440, 1),
(8065, 8565, 'sp_LinxCstIcmsFiscal_Sync', 'Procedure de sincronização entre as tabelas LinxCstIcmsFiscal raw e trusted', 1440, 1),
(8066, 8566, 'sp_LinxCstIpiFiscal_Sync', 'Procedure de sincronização entre as tabelas LinxCstIpiFiscal raw e trusted', 1440, 1),
(8067, 8567, 'sp_LinxCstPisFiscal_Sync', 'Procedure de sincronização entre as tabelas LinxCstPisFiscal raw e trusted', 1440, 1),
(8068, 8568, 'sp_LinxCstCofinsFiscal_Sync', 'Procedure de sincronização entre as tabelas LinxCstCofinsFiscal raw e trusted', 1440, 1),
(8069, 8569, 'sp_LinxCsosnFiscal_Sync', 'Procedure de sincronização entre as tabelas LinxCsosnFiscal raw e trusted', 1440, 1),
(8070, 8570, 'sp_LinxClasseFiscal_Sync', 'Procedure de sincronização entre as tabelas LinxClasseFiscal raw e trusted', 1440, 1),
(8071, 8571, 'sp_LinxCest_Sync', 'Procedure de sincronização entre as tabelas LinxCest raw e trusted', 1440, 1),
(8072, 8572, 'sp_LinxNcm_Sync', 'Procedure de sincronização entre as tabelas LinxNcm raw e trusted', 1440, 1),
(8073, 8573, 'sp_LinxEnquadramentoIpi_Sync', 'Procedure de sincronização entre as tabelas LinxEnquadramentoIpi raw e trusted', 1440, 1),
(8074, 8574, 'sp_LinxMotivosDesoneracaoIcms_Sync', 'Procedure de sincronização entre as tabelas LinxMotivosDesoneracaoIcms raw e trusted', 1440, 1),
(8075, 8575, 'sp_LinxSpedTipoItem_Sync', 'Procedure de sincronização entre as tabelas LinxSpedTipoItem raw e trusted', 1440, 1),
(8076, 8576, 'sp_LinxSpedTipoBaseCredito_Sync', 'Procedure de sincronização entre as tabelas LinxSpedTipoBaseCredito raw e trusted', 1440, 1),
(8077, 8577, 'sp_LinxRotinaOrigem_Sync', 'Procedure de sincronização entre as tabelas LinxRotinaOrigem raw e trusted', 1440, 1),
(8078, 8578, 'sp_LinxNfeIbgeMunicipio_Sync', 'Procedure de sincronização entre as tabelas LinxNfeIbgeMunicipio raw e trusted', 1440, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8063, 'LinxNaturezaOperacao', 'Integração de dados da tabela LinxNaturezaOperacao, uma vez por dia', 1440, 2),
(80, 8064, 'LinxCfopFiscal', 'Integração de dados da tabela LinxCfopFiscal, uma vez por dia', 1440, 2),
(80, 8065, 'LinxCstIcmsFiscal', 'Integração de dados da tabela LinxCstIcmsFiscal, uma vez por dia', 1440, 2),
(80, 8066, 'LinxCstIpiFiscal', 'Integração de dados da tabela LinxCstIpiFiscal, uma vez por dia', 1440, 2),
(80, 8067, 'LinxCstPisFiscal', 'Integração de dados da tabela LinxCstPisFiscal, uma vez por dia', 1440, 2),
(80, 8068, 'LinxCstCofinsFiscal', 'Integração de dados da tabela LinxCstCofinsFiscal, uma vez por dia', 1440, 2),
(80, 8069, 'LinxCsosnFiscal', 'Integração de dados da tabela LinxCsosnFiscal, uma vez por dia', 1440, 2),
(80, 8070, 'LinxClasseFiscal', 'Integração de dados da tabela LinxClasseFiscal, uma vez por dia', 1440, 2),
(80, 8071, 'LinxCest', 'Integração de dados da tabela LinxCest, uma vez por dia', 1440, 2),
(80, 8072, 'LinxNcm', 'Integração de dados da tabela LinxNcm, uma vez por dia', 1440, 2),
(80, 8073, 'LinxEnquadramentoIpi', 'Integração de dados da tabela LinxEnquadramentoIpi, uma vez por dia', 1440, 2),
(80, 8074, 'LinxMotivosDesoneracaoIcms', 'Integração de dados da tabela LinxMotivosDesoneracaoIcms, uma vez por dia', 1440, 2),
(80, 8075, 'LinxSpedTipoItem', 'Integração de dados da tabela LinxSpedTipoItem, uma vez por dia', 1440, 2),
(80, 8076, 'LinxSpedTipoBaseCredito', 'Integração de dados da tabela LinxSpedTipoBaseCredito, uma vez por dia', 1440, 2),
(80, 8077, 'LinxRotinaOrigem', 'Integração de dados da tabela LinxRotinaOrigem, uma vez por dia', 1440, 2),
(80, 8078, 'LinxNfeIbgeMunicipio', 'Integração de dados da tabela LinxNfeIbgeMunicipio, uma vez por dia', 1440, 2),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(80, 8197, 'LinxNaturezaOperacaoByCodNaturezaOperacao', 'Integração de LinxNaturezaOperacao por codNaturezaOperacao (GET LinxNaturezaOperacao/{docCompany}/{codNaturezaOperacao})', 0, 2);
