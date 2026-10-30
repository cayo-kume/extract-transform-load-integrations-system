/****** Checagens da fase 1 (docs/datawarehouse, 9.2, 9.3 e seção 11) ******/
--cada view devolve só as linhas com erro. O job conta as linhas, grava em dw_control.CheckResult e para se a checagem
--bloqueia. O nome da view diz o que ela confere; o limite de cada uma está na seção 11.

/****** grão do fato de venda: a PK já garante; a view fica para a carga do legado e para as reconstruções (bloqueia) ******/
create or alter view [datawarehouse].vw_Check_FactSalesItemGrain
as
select source_erp, source_key, count(*) as rows_found
  from [datawarehouse].FactSalesItem
 group by source_erp, source_key
having count(*) > 1;
GO

/****** Checagens da fase 4 (docs/datawarehouse, 9.2 e seção 11) ******/
/****** datawarehouse.vw_Check_UnknownMembers: mais de 5% de membro desconhecido ******/
--a das fases anteriores mais os fatos de compras, contas a pagar e recepção de loja. O cliente da recepção fica fora: quem
--entra na loja e não compra nem sempre tem cadastro.
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
	union all
	select 'FactPurchaseOrderItem', d.dimension, count(*), sum(iif(d.sk = replicate('0', 32), 1, 0))
	  from [datawarehouse].FactPurchaseOrderItem f
	 cross apply (values ('store', f.store_sk), ('product', f.product_sk), ('supplier', f.supplier_sk)) d(dimension, sk)
	 group by d.dimension
	union all
	select 'FactAccountsPayable', d.dimension, count(*), sum(iif(d.sk = replicate('0', 32), 1, 0))
	  from [datawarehouse].FactAccountsPayable f
	 cross apply (values ('store', f.store_sk), ('supplier', f.supplier_sk)) d(dimension, sk)
	 group by d.dimension
	union all
	select 'FactStoreReception', d.dimension, count(*), sum(iif(d.sk = replicate('0', 32), 1, 0))
	  from [datawarehouse].FactStoreReception f
	 cross apply (values ('store', f.store_sk), ('seller', f.seller_sk)) d(dimension, sk)
	 group by d.dimension
  ) x
 where 100.0 * unknown_rows / total_rows > 5;
GO

/****** soma dos produtos de um item = valor do item (7.2; alerta acima de 0,1% dos itens) ******/
create or alter view [datawarehouse].vw_Check_ItemProducts
as
select i.doc_company, i.branchCode, i.invoiceSequence, i.invoiceDate, i.[sequence], i.netValue as item_net_value, p.products_net_value
  from [dw_landing].TotvsInvoicesItems i
  join (
	select doc_company, branchCode, invoiceSequence, invoiceDate, itemSequence, sum(netValue) as products_net_value
	  from [dw_landing].TotvsInvoicesItemsProducts
	 where missing_since is null
	 group by doc_company, branchCode, invoiceSequence, invoiceDate, itemSequence
  ) p
    on p.doc_company = i.doc_company and p.branchCode = i.branchCode and p.invoiceSequence = i.invoiceSequence
   and p.invoiceDate = i.invoiceDate and p.itemSequence = i.[sequence]
 where i.missing_since is null
   and abs(isnull(i.netValue, 0) - isnull(p.products_net_value, 0)) > 0.01;
GO

/****** pedido sem tipo ou sem série reconhecidos (inconsistência 8 de docs/regras-prefixo-pedidos.md; alerta) ******/
create or alter view [datawarehouse].vw_Check_OrderTypeUnknown
as
select doc_company, branch_code, order_code, representative_name, operation_name, integration_code, order_series
  from [dw_intermediate].vw_OrderParts
 where channel is null
    or order_series is null;
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

/****** datamart.vw_Check_InvoicedReconciliation: com o legado Linx ******/
--a da fase 1 comparava DailySales só com a Totvs; agora as linhas Linx entram dos dois lados. A loja é identificada por
--origem, empresa e código, porque o código de uma loja só do Linx pode repetir o de uma loja da Totvs.
create or alter view [datamart].vw_Check_InvoicedReconciliation
as
with mart as (
	select s.source_erp, s.doc_company, s.branch_code, datefromparts(year(d.sale_date), month(d.sale_date), 1) as [month], sum(d.invoiced_amount) as amount
	  from [datamart].DailySales d
	  join [datawarehouse].DimStore s on s.sk = d.store_sk
	 group by s.source_erp, s.doc_company, s.branch_code, datefromparts(year(d.sale_date), month(d.sale_date), 1)
),
landing as (
	select source_erp, doc_company, branch_code, [month], sum(amount) as amount
	  from (
		select 'totvs' as source_erp, v.doc_company, v.branch_code, datefromparts(year(v.invoice_date), month(v.invoice_date), 1) as [month],
		       v.sign * v.net_amount as amount
		  from [dw_intermediate].vw_SalesItem v
		 where v.is_invoiced = 1
		   and v.is_sale = 1
		union all
		select st.source_erp, st.doc_company, st.branch_code, datefromparts(year(v.invoice_date), month(v.invoice_date), 1),
		       v.sign * v.net_amount
		  from [dw_intermediate].vw_LinxSalesItem v
		 outer apply (select top 1 d.source_erp, d.doc_company, d.branch_code
		                from [datawarehouse].DimStore d
		               where d.cnpj = v.cnpj_emp
		                 and d.valid_from <= v.invoice_datetime and (d.valid_to is null or d.valid_to > v.invoice_datetime)
		               order by iif(d.source_erp = 'totvs', 0, 1), d.valid_from desc) st
		 where v.is_invoiced = 1
		   and v.is_sale = 1
	  ) x
	 group by source_erp, doc_company, branch_code, [month]
)
select coalesce(m.doc_company, l.doc_company) as doc_company, coalesce(m.branch_code, l.branch_code) as branch_code,
       coalesce(m.[month], l.[month]) as [month], m.amount as datamart_amount, l.amount as landing_amount
  from mart m
  full join landing l
    on l.source_erp = m.source_erp and l.doc_company = m.doc_company and l.branch_code = m.branch_code and l.[month] = m.[month]
 where abs(isnull(m.amount, 0) - isnull(l.amount, 0)) > 1.00;
GO
