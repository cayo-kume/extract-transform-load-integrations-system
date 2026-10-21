/****** Tickets: tabelas untreated e general ******/

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

create table [general].MovideskTickets (
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

create table [untreated].MovideskTicketsTags (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	tag                      nvarchar(128)
);

create table [general].MovideskTicketsTags (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ticketId                 bigint not null,
	tag                      nvarchar(128) not null,
constraint [PK_MovideskTicketsTags] primary key (
	doc_company asc,
	ticketId asc,
	tag asc
));

create table [untreated].MovideskTicketsClients (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	clientId                 nvarchar(64),
	organizationId           nvarchar(64),
	isDeleted                bit
);

create table [general].MovideskTicketsClients (
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

create table [untreated].MovideskTicketsRelationships (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	relatedTicketId          bigint,
	relationType             nvarchar(10), --'Parent' | 'Child'
	[subject]                nvarchar(350),
	isDeleted                bit
);

create table [general].MovideskTicketsRelationships (
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

create table [general].MovideskTicketsOwnerHistories (
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

create table [general].MovideskTicketsStatusHistories (
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

create table [untreated].MovideskTicketsCustomFieldValues (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	customFieldId            int,
	customFieldRuleId        int,
	line                     int,
	[value]                  nvarchar(max)
);

create table [general].MovideskTicketsCustomFieldValues (
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

create table [general].MovideskTicketsCustomFieldValuesItems (
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

create table [general].MovideskTicketsAssets (
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

create table [general].MovideskTicketsActions (
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

create table [untreated].MovideskTicketsActionsTags (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ticketId                 bigint,
	actionId                 int,
	tag                      nvarchar(128)
);

create table [general].MovideskTicketsActionsTags (
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

create table [general].MovideskTicketsActionsTimeAppointments (
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

create table [general].MovideskTicketsActionsExpenses (
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

create table [general].MovideskTicketsActionsAttachments (
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

alter table [general].MovideskTicketsTags add constraint [FK_MovideskTicketsTags] foreign key (doc_company, ticketId) references [general].MovideskTickets (doc_company, id);

alter table [general].MovideskTicketsClients add constraint [FK_MovideskTicketsClients] foreign key (doc_company, ticketId) references [general].MovideskTickets (doc_company, id);

alter table [general].MovideskTicketsRelationships add constraint [FK_MovideskTicketsRelationships] foreign key (doc_company, ticketId) references [general].MovideskTickets (doc_company, id);

alter table [general].MovideskTicketsOwnerHistories add constraint [FK_MovideskTicketsOwnerHistories] foreign key (doc_company, ticketId) references [general].MovideskTickets (doc_company, id);

alter table [general].MovideskTicketsStatusHistories add constraint [FK_MovideskTicketsStatusHistories] foreign key (doc_company, ticketId) references [general].MovideskTickets (doc_company, id);

alter table [general].MovideskTicketsCustomFieldValues add constraint [FK_MovideskTicketsCustomFieldValues] foreign key (doc_company, ticketId) references [general].MovideskTickets (doc_company, id);

alter table [general].MovideskTicketsAssets add constraint [FK_MovideskTicketsAssets] foreign key (doc_company, ticketId) references [general].MovideskTickets (doc_company, id);

alter table [general].MovideskTicketsActions add constraint [FK_MovideskTicketsActions] foreign key (doc_company, ticketId) references [general].MovideskTickets (doc_company, id);


/****** Chaves estrangeiras: netas -> nível do meio (on delete cascade, ver docs/MOVIDESK-MODELAGEM-TICKETS.md 4.8) ******/

alter table [general].MovideskTicketsCustomFieldValuesItems add constraint [FK_MovideskTicketsCustomFieldValuesItems] foreign key (doc_company, ticketId, customFieldId, customFieldRuleId, line) references [general].MovideskTicketsCustomFieldValues (doc_company, ticketId, customFieldId, customFieldRuleId, line) on delete cascade;

alter table [general].MovideskTicketsActionsTags add constraint [FK_MovideskTicketsActionsTags] foreign key (doc_company, ticketId, actionId) references [general].MovideskTicketsActions (doc_company, ticketId, id) on delete cascade;

alter table [general].MovideskTicketsActionsTimeAppointments add constraint [FK_MovideskTicketsActionsTimeAppointments] foreign key (doc_company, ticketId, actionId) references [general].MovideskTicketsActions (doc_company, ticketId, id) on delete cascade;

alter table [general].MovideskTicketsActionsExpenses add constraint [FK_MovideskTicketsActionsExpenses] foreign key (doc_company, ticketId, actionId) references [general].MovideskTicketsActions (doc_company, ticketId, id) on delete cascade;

alter table [general].MovideskTicketsActionsAttachments add constraint [FK_MovideskTicketsActionsAttachments] foreign key (doc_company, ticketId, actionId) references [general].MovideskTicketsActions (doc_company, ticketId, id) on delete cascade;


/****** Tabelas de valores (iguais para qualquer conta Movidesk, sem doc_company e sem merge) ******/
create table [general].MovideskTicketsOrigins (
	id				int not null primary key,
	[name]			nvarchar(60) not null,
	[description]	nvarchar(128)
);

create table [general].MovideskTicketsTypes (
	id				int not null primary key,
	[description]	nvarchar(60) not null
);

insert into [general].MovideskTicketsOrigins (id, [name], [description])
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


insert into [general].MovideskTicketsTypes (id, [description])
values
(1, N'Interno'),
(2, N'Público');
