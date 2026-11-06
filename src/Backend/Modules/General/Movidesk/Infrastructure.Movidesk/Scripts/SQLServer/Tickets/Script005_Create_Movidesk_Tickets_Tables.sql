-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; .docs/MIGRATOR-CSX.md)
/****** Tickets: tabelas untreated e movidesk ******/

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTickets') = 1
create table [untreated].MovideskTickets (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	id                       bigint,
	protocol                 nvarchar(30),
	[type]                   int,
	[subject]                nvarchar(350),
	category                 nvarchar(128),
	urgency                  nvarchar(128),
	[status]                 nvarchar(128),
	baseStatus               nvarchar(128),
	justification            nvarchar(128),
	origin                   int,
	createdDate              datetime,
	originEmailAccount       nvarchar(128),
	ownerId                  nvarchar(64),
	ownerTeam                nvarchar(128),
	createdById              nvarchar(64),
	serviceFirstLevelId      int,
	serviceFirstLevel        nvarchar(1024),
	serviceSecondLevel       nvarchar(1024),
	serviceThirdLevel        nvarchar(1024),
	contactForm              nvarchar(128),
	cc                       nvarchar(1024),
	resolvedIn               datetime,
	reopenedIn               datetime,
	closedIn                 datetime,
	canceledIn               datetime, --não documentado, vem na resposta real
	isDeleted                bit, --não documentado, vem na resposta real
	lastActionDate           datetime,
	actionCount              int,
	lastUpdate               datetime,
	lifetimeWorkingTime      int,
	stoppedTime              int,
	stoppedTimeWorkingTime   int,
	resolvedInFirstCall      bit,
	chatWidget               nvarchar(128),
	chatGroup                nvarchar(128),
	chatTalkTime             int,
	chatWaitingTime          int,
	[sequence]               int,
	slaAgreement             nvarchar(128),
	slaAgreementRule         nvarchar(128),
	slaSolutionTime          int,
	slaResponseTime          int,
	slaSolutionChangedByUser bit,
	slaSolutionChangedById   nvarchar(64),
	slaSolutionDate          datetime,
	slaSolutionDateIsPaused  bit,
	slaResponseDate          datetime,
	slaRealResponseDate      datetime,
	jiraIssueKey             nvarchar(64),
	redmineIssueId           int
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTickets') = 1
create table [movidesk].MovideskTickets (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	id                       bigint not null,
	protocol                 nvarchar(30),
	[type]                   int,
	[subject]                nvarchar(350),
	category                 nvarchar(128),
	urgency                  nvarchar(128),
	[status]                 nvarchar(128),
	baseStatus               nvarchar(128),
	justification            nvarchar(128),
	origin                   int,
	createdDate              datetime,
	originEmailAccount       nvarchar(128),
	ownerId                  nvarchar(64),
	ownerTeam                nvarchar(128),
	createdById              nvarchar(64),
	serviceFirstLevelId      int,
	serviceFirstLevel        nvarchar(1024),
	serviceSecondLevel       nvarchar(1024),
	serviceThirdLevel        nvarchar(1024),
	contactForm              nvarchar(128),
	cc                       nvarchar(1024),
	resolvedIn               datetime,
	reopenedIn               datetime,
	closedIn                 datetime,
	canceledIn               datetime, --não documentado, vem na resposta real
	isDeleted                bit, --não documentado, vem na resposta real
	lastActionDate           datetime,
	actionCount              int,
	lastUpdate               datetime,
	lifetimeWorkingTime      int,
	stoppedTime              int,
	stoppedTimeWorkingTime   int,
	resolvedInFirstCall      bit,
	chatWidget               nvarchar(128),
	chatGroup                nvarchar(128),
	chatTalkTime             int,
	chatWaitingTime          int,
	[sequence]               int,
	slaAgreement             nvarchar(128),
	slaAgreementRule         nvarchar(128),
	slaSolutionTime          int,
	slaResponseTime          int,
	slaSolutionChangedByUser bit,
	slaSolutionChangedById   nvarchar(64),
	slaSolutionDate          datetime,
	slaSolutionDateIsPaused  bit,
	slaResponseDate          datetime,
	slaRealResponseDate      datetime,
	jiraIssueKey             nvarchar(64),
	redmineIssueId           int,
constraint [PK_MovideskTickets] primary key (
	doc_company asc,
	id asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsTags') = 1
create table [untreated].MovideskTicketsTags (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	tag                      nvarchar(128)
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsTags') = 1
create table [movidesk].MovideskTicketsTags (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	tag                      nvarchar(128) not null,
constraint [PK_MovideskTicketsTags] primary key (
	doc_company asc,
	ticketId asc,
	tag asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsClients') = 1
create table [untreated].MovideskTicketsClients (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	clientId                 nvarchar(64),
	organizationId           nvarchar(64),
	isDeleted                bit
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsClients') = 1
create table [movidesk].MovideskTicketsClients (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	clientId                 nvarchar(64) not null,
	organizationId           nvarchar(64),
	isDeleted                bit,
constraint [PK_MovideskTicketsClients] primary key (
	doc_company asc,
	ticketId asc,
	clientId asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsRelationships') = 1
create table [untreated].MovideskTicketsRelationships (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	relatedTicketId          bigint,
	relationType             nvarchar(10), --'Parent' | 'Child'
	[subject]                nvarchar(350),
	isDeleted                bit
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsRelationships') = 1
create table [movidesk].MovideskTicketsRelationships (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	relatedTicketId          bigint not null,
	relationType             nvarchar(10), --'Parent' | 'Child'
	[subject]                nvarchar(350),
	isDeleted                bit,
constraint [PK_MovideskTicketsRelationships] primary key (
	doc_company asc,
	ticketId asc,
	relatedTicketId asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsOwnerHistories') = 1
create table [untreated].MovideskTicketsOwnerHistories (
	integrated_or_updated_at  datetime,
	[doc_company]             nvarchar(14),
	ticketId                  bigint,
	id                        int, --gerado em C#, na ordem de changedDate
	ownerTeam                 nvarchar(128),
	ownerId                   nvarchar(64),
	permanencyTimeFullTime    decimal(18,2),
	permanencyTimeWorkingTime decimal(18,2),
	changedById               nvarchar(64),
	changedDate               datetime
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsOwnerHistories') = 1
create table [movidesk].MovideskTicketsOwnerHistories (
	integrated_or_updated_at  datetime,
	[doc_company]             nvarchar(14) not null,
	ticketId                  bigint not null,
	id                        int not null, --gerado em C#, na ordem de changedDate
	ownerTeam                 nvarchar(128),
	ownerId                   nvarchar(64),
	permanencyTimeFullTime    decimal(18,2),
	permanencyTimeWorkingTime decimal(18,2),
	changedById               nvarchar(64),
	changedDate               datetime,
constraint [PK_MovideskTicketsOwnerHistories] primary key (
	doc_company asc,
	ticketId asc,
	id asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsStatusHistories') = 1
create table [untreated].MovideskTicketsStatusHistories (
	integrated_or_updated_at  datetime,
	[doc_company]             nvarchar(14),
	ticketId                  bigint,
	id                        int, --gerado em C#, na ordem de changedDate
	[status]                  nvarchar(128),
	justification             nvarchar(128),
	permanencyTimeFullTime    decimal(18,2),
	permanencyTimeWorkingTime decimal(18,2),
	changedById               nvarchar(64),
	changedDate               datetime
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsStatusHistories') = 1
create table [movidesk].MovideskTicketsStatusHistories (
	integrated_or_updated_at  datetime,
	[doc_company]             nvarchar(14) not null,
	ticketId                  bigint not null,
	id                        int not null, --gerado em C#, na ordem de changedDate
	[status]                  nvarchar(128),
	justification             nvarchar(128),
	permanencyTimeFullTime    decimal(18,2),
	permanencyTimeWorkingTime decimal(18,2),
	changedById               nvarchar(64),
	changedDate               datetime,
constraint [PK_MovideskTicketsStatusHistories] primary key (
	doc_company asc,
	ticketId asc,
	id asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsCustomFieldValues') = 1
create table [untreated].MovideskTicketsCustomFieldValues (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	customFieldId            int,
	customFieldRuleId        int,
	line                     int,
	[value]                  nvarchar(max)
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsCustomFieldValues') = 1
create table [movidesk].MovideskTicketsCustomFieldValues (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	customFieldId            int not null,
	customFieldRuleId        int not null,
	line                     int not null,
	[value]                  nvarchar(max),
constraint [PK_MovideskTicketsCustomFieldValues] primary key (
	doc_company asc,
	ticketId asc,
	customFieldId asc,
	customFieldRuleId asc,
	line asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsCustomFieldValuesItems') = 1
create table [untreated].MovideskTicketsCustomFieldValuesItems (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	customFieldId            int,
	customFieldRuleId        int,
	line                     int,
	id                       int, --gerado em C#, posição na lista
	personId                 nvarchar(64),
	clientId                 nvarchar(64),
	team                     nvarchar(128),
	customFieldItem          nvarchar(256),
	storageFileGuid          nvarchar(36), --não documentado: campo adicional do tipo arquivo
	[fileName]               nvarchar(255) --não documentado: campo adicional do tipo arquivo
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsCustomFieldValuesItems') = 1
create table [movidesk].MovideskTicketsCustomFieldValuesItems (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	customFieldId            int not null,
	customFieldRuleId        int not null,
	line                     int not null,
	id                       int not null, --gerado em C#, posição na lista
	personId                 nvarchar(64),
	clientId                 nvarchar(64),
	team                     nvarchar(128),
	customFieldItem          nvarchar(256),
	storageFileGuid          nvarchar(36), --não documentado: campo adicional do tipo arquivo
	[fileName]               nvarchar(255), --não documentado: campo adicional do tipo arquivo
constraint [PK_MovideskTicketsCustomFieldValuesItems] primary key (
	doc_company asc,
	ticketId asc,
	customFieldId asc,
	customFieldRuleId asc,
	line asc,
	id asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsAssets') = 1
create table [untreated].MovideskTicketsAssets (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	id                       nvarchar(64),
	[name]                   nvarchar(128),
	label                    nvarchar(128),
	serialNumber             nvarchar(128),
	categoryFirstLevel       nvarchar(128),
	categorySecondLevel      nvarchar(128),
	categoryThirdLevel       nvarchar(128),
	isDeleted                bit
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsAssets') = 1
create table [movidesk].MovideskTicketsAssets (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	id                       nvarchar(64) not null,
	[name]                   nvarchar(128),
	label                    nvarchar(128),
	serialNumber             nvarchar(128),
	categoryFirstLevel       nvarchar(128),
	categorySecondLevel      nvarchar(128),
	categoryThirdLevel       nvarchar(128),
	isDeleted                bit,
constraint [PK_MovideskTicketsAssets] primary key (
	doc_company asc,
	ticketId asc,
	id asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsActions') = 1
create table [untreated].MovideskTicketsActions (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	id                       int,
	[type]                   int,
	origin                   int,
	[description]            nvarchar(max),
	htmlDescription          nvarchar(max), --só vem na busca por id
	[status]                 nvarchar(128),
	justification            nvarchar(128),
	createdDate              datetime,
	createdById              nvarchar(64),
	isDeleted                bit
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsActions') = 1
create table [movidesk].MovideskTicketsActions (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	id                       int not null,
	[type]                   int,
	origin                   int,
	[description]            nvarchar(max),
	htmlDescription          nvarchar(max), --só vem na busca por id
	[status]                 nvarchar(128),
	justification            nvarchar(128),
	createdDate              datetime,
	createdById              nvarchar(64),
	isDeleted                bit,
constraint [PK_MovideskTicketsActions] primary key (
	doc_company asc,
	ticketId asc,
	id asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsActionsTags') = 1
create table [untreated].MovideskTicketsActionsTags (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	actionId                 int,
	tag                      nvarchar(128)
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsActionsTags') = 1
create table [movidesk].MovideskTicketsActionsTags (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	actionId                 int not null,
	tag                      nvarchar(128) not null,
constraint [PK_MovideskTicketsActionsTags] primary key (
	doc_company asc,
	ticketId asc,
	actionId asc,
	tag asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsActionsTimeAppointments') = 1
create table [untreated].MovideskTicketsActionsTimeAppointments (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	actionId                 int,
	id                       bigint,
	activity                 nvarchar(128),
	[date]                   datetime,
	periodStart              time,
	periodEnd                time,
	workTime                 time,
	accountedTime            decimal(18,10),
	workTypeName             nvarchar(128),
	createdById              nvarchar(64),
	createdByTeamId          int,
	createdByTeamName        nvarchar(128)
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsActionsTimeAppointments') = 1
create table [movidesk].MovideskTicketsActionsTimeAppointments (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	actionId                 int not null,
	id                       bigint not null,
	activity                 nvarchar(128),
	[date]                   datetime,
	periodStart              time,
	periodEnd                time,
	workTime                 time,
	accountedTime            decimal(18,10),
	workTypeName             nvarchar(128),
	createdById              nvarchar(64),
	createdByTeamId          int,
	createdByTeamName        nvarchar(128),
constraint [PK_MovideskTicketsActionsTimeAppointments] primary key (
	doc_company asc,
	ticketId asc,
	actionId asc,
	id asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsActionsExpenses') = 1
create table [untreated].MovideskTicketsActionsExpenses (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	actionId                 int,
	id                       bigint,
	[type]                   nvarchar(128),
	serviceReport            nvarchar(128),
	createdById              nvarchar(64),
	createdByTeamId          int,
	createdByTeamName        nvarchar(128),
	[date]                   datetime,
	quantity                 int,
	[value]                  decimal(18,2)
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsActionsExpenses') = 1
create table [movidesk].MovideskTicketsActionsExpenses (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	actionId                 int not null,
	id                       bigint not null,
	[type]                   nvarchar(128),
	serviceReport            nvarchar(128),
	createdById              nvarchar(64),
	createdByTeamId          int,
	createdByTeamName        nvarchar(128),
	[date]                   datetime,
	quantity                 int,
	[value]                  decimal(18,2),
constraint [PK_MovideskTicketsActionsExpenses] primary key (
	doc_company asc,
	ticketId asc,
	actionId asc,
	id asc
));

if dbo.fn_Migrator_Creates(N'[untreated].MovideskTicketsActionsAttachments') = 1
create table [untreated].MovideskTicketsActionsAttachments (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	actionId                 int,
	[path]                   nvarchar(255), --hash do arquivo
	[fileName]               nvarchar(255),
	createdById              nvarchar(64),
	createdDate              datetime
);

if dbo.fn_Migrator_Creates(N'[movidesk].MovideskTicketsActionsAttachments') = 1
create table [movidesk].MovideskTicketsActionsAttachments (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	actionId                 int not null,
	[path]                   nvarchar(255) not null, --hash do arquivo
	[fileName]               nvarchar(255),
	createdById              nvarchar(64),
	createdDate              datetime,
constraint [PK_MovideskTicketsActionsAttachments] primary key (
	doc_company asc,
	ticketId asc,
	actionId asc,
	[path] asc
));

/****** Chaves estrangeiras: filhas -> ticket (sem cascade, o ticket nunca é apagado pela sincronização) ******/

if object_id(N'[movidesk].[FK_MovideskTicketsTags]') is null and object_id(N'[movidesk].MovideskTicketsTags') is not null and object_id(N'[movidesk].MovideskTickets') is not null
alter table [movidesk].MovideskTicketsTags add constraint [FK_MovideskTicketsTags] foreign key (doc_company, ticketId) references [movidesk].MovideskTickets (doc_company, id);

if object_id(N'[movidesk].[FK_MovideskTicketsClients]') is null and object_id(N'[movidesk].MovideskTicketsClients') is not null and object_id(N'[movidesk].MovideskTickets') is not null
alter table [movidesk].MovideskTicketsClients add constraint [FK_MovideskTicketsClients] foreign key (doc_company, ticketId) references [movidesk].MovideskTickets (doc_company, id);

if object_id(N'[movidesk].[FK_MovideskTicketsRelationships]') is null and object_id(N'[movidesk].MovideskTicketsRelationships') is not null and object_id(N'[movidesk].MovideskTickets') is not null
alter table [movidesk].MovideskTicketsRelationships add constraint [FK_MovideskTicketsRelationships] foreign key (doc_company, ticketId) references [movidesk].MovideskTickets (doc_company, id);

if object_id(N'[movidesk].[FK_MovideskTicketsOwnerHistories]') is null and object_id(N'[movidesk].MovideskTicketsOwnerHistories') is not null and object_id(N'[movidesk].MovideskTickets') is not null
alter table [movidesk].MovideskTicketsOwnerHistories add constraint [FK_MovideskTicketsOwnerHistories] foreign key (doc_company, ticketId) references [movidesk].MovideskTickets (doc_company, id);

if object_id(N'[movidesk].[FK_MovideskTicketsStatusHistories]') is null and object_id(N'[movidesk].MovideskTicketsStatusHistories') is not null and object_id(N'[movidesk].MovideskTickets') is not null
alter table [movidesk].MovideskTicketsStatusHistories add constraint [FK_MovideskTicketsStatusHistories] foreign key (doc_company, ticketId) references [movidesk].MovideskTickets (doc_company, id);

if object_id(N'[movidesk].[FK_MovideskTicketsCustomFieldValues]') is null and object_id(N'[movidesk].MovideskTicketsCustomFieldValues') is not null and object_id(N'[movidesk].MovideskTickets') is not null
alter table [movidesk].MovideskTicketsCustomFieldValues add constraint [FK_MovideskTicketsCustomFieldValues] foreign key (doc_company, ticketId) references [movidesk].MovideskTickets (doc_company, id);

if object_id(N'[movidesk].[FK_MovideskTicketsAssets]') is null and object_id(N'[movidesk].MovideskTicketsAssets') is not null and object_id(N'[movidesk].MovideskTickets') is not null
alter table [movidesk].MovideskTicketsAssets add constraint [FK_MovideskTicketsAssets] foreign key (doc_company, ticketId) references [movidesk].MovideskTickets (doc_company, id);

if object_id(N'[movidesk].[FK_MovideskTicketsActions]') is null and object_id(N'[movidesk].MovideskTicketsActions') is not null and object_id(N'[movidesk].MovideskTickets') is not null
alter table [movidesk].MovideskTicketsActions add constraint [FK_MovideskTicketsActions] foreign key (doc_company, ticketId) references [movidesk].MovideskTickets (doc_company, id);


/****** Chaves estrangeiras: netas -> nível do meio (on delete cascade, ver src/Backend/Modules/General/Movidesk/.docs/MOVIDESK-MODELAGEM-TICKETS.md 4.8) ******/

if object_id(N'[movidesk].[FK_MovideskTicketsCustomFieldValuesItems]') is null and object_id(N'[movidesk].MovideskTicketsCustomFieldValuesItems') is not null and object_id(N'[movidesk].MovideskTicketsCustomFieldValues') is not null
alter table [movidesk].MovideskTicketsCustomFieldValuesItems add constraint [FK_MovideskTicketsCustomFieldValuesItems] foreign key (doc_company, ticketId, customFieldId, customFieldRuleId, line) references [movidesk].MovideskTicketsCustomFieldValues (doc_company, ticketId, customFieldId, customFieldRuleId, line) on delete cascade;

if object_id(N'[movidesk].[FK_MovideskTicketsActionsTags]') is null and object_id(N'[movidesk].MovideskTicketsActionsTags') is not null and object_id(N'[movidesk].MovideskTicketsActions') is not null
alter table [movidesk].MovideskTicketsActionsTags add constraint [FK_MovideskTicketsActionsTags] foreign key (doc_company, ticketId, actionId) references [movidesk].MovideskTicketsActions (doc_company, ticketId, id) on delete cascade;

if object_id(N'[movidesk].[FK_MovideskTicketsActionsTimeAppointments]') is null and object_id(N'[movidesk].MovideskTicketsActionsTimeAppointments') is not null and object_id(N'[movidesk].MovideskTicketsActions') is not null
alter table [movidesk].MovideskTicketsActionsTimeAppointments add constraint [FK_MovideskTicketsActionsTimeAppointments] foreign key (doc_company, ticketId, actionId) references [movidesk].MovideskTicketsActions (doc_company, ticketId, id) on delete cascade;

if object_id(N'[movidesk].[FK_MovideskTicketsActionsExpenses]') is null and object_id(N'[movidesk].MovideskTicketsActionsExpenses') is not null and object_id(N'[movidesk].MovideskTicketsActions') is not null
alter table [movidesk].MovideskTicketsActionsExpenses add constraint [FK_MovideskTicketsActionsExpenses] foreign key (doc_company, ticketId, actionId) references [movidesk].MovideskTicketsActions (doc_company, ticketId, id) on delete cascade;

if object_id(N'[movidesk].[FK_MovideskTicketsActionsAttachments]') is null and object_id(N'[movidesk].MovideskTicketsActionsAttachments') is not null and object_id(N'[movidesk].MovideskTicketsActions') is not null
alter table [movidesk].MovideskTicketsActionsAttachments add constraint [FK_MovideskTicketsActionsAttachments] foreign key (doc_company, ticketId, actionId) references [movidesk].MovideskTicketsActions (doc_company, ticketId, id) on delete cascade;


/****** Tabelas de valores (iguais para qualquer conta Movidesk, sem doc_company e sem merge) ******/
if object_id(N'[movidesk].MovideskTicketsOrigins') is null
create table [movidesk].MovideskTicketsOrigins (
	id				int not null primary key,
	[name]			nvarchar(60) not null,
	[description]	nvarchar(128)
);

if object_id(N'[movidesk].MovideskTicketsTypes') is null
create table [movidesk].MovideskTicketsTypes (
	id				int not null primary key,
	[description]	nvarchar(60) not null
);

if not exists (select 1 from [movidesk].MovideskTicketsOrigins)
insert into [movidesk].MovideskTicketsOrigins (id, [name], [description])
values

(0, N'First Action', N'First Action'),
(1, N'Via web pelo cliente', N'Via web pelo cliente'),
(2, N'Via web pelo agente', N'Via web pelo agente'),
(3, N'Recebido via email', N'Recebido via email'),
(4, N'Gatilho do sistema', N'Gatilho do sistema'),
(5, N'Chat (online)', N'Chat (online)'),
(6, N'Chat (offline)', N'Chat (offline)'),
(7, N'Email enviado pelo sistema', N'Email enviado pelo sistema'),
(8, N'Formulário de contato', N'Formulário de contato'),
(9, N'Via web API', N'Via web API'),
(10, N'Agendamento automático', N'Abertura automática de tickets'),
(11, N'JiraIssue', N'Issue integração Jira'),
(12, N'RedmineIssue', N'Issue integração Redmine'),
(13, N'ReceivedCall', N'Chamada recebida integração Telefonia'),
(14, N'MadeCall', N'Chamada realizada integração Telefonia'),
(15, N'LostCall', N'Chamada perdida integração Telefonia'),
(16, N'DropoutCall', N'Chamada que obteve desistência na fila de espera integração Telefonia'),
(17, N'Acesso remoto', N'Acesso remoto (módulo descontinuado pela Movidesk em 12/2022)'),
(18, N'WhatsApp', N'WhatsApp'),
(19, N'MovideskIntegration', N'Integração Movidesk'),
(20, N'ZenviaChat', N'Integração Zenvia Chat'),
(21, N'NotAnsweredCall', N'Chamada não atendida integração Telefonia'),
(22, N'FacebookMessenger', N'FacebookMessenger'),
(23, N'WhatsApp Business Movidesk', N'WhatsApp Business Movidesk'),
(24, N'Altu', N'Altu'),
(25, N'WhatsApp Ativo', N'WhatsApp Ativo');


if not exists (select 1 from [movidesk].MovideskTicketsTypes)
insert into [movidesk].MovideskTicketsTypes (id, [description])
values
(1, N'Interno'),
(2, N'Público');
