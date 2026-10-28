INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas de documento da API Sales Order - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:SalesOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §7.4, fase SO-4)*/
/*Range 13000 - 13499 (Sales Order: 13190 - 13249)*/
(130, 13199, 'TotvsPostSalesOrderOrdersChangeStatus', 'Situação de um pedido de venda (POST SalesOrders/{docCompany}/Status)', 0, 2),
(130, 13200, 'TotvsPostSalesOrderHeader', 'Capa de um pedido de venda (POST SalesOrders/{docCompany}/Header)', 0, 2),
(130, 13205, 'TotvsPostSalesOrderItems', 'Inclusão de itens num pedido de venda (POST SalesOrders/{docCompany}/Items)', 0, 2),
(130, 13206, 'TotvsDeleteSalesOrderItems', 'Remoção de um item do pedido de venda (DELETE SalesOrders/{docCompany}/Items)', 0, 2),
(130, 13208, 'TotvsPostSalesOrderQuantityItems', 'Quantidade dos itens do pedido de venda (POST SalesOrders/{docCompany}/Items/Quantity)', 0, 2),
(130, 13211, 'TotvsPostSalesOrderRelationshipCounts', 'Contagens ligadas ao pedido de venda (POST SalesOrders/{docCompany}/Counts)', 0, 2),
(130, 13212, 'TotvsPostSalesOrderRelationshipBatchItems', 'Itens de lote ligados ao pedido de venda (POST SalesOrders/{docCompany}/BatchItems)', 0, 2),
(130, 13213, 'TotvsDeleteSalesOrderBatchItems', 'Remoção do vínculo de item de lote do pedido de venda (DELETE SalesOrders/{docCompany}/BatchItems)', 0, 2),
(130, 13214, 'TotvsPostSalesOrderSuggestBatchItemsNuance', 'Sugestão de itens de lote pelo nuance (POST SalesOrders/{docCompany}/BatchItems/SuggestNuance)', 0, 2);
