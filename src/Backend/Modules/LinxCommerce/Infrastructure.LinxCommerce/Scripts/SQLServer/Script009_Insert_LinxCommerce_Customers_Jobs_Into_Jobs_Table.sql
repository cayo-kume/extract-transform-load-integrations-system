INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 7500 - 7999 (clientes: 7512 - 7514)*/
(7002, 7512, 'sp_LinxCommerceCustomers_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceCustomers raw e trusted', 60, 1),
(7002, 7513, 'sp_LinxCommerceCustomersAddresses_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceCustomersAddresses raw e trusted', 60, 1),
(7002, 7514, 'sp_LinxCommerceCustomersGroups_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceCustomersGroups raw e trusted', 60, 1),

/*Integrações*/
/*Range 7000 - 7499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §7)*/
(70, 7002, 'LinxCommerceSearchCustomer', 'Integração dos clientes alterados (LinxCommerceCustomers e filhas), a cada hora', 60, 2);
