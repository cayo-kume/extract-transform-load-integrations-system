/****** Pessoas: tabelas untreated e general (docs/MOVIDESK-MODELAGEM-PESSOAS.md) ******/

create table [untreated].MovideskPeople (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	id                       nvarchar(64),
	codeReferenceAdditional  nvarchar(64),
	isActive                 bit,
	personType               int, --1 Pessoa, 2 Empresa, 4 Departamento
	profileType              int, --1 Agente, 2 Cliente, 3 Agente e Cliente
	accessProfile            nvarchar(128),
	corporateName            nvarchar(128),
	businessName             nvarchar(128),
	businessBranch           nvarchar(128),
	cpfCnpj                  nvarchar(14),
	userName                 nvarchar(64),
	[role]                   nvarchar(128),
	bossId                   nvarchar(64),
	bossName                 nvarchar(128),
	classification           nvarchar(128),
	cultureId                nvarchar(32),
	timeZoneId               nvarchar(64),
	authenticateOn           nvarchar(max),
	createdDate              datetime,
	createdById              nvarchar(128), --createdBy da API (Cód. Ref.)
	changedDate              datetime, --nulo para quem nunca foi alterado
	changedById              nvarchar(128), --changedBy da API (Cód. Ref.)
	observations             nvarchar(max)
);

create table [general].MovideskPeople (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	id                       nvarchar(64) not null,
	codeReferenceAdditional  nvarchar(64),
	isActive                 bit,
	personType               int, --1 Pessoa, 2 Empresa, 4 Departamento
	profileType              int, --1 Agente, 2 Cliente, 3 Agente e Cliente
	accessProfile            nvarchar(128),
	corporateName            nvarchar(128),
	businessName             nvarchar(128),
	businessBranch           nvarchar(128),
	cpfCnpj                  nvarchar(14),
	userName                 nvarchar(64),
	[role]                   nvarchar(128),
	bossId                   nvarchar(64),
	bossName                 nvarchar(128),
	classification           nvarchar(128),
	cultureId                nvarchar(32),
	timeZoneId               nvarchar(64),
	authenticateOn           nvarchar(max),
	createdDate              datetime,
	createdById              nvarchar(128), --createdBy da API (Cód. Ref.)
	changedDate              datetime, --nulo para quem nunca foi alterado
	changedById              nvarchar(128), --changedBy da API (Cód. Ref.)
	observations             nvarchar(max),
constraint [PK_MovideskPeople] primary key (
	doc_company asc,
	id asc
));

create table [untreated].MovideskPeopleAddresses (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	personId                 nvarchar(64),
	id                       int, --gerado em C#, posição na lista
	addressType              nvarchar(128),
	country                  nvarchar(128),
	countryId                int, --não documentado, vem na resposta real
	postalCode               nvarchar(32),
	[state]                  nvarchar(128),
	city                     nvarchar(128),
	district                 nvarchar(128),
	street                   nvarchar(128),
	number                   nvarchar(32),
	complement               nvarchar(128),
	reference                nvarchar(128),
	isDefault                bit
);

create table [general].MovideskPeopleAddresses (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	personId                 nvarchar(64) not null,
	id                       int not null, --gerado em C#, posição na lista
	addressType              nvarchar(128),
	country                  nvarchar(128),
	countryId                int, --não documentado, vem na resposta real
	postalCode               nvarchar(32),
	[state]                  nvarchar(128),
	city                     nvarchar(128),
	district                 nvarchar(128),
	street                   nvarchar(128),
	number                   nvarchar(32),
	complement               nvarchar(128),
	reference                nvarchar(128),
	isDefault                bit,
constraint [PK_MovideskPeopleAddresses] primary key (
	doc_company asc,
	personId asc,
	id asc
));

create table [untreated].MovideskPeopleContacts (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	personId                 nvarchar(64),
	id                       int, --gerado em C#, posição na lista
	contactType              nvarchar(128),
	contact                  nvarchar(128),
	isDefault                bit
);

create table [general].MovideskPeopleContacts (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	personId                 nvarchar(64) not null,
	id                       int not null, --gerado em C#, posição na lista
	contactType              nvarchar(128),
	contact                  nvarchar(128),
	isDefault                bit,
constraint [PK_MovideskPeopleContacts] primary key (
	doc_company asc,
	personId asc,
	id asc
));

create table [untreated].MovideskPeopleEmails (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	personId                 nvarchar(64),
	id                       int, --gerado em C#, posição na lista
	emailType                nvarchar(128),
	email                    nvarchar(128),
	isDefault                bit
);

create table [general].MovideskPeopleEmails (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	personId                 nvarchar(64) not null,
	id                       int not null, --gerado em C#, posição na lista
	emailType                nvarchar(128),
	email                    nvarchar(128),
	isDefault                bit,
constraint [PK_MovideskPeopleEmails] primary key (
	doc_company asc,
	personId asc,
	id asc
));

create table [untreated].MovideskPeopleTeams (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	personId                 nvarchar(64),
	team                     nvarchar(128)
);

create table [general].MovideskPeopleTeams (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	personId                 nvarchar(64) not null,
	team                     nvarchar(128) not null,
constraint [PK_MovideskPeopleTeams] primary key (
	doc_company asc,
	personId asc,
	team asc
));

create table [untreated].MovideskPeopleRelationships (
	integrated_or_updated_at         datetime,
	[doc_company]                    nvarchar(14),
	personId                         nvarchar(64),
	id                               nvarchar(64), --Cód. Ref. da organização ('' quando a API manda null)
	[name]                           nvarchar(128),
	slaAgreement                     nvarchar(128),
	forceChildrenToHaveSomeAgreement bit,
	allowAllServices                 bit,
	includeInParents                 bit,
	loadChildOrganizations           bit
);

create table [general].MovideskPeopleRelationships (
	integrated_or_updated_at         datetime,
	[doc_company]                    nvarchar(14) not null,
	personId                         nvarchar(64) not null,
	id                               nvarchar(64) not null, --Cód. Ref. da organização ('' quando a API manda null)
	[name]                           nvarchar(128),
	slaAgreement                     nvarchar(128),
	forceChildrenToHaveSomeAgreement bit,
	allowAllServices                 bit,
	includeInParents                 bit,
	loadChildOrganizations           bit,
constraint [PK_MovideskPeopleRelationships] primary key (
	doc_company asc,
	personId asc,
	id asc
));

create table [untreated].MovideskPeopleRelationshipsServices (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	personId                 nvarchar(64),
	relationshipId           nvarchar(64),
	id                       int, --id do serviço na Movidesk
	[name]                   nvarchar(128),
	copyToChildren           bit
);

create table [general].MovideskPeopleRelationshipsServices (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	personId                 nvarchar(64) not null,
	relationshipId           nvarchar(64) not null,
	id                       int not null, --id do serviço na Movidesk
	[name]                   nvarchar(128),
	copyToChildren           bit,
constraint [PK_MovideskPeopleRelationshipsServices] primary key (
	doc_company asc,
	personId asc,
	relationshipId asc,
	id asc
));

create table [untreated].MovideskPeopleCustomFieldValues (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	personId                 nvarchar(64),
	customFieldId            int,
	customFieldRuleId        int,
	line                     int,
	[value]                  nvarchar(max)
);

create table [general].MovideskPeopleCustomFieldValues (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	personId                 nvarchar(64) not null,
	customFieldId            int not null,
	customFieldRuleId        int not null,
	line                     int not null,
	[value]                  nvarchar(max),
constraint [PK_MovideskPeopleCustomFieldValues] primary key (
	doc_company asc,
	personId asc,
	customFieldId asc,
	customFieldRuleId asc,
	line asc
));

create table [untreated].MovideskPeopleCustomFieldValuesItems (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	personId                 nvarchar(64),
	customFieldId            int,
	customFieldRuleId        int,
	line                     int,
	id                       int, --gerado em C#, posição na lista
	itemPersonId             nvarchar(64), --personId do item
	itemClientId             nvarchar(64), --clientId do item
	team                     nvarchar(128),
	customFieldItem          nvarchar(256),
	storageFileGuid          nvarchar(36), --campo do tipo arquivo, como nos tickets; não documentado
	[fileName]               nvarchar(255) --idem
);

create table [general].MovideskPeopleCustomFieldValuesItems (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	personId                 nvarchar(64) not null,
	customFieldId            int not null,
	customFieldRuleId        int not null,
	line                     int not null,
	id                       int not null, --gerado em C#, posição na lista
	itemPersonId             nvarchar(64), --personId do item
	itemClientId             nvarchar(64), --clientId do item
	team                     nvarchar(128),
	customFieldItem          nvarchar(256),
	storageFileGuid          nvarchar(36), --campo do tipo arquivo, como nos tickets; não documentado
	[fileName]               nvarchar(255), --idem
constraint [PK_MovideskPeopleCustomFieldValuesItems] primary key (
	doc_company asc,
	personId asc,
	customFieldId asc,
	customFieldRuleId asc,
	line asc,
	id asc
));

create table [untreated].MovideskPeopleAssets (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	personId                 nvarchar(64),
	id                       nvarchar(64),
	[name]                   nvarchar(64),
	label                    nvarchar(64)
);

create table [general].MovideskPeopleAssets (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	personId                 nvarchar(64) not null,
	id                       nvarchar(64) not null,
	[name]                   nvarchar(64),
	label                    nvarchar(64),
constraint [PK_MovideskPeopleAssets] primary key (
	doc_company asc,
	personId asc,
	id asc
));

/****** Chaves estrangeiras: filhas -> pessoa (sem cascade, a pessoa nunca é apagada pela sincronização) ******/

alter table [general].MovideskPeopleAddresses add constraint [FK_MovideskPeopleAddresses] foreign key (doc_company, personId) references [general].MovideskPeople (doc_company, id);

alter table [general].MovideskPeopleContacts add constraint [FK_MovideskPeopleContacts] foreign key (doc_company, personId) references [general].MovideskPeople (doc_company, id);

alter table [general].MovideskPeopleEmails add constraint [FK_MovideskPeopleEmails] foreign key (doc_company, personId) references [general].MovideskPeople (doc_company, id);

alter table [general].MovideskPeopleTeams add constraint [FK_MovideskPeopleTeams] foreign key (doc_company, personId) references [general].MovideskPeople (doc_company, id);

alter table [general].MovideskPeopleRelationships add constraint [FK_MovideskPeopleRelationships] foreign key (doc_company, personId) references [general].MovideskPeople (doc_company, id);

alter table [general].MovideskPeopleCustomFieldValues add constraint [FK_MovideskPeopleCustomFieldValues] foreign key (doc_company, personId) references [general].MovideskPeople (doc_company, id);

alter table [general].MovideskPeopleAssets add constraint [FK_MovideskPeopleAssets] foreign key (doc_company, personId) references [general].MovideskPeople (doc_company, id);


/****** Chaves estrangeiras: netas -> nível do meio (on delete cascade) ******/

alter table [general].MovideskPeopleRelationshipsServices add constraint [FK_MovideskPeopleRelationshipsServices] foreign key (doc_company, personId, relationshipId) references [general].MovideskPeopleRelationships (doc_company, personId, id) on delete cascade;

alter table [general].MovideskPeopleCustomFieldValuesItems add constraint [FK_MovideskPeopleCustomFieldValuesItems] foreign key (doc_company, personId, customFieldId, customFieldRuleId, line) references [general].MovideskPeopleCustomFieldValues (doc_company, personId, customFieldId, customFieldRuleId, line) on delete cascade;


/****** Tabelas de valores (iguais para qualquer conta Movidesk, sem doc_company e sem merge) ******/
create table [general].MovideskPersonTypes (
	id				int not null primary key,
	[description]	nvarchar(60) not null
);

create table [general].MovideskProfileTypes (
	id				int not null primary key,
	[description]	nvarchar(60) not null
);

insert into [general].MovideskPersonTypes (id, [description])
values
(1, N'Pessoa'),
(2, N'Empresa'),
(4, N'Departamento');

insert into [general].MovideskProfileTypes (id, [description])
values
(1, N'Agente'),
(2, N'Cliente'),
(3, N'Agente e Cliente');
