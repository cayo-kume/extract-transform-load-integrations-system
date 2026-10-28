INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Leituras sob demanda da onda 2 - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0). Só o
  GetPurchaseOrderByKey grava (como o GetSalesOrderByKey); as outras devolvem o JSON e registram a chamada em totvs.TotvsRequests
  (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Person: 13030 - 13069; Product: 13100 - 13105; Purchase Order: 13185; Sales Order: 13190 - 13249)*/
(130, 13036, 'TotvsGetBranches', 'Dados de uma loja pelo código ou CNPJ (GET Branches/{docCompany}/{branchId})', 0, 2),
(130, 13037, 'TotvsGetPersonStatistics', 'Estatísticas de compra e crédito de um cliente (GET PersonStatistics/{docCompany})', 0, 2),
(130, 13038, 'TotvsPostListBalanceBonus', 'Saldo de bônus de uma pessoa nas lojas (POST ListBalanceBonus/{docCompany})', 0, 2),
(130, 13100, 'TotvsGetProducts', 'Produto por código, pack, barras, RFID ou item de lote (GET Products/{docCompany}/Codes/{code})', 0, 2),
(130, 13101, 'TotvsGetKardexMovement', 'Kardex de um produto numa loja (GET Products/{docCompany}/Kardex)', 0, 2),
(130, 13102, 'TotvsGetGridFrequency', 'Grade de frequência de um produto (GET Products/{docCompany}/{productCode}/GridFrequency)', 0, 2),
(130, 13103, 'TotvsGetMeasurementConversions', 'Conversões de unidade de um produto ou grupo (GET Products/{docCompany}/MeasurementConversions)', 0, 2),
(130, 13104, 'TotvsGetCompositionProduct', 'Composição por produto (GET Products/{docCompany}/CompositionProduct)', 0, 2),
(130, 13105, 'TotvsGetCompositionGroupProduct', 'Composição por grupo (GET Products/{docCompany}/CompositionGroupProduct)', 0, 2),
(130, 13185, 'TotvsGetPurchaseOrderByKey', 'Integração de um pedido de compra por loja e número (GET PurchaseOrders/{docCompany}/{branchCode}/{orderCode})', 0, 2);
