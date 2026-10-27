/****** Regiões de atuação: tabelas untreated e totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §4.1, fase SE-1) ******/
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
