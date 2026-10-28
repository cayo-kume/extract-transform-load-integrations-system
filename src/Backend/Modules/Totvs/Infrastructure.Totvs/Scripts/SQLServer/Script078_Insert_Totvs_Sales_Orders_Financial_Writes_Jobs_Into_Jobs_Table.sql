INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas da API Sales Order com efeito financeiro (pedido B2C, cancelamentos, preço dos itens, transação PDV) - executadas pelo TotvsController,
  não são recorrentes (IntervalTimeInMinutes = 0); só rodam com Totvs:Writes:SalesOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §7.4,
  fase SO-5)*/
/*Range 13000 - 13499 (Sales Order: 13190 - 13249)*/
(130, 13197, 'TotvsPostSalesOrderB2COrders', 'Inclusão de pedido de venda B2C (POST SalesOrders/{docCompany}/B2C)', 0, 2),
(130, 13198, 'TotvsPostSalesOrderOrdersCancel', 'Cancelamento de pedido de venda (POST SalesOrders/{docCompany}/Cancel)', 0, 2),
(130, 13204, 'TotvsPostSalesOrderPdvTransactions', 'Transação PDV ligada ao pedido de venda (POST SalesOrders/{docCompany}/PdvTransactions)', 0, 2),
(130, 13209, 'TotvsPostSalesOrderPriceItems', 'Preço dos itens do pedido de venda (POST SalesOrders/{docCompany}/Items/Price)', 0, 2),
(130, 13210, 'TotvsPostSalesOrderCancelItems', 'Cancelamento de quantidades dos itens do pedido de venda (POST SalesOrders/{docCompany}/Items/Cancel)', 0, 2);
