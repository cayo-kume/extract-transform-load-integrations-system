/****** Fatos do atendimento e do pós-venda: FactTicket e FactRefund (docs/datawarehouse, 7.6 e seção 2, fase 2) ******/
--grãos diferentes, por isso dois fatos: um ticket do Movidesk e um reembolso do AfterSale. Recarga completa a cada execução
--(pergunta 8), como os fatos da fase 1. O cliente vem pelo doc_hash: o documento está em dw_personal_data.ServiceCustomerIdentified
--e o hash é calculado aqui, com o pepper recebido como parâmetro (8.3); customer_sk é o cliente da Totvs com o mesmo hash.

/****** busca do cliente pelo hash ******/
if not exists (select 1 from sys.indexes where object_id = object_id('[datawarehouse].DimCustomer') and name = 'ix_DimCustomer_doc_hash')
	create index [ix_DimCustomer_doc_hash] on [datawarehouse].DimCustomer (doc_hash) include (updated_at) where doc_hash is not null;
GO

create table [datawarehouse].FactTicket (
	doc_company              nvarchar(14)      not null,
	ticket_id                bigint            not null,
	created_date             date              null,
	customer_sk              char(32)          not null,
	customer_doc_hash        char(64)          null,       --mesmo hash de DimCustomer; liga o ticket ao cliente mesmo sem cadastro na Totvs
	protocol                 nvarchar(30)      null,
	created_at               datetime          null,
	resolved_at              datetime          null,
	closed_at                datetime          null,
	canceled_at              datetime          null,
	reopened_at              datetime          null,
	last_action_at           datetime          null,
	ticket_type              nvarchar(60)      null,       --Interno, Público
	origin                   nvarchar(60)      null,       --canal de abertura
	category                 nvarchar(128)     null,
	urgency                  nvarchar(128)     null,
	[status]                 nvarchar(128)     null,
	base_status              nvarchar(128)     null,       --New, InAttendance, Stopped, Canceled, Resolved, Closed
	justification            nvarchar(128)     null,
	owner_team               nvarchar(128)     null,
	owner_id                 nvarchar(64)      null,       --agente responsável (id do Movidesk)
	service_first_level      nvarchar(1024)    null,
	service_second_level     nvarchar(1024)    null,
	service_third_level      nvarchar(1024)    null,
	contact_form             nvarchar(128)     null,
	sla_agreement            nvarchar(128)     null,
	is_deleted               bit               not null,
	is_resolved              bit               not null,
	resolved_in_first_call   bit               not null,
	is_sla_response_met      bit               null,
	is_sla_solution_met      bit               null,
	action_count             int               not null,
	public_action_count      int               not null,
	internal_action_count    int               not null,
	lifetime_working_minutes int               null,
	stopped_working_minutes  int               null,
	sla_response_minutes     int               null,
	sla_solution_minutes     int               null,
	chat_waiting_seconds     int               null,
	chat_talk_seconds        int               null,
	loaded_at                datetime2(3)      not null constraint df_FactTicket_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactTicket] primary key (doc_company asc, ticket_id asc)
);
create index [ix_FactTicket_created_date] on [datawarehouse].FactTicket (created_date);

create table [datawarehouse].FactRefund (
	doc_company                nvarchar(14)      not null,
	refund_id                  bigint            not null,
	created_date               date              null,
	customer_sk                char(32)          not null,
	customer_doc_hash          char(64)          null,
	created_at                 datetime          null,
	updated_at                 datetime          null,
	refund_type                nvarchar(50)      null,
	refund_action              nvarchar(50)      null,
	status_id                  int               null,
	status_name                nvarchar(50)      null,
	shipping_method            nvarchar(50)      null,
	refunded_shipping_type     nvarchar(50)      null,
	product_price_type         nvarchar(50)      null,
	seller_info                nvarchar(50)      null,
	reverse_id                 bigint            null,
	reverse_type               nvarchar(50)      null,
	reverse_status_name        nvarchar(50)      null,
	reverse_created_at         datetime          null,
	brand_name                 nvarchar(50)      null,
	reverse_origin             nvarchar(50)      null,
	ecommerce_order_id         bigint            null,
	ecommerce_order            nvarchar(50)      null,       --número do pedido no e-commerce
	seller_invoice_number      nvarchar(50)      null,
	main_reason                nvarchar(150)     null,
	main_subreason             nvarchar(150)     null,
	free_shipping              bit               not null,
	freight_by_customer        bit               not null,
	product_lines              int               not null,
	requested_qty              int               not null,
	received_qty               int               not null,
	requested_raw_amount       decimal(10,2)     not null,
	requested_amount           decimal(10,2)     not null,
	requested_shipping_amount  decimal(10,2)     not null,
	requested_total_amount     decimal(10,2)     not null,
	received_raw_amount        decimal(10,2)     not null,
	received_amount            decimal(10,2)     not null,
	bonus_amount               decimal(10,2)     not null,
	added_taxes_amount         decimal(10,2)     not null,
	insurance_amount           decimal(10,2)     not null,
	total_amount               decimal(10,2)     not null,
	loaded_at                  datetime2(3)      not null constraint df_FactRefund_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactRefund] primary key (doc_company asc, refund_id asc)
);
create index [ix_FactRefund_created_date] on [datawarehouse].FactRefund (created_date);
GO

/****** datawarehouse.sp_FactTicket_Load ******/
create or alter procedure [datawarehouse].sp_FactTicket_Load
	@pepper nvarchar(200),
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	if nullif(@pepper, '') is null
		throw 50044, 'sp_FactTicket_Load: o pepper é obrigatório; sem ele o ticket não se liga ao cliente.', 1;

	declare @unknown char(32) = replicate('0', 32);

	begin transaction;

	merge [datawarehouse].FactTicket as target
	using (
		select t.*, h.doc_hash as customer_doc_hash, isnull(cu.sk, @unknown) as customer_sk
		  from [dw_intermediate].vw_Ticket t
		 outer apply (select [datawarehouse].fn_DocHash(@pepper, i.tax_id) as doc_hash
		                from [dw_personal_data].ServiceCustomerIdentified i
		               where i.source_system = 'movidesk' and i.doc_company = t.doc_company and i.customer_id = t.client_id) h
		 outer apply (select top 1 c.sk from [datawarehouse].DimCustomer c where c.doc_hash = h.doc_hash order by c.updated_at desc, c.sk) cu
	) as source
	on (target.doc_company = source.doc_company and target.ticket_id = source.ticket_id)
	when matched then
		update set created_date = source.created_date, customer_sk = source.customer_sk, customer_doc_hash = source.customer_doc_hash,
		           protocol = source.protocol, created_at = source.created_at, resolved_at = source.resolved_at, closed_at = source.closed_at,
		           canceled_at = source.canceled_at, reopened_at = source.reopened_at, last_action_at = source.last_action_at,
		           ticket_type = source.ticket_type, origin = source.origin, category = source.category, urgency = source.urgency,
		           [status] = source.[status], base_status = source.base_status, justification = source.justification,
		           owner_team = source.owner_team, owner_id = source.owner_id, service_first_level = source.service_first_level,
		           service_second_level = source.service_second_level, service_third_level = source.service_third_level,
		           contact_form = source.contact_form, sla_agreement = source.sla_agreement, is_deleted = source.is_deleted,
		           is_resolved = source.is_resolved, resolved_in_first_call = source.resolved_in_first_call,
		           is_sla_response_met = source.is_sla_response_met, is_sla_solution_met = source.is_sla_solution_met,
		           action_count = source.action_count, public_action_count = source.public_action_count,
		           internal_action_count = source.internal_action_count, lifetime_working_minutes = source.lifetime_working_minutes,
		           stopped_working_minutes = source.stopped_working_minutes, sla_response_minutes = source.sla_response_minutes,
		           sla_solution_minutes = source.sla_solution_minutes, chat_waiting_seconds = source.chat_waiting_seconds,
		           chat_talk_seconds = source.chat_talk_seconds,
		           loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (doc_company, ticket_id, created_date, customer_sk, customer_doc_hash, protocol, created_at, resolved_at, closed_at, canceled_at,
		        reopened_at, last_action_at, ticket_type, origin, category, urgency, [status], base_status, justification, owner_team, owner_id,
		        service_first_level, service_second_level, service_third_level, contact_form, sla_agreement, is_deleted, is_resolved,
		        resolved_in_first_call, is_sla_response_met, is_sla_solution_met, action_count, public_action_count, internal_action_count,
		        lifetime_working_minutes, stopped_working_minutes, sla_response_minutes, sla_solution_minutes, chat_waiting_seconds, chat_talk_seconds)
		values (source.doc_company, source.ticket_id, source.created_date, source.customer_sk, source.customer_doc_hash, source.protocol,
		        source.created_at, source.resolved_at, source.closed_at, source.canceled_at, source.reopened_at, source.last_action_at,
		        source.ticket_type, source.origin, source.category, source.urgency, source.[status], source.base_status, source.justification,
		        source.owner_team, source.owner_id, source.service_first_level, source.service_second_level, source.service_third_level,
		        source.contact_form, source.sla_agreement, source.is_deleted, source.is_resolved, source.resolved_in_first_call,
		        source.is_sla_response_met, source.is_sla_solution_met, source.action_count, source.public_action_count,
		        source.internal_action_count, source.lifetime_working_minutes, source.stopped_working_minutes, source.sla_response_minutes,
		        source.sla_solution_minutes, source.chat_waiting_seconds, source.chat_talk_seconds)
	when not matched by source then
		delete;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_FactRefund_Load ******/
create or alter procedure [datawarehouse].sp_FactRefund_Load
	@pepper nvarchar(200),
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	if nullif(@pepper, '') is null
		throw 50045, 'sp_FactRefund_Load: o pepper é obrigatório; sem ele o reembolso não se liga ao cliente.', 1;

	declare @unknown char(32) = replicate('0', 32);

	begin transaction;

	merge [datawarehouse].FactRefund as target
	using (
		select r.*, h.doc_hash as customer_doc_hash, isnull(cu.sk, @unknown) as customer_sk
		  from [dw_intermediate].vw_Refund r
		 outer apply (select [datawarehouse].fn_DocHash(@pepper, i.tax_id) as doc_hash
		                from [dw_personal_data].ServiceCustomerIdentified i
		               where i.source_system = 'aftersale' and i.doc_company = r.doc_company and i.customer_id = cast(r.customer_id as nvarchar(64))) h
		 outer apply (select top 1 c.sk from [datawarehouse].DimCustomer c where c.doc_hash = h.doc_hash order by c.updated_at desc, c.sk) cu
	) as source
	on (target.doc_company = source.doc_company and target.refund_id = source.refund_id)
	when matched then
		update set created_date = source.created_date, customer_sk = source.customer_sk, customer_doc_hash = source.customer_doc_hash,
		           created_at = source.created_at, updated_at = source.updated_at, refund_type = source.refund_type,
		           refund_action = source.refund_action, status_id = source.status_id, status_name = source.status_name,
		           shipping_method = source.shipping_method, refunded_shipping_type = source.refunded_shipping_type,
		           product_price_type = source.product_price_type, seller_info = source.seller_info, reverse_id = source.reverse_id,
		           reverse_type = source.reverse_type, reverse_status_name = source.reverse_status_name,
		           reverse_created_at = source.reverse_created_at, brand_name = source.brand_name, reverse_origin = source.reverse_origin,
		           ecommerce_order_id = source.ecommerce_order_id, ecommerce_order = source.ecommerce_order,
		           seller_invoice_number = source.seller_invoice_number, main_reason = source.main_reason,
		           main_subreason = source.main_subreason, free_shipping = source.free_shipping,
		           freight_by_customer = source.freight_by_customer, product_lines = source.product_lines,
		           requested_qty = source.requested_qty, received_qty = source.received_qty,
		           requested_raw_amount = source.requested_raw_amount, requested_amount = source.requested_amount,
		           requested_shipping_amount = source.requested_shipping_amount, requested_total_amount = source.requested_total_amount,
		           received_raw_amount = source.received_raw_amount, received_amount = source.received_amount,
		           bonus_amount = source.bonus_amount, added_taxes_amount = source.added_taxes_amount,
		           insurance_amount = source.insurance_amount, total_amount = source.total_amount,
		           loaded_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched by target then
		insert (doc_company, refund_id, created_date, customer_sk, customer_doc_hash, created_at, updated_at, refund_type, refund_action,
		        status_id, status_name, shipping_method, refunded_shipping_type, product_price_type, seller_info, reverse_id, reverse_type,
		        reverse_status_name, reverse_created_at, brand_name, reverse_origin, ecommerce_order_id, ecommerce_order,
		        seller_invoice_number, main_reason, main_subreason, free_shipping, freight_by_customer, product_lines, requested_qty,
		        received_qty, requested_raw_amount, requested_amount, requested_shipping_amount, requested_total_amount,
		        received_raw_amount, received_amount, bonus_amount, added_taxes_amount, insurance_amount, total_amount)
		values (source.doc_company, source.refund_id, source.created_date, source.customer_sk, source.customer_doc_hash, source.created_at,
		        source.updated_at, source.refund_type, source.refund_action, source.status_id, source.status_name, source.shipping_method,
		        source.refunded_shipping_type, source.product_price_type, source.seller_info, source.reverse_id, source.reverse_type,
		        source.reverse_status_name, source.reverse_created_at, source.brand_name, source.reverse_origin,
		        source.ecommerce_order_id, source.ecommerce_order, source.seller_invoice_number, source.main_reason,
		        source.main_subreason, source.free_shipping, source.freight_by_customer, source.product_lines, source.requested_qty,
		        source.received_qty, source.requested_raw_amount, source.requested_amount, source.requested_shipping_amount,
		        source.requested_total_amount, source.received_raw_amount, source.received_amount, source.bonus_amount,
		        source.added_taxes_amount, source.insurance_amount, source.total_amount)
	when not matched by source then
		delete;

	set @rows = @@rowcount;

	commit transaction;
end;
GO
