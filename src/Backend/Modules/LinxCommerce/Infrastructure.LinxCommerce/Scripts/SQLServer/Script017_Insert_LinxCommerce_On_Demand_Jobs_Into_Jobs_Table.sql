INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Integrações sob demanda - executadas pelo LinxCommerceController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Range 7000 - 7499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §7; sob demanda a partir de 7010)*/
(70, 7010, 'LinxCommerceGetOrder', 'Integração de um pedido pelo OrderID (GET Orders/{docCompany}/{orderId})', 0, 2),
(70, 7011, 'LinxCommerceGetOrderByNumber', 'Integração de um pedido pelo número (GET Orders/{docCompany}/Number/{orderNumber})', 0, 2),
(70, 7012, 'LinxCommerceGetCustomer', 'Integração de um cliente pelo CustomerID (GET Customers/{docCompany}/{customerId})', 0, 2),
(70, 7013, 'LinxCommerceGetProduct', 'Integração de um produto pelo ProductID (GET Products/{docCompany}/{productId})', 0, 2),
(70, 7014, 'LinxCommerceGetSKU', 'Integração de um SKU pelo ProductID (GET Skus/{docCompany}/{productId})', 0, 2),
(70, 7015, 'LinxCommerceGetSalesRepresentative', 'Integração de um vendedor pelo SalesRepresentativeID (GET SalesRepresentatives/{docCompany}/{salesRepresentativeId})', 0, 2);
