/****** Inutilizações e centros de custo da API Fiscal: tabelas untreated e totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.2 e §8.3, fase FI-1) ******/
--inutilizações pela janela de alteração (T9); centros de custo, sem data na resposta, lidos inteiros e comparados pelo json_hash (F5)

create table [untreated].TotvsInvoicesDisable (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	branchCode                        int,
	documentType                      int,
	serialCode                        nvarchar(3),
	initialInvoiceNumber              int,
	finalInvoiceNumber                int,
	branchCnpj                        nvarchar(14),
	receipt                           bigint,
	receivementDate                   datetime,
	maxChangeFilterDate               datetime
);

create table [totvs].TotvsInvoicesDisable (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	branchCode                        int not null,
	documentType                      int not null,
	serialCode                        nvarchar(3) not null,
	initialInvoiceNumber              int not null,
	finalInvoiceNumber                int,
	branchCnpj                        nvarchar(14),
	receipt                           bigint,
	receivementDate                   datetime,
	maxChangeFilterDate               datetime,
constraint [PK_TotvsInvoicesDisable] primary key (
	doc_company asc,
	branchCode asc,
	documentType asc,
	serialCode asc,
	initialInvoiceNumber asc
));

create table [untreated].TotvsCostCenters (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	[code]                            int,
	[description]                     nvarchar(100),
	[class]                           nvarchar(30),
	classType                         nvarchar(30),
	indirectType                      nvarchar(30),
	isInactive                        bit,
	json_hash                         nvarchar(64)
);

create table [totvs].TotvsCostCenters (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	[code]                            int not null,
	[description]                     nvarchar(100),
	[class]                           nvarchar(30),
	classType                         nvarchar(30),
	indirectType                      nvarchar(30),
	isInactive                        bit,
	json_hash                         nvarchar(64),
constraint [PK_TotvsCostCenters] primary key (
	doc_company asc,
	[code] asc
));

create table [untreated].TotvsCostCentersClassifications (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	costCenterCode                    int,
	typeCode                          int,
	[code]                            nvarchar(10),
	typeDescription                   nvarchar(40),
	[description]                     nvarchar(60)
);

create table [totvs].TotvsCostCentersClassifications (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	costCenterCode                    int not null,
	typeCode                          int not null,
	[code]                            nvarchar(10) not null,
	typeDescription                   nvarchar(40),
	[description]                     nvarchar(60),
constraint [PK_TotvsCostCentersClassifications] primary key (
	doc_company asc,
	costCenterCode asc,
	typeCode asc,
	[code] asc
));

/****** Chaves estrangeiras ******/
alter table [totvs].TotvsCostCentersClassifications add constraint [FK_TotvsCostCentersClassifications] foreign key (doc_company, costCenterCode) references [totvs].TotvsCostCenters (doc_company, [code]);
