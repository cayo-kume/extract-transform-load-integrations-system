INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Integrações sob demanda - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7; sob demanda a partir de 13010)*/
(130, 13010, 'TotvsGetCustomerByCode', 'Integração de uma pessoa física por código (GET Customers/{docCompany}/{code})', 0, 2),
(130, 13011, 'TotvsGetLegalEntityByCode', 'Integração de uma pessoa jurídica por código (GET LegalEntities/{docCompany}/{code})', 0, 2);
