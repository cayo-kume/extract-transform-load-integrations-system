create table general.MovideskTimeAgreements (
	[name]							varchar(128),
	id								int not null,
	emailSubject					varchar(1024),
	emailDescription				varchar(1024),
	emailAccount					varchar(1024),
	isActive						bit,
	differentiateHoursFranchise		bit,
	differentiateHoursConsumption	bit,
	accumulateUnusedHours			bit,
	renewalDay						int,
	contractedHours					int,
	consumptionDeadline				int,
	emailSendDay					int,
	baseAmount						decimal(18, 2),
	createdOn						datetime,
	disabledDate					datetime,
	constraint pk_TimeAgreement primary key (id)
);

create table general.MovideskTimeAgreementsTypeActivities (
	timeAgreementId		int not null,
	id					int not null,
	workingTimeType		varchar(128),
	activity			varchar(128),
	franchise			int,
	[value]				decimal(18, 2),
	valueExceededHour	decimal(18, 2),
	shootdownContract	bit,
	allowHoursExcedent	bit,
	activityId			int,
	constraint pk_TypeActivities primary key (timeAgreementId, id)
);

create table general.MovideskTimeAgreementsTimeTypeConsumptions (
	timeAgreementId			int not null,
	id						int not null,
	workingTimeType			varchar(128),
	[value]					decimal(18, 2),
	workingTimeAgreementId	int,
	constraint pk_TimeTypeConsumptions primary key (timeAgreementId, id)
);

create table general.MovideskTimeAgreementsEmailDestinations (
	timeAgreementId			int not null,
	id						int not null, --gerar com incremental no c-sharp
	[type]					int,
	email					varchar(128),
	constraint pk_EmailDestinations primary key (timeAgreementId, id)
);

create table general.MovideskTimeAgreementsClients (
	timeAgreementId		int not null,
	clientId			varchar(64) not null,
	organizationId		varchar(64) not null,
	constraint pk_Clients primary key (timeAgreementId, clientId, organizationId)
);

create table general.MovideskTimeAgreementConsumptionsDiscountsTypes (
	id	int not null,
	[description]	varchar(64),
	constraint pk_TimeAgreementConsumptionsDiscountsTypes primary key (id)
);

create table general.MovideskTimeAgreementConsumptions (
	id								varchar(64) not null,
	[name]							varchar(128),
	startPeriod						datetime,
	endPeriod						datetime,
	baseAmount						decimal(18,2),
	exceededHourAmount				decimal(18,2),
	renewalDay						int,
	contractedHours					int,
	differentiateHoursFranchise		bit,
	differentiateHoursConsumption	bit,
	[period]						datetime,
	consumedHours					decimal(18,2),
	discount						decimal(18,2),
	discountType					int,
	constraint pk_TimeAgreementConsumptions primary key (id)
);

create table general.MovideskTimeAgreementConsumptionsTimeAppointments (
	timeAgreementConsumptionId	varchar(64) not null,
	id							int not null,
	activity					varchar(128),
	[date]						datetime,
	periodStart					time,
	periodEnd					time,
	workTime					time,
	workTypeName				varchar(64),
	createdBy					varchar(64),
	createdByTeam				varchar(64),
	constraint pk_TimeAppointments primary key (timeAgreementConsumptionId, id)
);

create table general.MovideskTimeAgreementConsumptionsExpenses (
	timeAgreementConsumptionId	varchar(64) not null,
	id							int not null,
	[type]						varchar(128),
	serviceReport				varchar(128),
	createdBy					varchar(64),
	createdByTeam				varchar(64),
	[date]						datetime,
	quanttity					int,
	[value]						decimal(18,2),
	constraint pk_Expenses primary key (timeAgreementConsumptionId, id)
);

create table general.MovideskTimeAgreementConsumptionsClients (
	timeAgreementConsumptionId	varchar(64) not null,
	id							varchar(64) not null,
	organizationId				varchar(64) not null,
	constraint pk_TimeAgreementConsumptionsClients primary key (timeAgreementConsumptionId, id, organizationId)
);

alter table general.MovideskTimeAgreementsTypeActivities add constraint fk_TypeActivities foreign key (timeAgreementId) references general.MovideskTimeAgreements (id);
alter table general.MovideskTimeAgreementsTimeTypeConsumptions add constraint fk_TimeTypeConsumptions foreign key (timeAgreementId) references general.MovideskTimeAgreements (id);
alter table general.MovideskTimeAgreementsEmailDestinations add constraint fk_EmailDestinations foreign key (timeAgreementId) references general.MovideskTimeAgreements (id);
alter table general.MovideskTimeAgreementsClients add constraint fk_TimeAgreements foreign key (timeAgreementId) references general.MovideskTimeAgreements (id);
alter table general.MovideskTimeAgreementsClients add constraint fk_TimeAgreementsClients foreign key (clientId) references general.MovideskPeople (id);
alter table general.MovideskTimeAgreementsClients add constraint fk_TimeAgreementsOrganizations foreign key (organizationId) references general.MovideskPeople (id);
alter table general.MovideskTimeAgreementConsumptions add constraint fk_TimeAgreementConsumptionsDiscountsTypes foreign key (discountType) references general.MovideskTimeAgreementConsumptionsDiscountsTypes (id);
alter table general.MovideskTimeAgreementConsumptionsTimeAppointments add constraint fk_TimeAgreementConsumptionsTimeAppointments foreign key (timeAgreementConsumptionId) references general.MovideskTimeAgreementConsumptions (id);
alter table general.MovideskTimeAgreementConsumptionsTimeAppointments add constraint fk_TimeAppointmentsPeople foreign key (createdBy) references general.MovideskPeople (id);
--alter table general.MovideskTimeAgreementConsumptionsTimeAppointments add constraint fk_TimeAppointmentsTeam foreign key (createdByTeam) references general.MovideskTeams (id);
alter table general.MovideskTimeAgreementConsumptionsExpenses add constraint fk_TimeAgreementConsumptionsExpenses foreign key (timeAgreementConsumptionId) references general.MovideskTimeAgreementConsumptions (id);
alter table general.MovideskTimeAgreementConsumptionsExpenses add constraint fk_ExpensesPeople foreign key (createdBy) references general.MovideskPeople (id);
--alter table general.MovideskTimeAgreementConsumptionsExpenses add constraint fk_ExpensesTeam foreign key (createdByTeam) references general.MovideskTeams (id);
alter table general.MovideskTimeAgreementConsumptionsClients add constraint fk_ConsumptionsClients foreign key (timeAgreementConsumptionId) references geenral.MovideskTimeAgreementConsumptions (id);
alter table general.MovideskTimeAgreementConsumptionsClients add constraint fk_ConsumptionsPeople foreign key (id) references geenral.MovideskPeople (id);
alter table general.MovideskTimeAgreementConsumptionsClients add constraint fk_ConsumptionsOrganizations foreign key (organizationId) references geenral.MovideskPeople (id);

insert into general.MovideskTimeAgreementConsumptionsDiscountsTypes (id, [description]) values
(0, 'Percent'),
(1, 'Curency');