/****** Vendedores: tabelas untreated e totvs (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.3) ******/
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
