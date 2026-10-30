INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (pagamentos e caixa: 8610 - 8618)*/
(8110, 8610, 'sp_LinxMovimentoCartoes_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoCartoes raw e trusted', 3, NULL, 'default', 1),
(8111, 8611, 'sp_LinxMovimentoPlanos_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoPlanos raw e trusted', 3, NULL, 'default', 1),
(8112, 8612, 'sp_LinxPlanos_Sync', 'Procedure de sincronização entre as tabelas LinxPlanos raw e trusted', 3, NULL, 'default', 1),
(8113, 8613, 'sp_LinxPlanosParcelas_Sync', 'Procedure de sincronização entre as tabelas LinxPlanosParcelas raw e trusted', 3, NULL, 'default', 1),
(8114, 8614, 'sp_LinxPlanosBandeiras_Sync', 'Procedure de sincronização entre as tabelas LinxPlanosBandeiras raw e trusted', 3, NULL, 'default', 1),
(8115, 8615, 'sp_LinxMovimentoGiftCard_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoGiftCard raw e trusted', 3, NULL, 'default', 1),
(8116, 8616, 'sp_LinxValesComprasEnviadosAPI_Sync', 'Procedure de sincronização entre as tabelas LinxValesComprasEnviadosAPI raw e trusted', 3, NULL, 'default', 1),
(8117, 8617, 'sp_LinxSangriaSuprimentos_Sync', 'Procedure de sincronização entre as tabelas LinxSangriaSuprimentos raw e trusted', 3, NULL, 'default', 1),
(8118, 8618, 'sp_LinxFechamentoCaixa_Sync', 'Procedure de sincronização entre as tabelas LinxFechamentoCaixa raw e trusted', 3, NULL, 'default', 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8110, 'LinxMovimentoCartoes', 'Integração de dados da tabela LinxMovimentoCartoes, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8111, 'LinxMovimentoPlanos', 'Integração de dados da tabela LinxMovimentoPlanos, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8112, 'LinxPlanos', 'Integração de dados da tabela LinxPlanos, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8113, 'LinxPlanosParcelas', 'Integração de dados da tabela LinxPlanosParcelas, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8114, 'LinxPlanosBandeiras', 'Integração de dados da tabela LinxPlanosBandeiras, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8115, 'LinxMovimentoGiftCard', 'Integração de dados da tabela LinxMovimentoGiftCard, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8116, 'LinxValesComprasEnviadosAPI', 'Integração de dados da tabela LinxValesComprasEnviadosAPI, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8117, 'LinxSangriaSuprimentos', 'Integração de dados da tabela LinxSangriaSuprimentos, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8118, 'LinxFechamentoCaixa', 'Integração de dados da tabela LinxFechamentoCaixa, a cada 3 minutos', 3, NULL, 'default', 2),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(80, 8193, 'LinxMovimentoCartoesByIdentificador', 'Integração de LinxMovimentoCartoes por identificador (GET LinxMovimentoCartoes/{docCompany}/{cnpjEmp}/{identificador})', 0, NULL, 'default', 2),
(80, 8194, 'LinxMovimentoPlanosByIdentificador', 'Integração de LinxMovimentoPlanos por identificador (GET LinxMovimentoPlanos/{docCompany}/{cnpjEmp}/{identificador})', 0, NULL, 'default', 2),
(80, 8196, 'LinxPlanosByPlano', 'Integração de LinxPlanos por plano (GET LinxPlanos/{docCompany}/{plano})', 0, NULL, 'default', 2);
