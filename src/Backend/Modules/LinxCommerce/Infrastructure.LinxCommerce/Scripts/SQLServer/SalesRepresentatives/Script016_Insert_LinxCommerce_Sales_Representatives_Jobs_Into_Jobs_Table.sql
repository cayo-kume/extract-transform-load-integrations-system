INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 7500 - 7999 (vendedores: 7522 - 7524)*/
(7005, 7522, 'sp_LinxCommerceSalesRepresentatives_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceSalesRepresentatives raw e trusted', 1440, 1),
(7005, 7523, 'sp_LinxCommerceSalesRepresentativesAddresses_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceSalesRepresentativesAddresses raw e trusted', 1440, 1),
(7005, 7524, 'sp_LinxCommerceSalesRepresentativesCustomers_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceSalesRepresentativesCustomers raw e trusted', 1440, 1),

/*Integrações*/
/*Range 7000 - 7499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §7)*/
(70, 7005, 'LinxCommerceSearchSalesRepresentative', 'Integração dos vendedores (LinxCommerceSalesRepresentatives e filhas), só leitura, uma vez por dia', 1440, 2),

/*Integrações sob demanda - executadas pelo LinxCommerceController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(70, 7015, 'LinxCommerceGetSalesRepresentative', 'Integração de um vendedor pelo SalesRepresentativeID (GET SalesRepresentatives/{docCompany}/{salesRepresentativeId})', 0, 2);
