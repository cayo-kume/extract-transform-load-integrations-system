/****** Fatos da fase 4 (src/Backend/Modules/DataWarehouse/docs, 7.6) ******/
--compras, contas a pagar, comissões, recepção de loja e as chamadas às APIs dos SaaS. A chave de cada fato é a PK da origem.
--Recarga completa (pergunta 8), menos FactApiRequest: o log das chamadas só cresce, então a carga é incremental pelo
--loaded_at da landing (watermark source = datawarehouse, entity = FactApiRequest) e nada é apagado.

create table [datawarehouse].FactPurchaseOrderItem (
	doc_company               nvarchar(14)      not null,
	branch_code               int               not null,
	order_code                int               not null,
	product_code              int               not null,
	registration_date         date              null,
	store_sk                  char(32)          not null,
	product_sk                char(32)          not null,
	supplier_sk               char(32)          not null,
	operation_sk              char(32)          not null,
	supplier_order_code       nvarchar(60)      null,
	buyer_code                int               null,
	buyer_name                nvarchar(100)     null,       --comprador: funcionário
	[status]                  nvarchar(30)      null,
	payment_condition_name    nvarchar(100)     null,
	freight_type              nvarchar(30)      null,
	delivery_forecast_date    date              null,
	delivery_deadline_date    date              null,
	quantity                  decimal(18,6)     not null,
	settled_quantity          decimal(18,6)     not null,   --recebida
	canceled_quantity         decimal(18,6)     not null,
	extra_quantity            decimal(18,6)     not null,
	pending_quantity          decimal(18,6)     not null,   --pedida menos recebida e cancelada, nunca negativa
	unit_value                decimal(18,6)     null,
	discount_percentage       decimal(18,6)     null,
	gross_amount              decimal(18,6)     not null,   --quantidade × valor
	net_amount                decimal(18,6)     not null,   --com o desconto
	loaded_at                 datetime2(3)      not null constraint df_FactPurchaseOrderItem_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactPurchaseOrderItem] primary key (doc_company asc, branch_code asc, order_code asc, product_code asc)
);

create table [datawarehouse].FactAccountsPayable (
	doc_company               nvarchar(14)      not null,
	branch_code               int               not null,
	supplier_code             int               not null,
	duplicate_code            bigint            not null,
	installment_code          int               not null,
	issue_date                date              null,
	due_date                  date              null,
	settlement_date           date              null,
	entry_date                date              null,
	arrival_date              date              null,
	store_sk                  char(32)          not null,
	supplier_sk               char(32)          not null,
	[status]                  nvarchar(20)      null,
	bearer_code               int               null,
	inclusion_type            nvarchar(40)      null,
	user_inclusion_name       nvarchar(60)      null,
	is_settled                bit               not null,
	duplicate_value           decimal(18,6)     not null,
	fees_value                decimal(18,6)     not null,
	discount_value            decimal(18,6)     not null,
	paid_value                decimal(18,6)     not null,
	loaded_at                 datetime2(3)      not null constraint df_FactAccountsPayable_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactAccountsPayable] primary key (doc_company asc, branch_code asc, supplier_code asc, duplicate_code asc, installment_code asc)
);

create table [datawarehouse].FactCommission (
	doc_company                  nvarchar(14)      not null,
	closing_branch_code          int               not null,
	closing_code                 int               not null,
	commissioned_doc_hash        char(64)          not null,   --SHA-256(pepper + CPF/CNPJ do comissionado), a mesma conta de DimCustomer
	closing_date                 date              null,
	store_sk                     char(32)          not null,
	commissioned_name            nvarchar(60)      null,       --vendedor ou representante
	commission_status            int               null,
	billing_value                decimal(18,6)     null,
	billing_commission_value     decimal(18,6)     null,
	receiving_value              decimal(18,6)     null,
	receiving_commission_value   decimal(18,6)     null,
	irrf_value                   decimal(18,6)     null,
	issqn_value                  decimal(18,6)     null,
	rebate_value                 decimal(18,6)     null,
	discount_value               decimal(18,6)     null,
	return_value                 decimal(18,6)     null,
	gross_value                  decimal(18,6)     null,
	net_value                    decimal(18,6)     null,
	account_balance_value        decimal(18,6)     null,
	loaded_at                    datetime2(3)      not null constraint df_FactCommission_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactCommission] primary key (doc_company asc, closing_branch_code asc, closing_code asc, commissioned_doc_hash asc)
);

create table [datawarehouse].FactStoreReception (
	doc_company               nvarchar(14)      not null,
	branch_code               int               not null,
	movement_at               datetime          not null,
	[sequence]                int               not null,
	movement_date             date              not null,
	store_sk                  char(32)          not null,
	customer_sk               char(32)          not null,
	seller_sk                 char(32)          not null,
	representative_sk         char(32)          not null,
	guide_code                int               null,
	guide_name                nvarchar(100)     null,
	reception_type            nvarchar(10)      null,
	check_in_time             nvarchar(10)      null,
	reception_count           int               not null,   --1: soma de recepções, para a taxa de conversão (recepções × vendas por loja e dia)
	loaded_at                 datetime2(3)      not null constraint df_FactStoreReception_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactStoreReception] primary key (doc_company asc, branch_code asc, movement_at asc, [sequence] asc)
);
create index [ix_FactStoreReception_movement_date_store] on [datawarehouse].FactStoreReception (movement_date, store_sk);

create table [datawarehouse].FactApiRequest (
	source_module             varchar(20)       not null,   --totvs, linxmicrovix, linxcommerce, movidesk, aftersale
	request_id                bigint            not null,
	request_date              date              null,
	request_at                datetime          null,
	execution                 uniqueidentifier  null,
	job_id                    int               null,
	user_name                 nvarchar(100)     null,       --quem chamou: funcionário ou serviço
	http_method               varchar(10)       null,
	route                     nvarchar(400)     null,
	doc_company               nvarchar(14)      null,
	field_key_value           nvarchar(400)     null,       --qual registro
	is_success                bit               null,
	status_code               int               null,       --só Totvs e Linx Commerce gravam
	loaded_at                 datetime2(3)      not null constraint df_FactApiRequest_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactApiRequest] primary key (source_module asc, request_id asc)
);
create index [ix_FactApiRequest_request_date] on [datawarehouse].FactApiRequest (request_date, source_module);
GO

/****** datawarehouse.sp_FactPurchaseOrderItem_Load ******/
create or alter procedure [datawarehouse].sp_FactPurchaseOrderItem_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @unknown char(32) = replicate('0', 32);

	begin transaction;

	merge [datawarehouse].FactPurchaseOrderItem as target
	using (
		select i.doc_company, i.branchCode as branch_code, i.orderCode as order_code, i.productCode as product_code,
		       cast(o.registrationDate as date) as registration_date,
		       isnull(st.sk, @unknown) as store_sk, isnull(pr.sk, @unknown) as product_sk,
		       isnull(su.sk, @unknown) as supplier_sk, isnull(op.sk, @unknown) as operation_sk,
		       o.supplierOrderCode as supplier_order_code, o.buyerCode as buyer_code, o.buyerName as buyer_name, o.[status],
		       o.paymentConditionName as payment_condition_name, o.freightType as freight_type,
		       cast(coalesce(i.deliveryForecastDate, o.deliveryForecastDate) as date) as delivery_forecast_date,
		       cast(coalesce(i.deliveryDeadlineDate, o.deliveryDeadlineDate) as date) as delivery_deadline_date,
		       isnull(i.quantity, 0) as quantity, isnull(i.settledQuantity, 0) as settled_quantity,
		       isnull(i.canceledQuantity, 0) as canceled_quantity, isnull(i.extraQuantity, 0) as extra_quantity,
		       iif(isnull(i.quantity, 0) - isnull(i.settledQuantity, 0) - isnull(i.canceledQuantity, 0) > 0,
		           isnull(i.quantity, 0) - isnull(i.settledQuantity, 0) - isnull(i.canceledQuantity, 0), 0) as pending_quantity,
		       i.[value] as unit_value, i.discountPercentage as discount_percentage,
		       isnull(i.quantity * i.[value], 0) as gross_amount,
		       isnull(i.quantity * i.[value] * (1 - isnull(i.discountPercentage, 0) / 100), 0) as net_amount
		  from [dw_landing].TotvsPurchaseOrdersProducts i
		  join [dw_landing].TotvsPurchaseOrders o on o.doc_company = i.doc_company and o.branchCode = i.branchCode and o.orderCode = i.orderCode
		 outer apply (select top 1 d.sk from [datawarehouse].DimStore d
		               where d.doc_company = i.doc_company and d.branch_code = i.branchCode and d.source_erp = 'totvs'
		                 and d.valid_from <= isnull(o.registrationDate, getdate()) and (d.valid_to is null or d.valid_to > isnull(o.registrationDate, getdate()))) st
		 outer apply (select top 1 d.sk from [datawarehouse].DimProduct d
		               where d.doc_company = i.doc_company and d.source_erp = 'totvs' and d.product_code = cast(i.productCode as nvarchar(50))
		                 and d.valid_from <= isnull(o.registrationDate, getdate()) and (d.valid_to is null or d.valid_to > isnull(o.registrationDate, getdate()))) pr
		  left join [datawarehouse].DimSupplier su  on su.sk = convert(char(32), hashbytes('MD5', concat_ws('|', o.doc_company, o.supplierCode)), 2)
		  left join [datawarehouse].DimOperation op on op.sk = convert(char(32), hashbytes('MD5', concat_ws('|', o.doc_company, 'totvs', o.operationCode)), 2)
		 where i.missing_since is null
	) as source
	on (target.doc_company = source.doc_company and target.branch_code = source.branch_code and target.order_code = source.order_code
	    and target.product_code = source.product_code)
	when matched then
		update set registration_date = source.registration_date, store_sk = source.store_sk, product_sk = source.product_sk,
		           supplier_sk = source.supplier_sk, operation_sk = source.operation_sk, supplier_order_code = source.supplier_order_code,
		           buyer_code = source.buyer_code, buyer_name = source.buyer_name, [status] = source.[status],
		           payment_condition_name = source.payment_condition_name, freight_type = source.freight_type,
		           delivery_forecast_date = source.delivery_forecast_date, delivery_deadline_date = source.delivery_deadline_date,
		           quantity = source.quantity, settled_quantity = source.settled_quantity, canceled_quantity = source.canceled_quantity,
		           extra_quantity = source.extra_quantity, pending_quantity = source.pending_quantity, unit_value = source.unit_value,
		           discount_percentage = source.discount_percentage, gross_amount = source.gross_amount, net_amount = source.net_amount,
		           loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (doc_company, branch_code, order_code, product_code, registration_date, store_sk, product_sk, supplier_sk, operation_sk,
		        supplier_order_code, buyer_code, buyer_name, [status], payment_condition_name, freight_type, delivery_forecast_date,
		        delivery_deadline_date, quantity, settled_quantity, canceled_quantity, extra_quantity, pending_quantity, unit_value,
		        discount_percentage, gross_amount, net_amount)
		values (source.doc_company, source.branch_code, source.order_code, source.product_code, source.registration_date, source.store_sk,
		        source.product_sk, source.supplier_sk, source.operation_sk, source.supplier_order_code, source.buyer_code, source.buyer_name,
		        source.[status], source.payment_condition_name, source.freight_type, source.delivery_forecast_date,
		        source.delivery_deadline_date, source.quantity, source.settled_quantity, source.canceled_quantity, source.extra_quantity,
		        source.pending_quantity, source.unit_value, source.discount_percentage, source.gross_amount, source.net_amount)
	when not matched by source then
		delete;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_FactAccountsPayable_Load ******/
create or alter procedure [datawarehouse].sp_FactAccountsPayable_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @unknown char(32) = replicate('0', 32);

	begin transaction;

	merge [datawarehouse].FactAccountsPayable as target
	using (
		select d.doc_company, d.branchCode as branch_code, d.supplierCode as supplier_code, d.duplicateCode as duplicate_code,
		       d.installmentCode as installment_code,
		       cast(d.issueDate as date) as issue_date, cast(d.dueDate as date) as due_date, cast(d.settlementDate as date) as settlement_date,
		       cast(d.entryDate as date) as entry_date, cast(d.arrivalDate as date) as arrival_date,
		       isnull(st.sk, @unknown) as store_sk, isnull(su.sk, @unknown) as supplier_sk,
		       d.[status], d.bearerCode as bearer_code, d.inclusionType as inclusion_type, d.userInclusionName as user_inclusion_name,
		       cast(iif(d.settlementDate is null, 0, 1) as bit) as is_settled,
		       isnull(d.duplicateValue, 0) as duplicate_value, isnull(d.feesValue, 0) as fees_value,
		       isnull(d.discountValue, 0) as discount_value, isnull(d.paidValue, 0) as paid_value
		  from [dw_landing].TotvsDuplicates d
		 outer apply (select top 1 s.sk from [datawarehouse].DimStore s
		               where s.doc_company = d.doc_company and s.branch_code = d.branchCode and s.source_erp = 'totvs'
		                 and s.valid_from <= isnull(d.issueDate, getdate()) and (s.valid_to is null or s.valid_to > isnull(d.issueDate, getdate()))) st
		  left join [datawarehouse].DimSupplier su on su.sk = convert(char(32), hashbytes('MD5', concat_ws('|', d.doc_company, d.supplierCode)), 2)
	) as source
	on (target.doc_company = source.doc_company and target.branch_code = source.branch_code and target.supplier_code = source.supplier_code
	    and target.duplicate_code = source.duplicate_code and target.installment_code = source.installment_code)
	when matched then
		update set issue_date = source.issue_date, due_date = source.due_date, settlement_date = source.settlement_date,
		           entry_date = source.entry_date, arrival_date = source.arrival_date, store_sk = source.store_sk,
		           supplier_sk = source.supplier_sk, [status] = source.[status], bearer_code = source.bearer_code,
		           inclusion_type = source.inclusion_type, user_inclusion_name = source.user_inclusion_name, is_settled = source.is_settled,
		           duplicate_value = source.duplicate_value, fees_value = source.fees_value, discount_value = source.discount_value,
		           paid_value = source.paid_value, loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (doc_company, branch_code, supplier_code, duplicate_code, installment_code, issue_date, due_date, settlement_date, entry_date,
		        arrival_date, store_sk, supplier_sk, [status], bearer_code, inclusion_type, user_inclusion_name, is_settled, duplicate_value,
		        fees_value, discount_value, paid_value)
		values (source.doc_company, source.branch_code, source.supplier_code, source.duplicate_code, source.installment_code, source.issue_date,
		        source.due_date, source.settlement_date, source.entry_date, source.arrival_date, source.store_sk, source.supplier_sk,
		        source.[status], source.bearer_code, source.inclusion_type, source.user_inclusion_name, source.is_settled,
		        source.duplicate_value, source.fees_value, source.discount_value, source.paid_value)
	when not matched by source then
		delete;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_FactCommission_Load ******/
--lê totvs.TotvsCommissionsPaid direto, porque a tabela não tem landing (o documento está na chave, Script015), e grava só o
--doc_hash do comissionado, calculado com o pepper (8.3). O documento em claro não é copiado para o DW.
create or alter procedure [datawarehouse].sp_FactCommission_Load
	@pepper nvarchar(200),
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	if nullif(@pepper, '') is null
		throw 50043, 'sp_FactCommission_Load: o pepper é obrigatório; sem ele o comissionado não é identificado.', 1;

	declare @unknown char(32) = replicate('0', 32);

	begin transaction;

	merge [datawarehouse].FactCommission as target
	using (
		select c.doc_company, c.closingBranchCode as closing_branch_code, c.closingCode as closing_code,
		       [datawarehouse].fn_DocHash(@pepper, c.commissionedCpfCnpj) as commissioned_doc_hash,
		       cast(c.closingDate as date) as closing_date, isnull(st.sk, @unknown) as store_sk,
		       c.commissionedName as commissioned_name, c.commissionStatus as commission_status,
		       c.valueBilling as billing_value, c.valueBillingCommission as billing_commission_value,
		       c.valueReceiving as receiving_value, c.valueReceivingCommission as receiving_commission_value,
		       c.irrfValue as irrf_value, c.issqnValue as issqn_value, c.valueRebate as rebate_value, c.discountValue as discount_value,
		       c.returnValue as return_value, c.grossValue as gross_value, c.netValue as net_value, c.accountBalanceValue as account_balance_value
		  from [totvs].TotvsCommissionsPaid c
		 outer apply (select top 1 s.sk from [datawarehouse].DimStore s
		               where s.doc_company = c.doc_company and s.branch_code = c.closingBranchCode and s.source_erp = 'totvs'
		                 and s.valid_from <= isnull(c.closingDate, getdate()) and (s.valid_to is null or s.valid_to > isnull(c.closingDate, getdate()))) st
		 where nullif(c.commissionedCpfCnpj, '') is not null
	) as source
	on (target.doc_company = source.doc_company and target.closing_branch_code = source.closing_branch_code
	    and target.closing_code = source.closing_code and target.commissioned_doc_hash = source.commissioned_doc_hash)
	when matched then
		update set closing_date = source.closing_date, store_sk = source.store_sk, commissioned_name = source.commissioned_name,
		           commission_status = source.commission_status, billing_value = source.billing_value,
		           billing_commission_value = source.billing_commission_value, receiving_value = source.receiving_value,
		           receiving_commission_value = source.receiving_commission_value, irrf_value = source.irrf_value,
		           issqn_value = source.issqn_value, rebate_value = source.rebate_value, discount_value = source.discount_value,
		           return_value = source.return_value, gross_value = source.gross_value, net_value = source.net_value,
		           account_balance_value = source.account_balance_value,
		           loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (doc_company, closing_branch_code, closing_code, commissioned_doc_hash, closing_date, store_sk, commissioned_name, commission_status,
		        billing_value, billing_commission_value, receiving_value, receiving_commission_value, irrf_value, issqn_value, rebate_value,
		        discount_value, return_value, gross_value, net_value, account_balance_value)
		values (source.doc_company, source.closing_branch_code, source.closing_code, source.commissioned_doc_hash, source.closing_date,
		        source.store_sk, source.commissioned_name, source.commission_status, source.billing_value, source.billing_commission_value,
		        source.receiving_value, source.receiving_commission_value, source.irrf_value, source.issqn_value, source.rebate_value,
		        source.discount_value, source.return_value, source.gross_value, source.net_value, source.account_balance_value)
	when not matched by source then
		delete;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_FactStoreReception_Load ******/
create or alter procedure [datawarehouse].sp_FactStoreReception_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @unknown char(32) = replicate('0', 32);

	begin transaction;

	merge [datawarehouse].FactStoreReception as target
	using (
		select r.doc_company, r.branchCode as branch_code, r.movementDate as movement_at, r.[sequence], cast(r.movementDate as date) as movement_date,
		       isnull(st.sk, @unknown) as store_sk, isnull(cu.sk, @unknown) as customer_sk, isnull(se.sk, @unknown) as seller_sk,
		       isnull(rp.sk, @unknown) as representative_sk,
		       r.guideCode as guide_code, r.guideName as guide_name, r.receptionType as reception_type, r.checkInTime as check_in_time
		  from [dw_landing].TotvsReceptionSales r
		 outer apply (select top 1 s.sk from [datawarehouse].DimStore s
		               where s.doc_company = r.doc_company and s.branch_code = r.branchCode and s.source_erp = 'totvs'
		                 and s.valid_from <= r.movementDate and (s.valid_to is null or s.valid_to > r.movementDate)) st
		  left join [datawarehouse].DimCustomer cu on cu.sk = convert(char(32), hashbytes('MD5', concat_ws('|', r.doc_company, 'totvs', r.customerCode)), 2)
		  left join [datawarehouse].DimSeller se   on se.sk = convert(char(32), hashbytes('MD5', concat_ws('|', r.doc_company, 'totvs', r.sellerCode)), 2)
		  left join [datawarehouse].DimSalesRepresentative rp on rp.sk = convert(char(32), hashbytes('MD5', concat_ws('|', r.doc_company, r.representativeCode)), 2)
	) as source
	on (target.doc_company = source.doc_company and target.branch_code = source.branch_code and target.movement_at = source.movement_at
	    and target.[sequence] = source.[sequence])
	when matched then
		update set movement_date = source.movement_date, store_sk = source.store_sk, customer_sk = source.customer_sk, seller_sk = source.seller_sk,
		           representative_sk = source.representative_sk, guide_code = source.guide_code, guide_name = source.guide_name,
		           reception_type = source.reception_type, check_in_time = source.check_in_time,
		           loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (doc_company, branch_code, movement_at, [sequence], movement_date, store_sk, customer_sk, seller_sk, representative_sk, guide_code,
		        guide_name, reception_type, check_in_time, reception_count)
		values (source.doc_company, source.branch_code, source.movement_at, source.[sequence], source.movement_date, source.store_sk,
		        source.customer_sk, source.seller_sk, source.representative_sk, source.guide_code, source.guide_name, source.reception_type,
		        source.check_in_time, 1)
	when not matched by source then
		delete;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_FactApiRequest_Load ******/
--incremental: as chamadas que a landing gravou ou alterou desde o watermark menos 3 dias (5.2, regra 1).
create or alter procedure [datawarehouse].sp_FactApiRequest_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @since datetime2(3), @from datetime2(3), @until datetime2(3);

	exec [dw_control].sp_Watermark_Get N'datawarehouse', N'FactApiRequest', @since output;
	set @from = isnull(dateadd(day, -3, @since), '19000101');
	set @rows = 0;

	select source_module, id, request_date, execution, id_job, user_name, http_method, route, doc_company, field_key_value, is_success,
	       status_code, loaded_at
	  into #requests
	  from (
		select 'totvs' as source_module, id, request_date, execution, id_job, user_name, http_method, route, doc_company, field_key_value,
		       is_success, status_code, loaded_at
		  from [dw_landing].TotvsRequests where loaded_at >= @from
		union all
		select 'linxmicrovix', id, request_date, execution, id_job, user_name, http_method, route, doc_company, field_key_value, is_success, null, loaded_at
		  from [dw_landing].LinxMicrovixRequests where loaded_at >= @from
		union all
		select 'linxcommerce', id, request_date, execution, id_job, user_name, http_method, route, doc_company, field_key_value, is_success, status_code, loaded_at
		  from [dw_landing].LinxCommerceRequests where loaded_at >= @from
		union all
		select 'movidesk', id, request_date, execution, id_job, user_name, http_method, route, doc_company, field_key_value, is_success, null, loaded_at
		  from [dw_landing].MovideskRequests where loaded_at >= @from
		union all
		select 'aftersale', id, request_date, execution, id_job, user_name, http_method, route, doc_company, field_key_value, is_success, null, loaded_at
		  from [dw_landing].AfterSaleRequests where loaded_at >= @from
	  ) r;

	select @until = max(loaded_at) from #requests;

	if @until is null
		return;

	begin transaction;

	merge [datawarehouse].FactApiRequest as target
	using #requests as source
	on (target.source_module = source.source_module and target.request_id = source.id)
	when matched then
		update set request_date = cast(source.request_date as date), request_at = source.request_date, execution = source.execution,
		           job_id = source.id_job, user_name = source.user_name, http_method = source.http_method, route = source.route,
		           doc_company = source.doc_company, field_key_value = source.field_key_value, is_success = source.is_success,
		           status_code = source.status_code,
		           loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (source_module, request_id, request_date, request_at, execution, job_id, user_name, http_method, route, doc_company,
		        field_key_value, is_success, status_code)
		values (source.source_module, source.id, cast(source.request_date as date), source.request_date, source.execution, source.id_job,
		        source.user_name, source.http_method, source.route, source.doc_company, source.field_key_value, source.is_success,
		        source.status_code);

	set @rows = @@rowcount;

	exec [dw_control].sp_Watermark_Set N'datawarehouse', N'FactApiRequest', @until;

	commit transaction;
end;
GO
