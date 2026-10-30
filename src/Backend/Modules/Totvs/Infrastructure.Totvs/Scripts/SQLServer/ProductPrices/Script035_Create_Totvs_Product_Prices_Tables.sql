/****** Preços e custos: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9, fase PR-2) ******/
--uma linha por loja + produto + código (todos os códigos de preço, custo e saldo do cadastro). branchCode vem antes do produto
--na chave: a janela, a carga inicial e o cache de estado consultam por loja. Os dados do produto já estão em TotvsSkus.

create table [untreated].TotvsPrices (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	productCode                         int,
	priceCode                           int,
	priceName                           nvarchar(60),
	[price]                             decimal(18,6),
	promotionalPrice                    decimal(18,6),
	promotionalInformationBranchCode    int,
	promotionalInformationCode          int,
	promotionalInformationDescription   nvarchar(40),
	promotionalInformationStartDate     datetime,
	promotionalInformationEndDate       datetime,
	maxChangeFilterDate                 datetime
);

create table [totvs].TotvsPrices (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	productCode                         int not null,
	priceCode                           int not null,
	priceName                           nvarchar(60),
	[price]                             decimal(18,6),
	promotionalPrice                    decimal(18,6),
	promotionalInformationBranchCode    int,
	promotionalInformationCode          int,
	promotionalInformationDescription   nvarchar(40),
	promotionalInformationStartDate     datetime,
	promotionalInformationEndDate       datetime,
	maxChangeFilterDate                 datetime,
constraint [PK_TotvsPrices] primary key (
	doc_company asc,
	branchCode asc,
	productCode asc,
	priceCode asc
));

create table [untreated].TotvsPricesOtherPromotions (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	productCode                         int,
	priceCode                           int,
	promotionBranchCode                 int,
	promotionCode                       int,
	[description]                       nvarchar(40),
	startDate                           datetime,
	endDate                             datetime,
	[price]                             decimal(18,6)
);

create table [totvs].TotvsPricesOtherPromotions (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	productCode                         int not null,
	priceCode                           int not null,
	promotionBranchCode                 int not null,
	promotionCode                       int not null,
	[description]                       nvarchar(40),
	startDate                           datetime,
	endDate                             datetime,
	[price]                             decimal(18,6),
constraint [PK_TotvsPricesOtherPromotions] primary key (
	doc_company asc,
	branchCode asc,
	productCode asc,
	priceCode asc,
	promotionBranchCode asc,
	promotionCode asc
));

create table [untreated].TotvsCosts (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	productCode                         int,
	costCode                            int,
	costName                            nvarchar(60),
	[cost]                              decimal(18,6),
	maxChangeFilterDate                 datetime
);

create table [totvs].TotvsCosts (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	productCode                         int not null,
	costCode                            int not null,
	costName                            nvarchar(60),
	[cost]                              decimal(18,6),
	maxChangeFilterDate                 datetime,
constraint [PK_TotvsCosts] primary key (
	doc_company asc,
	branchCode asc,
	productCode asc,
	costCode asc
));

/****** Chaves estrangeiras ******/
alter table [totvs].TotvsPricesOtherPromotions add constraint [FK_TotvsPricesOtherPromotions] foreign key (doc_company, branchCode, productCode, priceCode) references [totvs].TotvsPrices (doc_company, branchCode, productCode, priceCode);

go

/****** Tabelas de preço (itens) e promoções: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.2, fase PR-2) ******/
--itens das tabelas de PEDFM011 (o cabeçalho é a TotvsPriceTables, do Script045) e itens de promoção de PRDFM002/PRDFM319

create table [untreated].TotvsPriceTablesItems (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	priceTableCode                       int,
	productCode                          int,
	referenceId                          int,
	maxChangeFilterDate                  datetime
);

create table [totvs].TotvsPriceTablesItems (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	priceTableCode                       int not null,
	productCode                          int not null,
	referenceId                          int,
	maxChangeFilterDate                  datetime,
constraint [PK_TotvsPriceTablesItems] primary key (
	doc_company asc,
	priceTableCode asc,
	productCode asc
));

create table [untreated].TotvsPriceTablesItemsPrices (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	priceTableCode                       int,
	productCode                          int,
	branchCode                           int,
	scaleCode                            int,
	originalPrice                        decimal(18,6),
	[price]                              decimal(18,6)
);

create table [totvs].TotvsPriceTablesItemsPrices (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	priceTableCode                       int not null,
	productCode                          int not null,
	branchCode                           int not null,
	scaleCode                            int not null,
	originalPrice                        decimal(18,6),
	[price]                              decimal(18,6),
constraint [PK_TotvsPriceTablesItemsPrices] primary key (
	doc_company asc,
	priceTableCode asc,
	productCode asc,
	branchCode asc,
	scaleCode asc
));

create table [untreated].TotvsPromotions (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14),
	branchCode                           int,
	promotionCode                        int,
	productCode                          int,
	promotionDescription                 nvarchar(40),
	startDate                            datetime,
	endDate                              datetime,
	previousValue                        decimal(18,6),
	promotionValue                       decimal(18,6),
	[status]                             nvarchar(10),
	maxChangeFilterDate                  datetime,
	json_hash                            nvarchar(64)
);

create table [totvs].TotvsPromotions (
	integrated_or_updated_at             datetime,
	doc_company                          nvarchar(14) not null,
	branchCode                           int not null,
	promotionCode                        int not null,
	productCode                          int not null,
	promotionDescription                 nvarchar(40),
	startDate                            datetime,
	endDate                              datetime,
	previousValue                        decimal(18,6),
	promotionValue                       decimal(18,6),
	[status]                             nvarchar(10),
	maxChangeFilterDate                  datetime,
	json_hash                            nvarchar(64),
constraint [PK_TotvsPromotions] primary key (
	doc_company asc,
	branchCode asc,
	promotionCode asc,
	productCode asc
));

/****** Chaves estrangeiras ******/
alter table [totvs].TotvsPriceTablesItems add constraint [FK_TotvsPriceTablesItems] foreign key (doc_company, priceTableCode) references [totvs].TotvsPriceTables (doc_company, [code]);
alter table [totvs].TotvsPriceTablesItemsPrices add constraint [FK_TotvsPriceTablesItemsPrices] foreign key (doc_company, priceTableCode, productCode) references [totvs].TotvsPriceTablesItems (doc_company, priceTableCode, productCode);
