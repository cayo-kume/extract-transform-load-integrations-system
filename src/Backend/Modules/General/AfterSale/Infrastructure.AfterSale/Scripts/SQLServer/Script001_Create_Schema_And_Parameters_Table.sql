/****** Create Schemas ******/

if not exists (select 1 from sys.schemas where name = 'general')
    exec('create schema general');

create table [general].AfterSaleParameters (
	integrated_or_updated_at datetime,
	id int not null identity primary key, 
	[doc_company] nvarchar(14) not null,
	token uniqueidentifier
);

create table [untreated].AfterSaleErrors (
	integrated_or_updated_at datetime,
	id_job bigint, 
	execution uniqueidentifier, 
	doc_company nvarchar(14), 
	reverse_id int, 
	error nvarchar(50),
	error_code nvarchar(50),
	[message] nvarchar(50),
	message_translated nvarchar(50),
);

create table [general].AfterSaleErrors (
	integrated_or_updated_at datetime,
	id_job bigint, 
	execution uniqueidentifier not null primary key, 
	doc_company nvarchar(14), 
	reverse_id int, 
	error nvarchar(50),
	error_code nvarchar(50),
	[message] nvarchar(50),
	message_translated nvarchar(50),
);