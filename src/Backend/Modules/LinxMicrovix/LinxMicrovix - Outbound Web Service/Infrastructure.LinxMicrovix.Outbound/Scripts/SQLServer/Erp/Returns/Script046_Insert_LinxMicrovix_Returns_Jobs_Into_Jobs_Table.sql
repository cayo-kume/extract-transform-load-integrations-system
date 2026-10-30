INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (trocas e devoluções: 8622 - 8634)*/
(8122, 8622, 'sp_LinxMovimentoTrocas_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoTrocas raw e trusted', 3, NULL, 'default', 1),
(8123, 8623, 'sp_LinxMovimentoTrocafone_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoTrocafone raw e trusted', 3, NULL, 'default', 1),
(8124, 8624, 'sp_LinxMovimentoDevolucoesItens_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoDevolucoesItens raw e trusted', 3, NULL, 'default', 1),
(8125, 8625, 'sp_LinxMovimentoOrigemDevolucoes_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoOrigemDevolucoes raw e trusted', 3, NULL, 'default', 1),
(8126, 8626, 'sp_LinxMotivoDevolucao_Sync', 'Procedure de sincronização entre as tabelas LinxMotivoDevolucao raw e trusted', 3, NULL, 'default', 1),
(8127, 8627, 'sp_LinxTrocaUnificadaResumoVendas_Sync', 'Procedure de sincronização entre as tabelas LinxTrocaUnificadaResumoVendas raw e trusted', 3, NULL, 'default', 1),
(8128, 8628, 'sp_LinxTrocaUnificadaResumoVendasItens_Sync', 'Procedure de sincronização entre as tabelas LinxTrocaUnificadaResumoVendasItens raw e trusted', 3, NULL, 'default', 1),
(8129, 8629, 'sp_LinxTrocaUnificadaResumoBaixa_Sync', 'Procedure de sincronização entre as tabelas LinxTrocaUnificadaResumoBaixa raw e trusted', 3, NULL, 'default', 1),
(8130, 8630, 'sp_LinxTradeinParceiro_Sync', 'Procedure de sincronização entre as tabelas LinxTradeinParceiro raw e trusted', 3, NULL, 'default', 1),
(8131, 8631, 'sp_LinxDevolucaoRemanejoFabrica_Sync', 'Procedure de sincronização entre as tabelas LinxDevolucaoRemanejoFabrica raw e trusted', 3, NULL, 'default', 1),
(8132, 8632, 'sp_LinxDevolucaoRemanejoFabricaItem_Sync', 'Procedure de sincronização entre as tabelas LinxDevolucaoRemanejoFabricaItem raw e trusted', 3, NULL, 'default', 1),
(8133, 8633, 'sp_LinxDevolucaoRemanejoFabricaStatus_Sync', 'Procedure de sincronização entre as tabelas LinxDevolucaoRemanejoFabricaStatus raw e trusted', 3, NULL, 'default', 1),
(8134, 8634, 'sp_LinxDevolucaoRemanejoFabricaTipo_Sync', 'Procedure de sincronização entre as tabelas LinxDevolucaoRemanejoFabricaTipo raw e trusted', 3, NULL, 'default', 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8122, 'LinxMovimentoTrocas', 'Integração de dados da tabela LinxMovimentoTrocas, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8123, 'LinxMovimentoTrocafone', 'Integração de dados da tabela LinxMovimentoTrocafone, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8124, 'LinxMovimentoDevolucoesItens', 'Integração de dados da tabela LinxMovimentoDevolucoesItens, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8125, 'LinxMovimentoOrigemDevolucoes', 'Integração de dados da tabela LinxMovimentoOrigemDevolucoes, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8126, 'LinxMotivoDevolucao', 'Integração de dados da tabela LinxMotivoDevolucao, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8127, 'LinxTrocaUnificadaResumoVendas', 'Integração de dados da tabela LinxTrocaUnificadaResumoVendas, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8128, 'LinxTrocaUnificadaResumoVendasItens', 'Integração de dados da tabela LinxTrocaUnificadaResumoVendasItens, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8129, 'LinxTrocaUnificadaResumoBaixa', 'Integração de dados da tabela LinxTrocaUnificadaResumoBaixa, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8130, 'LinxTradeinParceiro', 'Integração de dados da tabela LinxTradeinParceiro, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8131, 'LinxDevolucaoRemanejoFabrica', 'Integração de dados da tabela LinxDevolucaoRemanejoFabrica, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8132, 'LinxDevolucaoRemanejoFabricaItem', 'Integração de dados da tabela LinxDevolucaoRemanejoFabricaItem, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8133, 'LinxDevolucaoRemanejoFabricaStatus', 'Integração de dados da tabela LinxDevolucaoRemanejoFabricaStatus, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8134, 'LinxDevolucaoRemanejoFabricaTipo', 'Integração de dados da tabela LinxDevolucaoRemanejoFabricaTipo, a cada 3 minutos', 3, NULL, 'default', 2),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(80, 8195, 'LinxMovimentoTrocasByDocOrigem', 'Integração de LinxMovimentoTrocas por docOrigem (GET LinxMovimentoTrocas/{docCompany}/{cnpjEmp}/{docOrigem})', 0, NULL, 'default', 2);
