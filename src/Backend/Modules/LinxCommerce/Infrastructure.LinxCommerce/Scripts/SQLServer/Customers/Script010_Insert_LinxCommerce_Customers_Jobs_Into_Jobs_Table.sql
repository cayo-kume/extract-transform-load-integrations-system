INSERT INTO [auditing].[Catalog] (IdParent, IdJob, Title, [Description], IdType, IsOnDemand)
VALUES
/*Procedures de Sincronizações*/
/*Range 7500 - 7999 (clientes: 7512 - 7514)*/
(7002, 7512, 'sp_LinxCommerceCustomers_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceCustomers raw e trusted', 1, 0),
(7002, 7513, 'sp_LinxCommerceCustomersAddresses_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceCustomersAddresses raw e trusted', 1, 0),
(7002, 7514, 'sp_LinxCommerceCustomersGroups_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceCustomersGroups raw e trusted', 1, 0),

/*Integrações*/
/*Range 7000 - 7499 (ids reservados em src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §7)*/
(70, 7002, 'LinxCommerceSearchCustomer', 'Integração dos clientes alterados (LinxCommerceCustomers e filhas), a cada hora', 2, 0),

/*Integrações sob demanda - executadas pelo LinxCommerceController, não são recorrentes (IsOnDemand = 1)*/
(70, 7012, 'LinxCommerceGetCustomer', 'Integração de um cliente pelo CustomerID (GET Customers/{docCompany}/{customerId})', 2, 1);
