-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; docs/MIGRATOR-CSX.md)
/****** Representantes: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.1.1, fase PE-2) ******/
--chave: doc_company + code (o representante é uma pessoa do PESFM010). As filhas comuns com a PF têm as mesmas colunas das
--TotvsPeople* (Script013), com personCode = código do representante. Sem tamanho no swagger: nomes com 100.

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentatives') = 1
create table [untreated].TotvsRepresentatives (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	[code]                            int,
	cpfCnpj                           nvarchar(14),
	isInactive                        bit,
	[name]                            nvarchar(100),
	supervisorCode                    int,
	supervisorName                    nvarchar(100),
	branchInsertCode                  int,
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentatives') = 1
create table [totvs].TotvsRepresentatives (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	[code]                            int not null,
	cpfCnpj                           nvarchar(14),
	isInactive                        bit,
	[name]                            nvarchar(100),
	supervisorCode                    int,
	supervisorName                    nvarchar(100),
	branchInsertCode                  int,
	maxChangeFilterDate               datetime,
constraint [PK_TotvsRepresentatives] primary key (
	doc_company asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentativesAddresses') = 1
create table [untreated].TotvsRepresentativesAddresses (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	addressTypeCode                                       int,
	sequenceCode                                          int,
	addressType                                           nvarchar(20),
	publicPlace                                           nvarchar(10),
	[address]                                             nvarchar(60),
	addressNumber                                         int,
	complement                                            nvarchar(65),
	neighborhood                                          nvarchar(60),
	ibgeCityCode                                          int,
	cityName                                              nvarchar(60),
	stateAbbreviation                                     nvarchar(2),
	cep                                                   nvarchar(8),
	bcbCountryCode                                        int,
	countryName                                           nvarchar(60),
	postOfficeBox                                         int,
	[reference]                                           nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentativesAddresses') = 1
create table [totvs].TotvsRepresentativesAddresses (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	addressTypeCode                                       int not null,
	sequenceCode                                          int not null,
	addressType                                           nvarchar(20),
	publicPlace                                           nvarchar(10),
	[address]                                             nvarchar(60),
	addressNumber                                         int,
	complement                                            nvarchar(65),
	neighborhood                                          nvarchar(60),
	ibgeCityCode                                          int,
	cityName                                              nvarchar(60),
	stateAbbreviation                                     nvarchar(2),
	cep                                                   nvarchar(8),
	bcbCountryCode                                        int,
	countryName                                           nvarchar(60),
	postOfficeBox                                         int,
	[reference]                                           nvarchar(60),
constraint [PK_TotvsRepresentativesAddresses] primary key (
	doc_company asc,
	personCode asc,
	addressTypeCode asc,
	sequenceCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentativesPhones') = 1
create table [untreated].TotvsRepresentativesPhones (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[number]                                              nvarchar(20),
	branchLine                                            int,
	isDefault                                             bit
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentativesPhones') = 1
create table [totvs].TotvsRepresentativesPhones (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[number]                                              nvarchar(20),
	branchLine                                            int,
	isDefault                                             bit,
constraint [PK_TotvsRepresentativesPhones] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentativesEmails') = 1
create table [untreated].TotvsRepresentativesEmails (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[email]                                               nvarchar(60),
	isDefault                                             bit
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentativesEmails') = 1
create table [totvs].TotvsRepresentativesEmails (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[email]                                               nvarchar(60),
	isDefault                                             bit,
constraint [PK_TotvsRepresentativesEmails] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentativesObservations') = 1
create table [untreated].TotvsRepresentativesObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	observation                                           nvarchar(80),
	isMaintenance                                         bit
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentativesObservations') = 1
create table [totvs].TotvsRepresentativesObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	observation                                           nvarchar(80),
	isMaintenance                                         bit,
constraint [PK_TotvsRepresentativesObservations] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentativesAdditionalFields') = 1
create table [untreated].TotvsRepresentativesAdditionalFields (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[code]                                                int,
	[type]                                                nvarchar(20),
	[name]                                                nvarchar(60),
	[value]                                               nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentativesAdditionalFields') = 1
create table [totvs].TotvsRepresentativesAdditionalFields (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[code]                                                int not null,
	[type]                                                nvarchar(20),
	[name]                                                nvarchar(60),
	[value]                                               nvarchar(60),
constraint [PK_TotvsRepresentativesAdditionalFields] primary key (
	doc_company asc,
	personCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentativesClassifications') = 1
create table [untreated].TotvsRepresentativesClassifications (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	typeCode                                              int,
	[code]                                                nvarchar(10),
	typeName                                              nvarchar(40),
	[name]                                                nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentativesClassifications') = 1
create table [totvs].TotvsRepresentativesClassifications (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	typeCode                                              int not null,
	[code]                                                nvarchar(10) not null,
	typeName                                              nvarchar(40),
	[name]                                                nvarchar(60),
constraint [PK_TotvsRepresentativesClassifications] primary key (
	doc_company asc,
	personCode asc,
	typeCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentativesCustomers') = 1
create table [untreated].TotvsRepresentativesCustomers (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	personCode                        int,
	customerCode                      int,
	[name]                            nvarchar(100),
	cpfCnpj                           nvarchar(14)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentativesCustomers') = 1
create table [totvs].TotvsRepresentativesCustomers (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	personCode                        int not null,
	customerCode                      int not null,
	[name]                            nvarchar(100),
	cpfCnpj                           nvarchar(14),
constraint [PK_TotvsRepresentativesCustomers] primary key (
	doc_company asc,
	personCode asc,
	customerCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentativesPriceTables') = 1
create table [untreated].TotvsRepresentativesPriceTables (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	personCode                        int,
	priceTableCode                    int,
	[name]                            nvarchar(100)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentativesPriceTables') = 1
create table [totvs].TotvsRepresentativesPriceTables (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	personCode                        int not null,
	priceTableCode                    int not null,
	[name]                            nvarchar(100),
constraint [PK_TotvsRepresentativesPriceTables] primary key (
	doc_company asc,
	personCode asc,
	priceTableCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentativesSalesRegions') = 1
create table [untreated].TotvsRepresentativesSalesRegions (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	personCode                        int,
	salesRegionCode                   int,
	salesRegionName                   nvarchar(100),
	salesOrderClassificationTypeCode  int,
	salesOrderClassificationTypeName  nvarchar(100),
	salesOrderClassificationCode      nvarchar(10),
	salesOrderClassificationName      nvarchar(100)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentativesSalesRegions') = 1
create table [totvs].TotvsRepresentativesSalesRegions (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	personCode                        int not null,
	salesRegionCode                   int not null,
	salesRegionName                   nvarchar(100),
	salesOrderClassificationTypeCode  int not null,
	salesOrderClassificationTypeName  nvarchar(100),
	salesOrderClassificationCode      nvarchar(10) not null,
	salesOrderClassificationName      nvarchar(100),
constraint [PK_TotvsRepresentativesSalesRegions] primary key (
	doc_company asc,
	personCode asc,
	salesRegionCode asc,
	salesOrderClassificationTypeCode asc,
	salesOrderClassificationCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsRepresentativesParameters') = 1
create table [untreated].TotvsRepresentativesParameters (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	personCode                        int,
	branchCode                        int,
	parameterCode                     nvarchar(25),
	parameterValue                    nvarchar(2000)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsRepresentativesParameters') = 1
create table [totvs].TotvsRepresentativesParameters (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	personCode                        int not null,
	branchCode                        int not null,
	parameterCode                     nvarchar(25) not null,
	parameterValue                    nvarchar(2000),
constraint [PK_TotvsRepresentativesParameters] primary key (
	doc_company asc,
	personCode asc,
	branchCode asc,
	parameterCode asc
));

/****** Chaves estrangeiras ******/
if object_id(N'[totvs].[FK_TotvsRepresentativesAddresses]') is null and object_id(N'[totvs].TotvsRepresentativesAddresses') is not null and object_id(N'[totvs].TotvsRepresentatives') is not null
alter table [totvs].TotvsRepresentativesAddresses add constraint [FK_TotvsRepresentativesAddresses] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsRepresentativesPhones]') is null and object_id(N'[totvs].TotvsRepresentativesPhones') is not null and object_id(N'[totvs].TotvsRepresentatives') is not null
alter table [totvs].TotvsRepresentativesPhones add constraint [FK_TotvsRepresentativesPhones] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsRepresentativesEmails]') is null and object_id(N'[totvs].TotvsRepresentativesEmails') is not null and object_id(N'[totvs].TotvsRepresentatives') is not null
alter table [totvs].TotvsRepresentativesEmails add constraint [FK_TotvsRepresentativesEmails] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsRepresentativesObservations]') is null and object_id(N'[totvs].TotvsRepresentativesObservations') is not null and object_id(N'[totvs].TotvsRepresentatives') is not null
alter table [totvs].TotvsRepresentativesObservations add constraint [FK_TotvsRepresentativesObservations] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsRepresentativesAdditionalFields]') is null and object_id(N'[totvs].TotvsRepresentativesAdditionalFields') is not null and object_id(N'[totvs].TotvsRepresentatives') is not null
alter table [totvs].TotvsRepresentativesAdditionalFields add constraint [FK_TotvsRepresentativesAdditionalFields] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsRepresentativesClassifications]') is null and object_id(N'[totvs].TotvsRepresentativesClassifications') is not null and object_id(N'[totvs].TotvsRepresentatives') is not null
alter table [totvs].TotvsRepresentativesClassifications add constraint [FK_TotvsRepresentativesClassifications] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsRepresentativesCustomers]') is null and object_id(N'[totvs].TotvsRepresentativesCustomers') is not null and object_id(N'[totvs].TotvsRepresentatives') is not null
alter table [totvs].TotvsRepresentativesCustomers add constraint [FK_TotvsRepresentativesCustomers] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsRepresentativesPriceTables]') is null and object_id(N'[totvs].TotvsRepresentativesPriceTables') is not null and object_id(N'[totvs].TotvsRepresentatives') is not null
alter table [totvs].TotvsRepresentativesPriceTables add constraint [FK_TotvsRepresentativesPriceTables] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsRepresentativesSalesRegions]') is null and object_id(N'[totvs].TotvsRepresentativesSalesRegions') is not null and object_id(N'[totvs].TotvsRepresentatives') is not null
alter table [totvs].TotvsRepresentativesSalesRegions add constraint [FK_TotvsRepresentativesSalesRegions] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsRepresentativesParameters]') is null and object_id(N'[totvs].TotvsRepresentativesParameters') is not null and object_id(N'[totvs].TotvsRepresentatives') is not null
alter table [totvs].TotvsRepresentativesParameters add constraint [FK_TotvsRepresentativesParameters] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);

go

/****** Recepção de venda: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.1.2, fase PE-2) ******/
--chave: doc_company + branchCode + movementDate + sequence. A resposta não tem data de alteração: json_hash (SHA-256 do JSON
--recebido) diz se a recepção mudou (T9). Sem tamanho no swagger para nomes, tipo e observação.

if dbo.fn_Migrator_Creates(N'[untreated].TotvsReceptionSales') = 1
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

if dbo.fn_Migrator_Creates(N'[totvs].TotvsReceptionSales') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].TotvsReceptionSalesDocumentTypes') = 1
create table [untreated].TotvsReceptionSalesDocumentTypes (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	branchCode                        int,
	movementDate                      datetime,
	[sequence]                        int,
	documentType                      int
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsReceptionSalesDocumentTypes') = 1
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
if object_id(N'[totvs].[FK_TotvsReceptionSalesDocumentTypes]') is null and object_id(N'[totvs].TotvsReceptionSalesDocumentTypes') is not null and object_id(N'[totvs].TotvsReceptionSales') is not null
alter table [totvs].TotvsReceptionSalesDocumentTypes add constraint [FK_TotvsReceptionSalesDocumentTypes] foreign key (doc_company, branchCode, movementDate, [sequence]) references [totvs].TotvsReceptionSales (doc_company, branchCode, movementDate, [sequence]);
