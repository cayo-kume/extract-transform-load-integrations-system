create table general.MovideskServices (
	id						int not null,
	[name]					varchar(128),
	[description]			varchar(1024),
	parentServiceId			int,
	serviceForTicketType	int,
	isVisible				int,
	allowSelection			int,
	allowFinishTicket		bit,
	isActive				bit,
	automationMacro			varchar(64),
	defaultCategory			varchar(128),
	defaultUrgency			varchar(128),
	allowAllCategories		bit,
	constraint pk_Services primary key (id)
);

create table general.MovideskServicesCategories (
	serviceId	int not null,
	id			int not null, --gerar com incremental no csharp
	category	varchar(128),
	constraint pk_ServicesCategories primary key (serviceId, id)
);

create table general.MovideskServicesTypes (
	id		int not null primary key,
	[name]	varchar(128)
);

alter table general.MovideskServicesCategories add constraint fk_ServicesCategories foreign key (serviceId) references general.MovideskServices (id);
alter table general.MovideskServices add constraint fk_ServicesForTicketTypes foreign key (serviceForTicketType) references general.MovideskServicesTypes (id);
alter table general.MovideskServices add constraint fk_ServicesIsVisibles foreign key (isVisible) references general.MovideskProfilesTypes (profileType);
alter table general.MovideskServices add constraint fk_ServicesAllowSelections foreign key (allowSelection) references general.MovideskProfilesTypes (profileType);

insert into general.MovideskServicesTypes (id, [name]) values
(0, 'Publico'),
(1, 'Interno'),
(2, 'Publicos e Internos');