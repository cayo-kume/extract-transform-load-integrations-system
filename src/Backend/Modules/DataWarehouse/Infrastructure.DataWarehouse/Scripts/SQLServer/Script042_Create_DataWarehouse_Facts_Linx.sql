/****** Fatos do legado Linx Microvix (docs/datawarehouse, 7.5 e 10.2) ******/
--o código de produto do Microvix é bigint: as degeneradas dos fatos passam de int para bigint.
alter table [datawarehouse].FactSalesItem alter column product_code bigint null;
alter table [datawarehouse].FactOrderItem alter column product_code bigint null;
GO

/****** dw_intermediate.sp_LinxLegacy_Load ******/
--grava as linhas Linx em FactSalesItem (de dw_intermediate.vw_LinxSalesItem) e FactOrderItem (de vw_LinxOrderItem), com
--source_erp = linxmicrovix. Recarga completa das linhas Linx; as da Totvs não são tocadas, e as cargas da Totvs não apagam
--as do Linx (o delete delas filtra source_erp = 'totvs'). Loja: a linha da Totvs com o mesmo CNPJ vigente na data; sem ela,
--a loja só do Linx. Produto: o membro Linx vigente na data. Cliente e vendedor: os membros Linx. Roda na carga do legado
--(10.2), depois das dimensões, e pode ser reexecutada.
create or alter procedure [dw_intermediate].sp_LinxLegacy_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @unknown char(32) = replicate('0', 32), @sales int, @orders int;

	begin transaction;

	merge [datawarehouse].FactSalesItem as target
	using (
		select v.source_erp, v.source_key, v.doc_company, v.invoice_date,
		       isnull(st.sk, @unknown) as store_sk,
		       isnull(ch.sk, @unknown) as channel_sk,
		       isnull(pr.sk, @unknown) as product_sk,
		       isnull(cu.sk, @unknown) as customer_sk,
		       isnull(se.sk, @unknown) as seller_sk,
		       isnull(op.sk, @unknown) as operation_sk,
		       st.branch_code, v.documento as invoice_sequence, v.invoice_datetime, v.cod_produto as product_code,
		       v.cnpj_emp as store_tax_id, v.transacao as transaction_code, v.identificador as sale_id,
		       v.is_sale, v.is_cancelled, v.is_invoiced, v.is_invoiced as is_realized_operation,
		       v.quantity, v.gross_amount, v.discount_amount, v.freight_amount, v.tax_amount,
		       iif(v.is_invoiced = 1, v.sign * v.net_amount, 0) as invoiced_amount,
		       v.list_price, v.unit_cost
		  from [dw_intermediate].vw_LinxSalesItem v
		 outer apply (select top 1 d.sk, iif(d.source_erp = 'totvs', d.branch_code, null) as branch_code
		                from [datawarehouse].DimStore d
		               where d.cnpj = v.cnpj_emp
		                 and d.valid_from <= v.invoice_datetime and (d.valid_to is null or d.valid_to > v.invoice_datetime)
		               order by iif(d.source_erp = 'totvs', 0, 1), d.valid_from desc) st
		 outer apply (select top 1 d.sk from [datawarehouse].DimProduct d
		               where d.doc_company = v.doc_company and d.source_erp = 'linxmicrovix' and d.product_code = cast(v.cod_produto as nvarchar(50))
		                 and d.valid_from <= v.invoice_datetime and (d.valid_to is null or d.valid_to > v.invoice_datetime)
		               order by d.valid_from desc) pr
		  left join [datawarehouse].DimChannel ch   on ch.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.channel, isnull(v.order_type, '-'))), 2)
		  left join [datawarehouse].DimCustomer cu  on cu.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.customer_code)), 2)
		  left join [datawarehouse].DimSeller se    on se.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.seller_code)), 2)
		  left join [datawarehouse].DimOperation op on op.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.operation_code)), 2)
	) as source
	on (target.source_erp = source.source_erp and target.source_key = source.source_key)
	when matched then
		update set invoice_date = source.invoice_date, store_sk = source.store_sk, channel_sk = source.channel_sk, product_sk = source.product_sk,
		           customer_sk = source.customer_sk, seller_sk = source.seller_sk, operation_sk = source.operation_sk,
		           branch_code = source.branch_code, invoice_sequence = source.invoice_sequence, invoice_datetime = source.invoice_datetime,
		           product_code = source.product_code, store_tax_id = source.store_tax_id, transaction_code = source.transaction_code,
		           sale_id = source.sale_id, is_sale = source.is_sale, is_cancelled = source.is_cancelled, is_invoiced = source.is_invoiced,
		           is_realized_operation = source.is_realized_operation, quantity = source.quantity, gross_amount = source.gross_amount,
		           discount_amount = source.discount_amount, freight_amount = source.freight_amount, tax_amount = source.tax_amount,
		           invoiced_amount = source.invoiced_amount, list_price = source.list_price, unit_cost = source.unit_cost,
		           loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (source_erp, source_key, doc_company, invoice_date, store_sk, channel_sk, product_sk, customer_sk, seller_sk, operation_sk,
		        branch_code, invoice_sequence, invoice_datetime, product_code, store_tax_id, transaction_code, sale_id,
		        is_sale, is_cancelled, is_invoiced, is_realized_operation, quantity, gross_amount, discount_amount, freight_amount, tax_amount,
		        invoiced_amount, list_price, unit_cost)
		values (source.source_erp, source.source_key, source.doc_company, source.invoice_date, source.store_sk, source.channel_sk, source.product_sk,
		        source.customer_sk, source.seller_sk, source.operation_sk, source.branch_code, source.invoice_sequence, source.invoice_datetime,
		        source.product_code, source.store_tax_id, source.transaction_code, source.sale_id, source.is_sale, source.is_cancelled,
		        source.is_invoiced, source.is_realized_operation, source.quantity, source.gross_amount, source.discount_amount,
		        source.freight_amount, source.tax_amount, source.invoiced_amount, source.list_price, source.unit_cost)
	when not matched by source and target.source_erp = 'linxmicrovix' then
		delete;

	set @sales = @@rowcount;

	merge [datawarehouse].FactOrderItem as target
	using (
		select v.source_erp, v.source_key, v.doc_company, v.order_date,
		       isnull(st.sk, @unknown) as store_sk,
		       isnull(ch.sk, @unknown) as channel_sk,
		       isnull(pr.sk, @unknown) as product_sk,
		       isnull(cu.sk, @unknown) as customer_sk,
		       isnull(se.sk, @unknown) as seller_sk,
		       @unknown as operation_sk,
		       st.branch_code, v.order_code, v.product_code, v.customer_order_code, v.[status], v.is_captured,
		       v.quantity, v.price, v.amount
		  from [dw_intermediate].vw_LinxOrderItem v
		 outer apply (select top 1 d.sk, iif(d.source_erp = 'totvs', d.branch_code, null) as branch_code
		                from [datawarehouse].DimStore d
		               where d.cnpj = v.store_cnpj
		                 and d.valid_from <= v.order_date and (d.valid_to is null or d.valid_to > v.order_date)
		               order by iif(d.source_erp = 'totvs', 0, 1), d.valid_from desc) st
		 outer apply (select top 1 d.sk from [datawarehouse].DimProduct d
		               where d.doc_company = v.doc_company and d.source_erp = 'linxmicrovix' and d.product_code = cast(v.product_code as nvarchar(50))
		                 and d.valid_from <= v.order_date and (d.valid_to is null or d.valid_to > v.order_date)
		               order by d.valid_from desc) pr
		  left join [datawarehouse].DimChannel ch  on ch.sk = convert(char(32), hashbytes('MD5', concat_ws('|', 'ecommerce', 'ST')), 2)
		  left join [datawarehouse].DimCustomer cu on cu.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.customer_code)), 2)
		  left join [datawarehouse].DimSeller se   on se.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.seller_code)), 2)
	) as source
	on (target.source_erp = source.source_erp and target.source_key = source.source_key)
	when matched then
		update set order_date = source.order_date, store_sk = source.store_sk, channel_sk = source.channel_sk, product_sk = source.product_sk,
		           customer_sk = source.customer_sk, seller_sk = source.seller_sk, branch_code = source.branch_code,
		           order_code = source.order_code, product_code = source.product_code, customer_order_code = source.customer_order_code,
		           [status] = source.[status], is_captured = source.is_captured, quantity = source.quantity, price = source.price,
		           amount = source.amount, loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (source_erp, source_key, doc_company, order_date, store_sk, channel_sk, product_sk, customer_sk, seller_sk, operation_sk,
		        branch_code, order_code, product_code, customer_order_code, order_type, [status], is_sale, is_captured, quantity, price, amount)
		values (source.source_erp, source.source_key, source.doc_company, source.order_date, source.store_sk, source.channel_sk, source.product_sk,
		        source.customer_sk, source.seller_sk, source.operation_sk, source.branch_code, source.order_code, source.product_code,
		        source.customer_order_code, 'ST', source.[status], 1, source.is_captured, source.quantity, source.price, source.amount)
	when not matched by source and target.source_erp = 'linxmicrovix' then
		delete;

	set @orders = @@rowcount;
	set @rows = @sales + @orders;

	commit transaction;
end;
GO
