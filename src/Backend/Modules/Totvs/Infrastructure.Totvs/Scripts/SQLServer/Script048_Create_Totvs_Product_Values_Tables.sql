/****** Preços, custos e saldos: tabelas untreated e totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9, fase PR-2) ******/
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

create table [untreated].TotvsBalances (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	productCode                         int,
	stockCode                           int,
	stockDescription                    nvarchar(60),
	[stock]                             decimal(18,6),
	salesOrder                          decimal(18,6),
	inputTransaction                    decimal(18,6),
	outputTransaction                   decimal(18,6),
	productionPlanning                  decimal(18,6),
	purchaseOrder                       decimal(18,6),
	productionOrderProgress             decimal(18,6),
	productionOrderWaitLib              decimal(18,6),
	stockTemp                           decimal(18,6),
	maxChangeFilterDate                 datetime
);

create table [totvs].TotvsBalances (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	productCode                         int not null,
	stockCode                           int not null,
	stockDescription                    nvarchar(60),
	[stock]                             decimal(18,6),
	salesOrder                          decimal(18,6),
	inputTransaction                    decimal(18,6),
	outputTransaction                   decimal(18,6),
	productionPlanning                  decimal(18,6),
	purchaseOrder                       decimal(18,6),
	productionOrderProgress             decimal(18,6),
	productionOrderWaitLib              decimal(18,6),
	stockTemp                           decimal(18,6),
	maxChangeFilterDate                 datetime,
constraint [PK_TotvsBalances] primary key (
	doc_company asc,
	branchCode asc,
	productCode asc,
	stockCode asc
));

create table [untreated].TotvsBalancesLocations (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	productCode                         int,
	locationCode                        nvarchar(20),
	[description]                       nvarchar(60)
);

create table [totvs].TotvsBalancesLocations (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	productCode                         int not null,
	locationCode                        nvarchar(20) not null,
	[description]                       nvarchar(60),
constraint [PK_TotvsBalancesLocations] primary key (
	doc_company asc,
	branchCode asc,
	productCode asc,
	locationCode asc
));

/****** Chaves estrangeiras ******/
alter table [totvs].TotvsPricesOtherPromotions add constraint [FK_TotvsPricesOtherPromotions] foreign key (doc_company, branchCode, productCode, priceCode) references [totvs].TotvsPrices (doc_company, branchCode, productCode, priceCode);
--TotvsBalancesLocations é do produto na loja, não de um tipo de saldo: sem FK (a chave de TotvsBalances tem o stockCode)
