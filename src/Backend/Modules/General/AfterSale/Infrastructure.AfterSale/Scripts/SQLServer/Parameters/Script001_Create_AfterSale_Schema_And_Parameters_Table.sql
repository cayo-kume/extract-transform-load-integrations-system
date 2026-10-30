/****** Create Schemas ******/
--as tabelas finais do AfterSale ficam no schema aftersale; o untreated (bulk insert antes da merge) também é criado
--pelos scripts do Shared, mas a conferência aqui deixa os scripts do AfterSale independentes da ordem de execução
if not exists (select 1 from sys.schemas where name = 'aftersale')
    exec('create schema aftersale');

if not exists (select 1 from sys.schemas where name = 'untreated')
    exec('create schema untreated');
GO

create table [aftersale].AfterSaleParameters (
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

create table [aftersale].AfterSaleErrors (
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
