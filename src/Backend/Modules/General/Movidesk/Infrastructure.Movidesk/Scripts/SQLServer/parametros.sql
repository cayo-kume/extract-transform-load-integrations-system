create table general.MovideskProfilesTypes (
	profileType		int primary key not null,
	[description]	varchar(60) not null
);

insert into general.MovideskProfilesTypes (profileType, [description]) values
(1, 'Agente'),
(2, 'Cliente'),
(3, 'Agente e Cliente');