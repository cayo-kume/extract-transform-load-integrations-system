/****** Filiais: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.2) ******/
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

go

/****** Pessoas físicas e jurídicas: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.5) ******/
--tamanhos do swagger da API Person; objetos 1:1 (shippingCompany, preferences, individualAdditionals) como colunas do pai
--com o nome do objeto como prefixo (5.2.8). As filhas usam a mesma estrutura nas duas pessoas.

create table [untreated].TotvsPeople (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	[code]                                                int,
	cpf                                                   nvarchar(11),
	isInactive                                            bit,
	maritalStatus                                         nvarchar(20),
	gender                                                nvarchar(20),
	ctps                                                  bigint,
	ctpsSerial                                            bigint,
	rg                                                    nvarchar(20),
	rgFederalAgency                                       nvarchar(10),
	suframaCode                                           int,
	motherName                                            nvarchar(60),
	fatherName                                            nvarchar(60),
	nationality                                           nvarchar(30),
	birthDate                                             datetime,
	monthlyIncome                                         decimal(18,4),
	occupation                                            nvarchar(60),
	hireDate                                              datetime,
	workPlace                                             nvarchar(60),
	homeTown                                              nvarchar(60),
	branchInsertCode                                      int,
	[name]                                                nvarchar(60),
	homePage                                              nvarchar(100),
	codeMainRelated                                       int,
	cpfCnpjMainRelated                                    nvarchar(14),
	nameMainRelated                                       nvarchar(60),
	insertDate                                            datetime,
	maxChangeFilterDate                                   datetime,
	isCustomer                                            bit,
	isSupplier                                            bit,
	isRepresentative                                      bit,
	isPurchasingGuide                                     bit,
	isShippingCompany                                     bit,
	isEmployee                                            bit,
	employeeStatus                                        nvarchar(20),
	customerStatus                                        nvarchar(20),
	shippingCompanyPercentegeValue                        decimal(18,4),
	shippingCompanyKilogramsValue                         decimal(18,4),
	shippingCompanyMinimunValue                           decimal(18,4),
	shippingCompanyMaximumValue                           decimal(18,4),
	shippingCompanyAnttCode                               nvarchar(80),
	shippingCompanyTypeDeductIcms                         nvarchar(20),
	preferencesPaymentConditionCode                       int,
	preferencesPaymentConditionDescription                nvarchar(40),
	preferencesShippingCompanyCode                        int,
	preferencesShippingCompanyName                        nvarchar(60),
	preferencesFreightType                                nvarchar(30),
	preferencesBridgeShippingCompanyCode                  int,
	preferencesBridgeShippingCompanyName                  nvarchar(60),
	preferencesMaximumAverageTerm                         int,
	preferencesPriceTableCode                             int,
	preferencesPriceTableDescription                      nvarchar(40),
	preferencesOrderPriority                              int,
	preferencesBearerCode                                 int,
	preferencesBearerName                                 nvarchar(40),
	individualAdditionalsEducationLevel                   nvarchar(40),
	individualAdditionalsNumberOfChildren                 int,
	individualAdditionalsNumberOfDependents               int,
	individualAdditionalsResidenceMonths                  int,
	individualAdditionalsWorkMonths                       int,
	individualAdditionalsPreviousWorkPlace                nvarchar(60),
	individualAdditionalsResidingSinceDate                datetime,
	individualAdditionalsHomeType                         nvarchar(40),
	individualAdditionalsCarType                          nvarchar(40),
	individualAdditionalsScore                            decimal(18,4),
	individualAdditionalsRisk                             decimal(18,4),
	individualAdditionalsSpcUpdateDate                    datetime,
	individualAdditionalsIsStreetVendor                   bit,
	individualAdditionalsStreetVendorNumber               nvarchar(20),
	individualAdditionalsRgType                           int,
	individualAdditionalsRgIssueDate                      datetime,
	individualAdditionalsPassportNumber                   nvarchar(40),
	individualAdditionalsPassportCountryCode              int,
	individualAdditionalsDriverLicenseNumber              nvarchar(20),
	individualAdditionalsDriverLicenseExpirationDate      datetime,
	individualAdditionalsFirstDriverLicenseDate           datetime,
	individualAdditionalsDriverLicenseAgency              nvarchar(20),
	individualAdditionalsDriverLicenseCategory            nvarchar(40),
	individualAdditionalsVoterRegistrationNumber          nvarchar(20),
	individualAdditionalsVoterZone                        nvarchar(10),
	individualAdditionalsVoterSection                     nvarchar(10),
	individualAdditionalsMilitaryCertificateType          nvarchar(40),
	individualAdditionalsMilitaryCategory                 nvarchar(40),
	individualAdditionalsMilitaryDispensation             nvarchar(20),
	individualAdditionalsNitNumber                        nvarchar(20),
	individualAdditionalsCtpsState                        nvarchar(2),
	individualAdditionalsIsInformalWorker                 bit,
	individualAdditionalsPisNumber                        nvarchar(20),
	individualAdditionalsPisIssueDate                     datetime,
	individualAdditionalsPisState                         nvarchar(2),
	individualAdditionalsRuralProducerRegistration        nvarchar(18),
	individualAdditionalsRuralProducerState               nvarchar(2),
	individualAdditionalsCifIssuer                        nvarchar(20),
	individualAdditionalsCifState                         nvarchar(2),
	individualAdditionalsCifNumber                        nvarchar(20),
	individualAdditionalsBirthState                       nvarchar(2),
	individualAdditionalsPassportIssuer                   nvarchar(10),
	individualAdditionalsPassportState                    nvarchar(2),
	individualAdditionalsPassportIssueDate                datetime,
	individualAdditionalsPassportExpirationDate           datetime,
	individualAdditionalsRicNumber                        nvarchar(20),
	individualAdditionalsRicState                         nvarchar(2),
	individualAdditionalsRicIssuer                        nvarchar(20),
	individualAdditionalsRicCityCode                      int,
	individualAdditionalsRicIssueDate                     datetime,
	individualAdditionalsRicExpirationDate                datetime,
	individualAdditionalsForeignDocumentNumber            nvarchar(20),
	individualAdditionalsNickname                         nvarchar(40),
	individualAdditionalsCrcNumber                        nvarchar(20),
	individualAdditionalsDriverLicenseSecurityNumber      nvarchar(20),
	individualAdditionalsDriverLicenseRegistrationNumber  nvarchar(20),
	individualAdditionalsCrcRegistrationDate              datetime,
	individualAdditionalsCrcExpirationDate                datetime,
	individualAdditionalsSocialName                       nvarchar(70),
	individualAdditionalsHomeTown                         int
);

create table [totvs].TotvsPeople (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	[code]                                                int not null,
	cpf                                                   nvarchar(11),
	isInactive                                            bit,
	maritalStatus                                         nvarchar(20),
	gender                                                nvarchar(20),
	ctps                                                  bigint,
	ctpsSerial                                            bigint,
	rg                                                    nvarchar(20),
	rgFederalAgency                                       nvarchar(10),
	suframaCode                                           int,
	motherName                                            nvarchar(60),
	fatherName                                            nvarchar(60),
	nationality                                           nvarchar(30),
	birthDate                                             datetime,
	monthlyIncome                                         decimal(18,4),
	occupation                                            nvarchar(60),
	hireDate                                              datetime,
	workPlace                                             nvarchar(60),
	homeTown                                              nvarchar(60),
	branchInsertCode                                      int,
	[name]                                                nvarchar(60),
	homePage                                              nvarchar(100),
	codeMainRelated                                       int,
	cpfCnpjMainRelated                                    nvarchar(14),
	nameMainRelated                                       nvarchar(60),
	insertDate                                            datetime,
	maxChangeFilterDate                                   datetime,
	isCustomer                                            bit,
	isSupplier                                            bit,
	isRepresentative                                      bit,
	isPurchasingGuide                                     bit,
	isShippingCompany                                     bit,
	isEmployee                                            bit,
	employeeStatus                                        nvarchar(20),
	customerStatus                                        nvarchar(20),
	shippingCompanyPercentegeValue                        decimal(18,4),
	shippingCompanyKilogramsValue                         decimal(18,4),
	shippingCompanyMinimunValue                           decimal(18,4),
	shippingCompanyMaximumValue                           decimal(18,4),
	shippingCompanyAnttCode                               nvarchar(80),
	shippingCompanyTypeDeductIcms                         nvarchar(20),
	preferencesPaymentConditionCode                       int,
	preferencesPaymentConditionDescription                nvarchar(40),
	preferencesShippingCompanyCode                        int,
	preferencesShippingCompanyName                        nvarchar(60),
	preferencesFreightType                                nvarchar(30),
	preferencesBridgeShippingCompanyCode                  int,
	preferencesBridgeShippingCompanyName                  nvarchar(60),
	preferencesMaximumAverageTerm                         int,
	preferencesPriceTableCode                             int,
	preferencesPriceTableDescription                      nvarchar(40),
	preferencesOrderPriority                              int,
	preferencesBearerCode                                 int,
	preferencesBearerName                                 nvarchar(40),
	individualAdditionalsEducationLevel                   nvarchar(40),
	individualAdditionalsNumberOfChildren                 int,
	individualAdditionalsNumberOfDependents               int,
	individualAdditionalsResidenceMonths                  int,
	individualAdditionalsWorkMonths                       int,
	individualAdditionalsPreviousWorkPlace                nvarchar(60),
	individualAdditionalsResidingSinceDate                datetime,
	individualAdditionalsHomeType                         nvarchar(40),
	individualAdditionalsCarType                          nvarchar(40),
	individualAdditionalsScore                            decimal(18,4),
	individualAdditionalsRisk                             decimal(18,4),
	individualAdditionalsSpcUpdateDate                    datetime,
	individualAdditionalsIsStreetVendor                   bit,
	individualAdditionalsStreetVendorNumber               nvarchar(20),
	individualAdditionalsRgType                           int,
	individualAdditionalsRgIssueDate                      datetime,
	individualAdditionalsPassportNumber                   nvarchar(40),
	individualAdditionalsPassportCountryCode              int,
	individualAdditionalsDriverLicenseNumber              nvarchar(20),
	individualAdditionalsDriverLicenseExpirationDate      datetime,
	individualAdditionalsFirstDriverLicenseDate           datetime,
	individualAdditionalsDriverLicenseAgency              nvarchar(20),
	individualAdditionalsDriverLicenseCategory            nvarchar(40),
	individualAdditionalsVoterRegistrationNumber          nvarchar(20),
	individualAdditionalsVoterZone                        nvarchar(10),
	individualAdditionalsVoterSection                     nvarchar(10),
	individualAdditionalsMilitaryCertificateType          nvarchar(40),
	individualAdditionalsMilitaryCategory                 nvarchar(40),
	individualAdditionalsMilitaryDispensation             nvarchar(20),
	individualAdditionalsNitNumber                        nvarchar(20),
	individualAdditionalsCtpsState                        nvarchar(2),
	individualAdditionalsIsInformalWorker                 bit,
	individualAdditionalsPisNumber                        nvarchar(20),
	individualAdditionalsPisIssueDate                     datetime,
	individualAdditionalsPisState                         nvarchar(2),
	individualAdditionalsRuralProducerRegistration        nvarchar(18),
	individualAdditionalsRuralProducerState               nvarchar(2),
	individualAdditionalsCifIssuer                        nvarchar(20),
	individualAdditionalsCifState                         nvarchar(2),
	individualAdditionalsCifNumber                        nvarchar(20),
	individualAdditionalsBirthState                       nvarchar(2),
	individualAdditionalsPassportIssuer                   nvarchar(10),
	individualAdditionalsPassportState                    nvarchar(2),
	individualAdditionalsPassportIssueDate                datetime,
	individualAdditionalsPassportExpirationDate           datetime,
	individualAdditionalsRicNumber                        nvarchar(20),
	individualAdditionalsRicState                         nvarchar(2),
	individualAdditionalsRicIssuer                        nvarchar(20),
	individualAdditionalsRicCityCode                      int,
	individualAdditionalsRicIssueDate                     datetime,
	individualAdditionalsRicExpirationDate                datetime,
	individualAdditionalsForeignDocumentNumber            nvarchar(20),
	individualAdditionalsNickname                         nvarchar(40),
	individualAdditionalsCrcNumber                        nvarchar(20),
	individualAdditionalsDriverLicenseSecurityNumber      nvarchar(20),
	individualAdditionalsDriverLicenseRegistrationNumber  nvarchar(20),
	individualAdditionalsCrcRegistrationDate              datetime,
	individualAdditionalsCrcExpirationDate                datetime,
	individualAdditionalsSocialName                       nvarchar(70),
	individualAdditionalsHomeTown                         int,
constraint [PK_TotvsPeople] primary key (
	doc_company asc,
	[code] asc
));

create table [untreated].TotvsPeopleAddresses (
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

create table [totvs].TotvsPeopleAddresses (
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
constraint [PK_TotvsPeopleAddresses] primary key (
	doc_company asc,
	personCode asc,
	addressTypeCode asc,
	sequenceCode asc
));

create table [untreated].TotvsPeoplePhones (
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

create table [totvs].TotvsPeoplePhones (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[number]                                              nvarchar(20),
	branchLine                                            int,
	isDefault                                             bit,
constraint [PK_TotvsPeoplePhones] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsPeopleEmails (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[email]                                               nvarchar(60),
	isDefault                                             bit
);

create table [totvs].TotvsPeopleEmails (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[email]                                               nvarchar(60),
	isDefault                                             bit,
constraint [PK_TotvsPeopleEmails] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsPeopleObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	observation                                           nvarchar(80),
	isMaintenance                                         bit
);

create table [totvs].TotvsPeopleObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	observation                                           nvarchar(80),
	isMaintenance                                         bit,
constraint [PK_TotvsPeopleObservations] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsPeopleCustomerObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	observation                                           nvarchar(80),
	isMaintenance                                         bit,
	lastChangeDate                                        datetime
);

create table [totvs].TotvsPeopleCustomerObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	observation                                           nvarchar(80),
	isMaintenance                                         bit,
	lastChangeDate                                        datetime,
constraint [PK_TotvsPeopleCustomerObservations] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsPeopleAdditionalFields (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[code]                                                int,
	[type]                                                nvarchar(20),
	[name]                                                nvarchar(60),
	[value]                                               nvarchar(60)
);

create table [totvs].TotvsPeopleAdditionalFields (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[code]                                                int not null,
	[type]                                                nvarchar(20),
	[name]                                                nvarchar(60),
	[value]                                               nvarchar(60),
constraint [PK_TotvsPeopleAdditionalFields] primary key (
	doc_company asc,
	personCode asc,
	[code] asc
));

create table [untreated].TotvsPeopleClassifications (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	typeCode                                              int,
	[code]                                                nvarchar(10),
	typeName                                              nvarchar(40),
	[name]                                                nvarchar(60)
);

create table [totvs].TotvsPeopleClassifications (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	typeCode                                              int not null,
	[code]                                                nvarchar(10) not null,
	typeName                                              nvarchar(40),
	[name]                                                nvarchar(60),
constraint [PK_TotvsPeopleClassifications] primary key (
	doc_company asc,
	personCode asc,
	typeCode asc,
	[code] asc
));

create table [untreated].TotvsPeopleReferences (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	[type]                                                nvarchar(20),
	[description]                                         nvarchar(20),
	phoneNumber                                           nvarchar(20),
	responsiblePersonName                                 nvarchar(60),
	isInactive                                            bit
);

create table [totvs].TotvsPeopleReferences (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	[type]                                                nvarchar(20),
	[description]                                         nvarchar(20),
	phoneNumber                                           nvarchar(20),
	responsiblePersonName                                 nvarchar(60),
	isInactive                                            bit,
constraint [PK_TotvsPeopleReferences] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsPeopleRelateds (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[code]                                                int,
	cpfCnpj                                               nvarchar(14),
	[name]                                                nvarchar(60)
);

create table [totvs].TotvsPeopleRelateds (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[code]                                                int not null,
	cpfCnpj                                               nvarchar(14),
	[name]                                                nvarchar(60),
constraint [PK_TotvsPeopleRelateds] primary key (
	doc_company asc,
	personCode asc,
	[code] asc
));

create table [untreated].TotvsPeopleStatistics (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	branchCode                                            int,
	customerCode                                          int,
	averageDelay                                          int,
	maximumDelay                                          int,
	returnedCheck                                         int,
	installmentPaid                                       int,
	purchaseQuantity                                      int,
	higherDebt                                            decimal(18,4),
	limitValue                                            decimal(18,4),
	biggestPurchase                                       decimal(18,4),
	firstPurchase                                         decimal(18,4),
	lastPurchase                                          decimal(18,4),
	largestAccumulationDate                               datetime,
	biggestPurchaseDate                                   datetime,
	firstPurchaseDate                                     datetime,
	lastPurchaseDate                                      datetime,
	lastCardDate                                          datetime,
	totalPurchase                                         decimal(18,4)
);

create table [totvs].TotvsPeopleStatistics (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	branchCode                                            int not null,
	customerCode                                          int not null,
	averageDelay                                          int,
	maximumDelay                                          int,
	returnedCheck                                         int,
	installmentPaid                                       int,
	purchaseQuantity                                      int,
	higherDebt                                            decimal(18,4),
	limitValue                                            decimal(18,4),
	biggestPurchase                                       decimal(18,4),
	firstPurchase                                         decimal(18,4),
	lastPurchase                                          decimal(18,4),
	largestAccumulationDate                               datetime,
	biggestPurchaseDate                                   datetime,
	firstPurchaseDate                                     datetime,
	lastPurchaseDate                                      datetime,
	lastCardDate                                          datetime,
	totalPurchase                                         decimal(18,4),
constraint [PK_TotvsPeopleStatistics] primary key (
	doc_company asc,
	personCode asc,
	branchCode asc,
	customerCode asc
));

create table [untreated].TotvsPeopleRepresentatives (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	representativeCode                                    int,
	cpfCnpj                                               nvarchar(14),
	classificationCode                                    nvarchar(10),
	classificationTypeCode                                int
);

create table [totvs].TotvsPeopleRepresentatives (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	representativeCode                                    int not null,
	cpfCnpj                                               nvarchar(14),
	classificationCode                                    nvarchar(10),
	classificationTypeCode                                int,
constraint [PK_TotvsPeopleRepresentatives] primary key (
	doc_company asc,
	personCode asc,
	representativeCode asc
));

create table [untreated].TotvsPeopleFamiliars (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	[name]                                                nvarchar(60),
	birthDate                                             datetime,
	gender                                                nvarchar(20),
	kinshipDescription                                    nvarchar(60)
);

create table [totvs].TotvsPeopleFamiliars (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	[name]                                                nvarchar(60),
	birthDate                                             datetime,
	gender                                                nvarchar(20),
	kinshipDescription                                    nvarchar(60),
constraint [PK_TotvsPeopleFamiliars] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsLegalEntities (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	[code]                                                int,
	cnpj                                                  nvarchar(14),
	isInactive                                            bit,
	branchInsertCode                                      int,
	[name]                                                nvarchar(60),
	fantasyName                                           nvarchar(60),
	uf                                                    nvarchar(2),
	numberStateRegistration                               nvarchar(18),
	suframaCode                                           int,
	dateFoundation                                        nvarchar(20),
	codeActivity                                          int,
	numberEmployees                                       nvarchar(40),
	monthlyInvoicing                                      decimal(18,4),
	shareCapital                                          decimal(18,4),
	homePage                                              nvarchar(100),
	codeActivityCnae                                      int,
	codeActivityCnae2                                     int,
	typeTaxRegime                                         nvarchar(40),
	typeSubTributary                                      nvarchar(40),
	typeDescriptionSuFrama                                nvarchar(40),
	registrationMunicipal                                 nvarchar(18),
	descriptionJuntaCial                                  nvarchar(20),
	dateRegJuntaCial                                      datetime,
	codeMainRelated                                       int,
	cpfCnpjMainRelated                                    nvarchar(14),
	nameMainRelated                                       nvarchar(60),
	insertDate                                            datetime,
	maxChangeFilterDate                                   datetime,
	isCustomer                                            bit,
	isSupplier                                            bit,
	isRepresentative                                      bit,
	isPurchasingGuide                                     bit,
	isShippingCompany                                     bit,
	customerStatus                                        nvarchar(20),
	[status]                                              nvarchar(20),
	shippingCompanyPercentegeValue                        decimal(18,4),
	shippingCompanyKilogramsValue                         decimal(18,4),
	shippingCompanyMinimunValue                           decimal(18,4),
	shippingCompanyMaximumValue                           decimal(18,4),
	shippingCompanyAnttCode                               nvarchar(80),
	shippingCompanyTypeDeductIcms                         nvarchar(20),
	preferencesPaymentConditionCode                       int,
	preferencesPaymentConditionDescription                nvarchar(40),
	preferencesShippingCompanyCode                        int,
	preferencesShippingCompanyName                        nvarchar(60),
	preferencesFreightType                                nvarchar(30),
	preferencesBridgeShippingCompanyCode                  int,
	preferencesBridgeShippingCompanyName                  nvarchar(60),
	preferencesMaximumAverageTerm                         int,
	preferencesPriceTableCode                             int,
	preferencesPriceTableDescription                      nvarchar(40),
	preferencesOrderPriority                              int,
	preferencesBearerCode                                 int,
	preferencesBearerName                                 nvarchar(40)
);

create table [totvs].TotvsLegalEntities (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	[code]                                                int not null,
	cnpj                                                  nvarchar(14),
	isInactive                                            bit,
	branchInsertCode                                      int,
	[name]                                                nvarchar(60),
	fantasyName                                           nvarchar(60),
	uf                                                    nvarchar(2),
	numberStateRegistration                               nvarchar(18),
	suframaCode                                           int,
	dateFoundation                                        nvarchar(20),
	codeActivity                                          int,
	numberEmployees                                       nvarchar(40),
	monthlyInvoicing                                      decimal(18,4),
	shareCapital                                          decimal(18,4),
	homePage                                              nvarchar(100),
	codeActivityCnae                                      int,
	codeActivityCnae2                                     int,
	typeTaxRegime                                         nvarchar(40),
	typeSubTributary                                      nvarchar(40),
	typeDescriptionSuFrama                                nvarchar(40),
	registrationMunicipal                                 nvarchar(18),
	descriptionJuntaCial                                  nvarchar(20),
	dateRegJuntaCial                                      datetime,
	codeMainRelated                                       int,
	cpfCnpjMainRelated                                    nvarchar(14),
	nameMainRelated                                       nvarchar(60),
	insertDate                                            datetime,
	maxChangeFilterDate                                   datetime,
	isCustomer                                            bit,
	isSupplier                                            bit,
	isRepresentative                                      bit,
	isPurchasingGuide                                     bit,
	isShippingCompany                                     bit,
	customerStatus                                        nvarchar(20),
	[status]                                              nvarchar(20),
	shippingCompanyPercentegeValue                        decimal(18,4),
	shippingCompanyKilogramsValue                         decimal(18,4),
	shippingCompanyMinimunValue                           decimal(18,4),
	shippingCompanyMaximumValue                           decimal(18,4),
	shippingCompanyAnttCode                               nvarchar(80),
	shippingCompanyTypeDeductIcms                         nvarchar(20),
	preferencesPaymentConditionCode                       int,
	preferencesPaymentConditionDescription                nvarchar(40),
	preferencesShippingCompanyCode                        int,
	preferencesShippingCompanyName                        nvarchar(60),
	preferencesFreightType                                nvarchar(30),
	preferencesBridgeShippingCompanyCode                  int,
	preferencesBridgeShippingCompanyName                  nvarchar(60),
	preferencesMaximumAverageTerm                         int,
	preferencesPriceTableCode                             int,
	preferencesPriceTableDescription                      nvarchar(40),
	preferencesOrderPriority                              int,
	preferencesBearerCode                                 int,
	preferencesBearerName                                 nvarchar(40),
constraint [PK_TotvsLegalEntities] primary key (
	doc_company asc,
	[code] asc
));

create table [untreated].TotvsLegalEntitiesAddresses (
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

create table [totvs].TotvsLegalEntitiesAddresses (
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
constraint [PK_TotvsLegalEntitiesAddresses] primary key (
	doc_company asc,
	personCode asc,
	addressTypeCode asc,
	sequenceCode asc
));

create table [untreated].TotvsLegalEntitiesPhones (
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

create table [totvs].TotvsLegalEntitiesPhones (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[number]                                              nvarchar(20),
	branchLine                                            int,
	isDefault                                             bit,
constraint [PK_TotvsLegalEntitiesPhones] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsLegalEntitiesEmails (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[email]                                               nvarchar(60),
	isDefault                                             bit
);

create table [totvs].TotvsLegalEntitiesEmails (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[email]                                               nvarchar(60),
	isDefault                                             bit,
constraint [PK_TotvsLegalEntitiesEmails] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsLegalEntitiesObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	observation                                           nvarchar(80),
	isMaintenance                                         bit
);

create table [totvs].TotvsLegalEntitiesObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	observation                                           nvarchar(80),
	isMaintenance                                         bit,
constraint [PK_TotvsLegalEntitiesObservations] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsLegalEntitiesCustomerObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	observation                                           nvarchar(80),
	isMaintenance                                         bit,
	lastChangeDate                                        datetime
);

create table [totvs].TotvsLegalEntitiesCustomerObservations (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	observation                                           nvarchar(80),
	isMaintenance                                         bit,
	lastChangeDate                                        datetime,
constraint [PK_TotvsLegalEntitiesCustomerObservations] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsLegalEntitiesAdditionalFields (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[code]                                                int,
	[type]                                                nvarchar(20),
	[name]                                                nvarchar(60),
	[value]                                               nvarchar(60)
);

create table [totvs].TotvsLegalEntitiesAdditionalFields (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[code]                                                int not null,
	[type]                                                nvarchar(20),
	[name]                                                nvarchar(60),
	[value]                                               nvarchar(60),
constraint [PK_TotvsLegalEntitiesAdditionalFields] primary key (
	doc_company asc,
	personCode asc,
	[code] asc
));

create table [untreated].TotvsLegalEntitiesClassifications (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	typeCode                                              int,
	[code]                                                nvarchar(10),
	typeName                                              nvarchar(40),
	[name]                                                nvarchar(60)
);

create table [totvs].TotvsLegalEntitiesClassifications (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	typeCode                                              int not null,
	[code]                                                nvarchar(10) not null,
	typeName                                              nvarchar(40),
	[name]                                                nvarchar(60),
constraint [PK_TotvsLegalEntitiesClassifications] primary key (
	doc_company asc,
	personCode asc,
	typeCode asc,
	[code] asc
));

create table [untreated].TotvsLegalEntitiesReferences (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	[type]                                                nvarchar(20),
	[description]                                         nvarchar(20),
	phoneNumber                                           nvarchar(20),
	responsiblePersonName                                 nvarchar(60),
	isInactive                                            bit
);

create table [totvs].TotvsLegalEntitiesReferences (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	[type]                                                nvarchar(20),
	[description]                                         nvarchar(20),
	phoneNumber                                           nvarchar(20),
	responsiblePersonName                                 nvarchar(60),
	isInactive                                            bit,
constraint [PK_TotvsLegalEntitiesReferences] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsLegalEntitiesRelateds (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[code]                                                int,
	cpfCnpj                                               nvarchar(14),
	[name]                                                nvarchar(60)
);

create table [totvs].TotvsLegalEntitiesRelateds (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[code]                                                int not null,
	cpfCnpj                                               nvarchar(14),
	[name]                                                nvarchar(60),
constraint [PK_TotvsLegalEntitiesRelateds] primary key (
	doc_company asc,
	personCode asc,
	[code] asc
));

create table [untreated].TotvsLegalEntitiesPartners (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[code]                                                int,
	cpfCnpj                                               nvarchar(14),
	[name]                                                nvarchar(60),
	percentageParticipation                               decimal(18,4)
);

create table [totvs].TotvsLegalEntitiesPartners (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[code]                                                int not null,
	cpfCnpj                                               nvarchar(14),
	[name]                                                nvarchar(60),
	percentageParticipation                               decimal(18,4),
constraint [PK_TotvsLegalEntitiesPartners] primary key (
	doc_company asc,
	personCode asc,
	[code] asc
));

create table [untreated].TotvsLegalEntitiesContacts (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	[name]                                                nvarchar(60),
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[function]                                            nvarchar(20),
	phoneNumber                                           nvarchar(20),
	cellNumber                                            nvarchar(20),
	[email]                                               nvarchar(60),
	birthDate                                             datetime,
	isDefault                                             bit
);

create table [totvs].TotvsLegalEntitiesContacts (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	[name]                                                nvarchar(60),
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[function]                                            nvarchar(20),
	phoneNumber                                           nvarchar(20),
	cellNumber                                            nvarchar(20),
	[email]                                               nvarchar(60),
	birthDate                                             datetime,
	isDefault                                             bit,
constraint [PK_TotvsLegalEntitiesContacts] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsLegalEntitiesStatistics (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	branchCode                                            int,
	customerCode                                          int,
	averageDelay                                          int,
	maximumDelay                                          int,
	returnedCheck                                         int,
	installmentPaid                                       int,
	purchaseQuantity                                      int,
	higherDebt                                            decimal(18,4),
	limitValue                                            decimal(18,4),
	biggestPurchase                                       decimal(18,4),
	firstPurchase                                         decimal(18,4),
	lastPurchase                                          decimal(18,4),
	largestAccumulationDate                               datetime,
	biggestPurchaseDate                                   datetime,
	firstPurchaseDate                                     datetime,
	lastPurchaseDate                                      datetime,
	lastCardDate                                          datetime,
	totalPurchase                                         decimal(18,4)
);

create table [totvs].TotvsLegalEntitiesStatistics (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	branchCode                                            int not null,
	customerCode                                          int not null,
	averageDelay                                          int,
	maximumDelay                                          int,
	returnedCheck                                         int,
	installmentPaid                                       int,
	purchaseQuantity                                      int,
	higherDebt                                            decimal(18,4),
	limitValue                                            decimal(18,4),
	biggestPurchase                                       decimal(18,4),
	firstPurchase                                         decimal(18,4),
	lastPurchase                                          decimal(18,4),
	largestAccumulationDate                               datetime,
	biggestPurchaseDate                                   datetime,
	firstPurchaseDate                                     datetime,
	lastPurchaseDate                                      datetime,
	lastCardDate                                          datetime,
	totalPurchase                                         decimal(18,4),
constraint [PK_TotvsLegalEntitiesStatistics] primary key (
	doc_company asc,
	personCode asc,
	branchCode asc,
	customerCode asc
));

create table [untreated].TotvsLegalEntitiesSocialNetworks (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	[sequence]                                            int,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[address]                                             nvarchar(20)
);

create table [totvs].TotvsLegalEntitiesSocialNetworks (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	[sequence]                                            int not null,
	typeCode                                              int,
	typeName                                              nvarchar(20),
	[address]                                             nvarchar(20),
constraint [PK_TotvsLegalEntitiesSocialNetworks] primary key (
	doc_company asc,
	personCode asc,
	[sequence] asc
));

create table [untreated].TotvsLegalEntitiesRepresentatives (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	representativeCode                                    int,
	cpfCnpj                                               nvarchar(14),
	classificationCode                                    nvarchar(10),
	classificationTypeCode                                int
);

create table [totvs].TotvsLegalEntitiesRepresentatives (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	representativeCode                                    int not null,
	cpfCnpj                                               nvarchar(14),
	classificationCode                                    nvarchar(10),
	classificationTypeCode                                int,
constraint [PK_TotvsLegalEntitiesRepresentatives] primary key (
	doc_company asc,
	personCode asc,
	representativeCode asc
));

create table [untreated].TotvsLegalEntitiesPaymentMethods (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14),
	personCode                                            int,
	paymentMethod                                         int
);

create table [totvs].TotvsLegalEntitiesPaymentMethods (
	integrated_or_updated_at                              datetime,
	doc_company                                           nvarchar(14) not null,
	personCode                                            int not null,
	paymentMethod                                         int not null,
constraint [PK_TotvsLegalEntitiesPaymentMethods] primary key (
	doc_company asc,
	personCode asc,
	paymentMethod asc
));

/****** Chaves estrangeiras: filhas -> pessoa ******/

alter table [totvs].TotvsPeopleAddresses add constraint [FK_TotvsPeopleAddresses] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeoplePhones add constraint [FK_TotvsPeoplePhones] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeopleEmails add constraint [FK_TotvsPeopleEmails] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeopleObservations add constraint [FK_TotvsPeopleObservations] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeopleCustomerObservations add constraint [FK_TotvsPeopleCustomerObservations] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeopleAdditionalFields add constraint [FK_TotvsPeopleAdditionalFields] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeopleClassifications add constraint [FK_TotvsPeopleClassifications] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeopleReferences add constraint [FK_TotvsPeopleReferences] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeopleRelateds add constraint [FK_TotvsPeopleRelateds] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeopleStatistics add constraint [FK_TotvsPeopleStatistics] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeopleRepresentatives add constraint [FK_TotvsPeopleRepresentatives] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsPeopleFamiliars add constraint [FK_TotvsPeopleFamiliars] foreign key (doc_company, personCode) references [totvs].TotvsPeople (doc_company, code);
alter table [totvs].TotvsLegalEntitiesAddresses add constraint [FK_TotvsLegalEntitiesAddresses] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesPhones add constraint [FK_TotvsLegalEntitiesPhones] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesEmails add constraint [FK_TotvsLegalEntitiesEmails] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesObservations add constraint [FK_TotvsLegalEntitiesObservations] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesCustomerObservations add constraint [FK_TotvsLegalEntitiesCustomerObservations] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesAdditionalFields add constraint [FK_TotvsLegalEntitiesAdditionalFields] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesClassifications add constraint [FK_TotvsLegalEntitiesClassifications] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesReferences add constraint [FK_TotvsLegalEntitiesReferences] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesRelateds add constraint [FK_TotvsLegalEntitiesRelateds] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesPartners add constraint [FK_TotvsLegalEntitiesPartners] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesContacts add constraint [FK_TotvsLegalEntitiesContacts] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesStatistics add constraint [FK_TotvsLegalEntitiesStatistics] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesSocialNetworks add constraint [FK_TotvsLegalEntitiesSocialNetworks] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesRepresentatives add constraint [FK_TotvsLegalEntitiesRepresentatives] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
alter table [totvs].TotvsLegalEntitiesPaymentMethods add constraint [FK_TotvsLegalEntitiesPaymentMethods] foreign key (doc_company, personCode) references [totvs].TotvsLegalEntities (doc_company, code);
