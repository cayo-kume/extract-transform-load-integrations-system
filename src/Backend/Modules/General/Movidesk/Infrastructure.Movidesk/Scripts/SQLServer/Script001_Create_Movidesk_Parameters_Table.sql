/****** Create Schemas ******/
--as tabelas finais da Movidesk ficam no schema movidesk; o untreated (bulk insert antes da merge) também é criado
--pelos scripts do Shared, mas a conferência aqui deixa os scripts da Movidesk independentes da ordem de execução
if not exists (select 1 from sys.schemas where name = 'movidesk')
    exec('create schema movidesk');

if not exists (select 1 from sys.schemas where name = 'untreated')
    exec('create schema untreated');
GO

create table [movidesk].MovideskParameters (
	integrated_or_updated_at	datetime,
	id							int not null identity primary key,
	[doc_company]				nvarchar(14) not null,
	token						uniqueidentifier
);
