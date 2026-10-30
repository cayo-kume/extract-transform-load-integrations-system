create table [untreated].AfterSaleRefunds (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	id bigint,
	[type] nvarchar(50),
	[action] nvarchar(50),
	bonus_amount decimal(10, 2),
	bonus_amount_percent decimal(10, 2),
	requested_shipping_amount decimal(10, 2),
	shipping_method nvarchar(50),
	added_taxes_amount decimal(10, 2),
	requested_raw_amount decimal(10, 2),
	requested_amount decimal(10, 2),
	requested_total_amount decimal(10, 2),
	received_amount decimal(10, 2),
	received_raw_amount decimal(10, 2),
	total_amount decimal(10, 2),
	free_shipping bit,
	created_at datetime,
	updated_at datetime,
	refunded_shipping_type nvarchar(50),
	can_edit_wire_transfer bit,
	can_edit_email bit,
	has_wire_transfer_account bit,
	customer_retention_method_id nvarchar(50),
	external_order_url nvarchar(300),
	comments_count int,
	enable_payment bit,
	insurance_amount decimal(10, 2),
	customer_id bigint,
	customer_first_name nvarchar(50),
	customer_last_name nvarchar(100),
	customer_email nvarchar(50),
	customer_document nvarchar(14),
	status_id int,
	status_name nvarchar(50),
	status_description nvarchar(150),
	seller_info nvarchar(50),
	cashback_account nvarchar(50),
	voucher_redemption_code nvarchar(50),
	voucher_giftcard_id nvarchar(50),
	voucher_expiring_date datetime,
	should_ask_voucher_code bit,
	reverse_updated_at datetime,
	reverse_reverse_type nvarchar(50),
	reverse_status_id int,
	reverse_status_service_code nvarchar(50),
	reverse_status_name nvarchar(50),
	reverse_status_description nvarchar(150),
	reverse_id bigint,
	reverse_created_at datetime,
	reverse_refunds_count int,
	reverse_ecommerce_order_id bigint,
	reverse_ecommerce_order nvarchar(50),
	reverse_ecommerce_order_seller_invoice_number nvarchar(50),
	reverse_ecommerce_order_seller_invoice_url nvarchar(300),
	product_price_type nvarchar(50),
	product_price_type_label nvarchar(50)
);

create table [aftersale].AfterSaleRefunds (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	id bigint not null,
	[type] nvarchar(50),
	[action] nvarchar(50),
	bonus_amount decimal(10, 2),
	bonus_amount_percent decimal(10, 2),
	requested_shipping_amount decimal(10, 2),
	shipping_method nvarchar(50),
	added_taxes_amount decimal(10, 2),
	requested_raw_amount decimal(10, 2),
	requested_amount decimal(10, 2),
	requested_total_amount decimal(10, 2),
	received_amount decimal(10, 2),
	received_raw_amount decimal(10, 2),
	total_amount decimal(10, 2),
	free_shipping bit,
	created_at datetime,
	updated_at datetime,
	refunded_shipping_type nvarchar(50),
	can_edit_wire_transfer bit,
	can_edit_email bit,
	has_wire_transfer_account bit,
	customer_retention_method_id nvarchar(50),
	external_order_url nvarchar(300),
	comments_count int,
	enable_payment bit,
	insurance_amount decimal(10, 2),
	customer_id bigint,
	customer_first_name nvarchar(50),
	customer_last_name nvarchar(100),
	customer_email nvarchar(50),
	customer_document nvarchar(14),
	status_id int,
	status_name nvarchar(50),
	status_description nvarchar(150),
	seller_info nvarchar(50),
	cashback_account nvarchar(50),
	voucher_redemption_code nvarchar(50),
	voucher_giftcard_id nvarchar(50),
	voucher_expiring_date datetime,
	should_ask_voucher_code bit,
	reverse_updated_at datetime,
	reverse_reverse_type nvarchar(50),
	reverse_status_id int,
	reverse_status_service_code nvarchar(50),
	reverse_status_name nvarchar(50),
	reverse_status_description nvarchar(150),
	reverse_id bigint,
	reverse_created_at datetime,
	reverse_refunds_count int,
	reverse_ecommerce_order_id bigint,
	reverse_ecommerce_order nvarchar(50),
	reverse_ecommerce_order_seller_invoice_number nvarchar(50),
	reverse_ecommerce_order_seller_invoice_url nvarchar(300),
	product_price_type nvarchar(50),
	product_price_type_label nvarchar(50),
constraint [PK_AfterSaleRefunds] primary key (
	doc_company asc,
	id asc
));

create table [untreated].AfterSaleRefundsCouldChangeStatusFor (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	refund_id bigint,
	id int, --gerado no c-sharp (1, 2, 3...) na ordem da lista da API
	[description] nvarchar(50)
);

create table [aftersale].AfterSaleRefundsCouldChangeStatusFor (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	refund_id bigint not null,
	id int not null, --gerado no c-sharp (1, 2, 3...) na ordem da lista da API
	[description] nvarchar(50),
constraint [PK_AfterSaleRefundsCouldChangeStatusFor] primary key (
	doc_company asc,
	refund_id asc,
	id asc
));

create table [untreated].AfterSaleRefundsOrdersPaymentsMethods (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	refund_id bigint,
	id int, --gerado no c-sharp (1, 2, 3...) na ordem da lista da API
	payment_method nvarchar(50)
);

create table [aftersale].AfterSaleRefundsOrdersPaymentsMethods (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	refund_id bigint not null,
	id int not null, --gerado no c-sharp (1, 2, 3...) na ordem da lista da API
	payment_method nvarchar(50),
constraint [PK_AfterSaleRefundsOrdersPaymentsMethods] primary key (
	doc_company asc,
	refund_id asc,
	id asc
));

create table [untreated].AfterSaleRefundsStatusHistories (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	refund_id bigint,
	status_id int,
	status_service_code nvarchar(50),
	status_name nvarchar(50),
	status_description nvarchar(50),
	[date] datetime,
	comments nvarchar(300),
	[user_id] bigint,
	[user_name] nvarchar(50),
	user_first_name nvarchar(50),
	user_last_name nvarchar(100),
	user_email nvarchar(50),
	user_mobile_phone nvarchar(20),
	user_phone nvarchar(20),
	user_document nvarchar(14),
	user_wallet nvarchar(50)
);

--o mesmo status pode se repetir no histórico: a data entra na chave
create table [aftersale].AfterSaleRefundsStatusHistories (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	refund_id bigint not null,
	status_id int not null,
	status_service_code nvarchar(50),
	status_name nvarchar(50),
	status_description nvarchar(50),
	[date] datetime not null,
	comments nvarchar(300),
	[user_id] bigint,
	[user_name] nvarchar(50),
	user_first_name nvarchar(50),
	user_last_name nvarchar(100),
	user_email nvarchar(50),
	user_mobile_phone nvarchar(20),
	user_phone nvarchar(20),
	user_document nvarchar(14),
	user_wallet nvarchar(50),
constraint [PK_AfterSaleRefundsStatusHistories] primary key (
	doc_company asc,
	refund_id asc,
	status_id asc,
	[date] asc
));

create table [untreated].AfterSaleRefundsTotalAmountHistories (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	refund_id bigint,
	id bigint,
	[user_id] bigint,
	user_first_name nvarchar(50),
	user_last_name nvarchar(100),
	total_amount decimal(10, 2),
	[date] datetime,
	[type_id] int,
	[type_name] nvarchar(50),
	type_description nvarchar(150)
);

create table [aftersale].AfterSaleRefundsTotalAmountHistories (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	refund_id bigint not null,
	id bigint not null,
	[user_id] bigint,
	user_first_name nvarchar(50),
	user_last_name nvarchar(100),
	total_amount decimal(10, 2),
	[date] datetime,
	[type_id] int,
	[type_name] nvarchar(50),
	type_description nvarchar(150),
constraint [PK_AfterSaleRefundsTotalAmountHistories] primary key (
	doc_company asc,
	refund_id asc,
	id asc
));

create table [untreated].AfterSaleRefundsProducts (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	refund_id bigint,
	id bigint,
	reverse_id bigint,
	motive_id bigint,
	reason_id bigint,
	reason_ecommerce_id bigint,
	reason_description nvarchar(150),
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
	subreason_description nvarchar(150),
	subreason_created_at datetime,
	subreason_updated_at datetime,
	subreason_deleted_at datetime,
	ecommerce_order_product_id bigint,
	qty int,
	requested_qty int,
	received_qty int,
	product_received_comment nvarchar(300),
	comments nvarchar(300),
	reverse_action nvarchar(50),
	customer_retention_method_id int,
	protocol nvarchar(50),
	is_included_by_admin bit,
	is_bundle bit,
	product_id nvarchar(50),
	[hash] nvarchar(50),
	[name] nvarchar(300),
	sku nvarchar(50),
	reference nvarchar(50),
	brand nvarchar(50),
	situation nvarchar(50),
	bundle_hash nvarchar(50),
	price decimal(10, 2),
	selling_price decimal(10, 2),
	shipping_price decimal(10, 2),
	[weight] decimal(10, 2),
	order_id bigint,
	returned_invoice nvarchar(50),
	returned_invoice_url nvarchar(300),
	returned_invoice_url_xml nvarchar(300),
	invoice nvarchar(50),
	original_qty int
);

create table [aftersale].AfterSaleRefundsProducts (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	refund_id bigint not null,
	id bigint not null,
	reverse_id bigint,
	motive_id bigint,
	reason_id bigint,
	reason_ecommerce_id bigint,
	reason_description nvarchar(150),
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
	subreason_description nvarchar(150),
	subreason_created_at datetime,
	subreason_updated_at datetime,
	subreason_deleted_at datetime,
	ecommerce_order_product_id bigint,
	qty int,
	requested_qty int,
	received_qty int,
	product_received_comment nvarchar(300),
	comments nvarchar(300),
	reverse_action nvarchar(50),
	customer_retention_method_id int,
	protocol nvarchar(50),
	is_included_by_admin bit,
	is_bundle bit,
	product_id nvarchar(50),
	[hash] nvarchar(50),
	[name] nvarchar(300),
	sku nvarchar(50),
	reference nvarchar(50),
	brand nvarchar(50),
	situation nvarchar(50),
	bundle_hash nvarchar(50),
	price decimal(10, 2),
	selling_price decimal(10, 2),
	shipping_price decimal(10, 2),
	[weight] decimal(10, 2),
	order_id bigint,
	returned_invoice nvarchar(50),
	returned_invoice_url nvarchar(300),
	returned_invoice_url_xml nvarchar(300),
	invoice nvarchar(50),
	original_qty int,
constraint [PK_AfterSaleRefundsProducts] primary key (
	doc_company asc,
	refund_id asc,
	id asc
));

create table [untreated].AfterSaleRefundsProductsImages (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	refund_id bigint,
	refund_product_id bigint,
	id bigint,
	product_id nvarchar(50),
	image_path nvarchar(300),
	image_url nvarchar(300)
);

--refund_product_id: id do produto do reembolso dono da imagem (product_id é o código do produto vindo da API)
create table [aftersale].AfterSaleRefundsProductsImages (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	refund_id bigint not null,
	refund_product_id bigint not null,
	id bigint not null,
	product_id nvarchar(50),
	image_path nvarchar(300),
	image_url nvarchar(300),
constraint [PK_AfterSaleRefundsProductsImages] primary key (
	doc_company asc,
	refund_id asc,
	refund_product_id asc,
	id asc
));

create table [untreated].AfterSaleRefundsProductsImagesUrls (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	refund_id bigint,
	product_id bigint,
	id bigint, --gerado no c-sharp (1, 2, 3...) na ordem da lista da API
	[url] nvarchar(300)
);

--product_id: id do produto do reembolso dono da imagem
create table [aftersale].AfterSaleRefundsProductsImagesUrls (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	refund_id bigint not null,
	product_id bigint not null,
	id bigint not null, --gerado no c-sharp (1, 2, 3...) na ordem da lista da API
	[url] nvarchar(300),
constraint [PK_AfterSaleRefundsProductsImagesUrls] primary key (
	doc_company asc,
	refund_id asc,
	product_id asc,
	id asc
));

alter table [aftersale].AfterSaleRefundsCouldChangeStatusFor add constraint [FK_AfterSaleRefundsCouldChangeStatusFor] foreign key (doc_company, refund_id) references [aftersale].AfterSaleRefunds (doc_company, id);
alter table [aftersale].AfterSaleRefundsOrdersPaymentsMethods add constraint [FK_AfterSaleRefundsOrdersPaymentsMethods] foreign key (doc_company, refund_id) references [aftersale].AfterSaleRefunds (doc_company, id);
alter table [aftersale].AfterSaleRefundsStatusHistories add constraint [FK_AfterSaleRefundsStatusHistories] foreign key (doc_company, refund_id) references [aftersale].AfterSaleRefunds (doc_company, id);
alter table [aftersale].AfterSaleRefundsTotalAmountHistories add constraint [FK_AfterSaleRefundsTotalAmountHistories] foreign key (doc_company, refund_id) references [aftersale].AfterSaleRefunds (doc_company, id);
alter table [aftersale].AfterSaleRefundsProducts add constraint [FK_AfterSaleRefundsProducts] foreign key (doc_company, refund_id) references [aftersale].AfterSaleRefunds (doc_company, id);
alter table [aftersale].AfterSaleRefundsProductsImages add constraint [FK_AfterSaleRefundsProductsImages] foreign key (doc_company, refund_id, refund_product_id) references [aftersale].AfterSaleRefundsProducts (doc_company, refund_id, id);
alter table [aftersale].AfterSaleRefundsProductsImagesUrls add constraint [FK_AfterSaleRefundsProductsImagesUrls] foreign key (doc_company, refund_id, product_id) references [aftersale].AfterSaleRefundsProducts (doc_company, refund_id, id);

create table [untreated].AfterSaleSummarizedRefunds (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	id bigint,
	reverse_id bigint,
	reverse_partner_store nvarchar(50),
	[action] nvarchar(50),
	order_id nvarchar(50),
	ecommerce_order_id bigint,
	[type] nvarchar(50),
	requested_amount decimal(10, 2),
	bonus_amount decimal(10, 2),
	bonus_amount_percent decimal(10, 2),
	requested_shipping_amount decimal(10, 2),
	shipping_method nvarchar(50),
	requested_raw_amount decimal(10, 2),
	requested_total_amount decimal(10, 2),
	total_amount decimal(10, 2),
	created_at datetime,
	updated_at datetime,
	refund_attachment nvarchar(50),
	customer_id bigint,
	customer_first_name nvarchar(50),
	customer_last_name nvarchar(100), 
	status_description nvarchar(50), 
	status_name nvarchar(50), 
	status_id int,
	product_price_type nvarchar(50), 
	product_price_type_label nvarchar(50), 
	refunded_shipping_type nvarchar(50),
	last_status_history_date datetime
);

create table [aftersale].AfterSaleSummarizedRefunds (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	id bigint not null,
	reverse_id bigint,
	reverse_partner_store nvarchar(50),
	[action] nvarchar(50),
	order_id nvarchar(50),
	ecommerce_order_id bigint,
	[type] nvarchar(50),
	requested_amount decimal(10, 2),
	bonus_amount decimal(10, 2),
	bonus_amount_percent decimal(10, 2),
	requested_shipping_amount decimal(10, 2),
	shipping_method nvarchar(50),
	requested_raw_amount decimal(10, 2),
	requested_total_amount decimal(10, 2),
	total_amount decimal(10, 2),
	created_at datetime,
	updated_at datetime,
	refund_attachment nvarchar(50),
	customer_id bigint,
	customer_first_name nvarchar(50),
	customer_last_name nvarchar(100), 
	status_description nvarchar(50), 
	status_name nvarchar(50), 
	status_id int,
	product_price_type nvarchar(50), 
	product_price_type_label nvarchar(50), 
	refunded_shipping_type nvarchar(50),
	last_status_history_date datetime,
constraint [PK_AfterSaleSummarizedRefunds] primary key (
	doc_company asc,
	id asc
));

create table [untreated].AfterSaleStatusRefunds (
	integrated_or_updated_at datetime,
	id int,
	[doc_company] nvarchar(14),
	[name] nvarchar(50),
	[description] nvarchar(250)
);

create table [aftersale].AfterSaleStatusRefunds (
	integrated_or_updated_at datetime,
	id int not null,
	[doc_company] nvarchar(14) not null,
	[name] nvarchar(50),
	[description] nvarchar(250),
constraint [PK_AfterSaleStatusRefunds] primary key (
	id asc,
	doc_company asc
));

create table [untreated].AfterSaleActionsRefunds (
	integrated_or_updated_at datetime,
	id int,
	[doc_company] nvarchar(14),
	[name] nvarchar(50),
	[description] nvarchar(250)
);

create table [aftersale].AfterSaleActionsRefunds (
	integrated_or_updated_at datetime,
	id int not null,
	[doc_company] nvarchar(14) not null,
	[name] nvarchar(50),
	[description] nvarchar(250),
constraint [PK_AfterSaleActionsRefunds] primary key (
	id asc,
	doc_company asc
));

create table [untreated].AfterSaleBanksRefunds (
	integrated_or_updated_at datetime,
	id int,
	[doc_company] nvarchar(14),
	[name] nvarchar(250),
	[description] nvarchar(250)
);

create table [aftersale].AfterSaleBanksRefunds (
	integrated_or_updated_at datetime,
	id int not null,
	[doc_company] nvarchar(14) not null,
	[name] nvarchar(250),
	[description] nvarchar(250),
constraint [PK_AfterSaleBanksRefunds] primary key (
	id asc,
	doc_company asc
));

create table [untreated].AfterSaleTypesRefunds (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14),
	[name] nvarchar(50),
	[description] nvarchar(250)
);

--a API não devolve id para os tipos de reembolso: a chave é o nome dentro da empresa
create table [aftersale].AfterSaleTypesRefunds (
	integrated_or_updated_at datetime,
	[doc_company] nvarchar(14) not null,
	[name] nvarchar(50) not null,
	[description] nvarchar(250),
constraint [PK_AfterSaleTypesRefunds] primary key (
	doc_company asc,
	[name] asc
));
