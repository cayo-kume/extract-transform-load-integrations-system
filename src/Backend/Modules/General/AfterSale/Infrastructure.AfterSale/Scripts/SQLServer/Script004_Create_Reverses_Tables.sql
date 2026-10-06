create table [untreated].AfterSaleCourierAttributes (
	integrated_or_updated_at datetime,
	reverse_id bigint,
	fill_form_from_file nvarchar(50)
);

create table [general].AfterSaleCourierAttributes (
	integrated_or_updated_at datetime,
	reverse_id bigint not null primary key,
	fill_form_from_file nvarchar(50)
);

create table [untreated].AfterSaleCourierAttributesFields (
	integrated_or_updated_at datetime,
	[required] bit,
	id nvarchar(50),
	[name] nvarchar(250),
	[type] nvarchar(50),
	mime nvarchar(50),
	reverse_id bigint
);

create table [general].AfterSaleCourierAttributesFields (
	integrated_or_updated_at datetime,
	[required] bit,
	id nvarchar(50),
	[name] nvarchar(250),
	[type] nvarchar(50),
	mime nvarchar(50),
	reverse_id bigint,
constraint [PK_AfterSaleCourierAttributesFields] primary key (
	id asc,
	reverse_id asc
)); 

create table [untreated].AfterSaleTransportationsReverses (
	integrated_or_updated_at datetime,
	id bigint, --gerar com c-sharp
	--[doc_company] nvarchar(14),
	transportation nvarchar(50)
);

create table [general].AfterSaleTransportationsReverses (
	integrated_or_updated_at datetime,
	id bigint not null primary key,
	--[doc_company] nvarchar(14),
	transportation nvarchar(50)
);

create table [untreated].AfterSaleHistoriesReverses(
	integrated_or_updated_at datetime,
	id bigint, --gerar com c-sharp
	reverse_id bigint not null,
	link nvarchar(250)
);

create table [general].AfterSaleHistoriesReverses(
	integrated_or_updated_at datetime,
	id bigint not null primary key,
	reverse_id bigint not null,
	link nvarchar(250)
);

create table [untreated].AfterSaleStatusReverses (
	integrated_or_updated_at datetime,
	id int,
	--[doc_company] nvarchar(14),
	[name] nvarchar(50),
	[description] nvarchar(250)
);

create table [general].AfterSaleStatusReverses (
	integrated_or_updated_at datetime,
	id int not null primary key,
	--[doc_company] nvarchar(14),
	[name] nvarchar(50),
	[description] nvarchar(250)
);

create table [untreated].AfterSaleSummarizedReverses (
	integrated_or_updated_at datetime,
	cnpj_emp nvarchar(14),
	id bigint,
	ecommerce_order_id bigint,
	created_at datetime,
	updated_at datetime,
	customer_id bigint,
	customer_first_name nvarchar(50),
	customer_last_name nvarchar(100), 
	status_id int,
	status_name nvarchar(50), 
	status_description nvarchar(300), 
	refunds_count int,
	total_amount decimal(10, 2), 
	reverse_type nvarchar(50), 
	reverse_type_name nvarchar(50), 
	order_id nvarchar(50), 
	tracking_authorization_code nvarchar(50), 
	tracking_delivery_deadline datetime,
	tracking_shipping_amount nvarchar(50),
	tracking_code nvarchar(50)
);

create table [general].AfterSaleSummarizedReverses (
	integrated_or_updated_at datetime,
	cnpj_emp nvarchar(14),
	id bigint not null primary key,
	ecommerce_order_id bigint,
	created_at datetime,
	updated_at datetime,
	customer_id bigint,
	customer_first_name nvarchar(50),
	customer_last_name nvarchar(100), 
	status_id int,
	status_name nvarchar(50), 
	status_description nvarchar(300), 
	refunds_count int,
	total_amount decimal(10, 2), 
	reverse_type nvarchar(50), 
	reverse_type_name nvarchar(50), 
	order_id nvarchar(50), 
	tracking_authorization_code nvarchar(50), 
	tracking_delivery_deadline datetime,
	tracking_shipping_amount nvarchar(50),
	tracking_code nvarchar(50)
);

create table [untreated].[AfterSaleReverses] (
	integrated_or_updated_at datetime,
	id bigint,
	reverse_type nvarchar(50),
	courier_collect bit,
	courier_service_type nvarchar(50),
	service_type_change int,
	ecommerce_order_id bigint,
	store_id nvarchar(50),
	courier_contract_id int,
	is_awaiting_resend_webhook bit,
	is_erased bit,
	total_amount decimal(10,2),
	invoice int,
	courier_data_posting_card nvarchar(50),
	courier_data_is_store_seller_contract bit,
	store_expire_date nvarchar(50),
	skip_process_step int,
	freight_by_customer bit,
	origin nvarchar(50),
	external_source nvarchar(50),
	billing_item_id bigint,
	created_at datetime,
	updated_at datetime,
	deleted_at datetime,
	[type] nvarchar(50),
	origin_seller_id nvarchar(50),
	courier_data_encrypted_posting_card nvarchar(50),
	courier_data_encrypted_is_store_seller_contract bit,
	insurance_manual_needed int,
	ecommerce_id int,
	ecommerce_uuid uniqueidentifier,
	ecommerce_company_name nvarchar(250),
	ecommerce_trade_name nvarchar(250),
	ecommerce_display_name nvarchar(250),
	ecommerce_url nvarchar(300),
	ecommerce_is_active bit,
	ecommerce_phone nvarchar(20),
	ecommerce_email nvarchar(50),
	ecommerce_document nvarchar(14), --retirar caracteres especiais no c-sharp
	ecommerce_address_id bigint,
	ecommerce_maintenance_mode_global bit,
	ecommerce_last_order_report_date datetime,
	ecommerce_is_test bit,
	billing_date datetime,
	must_treat_refund bit,
	customer_nfd_attached nvarchar(50),
	customer_nfd_xml_attached nvarchar(max),
	returned_invoice nvarchar(50),
	reverse_type_name nvarchar(50),
	is_generic_courier bit,
	can_generate_voucher bit,
	could_cancel bit,
	can_send_correction_letter bit,
	correction_letter_link nvarchar(300),
	customer_url nvarchar(300),
	timeline_url nvarchar(300),
	collect_scheduling_link nvarchar(300),
	status_id int,
	status_service_code nvarchar(50),
	status_name nvarchar(50),
	status_description nvarchar(300),
	ecommerce_order_seller_invoice_number nvarchar(20),
	ecommerce_order_seller_invoice_url nvarchar(300),
	brand_name nvarchar(50),
	retry_options_can_retry bit
);

create table [general].[AfterSaleReverses] (
	integrated_or_updated_at datetime,
	id bigint not null primary key,
	reverse_type nvarchar(50),
	courier_collect bit,
	courier_service_type nvarchar(50),
	service_type_change int,
	ecommerce_order_id bigint,
	store_id nvarchar(50),
	courier_contract_id int,
	is_awaiting_resend_webhook bit,
	is_erased bit,
	total_amount decimal(10,2),
	invoice int,
	courier_data_posting_card nvarchar(50),
	courier_data_is_store_seller_contract bit,
	store_expire_date nvarchar(50),
	skip_process_step int,
	freight_by_customer bit,
	origin nvarchar(50),
	external_source nvarchar(50),
	billing_item_id bigint,
	created_at datetime,
	updated_at datetime,
	deleted_at datetime,
	[type] nvarchar(50),
	origin_seller_id nvarchar(50),
	courier_data_encrypted_posting_card nvarchar(50),
	courier_data_encrypted_is_store_seller_contract bit,
	insurance_manual_needed int,
	ecommerce_id int,
	ecommerce_uuid uniqueidentifier,
	ecommerce_company_name nvarchar(250),
	ecommerce_trade_name nvarchar(250),
	ecommerce_display_name nvarchar(250),
	ecommerce_url nvarchar(300),
	ecommerce_is_active bit,
	ecommerce_phone nvarchar(20),
	ecommerce_email nvarchar(50),
	ecommerce_document nvarchar(14), --retirar caracteres especiais no c-sharp
	ecommerce_address_id bigint,
	ecommerce_maintenance_mode_global bit,
	ecommerce_last_order_report_date datetime,
	ecommerce_is_test bit,
	billing_date datetime,
	must_treat_refund bit,
	customer_nfd_attached nvarchar(50),
	customer_nfd_xml_attached nvarchar(max),
	returned_invoice nvarchar(50),
	reverse_type_name nvarchar(50),
	is_generic_courier bit,
	can_generate_voucher bit,
	could_cancel bit,
	can_send_correction_letter bit,
	correction_letter_link nvarchar(300),
	customer_url nvarchar(300),
	timeline_url nvarchar(300),
	collect_scheduling_link nvarchar(300),
	status_id int,
	status_service_code nvarchar(50),
	status_name nvarchar(50),
	status_description nvarchar(300),
	ecommerce_order_seller_invoice_number nvarchar(20),
	ecommerce_order_seller_invoice_url nvarchar(300),
	brand_name nvarchar(50),
	retry_options_can_retry bit
);

create table [untreated].[AfterSaleReversesCustomers] (
	integrated_or_updated_at datetime,
	reverse_id bigint,
	id bigint,
	first_name nvarchar(50),
	last_name nvarchar(250),
	email nvarchar(50),
	contact_email nvarchar(50),
	document nvarchar(14),
	phone nvarchar(20),
	contact_phone nvarchar(20),
	is_restrict bit,
	is_erased bit,
	first_name_encrypted nvarchar(50),
	last_name_encrypted nvarchar(250),
	document_encrypted nvarchar(14),
	email_encrypted nvarchar(50),
	contact_email_encrypted nvarchar(50),
	contact_phone_encrypted nvarchar(20),
	phone_encrypted nvarchar(20),
	order_email nvarchar(50),
	order_phone nvarchar(20),
	address_id bigint,
	address_zip_code nvarchar(9),
	address_address nvarchar(250),
	address_complement nvarchar(50),
	address_number nvarchar(20),
	address_number_encrypted nvarchar(20),
	address_neighborhood nvarchar(50),
	address_city nvarchar(40),
	address_state nvarchar(20),
	address_country nvarchar(80),
	address_lat nvarchar(50),
	address_long nvarchar(50),
	address_is_restrict bit,
	address_updated_at datetime,
	address_deleted_at datetime,
	address_is_erased bit,
	shipping_address_id bigint,
	shipping_address_zip_code nvarchar(9),
	shipping_address_address nvarchar(250),
	shipping_address_complement nvarchar(50),
	shipping_address_number nvarchar(20),
	shipping_address_number_encrypted nvarchar(20),
	shipping_address_neighborhood nvarchar(50),
	shipping_address_city nvarchar(40),
	shipping_address_state nvarchar(20),
	shipping_address_country nvarchar(80),
	shipping_address_lat nvarchar(50),
	shipping_address_long nvarchar(50),
	shipping_address_is_restrict bit,
	shipping_address_updated_at datetime,
	shipping_address_deleted_at datetime,
	shipping_address_is_erased bit
);

create table [general].[AfterSaleReversesCustomers] (
	integrated_or_updated_at datetime,
	reverse_id bigint not null primary key,
	id bigint not null unique,
	first_name nvarchar(50),
	last_name nvarchar(250),
	email nvarchar(50),
	contact_email nvarchar(50),
	document nvarchar(14),
	phone nvarchar(20),
	contact_phone nvarchar(20),
	is_restrict bit,
	is_erased bit,
	first_name_encrypted nvarchar(50),
	last_name_encrypted nvarchar(250),
	document_encrypted nvarchar(14),
	email_encrypted nvarchar(50),
	contact_email_encrypted nvarchar(50),
	contact_phone_encrypted nvarchar(20),
	phone_encrypted nvarchar(20),
	order_email nvarchar(50),
	order_phone nvarchar(20),
	address_id bigint,
	address_zip_code nvarchar(9),
	address_address nvarchar(250),
	address_complement nvarchar(50),
	address_number nvarchar(20),
	address_number_encrypted nvarchar(20),
	address_neighborhood nvarchar(50),
	address_city nvarchar(40),
	address_state nvarchar(20),
	address_country nvarchar(80),
	address_lat nvarchar(50),
	address_long nvarchar(50),
	address_is_restrict bit,
	address_updated_at datetime,
	address_deleted_at datetime,
	address_is_erased bit,
	shipping_address_id bigint,
	shipping_address_zip_code nvarchar(9),
	shipping_address_address nvarchar(250),
	shipping_address_complement nvarchar(50),
	shipping_address_number nvarchar(20),
	shipping_address_number_encrypted nvarchar(20),
	shipping_address_neighborhood nvarchar(50),
	shipping_address_city nvarchar(40),
	shipping_address_state nvarchar(20),
	shipping_address_country nvarchar(80),
	shipping_address_lat nvarchar(50),
	shipping_address_long nvarchar(50),
	shipping_address_is_restrict bit,
	shipping_address_updated_at datetime,
	shipping_address_deleted_at datetime,
	shipping_address_is_erased bit
);

create table [untreated].[AfterSaleReversesDestinationsData] (
	integrated_or_updated_at datetime,
	reverse_id bigint,
	id bigint, --gerar com c-sharp
	[name] nvarchar(250),
	[type] nvarchar(50),
	address_id bigint,
	address_lat nvarchar(50),
	address_city nvarchar(40),
	address_long nvarchar(50),
	address_state nvarchar(20),
	address_number nvarchar(20),
	address_address nvarchar(250),
	address_country nvarchar(80),
	address_zip_code nvarchar(9), 
	address_complement nvarchar(50),
	address_deleted_at datetime,
	address_updated_at datetime,
	address_neighborhood nvarchar(50),
	[type_id] int,
	updated_at datetime,
	seller_info nvarchar(50),
	return_to_seller bit,
	was_manually_changed bit,
	[label] nvarchar(300)
);

create table [general].[AfterSaleReversesDestinationsData] (
	integrated_or_updated_at datetime,
	reverse_id bigint not null primary key,
	id bigint not null unique,
	[name] nvarchar(250),
	[type] nvarchar(50),
	address_id bigint,
	address_lat nvarchar(50),
	address_city nvarchar(40),
	address_long nvarchar(50),
	address_state nvarchar(20),
	address_number nvarchar(20),
	address_address nvarchar(250),
	address_country nvarchar(80),
	address_zip_code nvarchar(9), 
	address_complement nvarchar(50),
	address_deleted_at datetime,
	address_updated_at datetime,
	address_neighborhood nvarchar(50),
	[type_id] int,
	updated_at datetime,
	seller_info nvarchar(50),
	return_to_seller bit,
	was_manually_changed bit,
	[label] nvarchar(300)
);

create table [untreated].[AfterSaleReversesTrackings] (
	integrated_or_updated_at datetime,
	reverse_id bigint,
	id bigint,
	courier_contract_id int,
	authorization_code nvarchar(50),
	tracking_code nvarchar(50),
	shipping_amount decimal(10,2),
	package_amount decimal(10,2),
	courier_name nvarchar(250),
	tracking_url nvarchar(300),
	expire_date datetime,
	collect_date datetime,
	requested_collect_date datetime,
	[status] nvarchar(50),
	[message] nvarchar(800),
	status_updated_at datetime,
	is_change_collect_to_post bit,
	[type] nvarchar(50),
	qr_code nvarchar(800),
	service_type nvarchar(50),
	cte nvarchar(50),
	delivery_deadline datetime,
	updated_at datetime,
	deleted_at datetime
);

create table [general].[AfterSaleReversesTrackings] (
	integrated_or_updated_at datetime,
	reverse_id bigint not null primary key,
	id bigint not null unique,
	courier_contract_id int,
	authorization_code nvarchar(50),
	tracking_code nvarchar(50),
	shipping_amount decimal(10,2),
	package_amount decimal(10,2),
	courier_name nvarchar(250),
	tracking_url nvarchar(300),
	expire_date datetime,
	collect_date datetime,
	requested_collect_date datetime,
	[status] nvarchar(50),
	[message] nvarchar(800),
	status_updated_at datetime,
	is_change_collect_to_post bit,
	[type] nvarchar(50),
	qr_code nvarchar(800),
	service_type nvarchar(50),
	cte nvarchar(50),
	delivery_deadline datetime,
	updated_at datetime,
	deleted_at datetime
);

create table [untreated].[AfterSaleReversesTrackingHistories] (
	integrated_or_updated_at datetime,
	id bigint,
	reverse_id bigint,
	tracking_id bigint,
	[status] nvarchar(50),
	[message] nvarchar(800),
	status_updated_at datetime,
	created_at datetime,
	updated_at datetime,
	deleted_at datetime
);

create table [general].[AfterSaleReversesTrackingHistories] (
	integrated_or_updated_at datetime,
	id bigint not null,
	reverse_id bigint not null,
	tracking_id bigint not null,
	[status] nvarchar(50),
	[message] nvarchar(800),
	status_updated_at datetime,
	created_at datetime,
	updated_at datetime,
	deleted_at datetime
constraint [PK_AfterSaleReversesTrackingHistories] primary key (
	id asc,
	reverse_id asc,
	tracking_id asc
));

create table [untreated].[AfterSaleReversesDestinations] (
	integrated_or_updated_at datetime,
	reverse_id bigint,
	id bigint, --gerar com c-sharp
	is_omnichannel bit,
	ie_number nvarchar(50),
	cnpj nvarchar(14), --retirar caracteres especiais no c-sharp
	display_name nvarchar(250),
	address_zip_code nvarchar(9),
	address_address nvarchar(250),
	address_number nvarchar(20),
	address_complement nvarchar(50),
	address_neighborhood nvarchar(50),
	address_city nvarchar(40),
	address_state nvarchar(20),
	address_country nvarchar(80),
	address_lat nvarchar(50),
	address_long nvarchar(50),
	address_updated_at datetime,
	address_deleted_at datetime
);

create table [general].[AfterSaleReversesDestinations] (
	integrated_or_updated_at datetime,
	reverse_id bigint not null primary key,
	id bigint not null unique,
	is_omnichannel bit,
	ie_number nvarchar(50),
	cnpj nvarchar(14), --retirar caracteres especiais no c-sharp
	display_name nvarchar(250),
	address_zip_code nvarchar(9),
	address_address nvarchar(250),
	address_number nvarchar(20),
	address_complement nvarchar(50),
	address_neighborhood nvarchar(50),
	address_city nvarchar(40),
	address_state nvarchar(20),
	address_country nvarchar(80),
	address_lat nvarchar(50),
	address_long nvarchar(50),
	address_updated_at datetime,
	address_deleted_at datetime
);

create table [untreated].[AfterSaleReversesEcommerceOrders] (
	integrated_or_updated_at datetime,
	reverse_id bigint,
	order_id nvarchar(50),
	id bigint,
	invoice_number nvarchar(50),
	uuid uniqueidentifier,
	shipping_value decimal(10, 2),
	ordered_at datetime,
	invoiced_at datetime,
	delivered_at datetime,
	[value] decimal(10, 2),
	ecommerce_id bigint,
	customer_id bigint,
	is_online bit,
	is_marketplace bit,
	marketplace_name nvarchar(250),
	updated_at datetime,
	deleted_at datetime,
	is_erased bit,
	is_restrict bit
);

create table [general].[AfterSaleReversesEcommerceOrders] (
	integrated_or_updated_at datetime,
	reverse_id bigint not null primary key,
	order_id nvarchar(50),
	id bigint not null unique,
	invoice_number nvarchar(50),
	uuid uniqueidentifier,
	shipping_value decimal(10, 2),
	ordered_at datetime,
	invoiced_at datetime,
	delivered_at datetime,
	[value] decimal(10, 2),
	ecommerce_id bigint,
	customer_id bigint,
	is_online bit,
	is_marketplace bit,
	marketplace_name nvarchar(250),
	updated_at datetime,
	deleted_at datetime,
	is_erased bit,
	is_restrict bit
);

create table [untreated].[AfterSaleReversesEcommerceOrdersPaymentsMethods] (
	integrated_or_updated_at datetime,
	reverse_id bigint,
	ecommerce_order_id bigint,
	ecommerce_order_payment_id bigint, --gerar com incremental c-sharp
	payment_method nvarchar(50)
);

create table [general].[AfterSaleReversesEcommerceOrdersPaymentsMethods] (
	integrated_or_updated_at datetime,
	reverse_id bigint not null,
	ecommerce_order_id bigint not null,
	ecommerce_order_payment_id bigint not null, --gerar com incremental c-sharp
	payment_method nvarchar(50),
constraint [PK_AfterSaleReversesEcommerceOrdersPaymentsMethods] primary key (
	reverse_id asc,
	ecommerce_order_payment_id asc,
	ecommerce_order_id asc
));

create table [untreated].[AfterSaleReversesProducts](
	integrated_or_updated_at datetime,
	id bigint,
    reverse_id bigint,
    motive_id bigint,
	reason_id bigint,
	reason_ecommerce_id int,
	reason_description nvarchar(250),
	reason_action nvarchar(50),
	reason_should_approve bit,
	reason_upload_image nvarchar(50),
	reason_show_product_grid bit,
	reason_ord int,
	reason_created_at datetime,
	reason_updated_at datetime,
	reason_deleted_at datetime,
	subreason_id int,
	subreason_reason_id int,
	subreason_description nvarchar(250),
	subreason_created_at datetime,
	subreason_updated_at datetime,
	subreason_deleted_at datetime,
    ecommerce_order_product_id bigint,
    refund_id bigint,
    qty int,
    requested_qty int,
    received_qty int,
    product_received_comment int,
    comments nvarchar(800),
    reverse_action nvarchar(50),
    customer_retention_method_id nvarchar(50),
    protocol nvarchar(50),
	is_included_by_admin bit,
    product_id nvarchar(50),
    [hash] nvarchar(50),
    [name] nvarchar(250),
    sku nvarchar(50),
    reference nvarchar(50),
    price decimal(10,2),
    selling_price decimal(10,2),
    shipping_price decimal(10,2),
    brand nvarchar(50),
    [weight] decimal(10, 2),
    order_id bigint,
    returned_invoice nvarchar(50),
    returned_invoice_url nvarchar(300),
    returned_invoice_url_xml nvarchar(max),
    is_bundle bit,
    invoice nvarchar(50),
    original_qty int
);

create table [general].[AfterSaleReversesProducts](
	integrated_or_updated_at datetime,
	id bigint not null,
    reverse_id bigint not null,
    motive_id bigint,
	reason_id bigint,
	reason_ecommerce_id int,
	reason_description nvarchar(250),
	reason_action nvarchar(50),
	reason_should_approve bit,
	reason_upload_image nvarchar(50),
	reason_show_product_grid bit,
	reason_ord int,
	reason_created_at datetime,
	reason_updated_at datetime,
	reason_deleted_at datetime,
	subreason_id int,
	subreason_reason_id int,
	subreason_description nvarchar(250),
	subreason_created_at datetime,
	subreason_updated_at datetime,
	subreason_deleted_at datetime,
    ecommerce_order_product_id bigint,
    refund_id bigint,
    qty int,
    requested_qty int,
    received_qty int,
    product_received_comment int,
    comments nvarchar(800),
    reverse_action nvarchar(50),
    customer_retention_method_id nvarchar(50),
    protocol nvarchar(50),
	is_included_by_admin bit,
    product_id nvarchar(50),
    [hash] nvarchar(50),
    [name] nvarchar(250),
    sku nvarchar(50),
    reference nvarchar(50),
    price decimal(10,2),
    selling_price decimal(10,2),
    shipping_price decimal(10,2),
    brand nvarchar(50),
    [weight] decimal(10, 2),
    order_id bigint,
    returned_invoice nvarchar(50),
    returned_invoice_url nvarchar(300),
    returned_invoice_url_xml nvarchar(max),
    is_bundle bit,
    invoice nvarchar(50),
    original_qty int
constraint [PK_AfterSaleReversesProducts] primary key (
	id asc,
	reverse_id asc
));

create table [untreated].[AfterSaleReversesProductsImages](
	integrated_or_updated_at datetime,
	id bigint,
	reverse_id bigint,
	reverse_product_id bigint,
	product_id nvarchar(50),
	image_path nvarchar(300),
	image_url nvarchar(300)
);

create table [general].[AfterSaleReversesProductsImages](
	integrated_or_updated_at datetime,
	id bigint not null,
	reverse_id bigint not null,
	reverse_product_id bigint not null,
	product_id nvarchar(50),
	image_path nvarchar(300),
	image_url nvarchar(300)
constraint [PK_AfterSaleReversesProductsImages] primary key (
	id asc,
	reverse_id asc,
	reverse_product_id asc
));

create table [untreated].[AfterSaleReversesStatusHistories](
	integrated_or_updated_at datetime,
	id bigint,
    reverse_id bigint,
    customer_id bigint,
    [date] datetime,
    comments nvarchar(300),
    billing_message nvarchar(800),

	status_id bigint,
	status_service_code nvarchar(50),
	status_name nvarchar(250),
	status_description nvarchar(300),

	[user_id] bigint,
	[user_name] nvarchar(50),
	user_first_name nvarchar(50),
	user_last_name nvarchar(250),
	user_email nvarchar(50),
	user_mobile_phone nvarchar(20),
	user_phone nvarchar(20),
	user_document nvarchar(14),
	user_wallet nvarchar(50)
);

create table [general].[AfterSaleReversesStatusHistories](
	integrated_or_updated_at datetime,
	id bigint not null,
    reverse_id bigint not null,
    customer_id bigint,
    [date] datetime,
    comments nvarchar(300),
    billing_message nvarchar(800),
	status_id bigint,
	status_service_code nvarchar(50),
	status_name nvarchar(250),
	status_description nvarchar(300),
	[user_id] bigint,
	[user_name] nvarchar(50),
	user_first_name nvarchar(50),
	user_last_name nvarchar(250),
	user_email nvarchar(50),
	user_mobile_phone nvarchar(20),
	user_phone nvarchar(20),
	user_document nvarchar(14),
	user_wallet nvarchar(50),
constraint [PK_AfterSaleReversesStatusHistories] primary key (
	id asc,
	reverse_id asc
));

alter table [general].AfterSaleCourierAttributesFields add constraint [FK_AfterSaleCourierAttributesFields] foreign key (reverse_id) references [general].[AfterSaleCourierAttributes] (reverse_id);
alter table [general].[AfterSaleReversesCustomers] add constraint [FK_AfterSaleReversesCustomers] foreign key (reverse_id) references [general].[AfterSaleReverses] (id);
alter table [general].[AfterSaleReversesDestinationsData] add constraint [FK_AfterSaleReversesDestinationsData] foreign key (reverse_id) references [general].[AfterSaleReverses] (id);
alter table [general].[AfterSaleReversesTrackings] add constraint [FK_AfterSaleReversesTrackings] foreign key (reverse_id) references [general].[AfterSaleReverses] (id);
alter table [general].[AfterSaleReversesTrackingHistories] add constraint [FK_AfterSaleReversesTrackingHistories] foreign key (tracking_id) references [general].[AfterSaleReversesTrackings] (id);
alter table [general].[AfterSaleReversesDestinations] add constraint [FK_AfterSaleReversesDestinations] foreign key (reverse_id) references [general].[AfterSaleReverses] (id);
alter table [general].[AfterSaleReversesEcommerceOrders] add constraint [FK_AfterSaleReversesEcommerceOrders] foreign key (reverse_id) references [general].[AfterSaleReverses] (id);
alter table [general].[AfterSaleReversesEcommerceOrdersPaymentsMethods] add constraint [FK_AfterSaleReversesEcommerceOrdersPaymentsMethods] foreign key (ecommerce_order_id) references [general].[AfterSaleReversesEcommerceOrders] (id);
alter table [general].[AfterSaleReversesProducts] add constraint [FK_AfterSaleReversesProducts] foreign key (reverse_id) references [general].[AfterSaleReverses] (id);
alter table [general].[AfterSaleReversesProductsImages] add constraint [FK_AfterSaleReversesProductsImages] foreign key (reverse_product_id, reverse_id) references [general].[AfterSaleReversesProducts] (id, reverse_id);
alter table [general].[AfterSaleReversesStatusHistories] add constraint [FK_AfterSaleReversesStatusHistories] foreign key (reverse_id) references [general].[AfterSaleReverses] (id);