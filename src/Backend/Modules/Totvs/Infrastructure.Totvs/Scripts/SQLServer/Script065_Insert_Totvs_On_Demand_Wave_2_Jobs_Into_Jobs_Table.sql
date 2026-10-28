INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Leituras sob demanda da onda 2 - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0). Só o
  GetPurchaseOrderByKey grava (como o GetSalesOrderByKey); as outras devolvem o JSON e registram a chamada em totvs.TotvsRequests
  (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Person: 13030 - 13069; Product: 13100 - 13105; Purchase Order: 13185; Sales Order: 13190 - 13249)*/
(130, 13036, 'TotvsGetBranches', 'Dados de uma loja pelo código ou CNPJ (GET Branches/{docCompany}/{branchId})', 0, 2),
(130, 13037, 'TotvsGetPersonStatistics', 'Estatísticas de compra e crédito de um cliente (GET PersonStatistics/{docCompany})', 0, 2);
