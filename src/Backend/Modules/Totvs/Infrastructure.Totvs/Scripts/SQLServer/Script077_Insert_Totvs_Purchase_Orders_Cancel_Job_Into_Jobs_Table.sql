INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Cancelamento de pedido de compra (irreversível) - executado pelo TotvsController, não é recorrente (IntervalTimeInMinutes = 0); só roda com
  Totvs:Writes:PurchaseOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §6.4, fase PO-4)*/
/*Range 13000 - 13499 (Purchase Order: 13180 - 13189)*/
(130, 13183, 'TotvsPostPurchaseOrdersCancel', 'Cancelamento de pedido de compra (POST PurchaseOrders/{docCompany}/Cancel)', 0, 2);
