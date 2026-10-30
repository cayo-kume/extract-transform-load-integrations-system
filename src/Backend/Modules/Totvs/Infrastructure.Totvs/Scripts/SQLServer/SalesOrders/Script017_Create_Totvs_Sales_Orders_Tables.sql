/****** Pedidos de venda: tabelas untreated e totvs (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.1 e 5.2.6) ******/
--tamanhos do swagger da API Sales Order; objetos 1:1 como colunas do pai com o nome do objeto como prefixo (5.2.8).

create table [untreated].TotvsSaleOrders (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	orderId                              nvarchar(100),
	customerOrderCode                    nvarchar(100),
	insertDate                           datetime,
	integrationCode                      nvarchar(100),
	guideCode                            int,
	guideCpfCnpj                         nvarchar(14),
	sellerCode                           int,
	sellerCpfCnpj                        nvarchar(14),
	orderDate                            datetime,
	maxChangeFilterDate                  datetime,
	arrivalDate                          datetime,
	customerCode                         int,
	customerCpfCnpj                      nvarchar(14),
	customerName                         nvarchar(60),
	representativeCode                   int,
	representativeCpfCnpj                nvarchar(14),
	representativeName                   nvarchar(60),
	operationCode                        int,
	operationName                        nvarchar(80),
	paymentConditionCode                 int,
	paymentConditionName                 nvarchar(40),
	[quantity]                           decimal(18,6),
	grossValue                           decimal(18,6),
	discountValue                        decimal(18,6),
	netValue                             decimal(18,6),
	freightType                          nvarchar(30),
	freightValue                         decimal(18,6),
	shippingCompanyCode                  int,
	shippingCompanyCpfCnpj               nvarchar(14),
	shippingCompanyName                  nvarchar(60),
	redispatchingFreightType             nvarchar(30),
	redispatchingShippingCompanyCode     int,
	redispatchingShippingCompanyCpfCnpj  nvarchar(14),
	redispatchingShippingCompanyName     nvarchar(60),
	totalAmountOrder                     decimal(18,6),
	statusOrder                          nvarchar(30),
	reasonBlockingCode                   int,
	reasonBlockingDescription            nvarchar(60),
	reasonCancellationCode               int,
	reasonCancellationDescription        nvarchar(60),
	omniSiteId                           int,
	additionalValue                      decimal(18,6),
	experienceType                       nvarchar(30),
	hasPdvTransaction                    bit,
	hasFinancialProcessed                bit,
	priceTableCode                       int,
	shippingService                      nvarchar(10),
	shippingServiceName                  nvarchar(200),
	cancellationLimitDate                datetime,
	billingForecastDate                  datetime,
	billingLimitDate                     datetime,
	lastBillingDate                      datetime,
	paymentBaseDate                      datetime,
	batchOrder                           nvarchar(10),
	rollMeasureCode                      int,
	rollMeasureDescription               nvarchar(80),
	ecommerceStage                       nvarchar(40),
	shippingAddressAddressType           nvarchar(20),
	shippingAddressPublicPlace           nvarchar(10),
	shippingAddressAddress               nvarchar(60),
	shippingAddressNumber                int,
	shippingAddressComplement            nvarchar(65),
	shippingAddressAddressSequence       int,
	shippingAddressNeighborhood          nvarchar(65),
	shippingAddressIbgeCityCode          int,
	shippingAddressCityName              nvarchar(60),
	shippingAddressStateAbbreviation     nvarchar(3),
	shippingAddressCep                   nvarchar(8),
	shippingAddressBcbCountryCode        int,
	shippingAddressCountryName           nvarchar(60),
	shippingAddressPostOfficeBox         int,
	shippingAddressReference             nvarchar(60)
);

create table [totvs].TotvsSaleOrders (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	orderId                              nvarchar(100),
	customerOrderCode                    nvarchar(100),
	insertDate                           datetime,
	integrationCode                      nvarchar(100),
	guideCode                            int,
	guideCpfCnpj                         nvarchar(14),
	sellerCode                           int,
	sellerCpfCnpj                        nvarchar(14),
	orderDate                            datetime,
	maxChangeFilterDate                  datetime,
	arrivalDate                          datetime,
	customerCode                         int,
	customerCpfCnpj                      nvarchar(14),
	customerName                         nvarchar(60),
	representativeCode                   int,
	representativeCpfCnpj                nvarchar(14),
	representativeName                   nvarchar(60),
	operationCode                        int,
	operationName                        nvarchar(80),
	paymentConditionCode                 int,
	paymentConditionName                 nvarchar(40),
	[quantity]                           decimal(18,6),
	grossValue                           decimal(18,6),
	discountValue                        decimal(18,6),
	netValue                             decimal(18,6),
	freightType                          nvarchar(30),
	freightValue                         decimal(18,6),
	shippingCompanyCode                  int,
	shippingCompanyCpfCnpj               nvarchar(14),
	shippingCompanyName                  nvarchar(60),
	redispatchingFreightType             nvarchar(30),
	redispatchingShippingCompanyCode     int,
	redispatchingShippingCompanyCpfCnpj  nvarchar(14),
	redispatchingShippingCompanyName     nvarchar(60),
	totalAmountOrder                     decimal(18,6),
	statusOrder                          nvarchar(30),
	reasonBlockingCode                   int,
	reasonBlockingDescription            nvarchar(60),
	reasonCancellationCode               int,
	reasonCancellationDescription        nvarchar(60),
	omniSiteId                           int,
	additionalValue                      decimal(18,6),
	experienceType                       nvarchar(30),
	hasPdvTransaction                    bit,
	hasFinancialProcessed                bit,
	priceTableCode                       int,
	shippingService                      nvarchar(10),
	shippingServiceName                  nvarchar(200),
	cancellationLimitDate                datetime,
	billingForecastDate                  datetime,
	billingLimitDate                     datetime,
	lastBillingDate                      datetime,
	paymentBaseDate                      datetime,
	batchOrder                           nvarchar(10),
	rollMeasureCode                      int,
	rollMeasureDescription               nvarchar(80),
	ecommerceStage                       nvarchar(40),
	shippingAddressAddressType           nvarchar(20),
	shippingAddressPublicPlace           nvarchar(10),
	shippingAddressAddress               nvarchar(60),
	shippingAddressNumber                int,
	shippingAddressComplement            nvarchar(65),
	shippingAddressAddressSequence       int,
	shippingAddressNeighborhood          nvarchar(65),
	shippingAddressIbgeCityCode          int,
	shippingAddressCityName              nvarchar(60),
	shippingAddressStateAbbreviation     nvarchar(3),
	shippingAddressCep                   nvarchar(8),
	shippingAddressBcbCountryCode        int,
	shippingAddressCountryName           nvarchar(60),
	shippingAddressPostOfficeBox         int,
	shippingAddressReference             nvarchar(60),
constraint [PK_TotvsSaleOrders] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc
));

create table [untreated].TotvsSaleOrdersProducts (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	productCode                          int,
	[name]                               nvarchar(60),
	referenceCode                        nvarchar(60),
	referenceName                        nvarchar(100),
	productSku                           nvarchar(40),
	colorCode                            nvarchar(10),
	colorName                            nvarchar(40),
	sizeName                             nvarchar(10),
	[quantity]                           decimal(18,6),
	toSettleQuantity                     decimal(18,6),
	settledQuantity                      decimal(18,6),
	canceledQuantity                     decimal(18,6),
	extraQuantity                        decimal(18,6),
	pendingQuantity                      decimal(18,6),
	originalPrice                        decimal(18,6),
	price                                decimal(18,6),
	discountPercentage                   decimal(18,6),
	lastChangeUserInsert                 int,
	lastChangeDate                       datetime,
	billingForecastDate                  datetime,
	insertDate                           datetime,
	cancellationLimitDate                datetime,
	billingLimitDate                     datetime,
	lastBillingDate                      datetime,
	rollMeasureCode                      int,
	rollMeasureDescription               nvarchar(80),
	customerOrderCode                    int,
	customerProductCode                  nvarchar(20)
);

create table [totvs].TotvsSaleOrdersProducts (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	productCode                          int not null,
	[name]                               nvarchar(60),
	referenceCode                        nvarchar(60),
	referenceName                        nvarchar(100),
	productSku                           nvarchar(40),
	colorCode                            nvarchar(10),
	colorName                            nvarchar(40),
	sizeName                             nvarchar(10),
	[quantity]                           decimal(18,6),
	toSettleQuantity                     decimal(18,6),
	settledQuantity                      decimal(18,6),
	canceledQuantity                     decimal(18,6),
	extraQuantity                        decimal(18,6),
	pendingQuantity                      decimal(18,6),
	originalPrice                        decimal(18,6),
	price                                decimal(18,6),
	discountPercentage                   decimal(18,6),
	lastChangeUserInsert                 int,
	lastChangeDate                       datetime,
	billingForecastDate                  datetime,
	insertDate                           datetime,
	cancellationLimitDate                datetime,
	billingLimitDate                     datetime,
	lastBillingDate                      datetime,
	rollMeasureCode                      int,
	rollMeasureDescription               nvarchar(80),
	customerOrderCode                    int,
	customerProductCode                  nvarchar(20),
constraint [PK_TotvsSaleOrdersProducts] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	productCode asc
));

create table [untreated].TotvsSaleOrdersProductsBatchItems (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	productCode                          int,
	[sequence]                           int,
	batchBranchCode                      int,
	[code]                               int,
	[quantity]                           decimal(18,6)
);

create table [totvs].TotvsSaleOrdersProductsBatchItems (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	productCode                          int not null,
	[sequence]                           int not null,
	batchBranchCode                      int,
	[code]                               int,
	[quantity]                           decimal(18,6),
constraint [PK_TotvsSaleOrdersProductsBatchItems] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	productCode asc,
	[sequence] asc
));

create table [untreated].TotvsSaleOrdersInvoices (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	[code]                               int,
	serial                               nvarchar(3),
	accessKey                            nvarchar(200),
	issueDate                            datetime,
	[status]                             nvarchar(20),
	shippingCompanyName                  nvarchar(60),
	packageNumber                        int,
	grossWeight                          decimal(18,6),
	netWeight                            decimal(18,6),
	trackingCode                         nvarchar(40),
	discountPercentage                   decimal(18,6),
	[quantity]                           decimal(18,6),
	productValue                         decimal(18,6),
	additionalValue                      decimal(18,6),
	shippingValue                        decimal(18,6),
	insuranceValue                       decimal(18,6),
	ipiValue                             decimal(18,6),
	totalValue                           decimal(18,6),
	transactionBranchCode                int,
	transactionDate                      datetime,
	transactionCode                      int,
	electronicAccessKey                  nvarchar(200),
	electronicElectronicInvoiceStatus    nvarchar(20),
	electronicReceipt                    bigint,
	electronicReceivementDate            datetime
);

create table [totvs].TotvsSaleOrdersInvoices (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	[code]                               int not null,
	serial                               nvarchar(3) not null,
	accessKey                            nvarchar(200),
	issueDate                            datetime,
	[status]                             nvarchar(20),
	shippingCompanyName                  nvarchar(60),
	packageNumber                        int,
	grossWeight                          decimal(18,6),
	netWeight                            decimal(18,6),
	trackingCode                         nvarchar(40),
	discountPercentage                   decimal(18,6),
	[quantity]                           decimal(18,6),
	productValue                         decimal(18,6),
	additionalValue                      decimal(18,6),
	shippingValue                        decimal(18,6),
	insuranceValue                       decimal(18,6),
	ipiValue                             decimal(18,6),
	totalValue                           decimal(18,6),
	transactionBranchCode                int,
	transactionDate                      datetime,
	transactionCode                      int,
	electronicAccessKey                  nvarchar(200),
	electronicElectronicInvoiceStatus    nvarchar(20),
	electronicReceipt                    bigint,
	electronicReceivementDate            datetime,
constraint [PK_TotvsSaleOrdersInvoices] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	[code] asc,
	serial asc
));

create table [untreated].TotvsSaleOrdersObservations (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	[sequence]                           int,
	observation                          nvarchar(80)
);

create table [totvs].TotvsSaleOrdersObservations (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	[sequence]                           int not null,
	observation                          nvarchar(80),
constraint [PK_TotvsSaleOrdersObservations] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	[sequence] asc
));

create table [untreated].TotvsSaleOrdersInvoiceObservations (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	[sequence]                           int,
	observation                          nvarchar(80)
);

create table [totvs].TotvsSaleOrdersInvoiceObservations (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	[sequence]                           int not null,
	observation                          nvarchar(80),
constraint [PK_TotvsSaleOrdersInvoiceObservations] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	[sequence] asc
));

create table [untreated].TotvsSaleOrdersRepresentativeObservations (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	[sequence]                           int,
	observation                          nvarchar(80),
	visualizationType                    nvarchar(30)
);

create table [totvs].TotvsSaleOrdersRepresentativeObservations (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	[sequence]                           int not null,
	observation                          nvarchar(80),
	visualizationType                    nvarchar(30),
constraint [PK_TotvsSaleOrdersRepresentativeObservations] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	[sequence] asc
));

create table [untreated].TotvsSaleOrdersClassifications (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	typeCode                             int,
	[code]                               nvarchar(10),
	typeName                             nvarchar(40),
	[name]                               nvarchar(40)
);

create table [totvs].TotvsSaleOrdersClassifications (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	typeCode                             int not null,
	[code]                               nvarchar(10) not null,
	typeName                             nvarchar(40),
	[name]                               nvarchar(40),
constraint [PK_TotvsSaleOrdersClassifications] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	typeCode asc,
	[code] asc
));

create table [untreated].TotvsSaleOrdersDiscounts (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	typeDiscountCode                     int,
	[description]                        nvarchar(40),
	discountPercentage                   decimal(18,6),
	discountValue                        decimal(18,6)
);

create table [totvs].TotvsSaleOrdersDiscounts (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	typeDiscountCode                     int not null,
	[description]                        nvarchar(40),
	discountPercentage                   decimal(18,6),
	discountValue                        decimal(18,6),
constraint [PK_TotvsSaleOrdersDiscounts] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	typeDiscountCode asc
));

create table [untreated].TotvsSaleOrdersCommissioneds (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	representativeCode                   int,
	representativeName                   nvarchar(60),
	commissionPercentageBilling          decimal(18,6),
	commissionPercentageReceipt          decimal(18,6),
	commissionedType                     nvarchar(30)
);

create table [totvs].TotvsSaleOrdersCommissioneds (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	representativeCode                   int not null,
	representativeName                   nvarchar(60),
	commissionPercentageBilling          decimal(18,6),
	commissionPercentageReceipt          decimal(18,6),
	commissionedType                     nvarchar(30),
constraint [PK_TotvsSaleOrdersCommissioneds] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	representativeCode asc
));

create table [untreated].TotvsSaleOrdersCounts (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	orderCode                            int,
	countBranchCode                      int,
	[code]                               int,
	[description]                        nvarchar(40),
	statusCount                          nvarchar(20),
	packagingTypeCode                    int,
	packagingTypeDescription             nvarchar(60),
	[separator]                          int,
	separatorName                        nvarchar(60)
);

create table [totvs].TotvsSaleOrdersCounts (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	orderCode                            int not null,
	countBranchCode                      int not null,
	[code]                               int not null,
	[description]                        nvarchar(40),
	statusCount                          nvarchar(20),
	packagingTypeCode                    int,
	packagingTypeDescription             nvarchar(60),
	[separator]                          int,
	separatorName                        nvarchar(60),
constraint [PK_TotvsSaleOrdersCounts] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	countBranchCode asc,
	[code] asc
));

/****** Chaves estrangeiras ******/
--cascade só nas netas: um item que sai do pedido leva junto os itens de lote (o merge dos itens roda antes)

alter table [totvs].TotvsSaleOrdersProducts add constraint [FK_TotvsSaleOrdersProducts] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsSaleOrders (doc_company, branchCode, orderCode);
alter table [totvs].TotvsSaleOrdersProductsBatchItems add constraint [FK_TotvsSaleOrdersProductsBatchItems] foreign key (doc_company, branchCode, orderCode, productCode) references [totvs].TotvsSaleOrdersProducts (doc_company, branchCode, orderCode, productCode) on delete cascade;
alter table [totvs].TotvsSaleOrdersInvoices add constraint [FK_TotvsSaleOrdersInvoices] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsSaleOrders (doc_company, branchCode, orderCode);
alter table [totvs].TotvsSaleOrdersObservations add constraint [FK_TotvsSaleOrdersObservations] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsSaleOrders (doc_company, branchCode, orderCode);
alter table [totvs].TotvsSaleOrdersInvoiceObservations add constraint [FK_TotvsSaleOrdersInvoiceObservations] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsSaleOrders (doc_company, branchCode, orderCode);
alter table [totvs].TotvsSaleOrdersRepresentativeObservations add constraint [FK_TotvsSaleOrdersRepresentativeObservations] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsSaleOrders (doc_company, branchCode, orderCode);
alter table [totvs].TotvsSaleOrdersClassifications add constraint [FK_TotvsSaleOrdersClassifications] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsSaleOrders (doc_company, branchCode, orderCode);
alter table [totvs].TotvsSaleOrdersDiscounts add constraint [FK_TotvsSaleOrdersDiscounts] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsSaleOrders (doc_company, branchCode, orderCode);
alter table [totvs].TotvsSaleOrdersCommissioneds add constraint [FK_TotvsSaleOrdersCommissioneds] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsSaleOrders (doc_company, branchCode, orderCode);
alter table [totvs].TotvsSaleOrdersCounts add constraint [FK_TotvsSaleOrdersCounts] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsSaleOrders (doc_company, branchCode, orderCode);

go

/****** Catálogos e sugestões de faturamento da API Sales Order: tabelas untreated e totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §7.1 e §7.2, fase SO-1) ******/
--tamanhos do swagger da API Sales Order; os sem tamanho seguem as outras tabelas. Lidos pela janela de alteração (T9)

create table [untreated].TotvsSaleDiscountTypes (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	[code]                              int,
	[description]                       nvarchar(100),
	percentage                          decimal(18,6),
	maxChangeFilterDate                 datetime
);

create table [totvs].TotvsSaleDiscountTypes (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	[code]                              int not null,
	[description]                       nvarchar(100),
	percentage                          decimal(18,6),
	maxChangeFilterDate                 datetime,
constraint [PK_TotvsSaleDiscountTypes] primary key (
	doc_company asc,
	[code] asc
));

create table [untreated].TotvsSaleClassifications (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	typeCode                            int,
	typeDescription                     nvarchar(40),
	[type]                              nvarchar(20),
	[code]                              nvarchar(10),
	[description]                       nvarchar(40),
	maxChangeFilterDate                 datetime
);

create table [totvs].TotvsSaleClassifications (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	typeCode                            int not null,
	typeDescription                     nvarchar(40),
	[type]                              nvarchar(20),
	[code]                              nvarchar(10) not null,
	[description]                       nvarchar(40),
	maxChangeFilterDate                 datetime,
constraint [PK_TotvsSaleClassifications] primary key (
	doc_company asc,
	typeCode asc,
	[code] asc
));

create table [untreated].TotvsSaleOrderBatches (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	[code]                              nvarchar(10),
	[description]                       nvarchar(60),
	[status]                            int,
	startDate                           datetime,
	endDate                             datetime,
	closingDate                         datetime,
	maxChangeFilterDate                 datetime
);

create table [totvs].TotvsSaleOrderBatches (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	[code]                              nvarchar(10) not null,
	[description]                       nvarchar(60),
	[status]                            int,
	startDate                           datetime,
	endDate                             datetime,
	closingDate                         datetime,
	maxChangeFilterDate                 datetime,
constraint [PK_TotvsSaleOrderBatches] primary key (
	doc_company asc,
	[code] asc
));

create table [untreated].TotvsBillingSuggestions (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	suggestionCode                      int,
	statusSuggestion                    int,
	suggestionDate                      datetime,
	maxChangeFilterDate                 datetime
);

create table [totvs].TotvsBillingSuggestions (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	suggestionCode                      int not null,
	statusSuggestion                    int,
	suggestionDate                      datetime,
	maxChangeFilterDate                 datetime,
constraint [PK_TotvsBillingSuggestions] primary key (
	doc_company asc,
	branchCode asc,
	suggestionCode asc
));

create table [untreated].TotvsBillingSuggestionsOrders (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	suggestionCode                      int,
	orderBranchCode                     int,
	orderCode                           int,
	[quantity]                          decimal(18,6),
	suggestedQuantity                   decimal(18,6),
	pendingQuantity                     decimal(18,6),
	[value]                             decimal(18,6),
	suggestedValue                      decimal(18,6),
	pendingValue                        decimal(18,6)
);

create table [totvs].TotvsBillingSuggestionsOrders (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	suggestionCode                      int not null,
	orderBranchCode                     int not null,
	orderCode                           int not null,
	[quantity]                          decimal(18,6),
	suggestedQuantity                   decimal(18,6),
	pendingQuantity                     decimal(18,6),
	[value]                             decimal(18,6),
	suggestedValue                      decimal(18,6),
	pendingValue                        decimal(18,6),
constraint [PK_TotvsBillingSuggestionsOrders] primary key (
	doc_company asc,
	branchCode asc,
	suggestionCode asc,
	orderBranchCode asc,
	orderCode asc
));

create table [untreated].TotvsBillingSuggestionsOrdersItems (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	suggestionCode                      int,
	orderBranchCode                     int,
	orderCode                           int,
	productCode                         int,
	[description]                       nvarchar(60),
	standardBarCode                     nvarchar(40),
	sizeName                            nvarchar(10),
	colorName                           nvarchar(40),
	[quantity]                          decimal(18,6),
	suggestedQuantity                   decimal(18,6),
	pendingQuantity                     decimal(18,6),
	[price]                             decimal(18,6)
);

create table [totvs].TotvsBillingSuggestionsOrdersItems (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	suggestionCode                      int not null,
	orderBranchCode                     int not null,
	orderCode                           int not null,
	productCode                         int not null,
	[description]                       nvarchar(60),
	standardBarCode                     nvarchar(40),
	sizeName                            nvarchar(10),
	colorName                           nvarchar(40),
	[quantity]                          decimal(18,6),
	suggestedQuantity                   decimal(18,6),
	pendingQuantity                     decimal(18,6),
	[price]                             decimal(18,6),
constraint [PK_TotvsBillingSuggestionsOrdersItems] primary key (
	doc_company asc,
	branchCode asc,
	suggestionCode asc,
	orderBranchCode asc,
	orderCode asc,
	productCode asc
));

/****** Chaves estrangeiras ******/
alter table [totvs].TotvsBillingSuggestionsOrders add constraint [FK_TotvsBillingSuggestionsOrders] foreign key (doc_company, branchCode, suggestionCode) references [totvs].TotvsBillingSuggestions (doc_company, branchCode, suggestionCode);
alter table [totvs].TotvsBillingSuggestionsOrdersItems add constraint [FK_TotvsBillingSuggestionsOrdersItems] foreign key (doc_company, branchCode, suggestionCode, orderBranchCode, orderCode) references [totvs].TotvsBillingSuggestionsOrders (doc_company, branchCode, suggestionCode, orderBranchCode, orderCode);
