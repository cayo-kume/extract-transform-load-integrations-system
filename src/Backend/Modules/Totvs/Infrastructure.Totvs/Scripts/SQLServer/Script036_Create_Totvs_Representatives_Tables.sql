/****** Representantes: tabelas untreated e totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.1.1, fase PE-2) ******/
--chave: doc_company + code (o representante é uma pessoa do PESFM010). As filhas comuns com a PF têm as mesmas colunas das
--TotvsPeople* (Script013), com personCode = código do representante. Sem tamanho no swagger: nomes com 100.

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

create table [untreated].TotvsRepresentativesObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	observation                                           nvarchar(80),
	isMaintenance                                         bit
);

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

create table [untreated].TotvsRepresentativesAdditionalFields (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[code]                                                int,
	[type]                                                nvarchar(20),
	[name]                                                nvarchar(60),
	[value]                                               nvarchar(60)
);

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

create table [untreated].TotvsRepresentativesClassifications (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	typeCode                                              int,
	[code]                                                nvarchar(10),
	typeName                                              nvarchar(40),
	[name]                                                nvarchar(60)
);

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

create table [untreated].TotvsRepresentativesCustomers (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	personCode                        int,
	customerCode                      int,
	[name]                            nvarchar(100),
	cpfCnpj                           nvarchar(14)
);

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

create table [untreated].TotvsRepresentativesPriceTables (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	personCode                        int,
	priceTableCode                    int,
	[name]                            nvarchar(100)
);

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

create table [untreated].TotvsRepresentativesParameters (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	personCode                        int,
	branchCode                        int,
	parameterCode                     nvarchar(25),
	parameterValue                    nvarchar(2000)
);

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
alter table [totvs].TotvsRepresentativesAddresses add constraint [FK_TotvsRepresentativesAddresses] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
alter table [totvs].TotvsRepresentativesPhones add constraint [FK_TotvsRepresentativesPhones] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
alter table [totvs].TotvsRepresentativesEmails add constraint [FK_TotvsRepresentativesEmails] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
alter table [totvs].TotvsRepresentativesObservations add constraint [FK_TotvsRepresentativesObservations] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
alter table [totvs].TotvsRepresentativesAdditionalFields add constraint [FK_TotvsRepresentativesAdditionalFields] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
alter table [totvs].TotvsRepresentativesClassifications add constraint [FK_TotvsRepresentativesClassifications] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
alter table [totvs].TotvsRepresentativesCustomers add constraint [FK_TotvsRepresentativesCustomers] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
alter table [totvs].TotvsRepresentativesPriceTables add constraint [FK_TotvsRepresentativesPriceTables] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
alter table [totvs].TotvsRepresentativesSalesRegions add constraint [FK_TotvsRepresentativesSalesRegions] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
alter table [totvs].TotvsRepresentativesParameters add constraint [FK_TotvsRepresentativesParameters] foreign key (doc_company, personCode) references [totvs].TotvsRepresentatives (doc_company, [code]);
