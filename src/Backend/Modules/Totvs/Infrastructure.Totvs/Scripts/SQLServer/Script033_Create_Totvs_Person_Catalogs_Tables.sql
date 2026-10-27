/****** Catálogos da API Person: tabelas untreated e totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.2, fase PE-1) ******/
--tamanhos do swagger da API Person. Catálogos pequenos: lidos pela janela de alteração (T9), como as filiais; nada é apagado

create table [untreated].TotvsPersonClassifications (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	typeCode                  int,
	typeDescription           nvarchar(40),
	[code]                    nvarchar(10),
	[description]             nvarchar(60),
	maxChangeFilterDate       datetime
);

create table [totvs].TotvsPersonClassifications (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	typeCode                  int not null,
	typeDescription           nvarchar(40),
	[code]                    nvarchar(10) not null,
	[description]             nvarchar(60),
	maxChangeFilterDate       datetime,
constraint [PK_TotvsPersonClassifications] primary key (
	doc_company asc,
	typeCode asc,
	[code] asc
));

create table [untreated].TotvsEmailTypes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	[code]                    int,
	[description]             nvarchar(40),
	isDefault                 bit,
	maxChangeFilterDate       datetime
);

create table [totvs].TotvsEmailTypes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	[code]                    int not null,
	[description]             nvarchar(40),
	isDefault                 bit,
	maxChangeFilterDate       datetime,
constraint [PK_TotvsEmailTypes] primary key (
	doc_company asc,
	[code] asc
));

create table [untreated].TotvsPhoneTypes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	[code]                    int,
	[description]             nvarchar(60),
	isDefault                 bit,
	maxChangeFilterDate       datetime
);

create table [totvs].TotvsPhoneTypes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	[code]                    int not null,
	[description]             nvarchar(60),
	isDefault                 bit,
	maxChangeFilterDate       datetime,
constraint [PK_TotvsPhoneTypes] primary key (
	doc_company asc,
	[code] asc
));

create table [untreated].TotvsContactTypes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	[code]                    int,
	typeName                  nvarchar(60),
	maxChangeFilterDate       datetime
);

create table [totvs].TotvsContactTypes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	[code]                    int not null,
	typeName                  nvarchar(60),
	maxChangeFilterDate       datetime,
constraint [PK_TotvsContactTypes] primary key (
	doc_company asc,
	[code] asc
));
