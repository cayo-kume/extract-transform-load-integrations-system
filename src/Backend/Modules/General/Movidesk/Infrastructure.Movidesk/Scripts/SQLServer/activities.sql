create table general.MovideskActivities (
	id				int not null,
	[name]			varchar(128),
	isActive		bit,
	isAllowAllTeams	bit,
	constraint pk_Activities primary key (id)
);

create table general.MovideskActivitiesTeams (
	movideskActivitiesId	int not null,
	id						int not null, --gerar com laço foreach no c-sharp
	[name]					varchar(128),
	constraint pk_ActivitiesTeams primary key (movideskActivitiesId, id)
);