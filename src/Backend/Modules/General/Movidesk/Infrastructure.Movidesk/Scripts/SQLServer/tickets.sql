create table general.MovideskTicketsTeams (
    ticketId	varchar(10) not null,
    id			int			not null,
    [name]		varchar(50) not null
);

create table general.MovideskTickets (
	id							varchar(10) not null,
	protocol					varchar(30),
	[type]						int,
	[subject]					varchar(350),
	category					varchar(128),
	urgency						varchar(128),
	[status]					varchar(128),
	baseStatus					varchar(128),
	justification				varchar(128),
	origin						int,
	createdDate					datetime,
	originEmailAccount			varchar(128),
	[owner]						varchar(64),
	ownerTeam					varchar(128),
	createdBy					varchar(64),
	serviceFirstLevelId			int,
	serviceFirstLevel			varchar(1024),
	serviceSecondLevel			varchar(1024),
	serviceThirdLevel			varchar(1024),
	contactForm					varchar(128),
	cc							varchar(1024),
	resolvedIn					datetime,
	reopenedIn					datetime,
	canceledIn					datetime,
	closedIn					datetime,
	lastActionDate				datetime,
	actionCount					int,
	lastUpdate					datetime,
	lifetimeWorkingTime			int,
	stoppedTime					int,
	stoppedTimeWorkingTime		int,
	resolvedInFirstCall			bit,
	chatWidget					varchar(128),
	chatGroup					varchar(128),
	chatTalkTime				int,
	chatWaitingTime				int,
	[sequence]					int,
	slaAgreement				varchar(128),
	slaAgreementRule			varchar(128),
	slaSolutionTime				int,
	slaResponseTime				int,
	slaSolutionChangedByUser	bit,
	isDeleted					bit,
	slaSolutionChangedBy		varchar(64),
	slaSolutionDate				datetime,
	slaSolutionDateIsPaused		bit,
	slaResponseDate				datetime,
	slaRealResponseDate			datetime,
	jiraIssueKey				varchar(64),
	redmineIssueId				int,
	constraint pk_Tickets primary key (id)
);

create table general.MovideskTicketsTags (
	ticketId	varchar(10) not null,
	id			int not null, -- gerar com laço foreach no c-sharp
	[tag]		varchar(128),
	constraint pk_TicketsTags primary key (ticketId, id)
);

create table general.MovideskTicketsServicesFull (
	ticketId	varchar(10) not null,
	id			int not null, -- gerar com laço foreach no c-sharp
	[service]	varchar(128),
	constraint pk_TicketsServicesFull primary key (ticketId, id)
);

create table general.MovideskTicketsActions (
	ticketId		varchar(10) not null,
	id				int not null,
	[type]			int,
	origin			int,
	[description]	varchar(max),
	htmlDescription	varchar(max),
	[status]		varchar(128),
	justification	varchar(128),
	createdDate		datetime,
	createdBy		varchar(64),
	isDeleted		bit,
	constraint pk_TicketsActions primary key (ticketId, id)
);

create table general.MovideskTicketsActionsTags (
	ticketId	varchar(10) not null,
	actionId	int not null,
	id			int not null, -- gerar com laço foreach no c-sharp
	tag			varchar(128),
	constraint pk_TicketsActionsTags primary key (ticketId, actionId, id)
);

create table general.MovideskTicketsActionsTimeAppointments (
	ticketId		varchar(10) not null,
	actionId		int not null,
	id				int not null,
	activity		varchar(128),
	[date]			datetime,
	periodStart		time,
	periodEnd		time,
	workTime		time,
	accountedTime	decimal(10,10),
	workTypeName	varchar(64),
	createdBy		varchar(64),
	createdByTeam	varchar(128),
	constraint pk_TicketsActionsTimeAppointments primary key (ticketId, actionId, id)
);

create table general.MovideskTicketsActionsExpenses (
	ticketId		varchar(10) not null,
	actionId		int not null,
	id				int not null,
	[type]			varchar(128),
	serviceReport	varchar(128),
	createdBy		varchar(64),
	createdByTeam	varchar(128),
	[date]			datetime,
	quantity		int,
	[value]			decimal(18,2),
	constraint pk_TicketsActionsExpenses primary key (ticketId, actionId, id)
);

create table general.MovideskTicketsActionsAttachments (
	ticketId	varchar(10) not null,
	actionId	int not null,
	id			int not null, --gerar com incremental no csharp
	[fileName]	varchar(255),
	[path]		varchar(255),
	createdBy	varchar(64),
	createdDate	datetime,
	constraint pk_TicketsActionsAttachments primary key (ticketId, actionId, id)
);

create table general.MovideskTicketsFamilies (
	ticketId	varchar(10) not null,
	id			int not null,
	[subject]	varchar(128),
	isDeleted	bit,
	isParent	bit, --gerar com condicional no csharp
	isChildren	bit, --gerar com condicional no csharp
	constraint pk_TicketsFamilies primary key (ticketId, id)
);

create table general.MovideskTicketsOwnerHistories (
	ticketId					varchar(10) not null,
	id							int not null, --gerar com incremental no csharp
	ownerTeam					varchar(128),
	[owner]						varchar(64),
	permanencyTimeFullTime		decimal(10,2),
	permanencyTimeWorkingTime	decimal(10,2),
	changedBy					varchar(64),
	changedDate					datetime,
	constraint pk_TicketsOwnerHistories primary key (ticketId, id)
);

create table general.MovideskTicketsStatusHistories (
	ticketId					varchar(10) not null,
	id							int not null, --gerar com incremental no csharp
	[status]					varchar(128),
	justification				varchar(128),
	permanencyTimeFullTime		decimal(10,2),
	permanencyTimeWorkingTime	decimal(10,2),
	changedBy					varchar(64),
	changedDate					datetime,
	constraint pk_TicketsStatusHistories primary key (ticketId, id)
);

create table general.MovideskTicketsSatisfactionSurveyResponses (
	ticketId									varchar(10) not null,
	id											int not null,
	responseBy									varchar(64),
	responseDate								datetime,
	satisfactionSurveyModel						int,
	satisfactionSurveyNetPromoterScoreResponse	int,
	satisfactionSurveyPositiveNegativeResponse	int,
	satisfactionSurveySmileyFacesResponse		int,
	comments									varchar(max),
	constraint pk_TicketsSatisfactionSurveyResponses primary key (ticketId, id)
);

create table general.MovideskTicketsCustomFieldValues (
	ticketId			varchar(10) not null,
	customFieldId		int not null,
	customFieldRuleId	int,
	line				int,
	[value]				varchar(max), 
	constraint pk_TicketsCustomFieldValues primary key (ticketId, customFieldId)
);

create table general.MovideskTicketsCustomFieldValuesItens (
	ticketId		varchar(10) not null,
	customFieldId	int not null,
	id				int not null, --gerar com incremental no csharp
	personId		int,
	clientId		int,
	team			varchar(128),
	customFieldItem	varchar(256),
	constraint pk_TicketsCustomFieldValuesItens primary key (ticketId, customFieldId, id)
);

create table general.MovideskTicketsAssets (
	ticketId			varchar(10) not null,
	id					varchar(64) not null,
	[name]				varchar(128),
	[label]				varchar(128),
	serialNumber		varchar(128),
	categoryFirstLevel	varchar(128),
	categorySecondLevel	varchar(128),
	categoryThirdLevel	varchar(128),
	isDeleted			bit,
	constraint pk_TicketsAssets primary key (ticketId, id)
);

create table general.MovideskTicketsAssetsCategoriesFull (
	ticketId	varchar(10) not null,
	assetId		int not null,
	id			int not null, --gerar com incremental no c-sharp
	category	varchar(128),
	constraint pk_TicketsAssetsCategoriesFull primary key (ticketId, assetId, id)
);

create table general.MovideskTicketsOrigins (
	id				int not null primary key,
	[name]			varchar(64),
	[description]	varchar(250)
);

create table general.MovideskTicketsActionsTypes (
	id				int not null primary key,
	[description]	varchar(250)
);

create table general.MovideskTicketsClients (
	ticketId			varchar(10) not null,
	clientId			varchar(64) not null,
	organizationId		varchar(64) not null,
	constraint pk_TicketsClients primary key (ticketId, clientId, organizationId)
);

alter table general.MovideskTickets add constraint fk_TicketsOrigins foreign key (origin) references general.MovideskTicketsOrigins (id);
alter table general.MovideskTicketsTags add constraint fk_TicketsTags foreign key (ticketId) references general.MovideskTickets (id);
alter table general.MovideskTicketsServicesFull add constraint fk_TicketsServicesFull foreign key (ticketId) references general.MovideskTickets (id);
alter table general.MovideskTicketsClients add constraint fk_TicketsClients foreign key (ticketId) references general.MovideskTickets (id);
alter table general.MovideskTicketsTeams add constraint fk_TicketsTeams foreign key (ticketId) references general.MovideskTickets (id);
--alter table general.MovideskTicketsTeams add constraint fk_TicketTeamsTeams foreign key (teamId) references general.MovideskTeams (id);
alter table general.MovideskTicketsAssets add constraint fk_TicketsAssets foreign key (ticketId) references general.MovideskTickets (id);
alter table general.MovideskTicketsAssetsCategoriesFull add constraint fk_TicketsAssetsCategoriesFull foreign key (ticketId, assetId) references general.MovideskTicketsAssets (ticketId, id);
alter table general.MovideskTicketsCustomFieldValuesItens add constraint fk_TicketsCustomFieldValuesItens foreign key (ticketId, customFieldId) references general.MovideskTicketsCustomFieldValues (ticketId, customFieldId);
alter table general.MovideskTicketsCustomFieldValues add constraint fk_TicketsCustomFieldValues foreign key (ticketId) references general.MovideskTickets (id);
alter table general.MovideskTicketsSatisfactionSurveyResponses add constraint fk_TicketsSatisfactionSurveyResponses foreign key (ticketId) references general.MovideskTickets (id);
alter table general.MovideskTicketsStatusHistories add constraint fk_TicketsStatusHistories foreign key (ticketId) references general.MovideskTickets (id);
alter table general.MovideskTicketsOwnerHistories add constraint fk_TicketsOwnerHistories foreign key (ticketId) references general.MovideskTickets (id);
alter table general.MovideskTicketsFamilies add constraint fk_TicketsFamilies foreign key (ticketId) references general.MovideskTickets (id);
alter table general.MovideskTicketsActionsAttachments add constraint fk_TicketsActionsAttachments foreign key (ticketId, actionId) references general.MovideskTicketsActions (ticketId, id);
alter table general.MovideskTicketsActionsExpenses add constraint fk_TicketsActionsExpenses foreign key (ticketId, actionId) references general.MovideskTicketsActions (ticketId, id);
alter table general.MovideskTicketsActionsTimeAppointments add constraint fk_TicketsActionsTimeAppointments foreign key (ticketId, actionId) references general.MovideskTicketsActions (ticketId, id);
alter table general.MovideskTicketsActionsTags add constraint fk_TicketsActionsTags foreign key (ticketId, actionId) references general.MovideskTicketsActions (ticketId, id);
alter table general.MovideskTicketsActions add constraint fk_TicketsActions foreign key (ticketId) references general.MovideskTickets (id);
alter table general.MovideskTicketsActions add constraint fk_TicketsActionsTypes foreign key ([type]) references general.MovideskTicketsActionsTypes (id);
alter table general.MovideskTicketsClients add constraint fk_TicketsPeople foreign key (clientId) references general.MovideskPeople (id);
alter table general.MovideskTicketsClients add constraint fk_TicketsOrganizations foreign key (organizationId) references general.MovideskPeople (id);

insert into general.MovideskTicketsOrigins (id, [name], [description]) values
(0, 'First Action', 'First Action'),
(1, 'Via web pelo cliente', 'Via web pelo cliente'),
(2, 'Via web pelo agente', 'Via web pelo agente'),
(3, 'Recebido via email', 'Recebido via email'),
(4, 'Gatilho do sistema', 'Gatilho do sistema'),
(5, 'Chat (online)', 'Chat (online)'),
(6, 'Chat (offline)', 'Chat (offline)'),
(7, 'Email enviado pelo sistema', 'Email enviado pelo sistema'),
(8, 'Formulário de contato', 'Formulário de contato'),
(9, 'Via web API', 'Via web API'),
(10, 'Agendamento automático', 'Abertura automática de tickets'),
(11, 'JiraIssue', 'Issue integração Jira'),
(12, 'RedmineIssue', 'Issue integração Redmine'),
(13, 'ReceivedCall', 'Chamada recebida integração Telefonia'),
(14, 'MadeCall', 'Chamada realizada integração Telefonia'),
(15, 'LostCall', 'Chamada perdida integração Telefonia'),
(16, 'DropoutCall', 'Chamada que obteve desistência na fila de espera integração Telefonia'),
(17, 'Acesso remoto', 'Acesso remoto'),
(18, 'WhatsApp', 'WhatsApp'),
(19, 'MovideskIntegration', 'Integração Movidesk'),
(20, 'ZenviaChat', 'Integração Zenvia Chat'),
(21, 'NotAnsweredCall', 'Chamada não atendida integração Telefonia'),
(22, 'FacebookMessenger', 'FacebookMessenger'),
(23, 'WhatsApp Business Movidesk', 'WhatsApp Business Movidesk'),
(24, 'Altu', 'Altu'),
(25, 'WhatsApp Ativo', 'WhatsApp Ativo');

insert into general.MovideskTicketsActionsTypes (id, [description]) values
(1, 'Interna'),
(2, 'Publica');