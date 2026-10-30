/****** Pedidos de compra: tabelas untreated e totvs (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.6) ******/
--o swagger da API Purchase Order não traz tamanhos: os do código antigo, conferidos numa amostra real; objetos 1:1 como colunas do pai com o nome do objeto como prefixo (5.2.8).

create table [untreated].TotvsPurchaseOrders (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchCode                int,
	orderCode                 int,
	supplierOrderCode         nvarchar(60),
	supplierCode              int,
	supplierCpfCnpj           nvarchar(14),
	supplierName              nvarchar(100),
	buyerCode                 int,
	buyerCpfCnpj              nvarchar(14),
	buyerName                 nvarchar(100),
	operationCode             int,
	operationName             nvarchar(100),
	shippingCompanyName       nvarchar(100),
	paymentConditionCode      int,
	paymentConditionName      nvarchar(100),
	paymentType               nvarchar(30),
	[status]                  nvarchar(30),
	freightType               nvarchar(30),
	maxChangeFilterDate       datetime,
	registrationDate          datetime,
	deliveryForecastDate      datetime,
	deliveryDeadlineDate      datetime,
	basePaymentDate           datetime,
	productValue              decimal(18,6),
	[quantity]                decimal(18,6),
	ipiValue                  decimal(18,6),
	totalAmountOrder          decimal(18,6)
);

create table [totvs].TotvsPurchaseOrders (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchCode                int not null,
	orderCode                 int not null,
	supplierOrderCode         nvarchar(60),
	supplierCode              int,
	supplierCpfCnpj           nvarchar(14),
	supplierName              nvarchar(100),
	buyerCode                 int,
	buyerCpfCnpj              nvarchar(14),
	buyerName                 nvarchar(100),
	operationCode             int,
	operationName             nvarchar(100),
	shippingCompanyName       nvarchar(100),
	paymentConditionCode      int,
	paymentConditionName      nvarchar(100),
	paymentType               nvarchar(30),
	[status]                  nvarchar(30),
	freightType               nvarchar(30),
	maxChangeFilterDate       datetime,
	registrationDate          datetime,
	deliveryForecastDate      datetime,
	deliveryDeadlineDate      datetime,
	basePaymentDate           datetime,
	productValue              decimal(18,6),
	[quantity]                decimal(18,6),
	ipiValue                  decimal(18,6),
	totalAmountOrder          decimal(18,6),
constraint [PK_TotvsPurchaseOrders] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc
));

create table [untreated].TotvsPurchaseOrdersProducts (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchCode                int,
	orderCode                 int,
	productCode               int,
	[name]                    nvarchar(100),
	referenceCode             nvarchar(60),
	referenceName             nvarchar(100),
	productSku                nvarchar(60),
	colorCode                 nvarchar(20),
	colorName                 nvarchar(60),
	sizeName                  nvarchar(20),
	measuredUnit              nvarchar(10),
	[quantity]                decimal(18,6),
	lowQuantity               decimal(18,6),
	settledQuantity           decimal(18,6),
	canceledQuantity          decimal(18,6),
	extraQuantity             decimal(18,6),
	deliveryForecastDate      datetime,
	deliveryDeadlineDate      datetime,
	[value]                   decimal(18,6),
	discountPercentage        decimal(18,6),
	conversionUnit            nvarchar(10),
	conversionQuantity        decimal(18,6)
);

create table [totvs].TotvsPurchaseOrdersProducts (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchCode                int not null,
	orderCode                 int not null,
	productCode               int not null,
	[name]                    nvarchar(100),
	referenceCode             nvarchar(60),
	referenceName             nvarchar(100),
	productSku                nvarchar(60),
	colorCode                 nvarchar(20),
	colorName                 nvarchar(60),
	sizeName                  nvarchar(20),
	measuredUnit              nvarchar(10),
	[quantity]                decimal(18,6),
	lowQuantity               decimal(18,6),
	settledQuantity           decimal(18,6),
	canceledQuantity          decimal(18,6),
	extraQuantity             decimal(18,6),
	deliveryForecastDate      datetime,
	deliveryDeadlineDate      datetime,
	[value]                   decimal(18,6),
	discountPercentage        decimal(18,6),
	conversionUnit            nvarchar(10),
	conversionQuantity        decimal(18,6),
constraint [PK_TotvsPurchaseOrdersProducts] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	productCode asc
));

create table [untreated].TotvsPurchaseOrdersRequests (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchCode                int,
	orderCode                 int,
	requestBranchCode         int,
	requestCode               int,
	requestDate               datetime
);

create table [totvs].TotvsPurchaseOrdersRequests (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchCode                int not null,
	orderCode                 int not null,
	requestBranchCode         int not null,
	requestCode               int not null,
	requestDate               datetime,
constraint [PK_TotvsPurchaseOrdersRequests] primary key (
	doc_company asc,
	branchCode asc,
	orderCode asc,
	requestBranchCode asc,
	requestCode asc
));

/****** Chaves estrangeiras ******/
--cascade só nas netas: um item que sai do pedido leva junto os itens de lote (o merge dos itens roda antes)

alter table [totvs].TotvsPurchaseOrdersProducts add constraint [FK_TotvsPurchaseOrdersProducts] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsPurchaseOrders (doc_company, branchCode, orderCode);
alter table [totvs].TotvsPurchaseOrdersRequests add constraint [FK_TotvsPurchaseOrdersRequests] foreign key (doc_company, branchCode, orderCode) references [totvs].TotvsPurchaseOrders (doc_company, branchCode, orderCode);
