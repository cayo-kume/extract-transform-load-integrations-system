/****** Tabelas de preço (itens) e promoções: tabelas untreated e totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.2, fase PR-2) ******/
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
