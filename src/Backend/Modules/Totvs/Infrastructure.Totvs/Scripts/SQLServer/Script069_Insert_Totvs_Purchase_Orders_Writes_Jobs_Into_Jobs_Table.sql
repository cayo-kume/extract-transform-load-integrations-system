INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas da API Purchase Order - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:PurchaseOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §6.3, fase PO-2)*/
/*Range 13000 - 13499 (Purchase Order: 13180 - 13189)*/
(130, 13182, 'TotvsPostPurchaseOrdersChangeStatus', 'Troca da situação de um pedido de compra (POST PurchaseOrders/{docCompany}/ChangeStatus)', 0, 2);
