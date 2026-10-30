INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 7500 - 7999 (pedidos: 7500 - 7510; status de pedido: 7511)*/
(7000, 7500, 'sp_LinxCommerceOrders_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrders raw e trusted', 3, 1),
(7000, 7501, 'sp_LinxCommerceOrdersItems_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrdersItems raw e trusted', 3, 1),
(7000, 7502, 'sp_LinxCommerceOrdersTags_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrdersTags raw e trusted', 3, 1),
(7000, 7503, 'sp_LinxCommerceOrdersAddresses_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrdersAddresses raw e trusted', 3, 1),
(7000, 7504, 'sp_LinxCommerceOrdersPaymentMethods_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrdersPaymentMethods raw e trusted', 3, 1),
(7000, 7505, 'sp_LinxCommerceOrdersDeliveryMethods_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrdersDeliveryMethods raw e trusted', 3, 1),
(7000, 7506, 'sp_LinxCommerceOrdersDiscounts_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrdersDiscounts raw e trusted', 3, 1),
(7000, 7507, 'sp_LinxCommerceOrdersShipments_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrdersShipments raw e trusted', 3, 1),
(7000, 7508, 'sp_LinxCommerceOrdersShipmentsPackages_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrdersShipmentsPackages raw e trusted', 3, 1),
(7000, 7509, 'sp_LinxCommerceOrdersInvoices_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrdersInvoices raw e trusted', 3, 1),
(7000, 7510, 'sp_LinxCommerceOrdersProperties_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrdersProperties raw e trusted', 3, 1),
(7001, 7511, 'sp_LinxCommerceOrderStatus_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceOrderStatus raw e trusted', 1440, 1),

/*Integrações*/
/*Range 7000 - 7499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §7)*/
(70, 7000, 'LinxCommerceSearchOrders', 'Integração dos pedidos alterados (LinxCommerceOrders e filhas), a cada 3 minutos', 3, 2),
(70, 7001, 'LinxCommerceSearchOrderStatus', 'Integração dos status de pedido (LinxCommerceOrderStatus), uma vez por dia', 1440, 2),

/*Integrações sob demanda - executadas pelo LinxCommerceController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(70, 7010, 'LinxCommerceGetOrder', 'Integração de um pedido pelo OrderID (GET Orders/{docCompany}/{orderId})', 0, 2),
(70, 7011, 'LinxCommerceGetOrderByNumber', 'Integração de um pedido pelo número (GET Orders/{docCompany}/Number/{orderNumber})', 0, 2);
