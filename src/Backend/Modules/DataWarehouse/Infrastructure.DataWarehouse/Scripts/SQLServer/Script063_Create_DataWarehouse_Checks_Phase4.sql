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
