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
	token						uniqueidentifier,
	--marca d'água do GetTickets (UTC, como o lastUpdate da API): o início da listagem da última execução completa da empresa,
	--menos 1 h de folga. Só avança quando todos os tickets alterados foram gravados; nula, o GetTickets parte do maior
	--lastUpdate gravado (.docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md, 2.1)
	tickets_synced_until		datetime null,
	base_url					nvarchar(200) null,       --endereço da API da empresa (aba Parâmetros do WebApp); nulo usa o Movidesk:BaseUrl do appsettings
	homolog_url					nvarchar(200) null, --endereço da API de homologação da empresa (aba Parâmetros do WebApp), usado quando use_homolog_url = 1
	use_homolog_url bit not null default 0, --1: os jobs usam a homolog_url e as colunas hml_ (aba Parâmetros do WebApp); 0: a base_url e as de produção
	hml_token uniqueidentifier null --token de homologação
);
