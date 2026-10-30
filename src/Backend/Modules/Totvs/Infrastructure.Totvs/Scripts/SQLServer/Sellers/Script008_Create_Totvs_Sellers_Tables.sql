/****** Vendedores: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.3) ******/
--o swagger da API Seller não traz tamanhos: valem os do código antigo

create table [untreated].TotvsSellers (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	sellerCode               int,
	sellerName               nvarchar(250),
	personCode               int,
	personName               nvarchar(250),
	maxChangeFilterDate      datetime
);

create table [totvs].TotvsSellers (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	sellerCode               int not null,
	sellerName               nvarchar(250),
	personCode               int,           --veio nulo nos vendedores testados
	personName               nvarchar(250),
	maxChangeFilterDate      datetime,      --maior alteração do vendedor ou das informações por empresa
constraint [PK_TotvsSellers] primary key (
	doc_company asc,
	sellerCode asc
));

create table [untreated].TotvsSellersBranchInformations (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	sellerCode               int,
	branchCode               int,
	auxiliaryCode            int,
	isInactive               bit,
	sellerTypeCode           int,
	sellerTypeDescription    nvarchar(60)
);

create table [totvs].TotvsSellersBranchInformations (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	sellerCode               int not null,
	branchCode               int not null,  --só vêm as empresas enviadas no branchCodeList (todas as de TotvsBranches)
	auxiliaryCode            int,
	isInactive               bit,
	sellerTypeCode           int,
	sellerTypeDescription    nvarchar(60),
constraint [PK_TotvsSellersBranchInformations] primary key (
	doc_company asc,
	sellerCode asc,
	branchCode asc
));

create table [untreated].TotvsSellersCustomers (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	sellerCode               int,
	personCode               int,
	[document]               nvarchar(14),
	[name]                   nvarchar(250)
);

create table [totvs].TotvsSellersCustomers (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	sellerCode               int not null,
	personCode               int not null,
	[document]               nvarchar(14),  --CPF ou CNPJ do cliente
	[name]                   nvarchar(250),
constraint [PK_TotvsSellersCustomers] primary key (
	doc_company asc,
	sellerCode asc,
	personCode asc
));

/****** Chaves estrangeiras: filhas -> vendedor ******/

alter table [totvs].TotvsSellersBranchInformations add constraint [FK_TotvsSellersBranchInformations] foreign key (doc_company, sellerCode) references [totvs].TotvsSellers (doc_company, sellerCode);
alter table [totvs].TotvsSellersCustomers add constraint [FK_TotvsSellersCustomers] foreign key (doc_company, sellerCode) references [totvs].TotvsSellers (doc_company, sellerCode);

go

/****** Regiões de atuação: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §4.1, fase SE-1) ******/
--chave: doc_company + operationalAreaCode; as filhas (estados, CEPs e cidades) levam o código da região. Sem tamanho no swagger
--para os textos. Sem maxChangeFilterDate: a leitura é inteira, e json_hash diz se a região mudou (T9).

create table [untreated].TotvsOperationalAreas (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	operationalAreaCode               int,
	operationalAreaName               nvarchar(100),
	operationalAreaType               nvarchar(30),
	operationalAreaGroupCode          int,
	operationalAreaGroupDescription   nvarchar(100),
	numberOfDays                      int,
	json_hash                         nvarchar(64)
);

create table [totvs].TotvsOperationalAreas (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	operationalAreaCode               int not null,
	operationalAreaName               nvarchar(100),
	operationalAreaType               nvarchar(30),
	operationalAreaGroupCode          int,
	operationalAreaGroupDescription   nvarchar(100),
	numberOfDays                      int,
	json_hash                         nvarchar(64),
constraint [PK_TotvsOperationalAreas] primary key (
	doc_company asc,
	operationalAreaCode asc
));

create table [untreated].TotvsOperationalAreasStates (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	operationalAreaCode               int,
	stateCode                         int,
	stateName                         nvarchar(60)
);

create table [totvs].TotvsOperationalAreasStates (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	operationalAreaCode               int not null,
	stateCode                         int not null,
	stateName                         nvarchar(60),
constraint [PK_TotvsOperationalAreasStates] primary key (
	doc_company asc,
	operationalAreaCode asc,
	stateCode asc
));

create table [untreated].TotvsOperationalAreasCeps (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	operationalAreaCode               int,
	cep                               nvarchar(10)
);

create table [totvs].TotvsOperationalAreasCeps (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	operationalAreaCode               int not null,
	cep                               nvarchar(10) not null,
constraint [PK_TotvsOperationalAreasCeps] primary key (
	doc_company asc,
	operationalAreaCode asc,
	cep asc
));

create table [untreated].TotvsOperationalAreasCities (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	operationalAreaCode               int,
	cityCode                          int,
	cityName                          nvarchar(100),
	stateCode                         int,
	stateAbbreviation                 nvarchar(2),
	stateName                         nvarchar(60)
);

create table [totvs].TotvsOperationalAreasCities (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	operationalAreaCode               int not null,
	cityCode                          int not null,
	cityName                          nvarchar(100),
	stateCode                         int,
	stateAbbreviation                 nvarchar(2),
	stateName                         nvarchar(60),
constraint [PK_TotvsOperationalAreasCities] primary key (
	doc_company asc,
	operationalAreaCode asc,
	cityCode asc
));

/****** Chaves estrangeiras ******/
alter table [totvs].TotvsOperationalAreasStates add constraint [FK_TotvsOperationalAreasStates] foreign key (doc_company, operationalAreaCode) references [totvs].TotvsOperationalAreas (doc_company, operationalAreaCode);
alter table [totvs].TotvsOperationalAreasCeps add constraint [FK_TotvsOperationalAreasCeps] foreign key (doc_company, operationalAreaCode) references [totvs].TotvsOperationalAreas (doc_company, operationalAreaCode);
alter table [totvs].TotvsOperationalAreasCities add constraint [FK_TotvsOperationalAreasCities] foreign key (doc_company, operationalAreaCode) references [totvs].TotvsOperationalAreas (doc_company, operationalAreaCode);
