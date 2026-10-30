/****** Fatos da fase 1 (src/Backend/Modules/DataWarehouse/docs, 7.2 e 8.4) ******/
--PK (source_erp, source_key): o grão da origem, sem que as colunas de um ERP precisem existir no outro.
--carga por recarga completa (pergunta 8): MERGE da view inteira e exclusão das linhas que saíram dela (filha apagada
--na origem). Chave que não casa com a dimensão grava o membro desconhecido (32 zeros).
--loja e produto são SCD2: o fato grava a versão vigente na data do evento.

create table [datawarehouse].FactSalesItem (
	source_erp               varchar(20)       not null,
	source_key               varchar(200)      not null,
	doc_company              nvarchar(14)      not null,
	invoice_date             date              not null,
	store_sk                 char(32)          not null,
	channel_sk               char(32)          not null,
	product_sk               char(32)          not null,
	customer_sk              char(32)          not null,
	seller_sk                char(32)          not null,
	operation_sk             char(32)          not null,
	--degeneradas da Totvs (nulas no Linx)
	branch_code              int               null,
	invoice_sequence         int               null,
	invoice_datetime         datetime          null,
	item_sequence            int               null,
	product_code             bigint            null,   --bigint: os códigos do legado Linx passam de int (7.5)
	source_order             varchar(50)       null,
	--degeneradas do Linx (nulas na Totvs; fase 3)
	store_tax_id             nvarchar(14)      null,
	transaction_code         int               null,
	sale_id                  uniqueidentifier  null,
	--medidas
	is_sale                  bit               not null,
	is_cancelled             bit               not null,
	is_invoiced              bit               not null,
	is_realized_operation    bit               not null,
	quantity                 decimal(18,6)     not null,
	gross_amount             decimal(18,6)     not null,
	discount_amount          decimal(18,6)     not null,
	freight_amount           decimal(18,6)     not null,
	tax_amount               decimal(18,6)     null,
	invoiced_amount          decimal(18,6)     not null,   --líquido com sinal, só nas notas faturadas (7.3)
	list_price               decimal(18,6)     null,
	original_price           decimal(18,6)     null,
	promotional_price        decimal(18,6)     null,
	unit_cost                decimal(18,6)     null,
	loaded_at                datetime2(3)      not null constraint df_FactSalesItem_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactSalesItem] primary key (source_erp asc, source_key asc)
);
create index [ix_FactSalesItem_invoice_date_store] on [datawarehouse].FactSalesItem (invoice_date, store_sk);

create table [datawarehouse].FactOrderItem (
	source_erp               varchar(20)       not null,
	source_key               varchar(200)      not null,
	doc_company              nvarchar(14)      not null,
	order_date               date              not null,
	store_sk                 char(32)          not null,
	channel_sk               char(32)          not null,
	product_sk               char(32)          not null,
	customer_sk              char(32)          not null,
	seller_sk                char(32)          not null,
	operation_sk             char(32)          not null,
	--degeneradas e partes do número do pedido (4.2)
	branch_code              int               null,
	order_code               int               null,
	product_code             bigint            null,   --bigint: os códigos do legado Linx passam de int (7.5)
	customer_order_code      nvarchar(100)     null,
	order_series             varchar(5)        null,
	order_type               varchar(5)        null,
	erp_suffix               varchar(2)        null,
	status                   nvarchar(30)      null,
	--medidas
	is_sale                  bit               not null,
	is_captured              bit               not null,
	quantity                 decimal(18,6)     not null,
	settled_quantity         decimal(18,6)     null,
	canceled_quantity        decimal(18,6)     null,
	pending_quantity         decimal(18,6)     null,
	original_price           decimal(18,6)     null,
	price                    decimal(18,6)     null,
	amount                   decimal(18,6)     not null,
	loaded_at                datetime2(3)      not null constraint df_FactOrderItem_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactOrderItem] primary key (source_erp asc, source_key asc)
);
create index [ix_FactOrderItem_order_date_store] on [datawarehouse].FactOrderItem (order_date, store_sk);

create table [datawarehouse].FactInvoicePayment (
	source_erp               varchar(20)       not null,
	source_key               varchar(200)      not null,
	doc_company              nvarchar(14)      not null,
	invoice_date             date              not null,
	store_sk                 char(32)          not null,
	payment_method_sk        char(32)          not null,
	branch_code              int               null,
	invoice_sequence         int               null,
	document_number          bigint            null,
	installment              int               null,
	expiration_date          date              null,
	card_flag                nvarchar(60)      null,
	payment_amount           decimal(18,6)     not null,
	loaded_at                datetime2(3)      not null constraint df_FactInvoicePayment_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactInvoicePayment] primary key (source_erp asc, source_key asc)
);
GO

/****** datawarehouse.sp_FactSalesItem_Load ******/
create or alter procedure [datawarehouse].sp_FactSalesItem_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @unknown char(32) = replicate('0', 32);

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
		       v.branch_code, v.invoice_sequence, v.invoice_datetime, v.item_sequence, v.product_code, nullif(v.source_order, '') as source_order,
		       v.is_sale, v.is_cancelled, v.is_invoiced, v.is_realized_operation,
		       isnull(v.quantity, 0) as quantity, isnull(v.gross_amount, 0) as gross_amount, isnull(v.discount_amount, 0) as discount_amount,
		       isnull(v.freight_amount, 0) as freight_amount, v.tax_amount,
		       iif(v.is_invoiced = 1, v.sign * isnull(v.net_amount, 0), 0) as invoiced_amount,
		       v.list_price, v.original_price, v.promotional_price, v.unit_cost
		  from [dw_intermediate].vw_SalesItem v
		 outer apply (select top 1 d.sk from [datawarehouse].DimStore d
		               where d.doc_company = v.doc_company and d.branch_code = v.branch_code
		                 and d.valid_from <= v.invoice_datetime and (d.valid_to is null or d.valid_to > v.invoice_datetime)) st
		 outer apply (select top 1 d.sk from [datawarehouse].DimProduct d
		               where d.doc_company = v.doc_company and d.source_erp = v.source_erp and d.product_code = cast(v.product_code as nvarchar(50))
		                 and d.valid_from <= v.invoice_datetime and (d.valid_to is null or d.valid_to > v.invoice_datetime)) pr
		  left join [datawarehouse].DimChannel ch   on ch.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.channel, isnull(v.order_type, '-'))), 2)
		  left join [datawarehouse].DimCustomer cu  on cu.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.customer_code)), 2)
		  left join [datawarehouse].DimSeller se    on se.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.seller_code)), 2)
		  left join [datawarehouse].DimOperation op on op.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.operation_code)), 2)
	) as source
	on (target.source_erp = source.source_erp and target.source_key = source.source_key)
	when matched then
		update set doc_company = source.doc_company, invoice_date = source.invoice_date, store_sk = source.store_sk, channel_sk = source.channel_sk,
		           product_sk = source.product_sk, customer_sk = source.customer_sk, seller_sk = source.seller_sk, operation_sk = source.operation_sk,
		           branch_code = source.branch_code, invoice_sequence = source.invoice_sequence, invoice_datetime = source.invoice_datetime,
		           item_sequence = source.item_sequence, product_code = source.product_code, source_order = source.source_order,
		           is_sale = source.is_sale, is_cancelled = source.is_cancelled, is_invoiced = source.is_invoiced, is_realized_operation = source.is_realized_operation,
		           quantity = source.quantity, gross_amount = source.gross_amount, discount_amount = source.discount_amount, freight_amount = source.freight_amount,
		           tax_amount = source.tax_amount, invoiced_amount = source.invoiced_amount, list_price = source.list_price, original_price = source.original_price,
		           promotional_price = source.promotional_price, unit_cost = source.unit_cost,
		           loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (source_erp, source_key, doc_company, invoice_date, store_sk, channel_sk, product_sk, customer_sk, seller_sk, operation_sk,
		        branch_code, invoice_sequence, invoice_datetime, item_sequence, product_code, source_order,
		        is_sale, is_cancelled, is_invoiced, is_realized_operation, quantity, gross_amount, discount_amount, freight_amount, tax_amount,
		        invoiced_amount, list_price, original_price, promotional_price, unit_cost)
		values (source.source_erp, source.source_key, source.doc_company, source.invoice_date, source.store_sk, source.channel_sk, source.product_sk,
		        source.customer_sk, source.seller_sk, source.operation_sk, source.branch_code, source.invoice_sequence, source.invoice_datetime,
		        source.item_sequence, source.product_code, source.source_order, source.is_sale, source.is_cancelled, source.is_invoiced,
		        source.is_realized_operation, source.quantity, source.gross_amount, source.discount_amount, source.freight_amount, source.tax_amount,
		        source.invoiced_amount, source.list_price, source.original_price, source.promotional_price, source.unit_cost)
	when not matched by source and target.source_erp = 'totvs' then
		delete;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_FactOrderItem_Load ******/
create or alter procedure [datawarehouse].sp_FactOrderItem_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @unknown char(32) = replicate('0', 32);

	begin transaction;

	merge [datawarehouse].FactOrderItem as target
	using (
		select v.source_erp, v.source_key, v.doc_company, isnull(v.order_date, '19000101') as order_date,
		       isnull(st.sk, @unknown) as store_sk,
		       isnull(ch.sk, @unknown) as channel_sk,
		       isnull(pr.sk, @unknown) as product_sk,
		       isnull(cu.sk, @unknown) as customer_sk,
		       isnull(se.sk, @unknown) as seller_sk,
		       isnull(op.sk, @unknown) as operation_sk,
		       v.branch_code, v.order_code, v.product_code, v.customer_order_code, v.order_series, v.order_type, v.erp_suffix, v.status,
		       v.is_sale, v.is_captured, isnull(v.quantity, 0) as quantity, v.settled_quantity, v.canceled_quantity, v.pending_quantity,
		       v.original_price, v.price, isnull(v.amount, 0) as amount
		  from [dw_intermediate].vw_OrderItem v
		 outer apply (select top 1 d.sk from [datawarehouse].DimStore d
		               where d.doc_company = v.doc_company and d.branch_code = v.branch_code
		                 and d.valid_from <= v.order_datetime and (d.valid_to is null or d.valid_to > v.order_datetime)) st
		 outer apply (select top 1 d.sk from [datawarehouse].DimProduct d
		               where d.doc_company = v.doc_company and d.source_erp = v.source_erp and d.product_code = cast(v.product_code as nvarchar(50))
		                 and d.valid_from <= v.order_datetime and (d.valid_to is null or d.valid_to > v.order_datetime)) pr
		  left join [datawarehouse].DimChannel ch   on v.channel is not null and ch.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.channel, isnull(v.order_type, '-'))), 2)
		  left join [datawarehouse].DimCustomer cu  on cu.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.customer_code)), 2)
		  left join [datawarehouse].DimSeller se    on se.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.seller_code)), 2)
		  left join [datawarehouse].DimOperation op on op.sk = convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, v.source_erp, v.operation_code)), 2)
	) as source
	on (target.source_erp = source.source_erp and target.source_key = source.source_key)
	when matched then
		update set doc_company = source.doc_company, order_date = source.order_date, store_sk = source.store_sk, channel_sk = source.channel_sk,
		           product_sk = source.product_sk, customer_sk = source.customer_sk, seller_sk = source.seller_sk, operation_sk = source.operation_sk,
		           branch_code = source.branch_code, order_code = source.order_code, product_code = source.product_code,
		           customer_order_code = source.customer_order_code, order_series = source.order_series, order_type = source.order_type,
		           erp_suffix = source.erp_suffix, status = source.status, is_sale = source.is_sale, is_captured = source.is_captured,
		           quantity = source.quantity, settled_quantity = source.settled_quantity, canceled_quantity = source.canceled_quantity,
		           pending_quantity = source.pending_quantity, original_price = source.original_price, price = source.price, amount = source.amount,
		           loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (source_erp, source_key, doc_company, order_date, store_sk, channel_sk, product_sk, customer_sk, seller_sk, operation_sk,
		        branch_code, order_code, product_code, customer_order_code, order_series, order_type, erp_suffix, status,
		        is_sale, is_captured, quantity, settled_quantity, canceled_quantity, pending_quantity, original_price, price, amount)
		values (source.source_erp, source.source_key, source.doc_company, source.order_date, source.store_sk, source.channel_sk, source.product_sk,
		        source.customer_sk, source.seller_sk, source.operation_sk, source.branch_code, source.order_code, source.product_code,
		        source.customer_order_code, source.order_series, source.order_type, source.erp_suffix, source.status, source.is_sale, source.is_captured,
		        source.quantity, source.settled_quantity, source.canceled_quantity, source.pending_quantity, source.original_price, source.price, source.amount)
	when not matched by source and target.source_erp = 'totvs' then
		delete;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_FactInvoicePayment_Load ******/
create or alter procedure [datawarehouse].sp_FactInvoicePayment_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @unknown char(32) = replicate('0', 32);

	begin transaction;

	merge [datawarehouse].FactInvoicePayment as target
	using (
		select 'totvs' as source_erp,
		       concat_ws('|', p.doc_company, p.branchCode, p.invoiceSequence, convert(char(8), p.invoiceDate, 112), p.documentNumber, p.installment) as source_key,
		       p.doc_company, cast(p.invoiceDate as date) as invoice_date,
		       isnull(st.sk, @unknown) as store_sk,
		       isnull(pm.sk, @unknown) as payment_method_sk,
		       p.branchCode as branch_code, p.invoiceSequence as invoice_sequence, p.documentNumber as document_number, p.installment,
		       cast(p.expirationDate as date) as expiration_date, p.cardInformationCardFlag as card_flag, isnull(p.paymentValue, 0) as payment_amount
		  from [dw_landing].TotvsInvoicesPayments p
		 outer apply (select top 1 d.sk from [datawarehouse].DimStore d
		               where d.doc_company = p.doc_company and d.branch_code = p.branchCode
		                 and d.valid_from <= p.invoiceDate and (d.valid_to is null or d.valid_to > p.invoiceDate)) st
		  left join [datawarehouse].DimPaymentMethod pm on pm.sk = convert(char(32), hashbytes('MD5', concat_ws('|', p.doc_company, 'totvs', p.documentTypeCode)), 2)
		 where p.missing_since is null
	) as source
	on (target.source_erp = source.source_erp and target.source_key = source.source_key)
	when matched then
		update set invoice_date = source.invoice_date, store_sk = source.store_sk, payment_method_sk = source.payment_method_sk,
		           expiration_date = source.expiration_date, card_flag = source.card_flag, payment_amount = source.payment_amount,
		           loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (source_erp, source_key, doc_company, invoice_date, store_sk, payment_method_sk, branch_code, invoice_sequence, document_number,
		        installment, expiration_date, card_flag, payment_amount)
		values (source.source_erp, source.source_key, source.doc_company, source.invoice_date, source.store_sk, source.payment_method_sk,
		        source.branch_code, source.invoice_sequence, source.document_number, source.installment, source.expiration_date, source.card_flag,
		        source.payment_amount)
	when not matched by source and target.source_erp = 'totvs' then
		delete;

	set @rows = @@rowcount;

	commit transaction;
end;
