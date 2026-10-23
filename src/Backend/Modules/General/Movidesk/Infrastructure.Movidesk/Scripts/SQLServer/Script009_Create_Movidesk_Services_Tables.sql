/****** Serviços: tabelas untreated e movidesk (docs/IMPLEMENTACAO-MOVIDESK-SERVICES.md) ******/

create table [untreated].MovideskServices (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	id                       int,
	[name]                   nvarchar(128),
	[description]            nvarchar(max),
	parentServiceId          int, --serviço pai (nulo na raiz)
	serviceForTicketType     int, --0 Público, 1 Interno, 2 Públicos e internos
	isVisible                int, --1 Agente, 2 Cliente, 3 Agente e Cliente
	allowSelection           int, --1 Agente, 2 Cliente, 3 Agente e Cliente (0 visto na resposta real, não documentado)
	allowFinishTicket        bit,
	isActive                 bit, --já vem false para filhos de um serviço desabilitado
	automationMacro          nvarchar(64),
	defaultCategory          nvarchar(128),
	defaultUrgency           nvarchar(128),
	allowAllCategories       bit
);

create table [movidesk].MovideskServices (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	id                       int not null,
	[name]                   nvarchar(128),
	[description]            nvarchar(max),
	parentServiceId          int, --serviço pai (nulo na raiz)
	serviceForTicketType     int, --0 Público, 1 Interno, 2 Públicos e internos
	isVisible                int, --1 Agente, 2 Cliente, 3 Agente e Cliente
	allowSelection           int, --1 Agente, 2 Cliente, 3 Agente e Cliente (0 visto na resposta real, não documentado)
	allowFinishTicket        bit,
	isActive                 bit, --já vem false para filhos de um serviço desabilitado
	automationMacro          nvarchar(64),
	defaultCategory          nvarchar(128),
	defaultUrgency           nvarchar(128),
	allowAllCategories       bit,
constraint [PK_MovideskServices] primary key (
	doc_company asc,
	id asc
));

create table [untreated].MovideskServicesCategories (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	serviceId                int,
	category                 nvarchar(128)
);

create table [movidesk].MovideskServicesCategories (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	serviceId                int not null,
	category                 nvarchar(128) not null,
constraint [PK_MovideskServicesCategories] primary key (
	doc_company asc,
	serviceId asc,
	category asc
));

/****** Chave estrangeira: categorias -> serviço (sem FK em parentServiceId: pai e filho chegam no mesmo lote) ******/

alter table [movidesk].MovideskServicesCategories add constraint [FK_MovideskServicesCategories] foreign key (doc_company, serviceId) references [movidesk].MovideskServices (doc_company, id);


/****** Tabelas de valores (iguais para qualquer conta Movidesk, sem doc_company e sem merge) ******/
create table [movidesk].MovideskServiceTicketTypes (
	id				int not null primary key,
	[description]	nvarchar(60) not null
);

create table [movidesk].MovideskServiceAudiences (
	id				int not null primary key,
	[description]	nvarchar(60) not null
);

insert into [movidesk].MovideskServiceTicketTypes (id, [description])
values
(0, N'Público'),
(1, N'Interno'),
(2, N'Públicos e internos');

--usada por isVisible e allowSelection
insert into [movidesk].MovideskServiceAudiences (id, [description])
values
(0, N'Ninguém (não documentado)'),
(1, N'Agente'),
(2, N'Cliente'),
(3, N'Agente e Cliente');
