INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 5500 - 5999*/
(5000, 5530, 'sp_AfterSaleEcommerces_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, NULL, 'default', 1),
(5000, 5531, 'sp_AfterSaleEcommercesMessagesPages_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, NULL, 'default', 1),
(5000, 5532, 'sp_AfterSaleEcommercesMessagesMails_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, NULL, 'default', 1),
(5000, 5533, 'sp_AfterSaleEcommercesReasons_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, NULL, 'default', 1),
(5000, 5534, 'sp_AfterSaleEcommercesSubReasons_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, NULL, 'default', 1),
(5000, 5535, 'sp_AfterSaleEcommercesStringValues_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, NULL, 'default', 1),

/*Integrações*/
/*Range 5000 - 5499*/
(50, 5000, 'AfterSaleEcommerces', 'Integração de dados das tabelas AfterSaleEcommerces, AfterSaleEcommercesMessagesPages, AfterSaleEcommercesMessagesMails, AfterSaleEcommercesReasons, AfterSaleEcommercesSubReasons e AfterSaleEcommercesStringValues', 3, NULL, 'default', 2);
