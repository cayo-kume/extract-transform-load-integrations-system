-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; docs/MIGRATOR-CSX.md)
/****** Serviços: tabelas untreated e movidesk (src/Backend/Modules/General/Movidesk/docs/IMPLEMENTACAO-MOVIDESK-SERVICES.md) ******/

if dbo.fn_Migrator_Creates(N'[untreated].MovideskServices') = 1
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

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskServices') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].MovideskServicesCategories') = 1
create table [untreated].MovideskServicesCategories (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	serviceId                int,
	category                 nvarchar(128)
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskServicesCategories') = 1
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

if object_id(N'[movidesk].[FK_MovideskServicesCategories]') is null and object_id(N'[movidesk].MovideskServicesCategories') is not null and object_id(N'[movidesk].MovideskServices') is not null
alter table [movidesk].MovideskServicesCategories add constraint [FK_MovideskServicesCategories] foreign key (doc_company, serviceId) references [movidesk].MovideskServices (doc_company, id);


/****** Tabelas de valores (iguais para qualquer conta Movidesk, sem doc_company e sem merge) ******/
if object_id(N'[movidesk].MovideskServiceTicketTypes') is null
create table [movidesk].MovideskServiceTicketTypes (
	id				int not null primary key,
	[description]	nvarchar(60) not null
);

if object_id(N'[movidesk].MovideskServiceAudiences') is null
create table [movidesk].MovideskServiceAudiences (
	id				int not null primary key,
	[description]	nvarchar(60) not null
);

if not exists (select 1 from [movidesk].MovideskServiceTicketTypes)
insert into [movidesk].MovideskServiceTicketTypes (id, [description])
values
(0, N'Público'),
(1, N'Interno'),
(2, N'Públicos e internos');

--usada por isVisible e allowSelection
if not exists (select 1 from [movidesk].MovideskServiceAudiences)
insert into [movidesk].MovideskServiceAudiences (id, [description])
values
(0, N'Ninguém (não documentado)'),
(1, N'Agente'),
(2, N'Cliente'),
(3, N'Agente e Cliente');
