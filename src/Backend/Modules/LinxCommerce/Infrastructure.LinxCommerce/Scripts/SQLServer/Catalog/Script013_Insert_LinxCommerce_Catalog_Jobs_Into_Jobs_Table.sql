INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 7500 - 7999 (produtos: 7515 - 7518; SKUs: 7519 - 7521)*/
(7003, 7515, 'sp_LinxCommerceProducts_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceProducts raw e trusted', 60, NULL, 'default', 1),
(7003, 7516, 'sp_LinxCommerceProductsMetadataValues_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceProductsMetadataValues raw e trusted', 60, NULL, 'default', 1),
(7003, 7517, 'sp_LinxCommerceProductsMedias_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceProductsMedias raw e trusted', 60, NULL, 'default', 1),
(7003, 7518, 'sp_LinxCommerceProductsMediaAssociations_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceProductsMediaAssociations raw e trusted', 60, NULL, 'default', 1),
(7004, 7519, 'sp_LinxCommerceSkus_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceSkus raw e trusted', 60, NULL, 'default', 1),
(7004, 7520, 'sp_LinxCommerceSkusMetadataValues_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceSkusMetadataValues raw e trusted', 60, NULL, 'default', 1),
(7004, 7521, 'sp_LinxCommerceSkusParentRelations_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceSkusParentRelations raw e trusted', 60, NULL, 'default', 1),

/*Integrações*/
/*Range 7000 - 7499 (ids reservados em src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §7)*/
(70, 7003, 'LinxCommerceSearchProduct', 'Integração dos produtos alterados (LinxCommerceProducts e filhas), a cada hora', 60, NULL, 'default', 2),
(70, 7004, 'LinxCommerceSearchSKU', 'Integração dos SKUs alterados (LinxCommerceSkus e filhas), a cada hora', 60, NULL, 'default', 2),

/*Integrações sob demanda - executadas pelo LinxCommerceController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(70, 7013, 'LinxCommerceGetProduct', 'Integração de um produto pelo ProductID (GET Products/{docCompany}/{productId})', 0, NULL, 'default', 2),
(70, 7014, 'LinxCommerceGetSKU', 'Integração de um SKU pelo ProductID (GET Skus/{docCompany}/{productId})', 0, NULL, 'default', 2);
