INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Integrações sob demanda - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7; sob demanda a partir de 13010)*/
(130, 13014, 'TotvsGetProductByCode', 'Integração da referência de um produto por código (GET Products/{docCompany}/{code})', 0, 2);
