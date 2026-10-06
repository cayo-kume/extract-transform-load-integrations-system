create table general.MovideskPeople (
	id						varchar(64) not null,
	codeReferenceAdditional	varchar(64),
	isActive				bit not null,
	personType				int not null,
	profileType				int not null,
	accessProfile			varchar(128),
	corporateName			varchar(128),
	businessName			varchar(128) not null,
	businessBranch			varchar(128),
	cpfCnpj					varchar(14),
	userName				varchar(64),
	[password]				varchar(32),
	[role]					varchar(128),
	bossId					varchar(64),
	bossName				varchar(128),
	[classification]		varchar(128),
	cultureId				varchar(32),
	timeZoneId				varchar(64),
	authenticateOn			varchar(max),
	createdDate				datetime,
	createdBy				varchar(128),
	changedDate				datetime,
	changedBy				varchar(128),
	observations			varchar(max),
	constraint pk_People primary key (id)
);

create table general.MovideskPeopleAddresses (
	personId						varchar(64) not null,
	addressType						varchar(128) not null,
	country							varchar(128),
	postalCode						varchar(32),
	[state]							varchar(128),
	city							varchar(128),
	district						varchar(128),
	street							varchar(128),
	number							varchar(32),
	complement						varchar(128),
	reference						varchar(128),
	isDefault						bit not null,
	countryId						int,
	constraint pk_PeopleAddresses primary key (personId, addressType)
);

create table general.MovideskPeopleContacts (
	personId						varchar(64) not null,
	contactType						varchar(128) not null,
	contact							varchar(128),
	isDefault						bit,
	constraint pk_PeopleContacts primary key (personId, contactType)
);

create table general.MovideskPeopleEmails (
	personId						varchar(64) not null,
	emailType						varchar(128) not null,
	email							varchar(128) not null,
	isDefault						bit,
	constraint pk_PeopleEmails primary key (personId, emailType)
);

create table general.MovideskPeopleRelationships (
	personId							varchar(64) not null,
	id									varchar(64) not null,
	[name]								varchar(128),
	slaAgreement						varchar(128),
	forceChildrenToHaveSomeAgreement	bit,
	allowAllServices					bit,
	includeInParents					bit,
	loadChildOrganizations				bit,
	isGetMethod							bit,
	constraint pk_PeopleRelationships primary key (personId, id)
);

create table general.MovideskPeopleRelationshipsServices (
	personId						varchar(64) not null,
	relationshipId					varchar(64) not null,
	id								int not null,
	[name]							varchar(128),
	copyToChildren					bit,
	constraint pk_PeopleRelationshipsServices primary key (personId, relationshipId, id)
);

create table general.MovideskPeopleCustomFieldValues (
	personId						varchar(64) not null,
	customFieldId					int not null,
	customFieldRuleId				int,
	line							int,
	[value]							varchar(max),
	constraint pk_PeopleCustomFieldValues primary key (personId, customFieldId)
);

create table general.MovideskPeopleCustomFieldValuesItens (
	personId						varchar(64) not null,
	customFieldId					int not null,
	clientId						int not null,
	team							varchar(128),
	customFieldItem					varchar(256),
	constraint pk_PeopleCustomFieldValuesItens primary key (personId, customFieldId, clientId)
);

create table general.MovideskPeopleAssets (
	personId						varchar(64) not null,
	id								varchar(64) not null,
	[name]							varchar(64),
	[label]							varchar(64),
	constraint pk_PeopleAssets primary key (personId, id)
);

create table general.MovideskPeopleTypes (
	personType		int primary key not null,
	[description]	varchar(60) not null
);

create table general.MovideskPeopleTeams (
	personId	varchar(64) not null,
	id			int primary key not null, --gerar no laço foreach no c-sharp
	[team]		varchar(128)
);

insert into general.MovideskPeopleTypes (personType, [description]) values
(1, 'Pessoa'),
(2, 'Empresa'),
(4, 'Departamento');

alter table general.MovideskPeople add constraint fk_PeopleTypes foreign key (personType) references general.MovideskPeopleTypes (personType);
alter table general.MovideskPeople add constraint fk_PeopleProfilesTypes foreign key (profileType) references general.MovideskProfilesTypes (profileType);
alter table general.MovideskPeopleTeams add constraint fk_PeopleTeams foreign key (personId) references general.MovideskPeople (id);
alter table general.MovideskPeopleAssets add constraint fk_PeopleAssets foreign key (personId) references general.MovideskPeople (id);
alter table general.MovideskPeopleCustomFieldValuesItens add constraint fk_CustomFieldValuesItens foreign key (personId, customFieldId) references general.MovideskPeopleCustomFieldValues (personId, customFieldId);
alter table general.MovideskPeopleCustomFieldValues add constraint fk_CustomFieldValues foreign key (personId) references general.MovideskPeople (id);
alter table general.MovideskPeopleRelationshipsServices add constraint fk_RelationshipsServices foreign key (personId, relationshipId) references general.MovideskPeopleRelationships (personId, id);
alter table general.MovideskPeopleRelationships add constraint fk_Relationships foreign key (personId) references general.MovideskPeople (id);
alter table general.MovideskPeopleEmails add constraint fk_PeopleEmails foreign key (personId) references general.MovideskPeople (id);
alter table general.MovideskPeopleContacts add constraint fk_PeopleContacts foreign key (personId) references general.MovideskPeople (id);
alter table general.MovideskPeopleAddresses add constraint fk_PeopleAddresses foreign key (personId) references general.MovideskPeople (id);