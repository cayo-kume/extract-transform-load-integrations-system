/****** Similares e lotes: tabelas untreated e totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.2, fase PR-2) ******/
--sem data de alteração útil nas respostas: comparados pelo json_hash (T9). Os similares guardam em changeWindowEnd o fim da
--janela que os trouxe, de onde a próxima leitura começa; os lotes são lidos inteiros

create table [untreated].TotvsProductsSimilar (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	productCode                         int,
	groupSequenceCode                   int,
	changeWindowEnd                     datetime,
	json_hash                           nvarchar(64)
);

create table [totvs].TotvsProductsSimilar (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	productCode                         int not null,
	groupSequenceCode                   int,
	changeWindowEnd                     datetime,
	json_hash                           nvarchar(64),
constraint [PK_TotvsProductsSimilar] primary key (
	doc_company asc,
	productCode asc
));

create table [untreated].TotvsProductsSimilarItems (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	productCode                         int,
	listType                            nvarchar(20),
	similarProductCode                  int
);

create table [totvs].TotvsProductsSimilarItems (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	productCode                         int not null,
	listType                            nvarchar(20) not null,
	similarProductCode                  int not null,
constraint [PK_TotvsProductsSimilarItems] primary key (
	doc_company asc,
	productCode asc,
	listType asc,
	similarProductCode asc
));

create table [untreated].TotvsBatches (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	batchCode                           int,
	productCode                         int,
	batchProvider                       nvarchar(40),
	batchClient                         nvarchar(40),
	batchManufacturingDate              datetime,
	batchExpirationDate                 datetime,
	batchType                           nvarchar(30),
	json_hash                           nvarchar(64)
);

create table [totvs].TotvsBatches (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	batchCode                           int not null,
	productCode                         int not null,
	batchProvider                       nvarchar(40),
	batchClient                         nvarchar(40),
	batchManufacturingDate              datetime,
	batchExpirationDate                 datetime,
	batchType                           nvarchar(30),
	json_hash                           nvarchar(64),
constraint [PK_TotvsBatches] primary key (
	doc_company asc,
	branchCode asc,
	batchCode asc,
	productCode asc
));

create table [untreated].TotvsBatchesItems (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	batchCode                           int,
	productCode                         int,
	batchItemSequential                 int,
	[balance]                           decimal(18,6),
	quantityItem                        decimal(18,6),
	quantityIssued                      decimal(18,6),
	quantityTare                        decimal(18,6),
	quantityWeigth                      decimal(18,6),
	quantityMeters                      decimal(18,6),
	quantityCanceled                    decimal(18,6),
	quantityYield                       decimal(18,6),
	itemProvider                        nvarchar(40),
	itemClient                          nvarchar(40),
	locationBatch                       nvarchar(5),
	[status]                            nvarchar(30),
	conditioningType                    nvarchar(10),
	nuance                              nvarchar(20)
);

create table [totvs].TotvsBatchesItems (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	batchCode                           int not null,
	productCode                         int not null,
	batchItemSequential                 int not null,
	[balance]                           decimal(18,6),
	quantityItem                        decimal(18,6),
	quantityIssued                      decimal(18,6),
	quantityTare                        decimal(18,6),
	quantityWeigth                      decimal(18,6),
	quantityMeters                      decimal(18,6),
	quantityCanceled                    decimal(18,6),
	quantityYield                       decimal(18,6),
	itemProvider                        nvarchar(40),
	itemClient                          nvarchar(40),
	locationBatch                       nvarchar(5),
	[status]                            nvarchar(30),
	conditioningType                    nvarchar(10),
	nuance                              nvarchar(20),
constraint [PK_TotvsBatchesItems] primary key (
	doc_company asc,
	branchCode asc,
	batchCode asc,
	productCode asc,
	batchItemSequential asc
));

create table [untreated].TotvsBatchesItemsAdditionalFields (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	batchCode                           int,
	productCode                         int,
	batchItemSequential                 int,
	[code]                              int,
	[description]                       nvarchar(60),
	[value]                             nvarchar(250)
);

create table [totvs].TotvsBatchesItemsAdditionalFields (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	batchCode                           int not null,
	productCode                         int not null,
	batchItemSequential                 int not null,
	[code]                              int not null,
	[description]                       nvarchar(60),
	[value]                             nvarchar(250),
constraint [PK_TotvsBatchesItemsAdditionalFields] primary key (
	doc_company asc,
	branchCode asc,
	batchCode asc,
	productCode asc,
	batchItemSequential asc,
	[code] asc
));

/****** Chaves estrangeiras ******/
alter table [totvs].TotvsProductsSimilarItems add constraint [FK_TotvsProductsSimilarItems] foreign key (doc_company, productCode) references [totvs].TotvsProductsSimilar (doc_company, productCode);
alter table [totvs].TotvsBatchesItems add constraint [FK_TotvsBatchesItems] foreign key (doc_company, branchCode, batchCode, productCode) references [totvs].TotvsBatches (doc_company, branchCode, batchCode, productCode);
alter table [totvs].TotvsBatchesItemsAdditionalFields add constraint [FK_TotvsBatchesItemsAdditionalFields] foreign key (doc_company, branchCode, batchCode, productCode, batchItemSequential) references [totvs].TotvsBatchesItems (doc_company, branchCode, batchCode, productCode, batchItemSequential);
