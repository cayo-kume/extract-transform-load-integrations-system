INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas de documento da API Purchase Order - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:PurchaseOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §6.1, §6.2 e §6.5, fase PO-3)*/
/*Range 13000 - 13499 (Purchase Order: 13180 - 13189)*/
(130, 13180, 'TotvsPostPurchaseOrders', 'Inclusão de pedido de compra (POST PurchaseOrders/{docCompany})', 0, 2),
(130, 13181, 'TotvsPostPurchaseOrdersQuantityItems', 'Quantidade pedida dos itens de um pedido de compra (POST PurchaseOrders/{docCompany}/QuantityItems)', 0, 2),
(130, 13184, 'TotvsPostPurchaseOrderBatchLink', 'Vínculo de item de lote com pedido de compra (POST PurchaseOrders/{docCompany}/BatchLink)', 0, 2);
