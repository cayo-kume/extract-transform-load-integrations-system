INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (similares e lotes: 13637 - 13641)*/
(13094, 13637, 'sp_TotvsProductsSimilar_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsSimilar raw e trusted', 120, 1),
(13094, 13638, 'sp_TotvsProductsSimilarItems_Sync', 'Procedure de sincronização entre as tabelas TotvsProductsSimilarItems raw e trusted', 120, 1),
(13095, 13639, 'sp_TotvsBatches_Sync', 'Procedure de sincronização entre as tabelas TotvsBatches raw e trusted', 120, 1),
(13095, 13640, 'sp_TotvsBatchesItems_Sync', 'Procedure de sincronização entre as tabelas TotvsBatchesItems raw e trusted', 120, 1),
(13095, 13641, 'sp_TotvsBatchesItemsAdditionalFields_Sync', 'Procedure de sincronização entre as tabelas TotvsBatchesItemsAdditionalFields raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (Product: 13080 - 13099, docs/PLANO-TOTVS-METODOS-FALTANTES.md); 13096 reservado (omni-changed-balances, PR2)*/
(130, 13094, 'TotvsGetSimilar', 'Integração dos produtos similares (TotvsProductsSimilar e itens), a cada 2 horas', 120, 2),
(130, 13095, 'TotvsPostSearchBatch', 'Integração dos lotes de todas as lojas (TotvsBatches, itens e campos adicionais), a cada 2 horas', 120, 2),
(130, 13097, 'TotvsPostSearchProductCodes', 'Conferência diária dos produtos de cada marca com as referências gravadas; recupera as referências dos que faltam', 1440, 2);
