-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; .docs/MIGRATOR-CSX.md)
/****** Catálogos da API Person: tabelas untreated e totvs (src/Backend/Modules/Totvs/.docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.2, fase PE-1) ******/
--tamanhos do swagger da API Person. Catálogos pequenos: lidos pela janela de alteração (T9), como as filiais; nada é apagado

if dbo.fn_Migrator_Creates(N'[untreated].TotvsPersonClassifications') = 1
create table [untreated].TotvsPersonClassifications (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	typeCode                  int,
	typeDescription           nvarchar(40),
	[code]                    nvarchar(10),
	[description]             nvarchar(60),
	maxChangeFilterDate       datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsPersonClassifications') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].TotvsEmailTypes') = 1
create table [untreated].TotvsEmailTypes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	[code]                    int,
	[description]             nvarchar(40),
	isDefault                 bit,
	maxChangeFilterDate       datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsEmailTypes') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].TotvsPhoneTypes') = 1
create table [untreated].TotvsPhoneTypes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	[code]                    int,
	[description]             nvarchar(60),
	isDefault                 bit,
	maxChangeFilterDate       datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsPhoneTypes') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].TotvsContactTypes') = 1
create table [untreated].TotvsContactTypes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	[code]                    int,
	typeName                  nvarchar(60),
	maxChangeFilterDate       datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsContactTypes') = 1
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
