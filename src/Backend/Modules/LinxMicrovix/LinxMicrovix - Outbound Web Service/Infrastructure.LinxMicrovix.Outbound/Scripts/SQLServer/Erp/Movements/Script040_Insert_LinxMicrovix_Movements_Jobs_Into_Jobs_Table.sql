INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (movimento: 8595 - 8606)*/
(8095, 8595, 'sp_LinxMovimento_Sync', 'Procedure de sincronização entre as tabelas LinxMovimento raw e trusted', 3, 1),
(8096, 8596, 'sp_LinxMovimentoPrincipal_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoPrincipal raw e trusted', 3, 1),
(8097, 8597, 'sp_LinxMovimentoExtensao_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoExtensao raw e trusted', 3, 1),
(8098, 8598, 'sp_LinxMovimentoCamposAdicionais_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoCamposAdicionais raw e trusted', 3, 1),
(8099, 8599, 'sp_LinxMovimentoObservacoes_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoObservacoes raw e trusted', 3, 1),
(8100, 8600, 'sp_LinxMovimentoObservacaoCF_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoObservacaoCF raw e trusted', 3, 1),
(8101, 8601, 'sp_LinxMovimentoIndicacoes_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoIndicacoes raw e trusted', 3, 1),
(8102, 8602, 'sp_LinxMovimentoSerial_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoSerial raw e trusted', 3, 1),
(8103, 8603, 'sp_LinxSerialVenda_Sync', 'Procedure de sincronização entre as tabelas LinxSerialVenda raw e trusted', 3, 1),
(8104, 8604, 'sp_LinxMovimentoVendaConjugada_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoVendaConjugada raw e trusted', 3, 1),
(8105, 8605, 'sp_LinxPerguntaVenda_Sync', 'Procedure de sincronização entre as tabelas LinxPerguntaVenda raw e trusted', 3, 1),
(8106, 8606, 'sp_LinxRespostaVenda_Sync', 'Procedure de sincronização entre as tabelas LinxRespostaVenda raw e trusted', 3, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8095, 'LinxMovimento', 'Integração de dados da tabela LinxMovimento, a cada 3 minutos', 3, 2),
(80, 8096, 'LinxMovimentoPrincipal', 'Integração de dados da tabela LinxMovimentoPrincipal, a cada 3 minutos', 3, 2),
(80, 8097, 'LinxMovimentoExtensao', 'Integração de dados da tabela LinxMovimentoExtensao, a cada 3 minutos', 3, 2),
(80, 8098, 'LinxMovimentoCamposAdicionais', 'Integração de dados da tabela LinxMovimentoCamposAdicionais, a cada 3 minutos', 3, 2),
(80, 8099, 'LinxMovimentoObservacoes', 'Integração de dados da tabela LinxMovimentoObservacoes, a cada 3 minutos', 3, 2),
(80, 8100, 'LinxMovimentoObservacaoCF', 'Integração de dados da tabela LinxMovimentoObservacaoCF, a cada 3 minutos', 3, 2),
(80, 8101, 'LinxMovimentoIndicacoes', 'Integração de dados da tabela LinxMovimentoIndicacoes, a cada 3 minutos', 3, 2),
(80, 8102, 'LinxMovimentoSerial', 'Integração de dados da tabela LinxMovimentoSerial, a cada 3 minutos', 3, 2),
(80, 8103, 'LinxSerialVenda', 'Integração de dados da tabela LinxSerialVenda, a cada 3 minutos', 3, 2),
(80, 8104, 'LinxMovimentoVendaConjugada', 'Integração de dados da tabela LinxMovimentoVendaConjugada, a cada 3 minutos', 3, 2),
(80, 8105, 'LinxPerguntaVenda', 'Integração de dados da tabela LinxPerguntaVenda, a cada 3 minutos', 3, 2),
(80, 8106, 'LinxRespostaVenda', 'Integração de dados da tabela LinxRespostaVenda, a cada 3 minutos', 3, 2),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(80, 8192, 'LinxMovimentoByDocumento', 'Integração de LinxMovimento por documento (GET LinxMovimento/{docCompany}/{cnpjEmp}/{documento})', 0, 2);
