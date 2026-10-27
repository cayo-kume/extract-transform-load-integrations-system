/****** Recepção de venda: tabelas untreated e totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.1.2, fase PE-2) ******/
--chave: doc_company + branchCode + movementDate + sequence. A resposta não tem data de alteração: json_hash (SHA-256 do JSON
--recebido) diz se a recepção mudou (T9). Sem tamanho no swagger para nomes, tipo e observação.

create table [untreated].TotvsReceptionSales (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	branchCode                        int,
	movementDate                      datetime,
	[sequence]                        int,
	customerCode                      int,
	customerName                      nvarchar(100),
	sellerCode                        int,
	sellerName                        nvarchar(100),
	representativeCode                int,
	representativeName                nvarchar(100),
	guideCode                         int,
	guideName                         nvarchar(100),
	receptionType                     nvarchar(10),
	checkInTime                       nvarchar(10),
	[observation]                     nvarchar(400),
	json_hash                         nvarchar(64)
);

create table [totvs].TotvsReceptionSales (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	branchCode                        int not null,
	movementDate                      datetime not null,
	[sequence]                        int not null,
	customerCode                      int,
	customerName                      nvarchar(100),
	sellerCode                        int,
	sellerName                        nvarchar(100),
	representativeCode                int,
	representativeName                nvarchar(100),
	guideCode                         int,
	guideName                         nvarchar(100),
	receptionType                     nvarchar(10),
	checkInTime                       nvarchar(10),
	[observation]                     nvarchar(400),
	json_hash                         nvarchar(64),
constraint [PK_TotvsReceptionSales] primary key (
	doc_company asc,
	branchCode asc,
	movementDate asc,
	[sequence] asc
));

create table [untreated].TotvsReceptionSalesDocumentTypes (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	branchCode                        int,
	movementDate                      datetime,
	[sequence]                        int,
	documentType                      int
);

create table [totvs].TotvsReceptionSalesDocumentTypes (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	branchCode                        int not null,
	movementDate                      datetime not null,
	[sequence]                        int not null,
	documentType                      int not null,
constraint [PK_TotvsReceptionSalesDocumentTypes] primary key (
	doc_company asc,
	branchCode asc,
	movementDate asc,
	[sequence] asc,
	documentType asc
));

/****** Chaves estrangeiras ******/
alter table [totvs].TotvsReceptionSalesDocumentTypes add constraint [FK_TotvsReceptionSalesDocumentTypes] foreign key (doc_company, branchCode, movementDate, [sequence]) references [totvs].TotvsReceptionSales (doc_company, branchCode, movementDate, [sequence]);
