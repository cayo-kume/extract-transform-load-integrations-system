create table general.MovideskSurveyResponsesValues (
	movideskSurveyTypesId	int not null,
	id						int not null identity (1,1),
	[description]			varchar(128),
	constraint pk_SurveyResponsesValues primary key (movideskSurveyTypesId, id)
);

create table general.MovideskSurveyTypes (
	id				int not null,
	[description]	varchar(128),
	constraint pk_SurveyTypes primary key (id)
);

create table general.MovideskSurveyQuestions (
	id			varchar(64) not null,
	isActive	bit,
	[type]		int,
	constraint pk_Questions primary key (id)
);

create table general.MovideskSurveyQuestionsLanguages (
	surveyQuestionsId	varchar(64) not null,
	id					int not null, --gerar com laço foreach no c-sharp
	cultureId			varchar(128),
	[description]		varchar(1024),
	constraint pk_SurveyQuestionsLanguages primary key (surveyQuestionsId, id)
);

create table general.MovideskSurveyResponses (
	id				varchar(64) not null,
	questionId		varchar(64) not null,
	[type]			int,
	clientId		varchar(64),
	ticketId		int,
	responseDate	datetime,
	commentary		varchar(max),
	[value]			int,

);

alter table general.MovideskSurveyQuestionsLanguages add constraint fk_SurveyQuestionsLanguages foreign key (surveyQuestionsId) references general.MovideskSurveyQuestions (id);
alter table general.MovideskSurveyResponses add constraint fk_SurveyResponses foreign key (questionId) references general.MovideskSurveyQuestions (id);
alter table general.MovideskSurveyQuestions add constraint fk_SurveyQuestionsTypes foreign key ([type]) references general.MovideskSurveyTypes (id);
alter table general.MovideskSurveyResponses add constraint fk_SurveyResponsesTypes foreign key ([type])	references general.MovideskSurveyTypes (id);
alter table general.MovideskSurveyResponsesValues add constraint fk_ResponsesValues foreign key (movideskSurveyTypesId) references general.MovideskSurveyTypes (id);

insert into general.MovideskSurveyResponsesValues (movideskSurveyTypesId, [description]) values
(1, '(1 = Satisfeito, 2 = Insatisfeito)'),
(2, '(1 = Muito insatisfeito, 2 = Insatisfeito, 3 = Neutro, 4 = Satisfeito, 5 = Muito satisfeito)'),
(3, '(0 a 10)'),
(4, '(1 = Sim, 2 = Não)');

insert into general.MovideskSurveyTypes (id, [description]) values
(1, 'Satisfeito ou Insatisfeito'),
(2, 'Faces Sorridentes'),
(3, 'NPS (Net Promoter Score)'),
(4, 'Sim ou Nao');