INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (remessas: 8635 - 8641)*/
(8135, 8635, 'sp_LinxMovimentoRemessas_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoRemessas raw e trusted', 3, 1),
(8136, 8636, 'sp_LinxMovimentoRemessasItens_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoRemessasItens raw e trusted', 3, 1),
(8137, 8637, 'sp_LinxMovimentoRemessasAcertos_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoRemessasAcertos raw e trusted', 3, 1),
(8138, 8638, 'sp_LinxMovimentoRemessasAcertosItens_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoRemessasAcertosItens raw e trusted', 3, 1),
(8139, 8639, 'sp_LinxRemessasIdentificadores_Sync', 'Procedure de sincronização entre as tabelas LinxRemessasIdentificadores raw e trusted', 3, 1),
(8140, 8640, 'sp_LinxRemessasOperacoes_Sync', 'Procedure de sincronização entre as tabelas LinxRemessasOperacoes raw e trusted', 3, 1),
(8141, 8641, 'sp_LinxRemessasRetornoTipos_Sync', 'Procedure de sincronização entre as tabelas LinxRemessasRetornoTipos raw e trusted', 3, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8135, 'LinxMovimentoRemessas', 'Integração de dados da tabela LinxMovimentoRemessas, a cada 3 minutos', 3, 2),
(80, 8136, 'LinxMovimentoRemessasItens', 'Integração de dados da tabela LinxMovimentoRemessasItens, a cada 3 minutos', 3, 2),
(80, 8137, 'LinxMovimentoRemessasAcertos', 'Integração de dados da tabela LinxMovimentoRemessasAcertos, a cada 3 minutos', 3, 2),
(80, 8138, 'LinxMovimentoRemessasAcertosItens', 'Integração de dados da tabela LinxMovimentoRemessasAcertosItens, a cada 3 minutos', 3, 2),
(80, 8139, 'LinxRemessasIdentificadores', 'Integração de dados da tabela LinxRemessasIdentificadores, a cada 3 minutos', 3, 2),
(80, 8140, 'LinxRemessasOperacoes', 'Integração de dados da tabela LinxRemessasOperacoes, a cada 3 minutos', 3, 2),
(80, 8141, 'LinxRemessasRetornoTipos', 'Integração de dados da tabela LinxRemessasRetornoTipos, a cada 3 minutos', 3, 2);
