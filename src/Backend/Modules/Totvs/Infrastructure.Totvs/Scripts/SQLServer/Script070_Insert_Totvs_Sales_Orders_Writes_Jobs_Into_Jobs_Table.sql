INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas da API Sales Order - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:SalesOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §7.4, fase SO-3)*/
/*Range 13000 - 13499 (Sales Order: 13190 - 13249)*/
(130, 13201, 'TotvsPostSalesOrderAdditionalOrder', 'Dados adicionais de um pedido de venda (POST SalesOrders/{docCompany}/Additional)', 0, 2),
(130, 13202, 'TotvsPostSalesOrderShippingOrder', 'Transporte e frete de um pedido de venda (POST SalesOrders/{docCompany}/Shipping)', 0, 2),
(130, 13203, 'TotvsPostSalesOrderObservationsOrder', 'Inclusão de observações num pedido de venda (POST SalesOrders/{docCompany}/Observations)', 0, 2),
(130, 13207, 'TotvsPostSalesOrderAdditionalItems', 'Dados adicionais dos itens de um pedido de venda (POST SalesOrders/{docCompany}/Items/Additional)', 0, 2);
