create table general.MovideskArticlesStatus (
	id				int not null,
	[description]	varchar(128),
	constraint pk_ArticlesServices primary key (id)
);

create table general.MovideskArticles (
	id				int not null,
	articleStatus	int,
	contentHtml		varchar(max),
	contentText		varchar(max),
	createdDate		datetime,
	updatedDate		datetime,
	menuId			int,
	menuName		varchar(64),
	revisionId		int,
	slug			varchar(256),
	summary			varchar(max),
	title			varchar(512),
	readingTime		time,
	constraint pk_Articles primary key (id)
);

create table general.MovideskArticlesAttachments (
	articleId	int not null,
	id			int not null,
	[name]		varchar(255),
	[hash]		uniqueidentifier,
	constraint pk_ArticlesAttachments primary key (articleId, id)
);

create table general.MovideskArticlesCategories (
	articleId	int not null,
	id			int not null,
	[name]		varchar(128)
	constraint pk_ArticlesCategories primary key (articleId, id)
);

create table general.MovideskArticlesRelateds (
	articleId	int not null,
	id			int not null,
	title		varchar(512),
	path		varchar(max)
	constraint pk_ArticlesRelateds primary key (articleId, id)
);

create table general.MovideskArticlesServices (
	articleId	int not null,
	id			int not null,
	[name]		varchar(128)
	constraint pk_ArticlesServices primary key (articleId, id)
);

create table general.MovideskArticlesTags (
	articleId	int not null,
	id			int not null, --gerar com laço foreach no c-sharp
	tag			varchar(128)
	constraint pk_ArticlesTags primary key (articleId, id)
);

alter table general.MovideskArticlesAttachments add constraint fk_ArticlesAttachments foreign key (articleId) references general.MovideskArticles (id);
alter table general.MovideskArticlesCategories add constraint fk_ArticlesCategories foreign key (articleId) references general.MovideskArticles (id);
alter table general.MovideskArticlesRelateds add constraint fk_ArticlesRelateds foreign key (articleId) references general.MovideskArticles (id);
alter table general.MovideskArticlesServices add constraint fk_ArticlesServices foreign key (articleId) references general.MovideskArticles (id);
alter table general.MovideskArticlesTags add constraint fk_ArticlesTags foreign key (articleId) references general.MovideskArticles (id);
alter table general.MovideskArticles add constraint fk_ArticlesStatus foreign key (articleStatus) references general.MovideskArticlesStatus (id);

insert into general.MovideskArticlesStatus (id, [description]) values 
(1, 'Publicado'),
(2, 'Suspenso');