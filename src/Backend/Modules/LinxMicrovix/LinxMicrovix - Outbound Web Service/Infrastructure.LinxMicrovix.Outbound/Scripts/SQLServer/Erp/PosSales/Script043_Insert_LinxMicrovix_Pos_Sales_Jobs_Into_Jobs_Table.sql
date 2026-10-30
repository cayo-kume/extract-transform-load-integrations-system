INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (vendas pos temporárias: 8607 - 8609)*/
(8107, 8607, 'sp_LinxVendasPosTmp_Sync', 'Procedure de sincronização entre as tabelas LinxVendasPosTmp raw e trusted', 3, 1),
(8108, 8608, 'sp_LinxVendasPosProdutosTmp_Sync', 'Procedure de sincronização entre as tabelas LinxVendasPosProdutosTmp raw e trusted', 3, 1),
(8109, 8609, 'sp_LinxVendasPosAgrupamentoTmp_Sync', 'Procedure de sincronização entre as tabelas LinxVendasPosAgrupamentoTmp raw e trusted', 3, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8107, 'LinxVendasPosTmp', 'Integração de dados da tabela LinxVendasPosTmp, a cada 3 minutos', 3, 2),
(80, 8108, 'LinxVendasPosProdutosTmp', 'Integração de dados da tabela LinxVendasPosProdutosTmp, a cada 3 minutos', 3, 2),
(80, 8109, 'LinxVendasPosAgrupamentoTmp', 'Integração de dados da tabela LinxVendasPosAgrupamentoTmp, a cada 3 minutos', 3, 2);
