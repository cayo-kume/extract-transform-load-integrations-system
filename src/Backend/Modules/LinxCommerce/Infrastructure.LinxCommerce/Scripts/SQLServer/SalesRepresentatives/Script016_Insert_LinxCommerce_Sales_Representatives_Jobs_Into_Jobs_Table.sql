INSERT INTO [auditing].[Catalog] (IdParent, IdJob, Title, [Description], IdType, IsOnDemand)
VALUES
/*Procedures de Sincronizações*/
/*Range 7500 - 7999 (vendedores: 7522 - 7524)*/
(7005, 7522, 'sp_LinxCommerceSalesRepresentatives_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceSalesRepresentatives raw e trusted', 1, 0),
(7005, 7523, 'sp_LinxCommerceSalesRepresentativesAddresses_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceSalesRepresentativesAddresses raw e trusted', 1, 0),
(7005, 7524, 'sp_LinxCommerceSalesRepresentativesCustomers_Sync', 'Procedure de sincronização entre as tabelas LinxCommerceSalesRepresentativesCustomers raw e trusted', 1, 0),

/*Integrações*/
/*Range 7000 - 7499 (ids reservados em src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §7)*/
(70, 7005, 'LinxCommerceSearchSalesRepresentative', 'Integração dos vendedores (LinxCommerceSalesRepresentatives e filhas), só leitura, uma vez por dia', 2, 0),

/*Integrações sob demanda - executadas pelo LinxCommerceController, não são recorrentes (IsOnDemand = 1)*/
(70, 7015, 'LinxCommerceGetSalesRepresentative', 'Integração de um vendedor pelo SalesRepresentativeID (GET SalesRepresentatives/{docCompany}/{salesRepresentativeId})', 2, 1);
