/****** Checagens da fase 2 (docs/datawarehouse, 9.2 e seção 11) ******/
--uma checagem nova (estoque) e duas da fase 1 refeitas para cobrir as origens e o fato novos. Nenhuma bloqueia.

/****** datawarehouse.vw_Check_StockNegativeOrNoCost: posição de estoque negativa ou sem custo ******/
--só a última posição carregada, por loja: quantas linhas têm saldo negativo e quantas têm saldo sem custo da margem.
--Alerta mensal (seção 11); o estoque negativo costuma ser ajuste pendente na loja.
create or alter view [datawarehouse].vw_Check_StockNegativeOrNoCost
as
select f.position_date, f.doc_company, f.branch_code,
       sum(iif(f.quantity < 0, 1, 0)) as negative_rows,
       sum(iif(f.quantity > 0 and f.unit_cost is null, 1, 0)) as no_cost_rows
  from [datawarehouse].FactMonthlyStock f
 where f.position_date = (select max(position_date) from [datawarehouse].FactMonthlyStock)
 group by f.position_date, f.doc_company, f.branch_code
having sum(iif(f.quantity < 0, 1, 0)) > 0
    or sum(iif(f.quantity > 0 and f.unit_cost is null, 1, 0)) > 0;
GO

/****** datawarehouse.vw_Check_UnknownMembers: mais de 5% de membro desconhecido ******/
--a da fase 1 mais FactMonthlyStock (loja e produto). FactTicket e FactRefund ficam fora: o cliente do atendimento nem sempre
--tem cadastro na Totvs, e o desconhecido ali é esperado.
create or alter view [datawarehouse].vw_Check_UnknownMembers
as
select fact_name, dimension, total_rows, unknown_rows, cast(100.0 * unknown_rows / total_rows as decimal(9,2)) as unknown_percent
  from (
	select 'FactSalesItem' as fact_name, d.dimension, count(*) as total_rows,
	       sum(iif(d.sk = replicate('0', 32), 1, 0)) as unknown_rows
	  from [datawarehouse].FactSalesItem f
	 cross apply (values ('store', f.store_sk), ('product', f.product_sk), ('customer', f.customer_sk), ('channel', f.channel_sk), ('operation', f.operation_sk)) d(dimension, sk)
	 group by d.dimension
	union all
	select 'FactOrderItem', d.dimension, count(*), sum(iif(d.sk = replicate('0', 32), 1, 0))
	  from [datawarehouse].FactOrderItem f
	 cross apply (values ('store', f.store_sk), ('product', f.product_sk), ('customer', f.customer_sk), ('channel', f.channel_sk)) d(dimension, sk)
	 group by d.dimension
	union all
	select 'FactMonthlyStock', d.dimension, count(*), sum(iif(d.sk = replicate('0', 32), 1, 0))
	  from [datawarehouse].FactMonthlyStock f
	 cross apply (values ('store', f.store_sk), ('product', f.product_sk)) d(dimension, sk)
	 where f.position_date = (select max(position_date) from [datawarehouse].FactMonthlyStock)
	 group by d.dimension
  ) x
 where 100.0 * unknown_rows / total_rows > 5;
GO

/****** datawarehouse.vw_Check_MissingChildren: filhas apagadas na origem acima de 0,5% ******/
--a da fase 1 mais as filhas do Movidesk com detecção de exclusão (Script014).
create or alter view [datawarehouse].vw_Check_MissingChildren
as
select table_name, total_rows, missing_rows, cast(100.0 * missing_rows / total_rows as decimal(9,2)) as missing_percent
  from (
	select 'TotvsInvoicesItems' as table_name, count(*) as total_rows, sum(iif(missing_since is null, 0, 1)) as missing_rows from [dw_landing].TotvsInvoicesItems
	union all select 'TotvsInvoicesItemsProducts', count(*), sum(iif(missing_since is null, 0, 1)) from [dw_landing].TotvsInvoicesItemsProducts
	union all select 'TotvsInvoicesItemsTaxes', count(*), sum(iif(missing_since is null, 0, 1)) from [dw_landing].TotvsInvoicesItemsTaxes
	union all select 'TotvsInvoicesPayments', count(*), sum(iif(missing_since is null, 0, 1)) from [dw_landing].TotvsInvoicesPayments
	union all select 'TotvsInvoicesSalesOrders', count(*), sum(iif(missing_since is null, 0, 1)) from [dw_landing].TotvsInvoicesSalesOrders
	union all select 'TotvsSaleOrdersProducts', count(*), sum(iif(missing_since is null, 0, 1)) from [dw_landing].TotvsSaleOrdersProducts
	union all select 'TotvsProductsSuppliers', count(*), sum(iif(missing_since is null, 0, 1)) from [dw_landing].TotvsProductsSuppliers
	union all select 'TotvsSkusReferenceCategories', count(*), sum(iif(missing_since is null, 0, 1)) from [dw_landing].TotvsSkusReferenceCategories
	union all select 'TotvsGridsSizes', count(*), sum(iif(missing_since is null, 0, 1)) from [dw_landing].TotvsGridsSizes
	union all select 'MovideskTicketsActions', count(*), sum(iif(missing_since is null, 0, 1)) from [dw_landing].MovideskTicketsActions
	union all select 'MovideskTicketsClients', count(*), sum(iif(missing_since is null, 0, 1)) from [dw_landing].MovideskTicketsClients
  ) x
 where total_rows > 0
   and 100.0 * missing_rows / total_rows > 0.5;
GO
