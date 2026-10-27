INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (ordens de serviço: 8655 - 8665)*/
(8155, 8655, 'sp_LinxOrdensServico_Sync', 'Procedure de sincronização entre as tabelas LinxOrdensServico raw e trusted', 3, 1),
(8156, 8656, 'sp_LinxOrdensServicoProdutos_Sync', 'Procedure de sincronização entre as tabelas LinxOrdensServicoProdutos raw e trusted', 3, 1),
(8157, 8657, 'sp_LinxServicos_Sync', 'Procedure de sincronização entre as tabelas LinxServicos raw e trusted', 3, 1),
(8158, 8658, 'sp_LinxServicosDetalhes_Sync', 'Procedure de sincronização entre as tabelas LinxServicosDetalhes raw e trusted', 3, 1),
(8159, 8659, 'sp_LinxServicoClientes_Sync', 'Procedure de sincronização entre as tabelas LinxServicoClientes raw e trusted', 3, 1),
(8160, 8660, 'sp_LinxServicoAgendaComandas_Sync', 'Procedure de sincronização entre as tabelas LinxServicoAgendaComandas raw e trusted', 3, 1),
(8161, 8661, 'sp_LinxServicoAgendaEventos_Sync', 'Procedure de sincronização entre as tabelas LinxServicoAgendaEventos raw e trusted', 3, 1),
(8162, 8662, 'sp_LinxOrdemServicoExternaDav_Sync', 'Procedure de sincronização entre as tabelas LinxOrdemServicoExternaDav raw e trusted', 3, 1),
(8163, 8663, 'sp_LinxMovimentoOrdemServicoExterna_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoOrdemServicoExterna raw e trusted', 3, 1),
(8164, 8664, 'sp_LinxValeOrdemServicoExterna_Sync', 'Procedure de sincronização entre as tabelas LinxValeOrdemServicoExterna raw e trusted', 3, 1),
(8165, 8665, 'sp_LinxPagamentoParcialDav_Sync', 'Procedure de sincronização entre as tabelas LinxPagamentoParcialDav raw e trusted', 3, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8155, 'LinxOrdensServico', 'Integração de dados da tabela LinxOrdensServico, a cada 3 minutos', 3, 2),
(80, 8156, 'LinxOrdensServicoProdutos', 'Integração de dados da tabela LinxOrdensServicoProdutos, a cada 3 minutos', 3, 2),
(80, 8157, 'LinxServicos', 'Integração de dados da tabela LinxServicos, a cada 3 minutos', 3, 2),
(80, 8158, 'LinxServicosDetalhes', 'Integração de dados da tabela LinxServicosDetalhes, a cada 3 minutos', 3, 2),
(80, 8159, 'LinxServicoClientes', 'Integração de dados da tabela LinxServicoClientes, a cada 3 minutos', 3, 2),
(80, 8160, 'LinxServicoAgendaComandas', 'Integração de dados da tabela LinxServicoAgendaComandas, a cada 3 minutos', 3, 2),
(80, 8161, 'LinxServicoAgendaEventos', 'Integração de dados da tabela LinxServicoAgendaEventos, a cada 3 minutos', 3, 2),
(80, 8162, 'LinxOrdemServicoExternaDav', 'Integração de dados da tabela LinxOrdemServicoExternaDav, a cada 3 minutos', 3, 2),
(80, 8163, 'LinxMovimentoOrdemServicoExterna', 'Integração de dados da tabela LinxMovimentoOrdemServicoExterna, a cada 3 minutos', 3, 2),
(80, 8164, 'LinxValeOrdemServicoExterna', 'Integração de dados da tabela LinxValeOrdemServicoExterna, a cada 3 minutos', 3, 2),
(80, 8165, 'LinxPagamentoParcialDav', 'Integração de dados da tabela LinxPagamentoParcialDav, a cada 3 minutos', 3, 2);
