/****** Filiais: tabelas untreated e totvs (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.2) ******/
--tamanhos do swagger da API Person (BranchListModel e ViewAddressModel)

create table [untreated].TotvsBranches (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	code                     int,
	[description]            nvarchar(60),
	cnpj                     nvarchar(14),
	personCode               int,
	personName               nvarchar(60),
	fantasyName              nvarchar(60),
	branchGroupName          nvarchar(100), --sem tamanho no swagger
	lastchange               datetime,
	maxChangeFilterDate      datetime       --só vem com StartChangeDate/EndChangeDate na chamada
);

create table [totvs].TotvsBranches (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	code                     int not null,
	[description]            nvarchar(60),
	cnpj                     nvarchar(14),
	personCode               int,
	personName               nvarchar(60),
	fantasyName              nvarchar(60),
	branchGroupName          nvarchar(100), --sem tamanho no swagger
	lastchange               datetime,
	maxChangeFilterDate      datetime,      --só vem com StartChangeDate/EndChangeDate na chamada
constraint [PK_TotvsBranches] primary key (
	doc_company asc,
	code asc
));

create table [untreated].TotvsBranchesAddresses (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	branchCode               int,
	addressTypeCode          int,
	sequenceCode             int,
	addressType              nvarchar(20),
	publicPlace              nvarchar(10),
	[address]                nvarchar(60),
	addressNumber            int,
	complement               nvarchar(65),
	neighborhood             nvarchar(60),
	ibgeCityCode             int,
	cityName                 nvarchar(60),
	stateAbbreviation        nvarchar(2),
	cep                      nvarchar(8),
	bcbCountryCode           int,
	countryName              nvarchar(60),
	postOfficeBox            int,
	reference                nvarchar(60)
);

create table [totvs].TotvsBranchesAddresses (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	branchCode               int not null,
	addressTypeCode          int not null,  --1 Comercial, 2 Residencial, 3 Cobrança, 4 Compras, 5 Entrega...
	sequenceCode             int not null,  --anulável na API: nulo vira 0
	addressType              nvarchar(20),  --nome do enum (Commercial, Residential, Billing...)
	publicPlace              nvarchar(10),
	[address]                nvarchar(60),
	addressNumber            int,
	complement               nvarchar(65),
	neighborhood             nvarchar(60),
	ibgeCityCode             int,
	cityName                 nvarchar(60),
	stateAbbreviation        nvarchar(2),
	cep                      nvarchar(8),
	bcbCountryCode           int,
	countryName              nvarchar(60),
	postOfficeBox            int,
	reference                nvarchar(60),
constraint [PK_TotvsBranchesAddresses] primary key (
	doc_company asc,
	branchCode asc,
	addressTypeCode asc,
	sequenceCode asc
));

/****** Chave estrangeira: endereços -> filial ******/

alter table [totvs].TotvsBranchesAddresses add constraint [FK_TotvsBranchesAddresses] foreign key (doc_company, branchCode) references [totvs].TotvsBranches (doc_company, code);
