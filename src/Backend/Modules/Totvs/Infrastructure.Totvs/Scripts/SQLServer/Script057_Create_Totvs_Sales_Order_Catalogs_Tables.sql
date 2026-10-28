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
