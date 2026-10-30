INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (financeiro: 8676 - 8690)*/
(8176, 8676, 'sp_LinxFaturas_Sync', 'Procedure de sincronização entre as tabelas LinxFaturas raw e trusted', 120, NULL, 'default', 1),
(8177, 8677, 'sp_LinxFaturasOrigem_Sync', 'Procedure de sincronização entre as tabelas LinxFaturasOrigem raw e trusted', 120, NULL, 'default', 1),
(8178, 8678, 'sp_LinxPortadores_Sync', 'Procedure de sincronização entre as tabelas LinxPortadores raw e trusted', 120, NULL, 'default', 1),
(8179, 8679, 'sp_LinxAntecipacoesFinanceiras_Sync', 'Procedure de sincronização entre as tabelas LinxAntecipacoesFinanceiras raw e trusted', 120, NULL, 'default', 1),
(8180, 8680, 'sp_LinxAntecipacoesFinanceirasPlanos_Sync', 'Procedure de sincronização entre as tabelas LinxAntecipacoesFinanceirasPlanos raw e trusted', 120, NULL, 'default', 1),
(8181, 8681, 'sp_LinxDesfazimentosAntecipacoes_Sync', 'Procedure de sincronização entre as tabelas LinxDesfazimentosAntecipacoes raw e trusted', 120, NULL, 'default', 1),
(8182, 8682, 'sp_LinxDesfazimentosAntecipacoesDocumento_Sync', 'Procedure de sincronização entre as tabelas LinxDesfazimentosAntecipacoesDocumento raw e trusted', 120, NULL, 'default', 1),
(8183, 8683, 'sp_LinxDesfazimentosAntecipacoesItens_Sync', 'Procedure de sincronização entre as tabelas LinxDesfazimentosAntecipacoesItens raw e trusted', 120, NULL, 'default', 1),
(8184, 8684, 'sp_LinxCategoriasFinanceiras_Sync', 'Procedure de sincronização entre as tabelas LinxCategoriasFinanceiras raw e trusted', 120, NULL, 'default', 1),
(8185, 8685, 'sp_LinxCentroCusto_Sync', 'Procedure de sincronização entre as tabelas LinxCentroCusto raw e trusted', 120, NULL, 'default', 1),
(8186, 8686, 'sp_LinxPlanoContas_Sync', 'Procedure de sincronização entre as tabelas LinxPlanoContas raw e trusted', 120, NULL, 'default', 1),
(8187, 8687, 'sp_LinxLancContabil_Sync', 'Procedure de sincronização entre as tabelas LinxLancContabil raw e trusted', 120, NULL, 'default', 1),
(8188, 8688, 'sp_LinxHistoricosContabeis_Sync', 'Procedure de sincronização entre as tabelas LinxHistoricosContabeis raw e trusted', 120, NULL, 'default', 1),
(8189, 8689, 'sp_LinxExtratoBancarioLancamentos_Sync', 'Procedure de sincronização entre as tabelas LinxExtratoBancarioLancamentos raw e trusted', 120, NULL, 'default', 1),
(8190, 8690, 'sp_LinxExtratoBancarioConciliacoes_Sync', 'Procedure de sincronização entre as tabelas LinxExtratoBancarioConciliacoes raw e trusted', 120, NULL, 'default', 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8176, 'LinxFaturas', 'Integração de dados da tabela LinxFaturas, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8177, 'LinxFaturasOrigem', 'Integração de dados da tabela LinxFaturasOrigem, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8178, 'LinxPortadores', 'Integração de dados da tabela LinxPortadores, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8179, 'LinxAntecipacoesFinanceiras', 'Integração de dados da tabela LinxAntecipacoesFinanceiras, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8180, 'LinxAntecipacoesFinanceirasPlanos', 'Integração de dados da tabela LinxAntecipacoesFinanceirasPlanos, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8181, 'LinxDesfazimentosAntecipacoes', 'Integração de dados da tabela LinxDesfazimentosAntecipacoes, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8182, 'LinxDesfazimentosAntecipacoesDocumento', 'Integração de dados da tabela LinxDesfazimentosAntecipacoesDocumento, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8183, 'LinxDesfazimentosAntecipacoesItens', 'Integração de dados da tabela LinxDesfazimentosAntecipacoesItens, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8184, 'LinxCategoriasFinanceiras', 'Integração de dados da tabela LinxCategoriasFinanceiras, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8185, 'LinxCentroCusto', 'Integração de dados da tabela LinxCentroCusto, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8186, 'LinxPlanoContas', 'Integração de dados da tabela LinxPlanoContas, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8187, 'LinxLancContabil', 'Integração de dados da tabela LinxLancContabil, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8188, 'LinxHistoricosContabeis', 'Integração de dados da tabela LinxHistoricosContabeis, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8189, 'LinxExtratoBancarioLancamentos', 'Integração de dados da tabela LinxExtratoBancarioLancamentos, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8190, 'LinxExtratoBancarioConciliacoes', 'Integração de dados da tabela LinxExtratoBancarioConciliacoes, a cada 2 horas', 120, NULL, 'default', 2);
