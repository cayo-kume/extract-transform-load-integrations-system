INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (fidelidade: 8619 - 8621)*/
(8119, 8619, 'sp_LinxFidelidade_Sync', 'Procedure de sincronização entre as tabelas LinxFidelidade raw e trusted', 3, 1),
(8120, 8620, 'sp_LinxMovimentoReshop_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoReshop raw e trusted', 3, 1),
(8121, 8621, 'sp_LinxMovimentoReshopItens_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoReshopItens raw e trusted', 3, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8119, 'LinxFidelidade', 'Integração de dados da tabela LinxFidelidade, a cada 3 minutos', 3, 2),
(80, 8120, 'LinxMovimentoReshop', 'Integração de dados da tabela LinxMovimentoReshop, a cada 3 minutos', 3, 2),
(80, 8121, 'LinxMovimentoReshopItens', 'Integração de dados da tabela LinxMovimentoReshopItens, a cada 3 minutos', 3, 2);
