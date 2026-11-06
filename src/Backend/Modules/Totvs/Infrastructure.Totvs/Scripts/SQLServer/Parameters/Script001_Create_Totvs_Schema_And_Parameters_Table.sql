/****** Schema do módulo (src/Backend/Modules/Totvs/.docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.0) ******/

if not exists (select 1 from sys.schemas where name = 'totvs')
    exec('create schema totvs');

/****** Parâmetros: uma linha, a holding Newbloomers; as lojas vêm de totvs.TotvsBranches (5.1) ******/

create table [totvs].TotvsParameters (
	integrated_or_updated_at datetime,
	id int not null identity primary key,
	[doc_company] nvarchar(14) not null,   --CNPJ da holding (chave em todas as tabelas do módulo)
	client_id nvarchar(100) not null,
	client_secret nvarchar(200) not null,
	username nvarchar(100) not null,
	[password] nvarchar(200) not null,
	base_url nvarchar(200) null,  --endereço da API da empresa (aba Parâmetros do WebApp); nulo usa o Totvs:BaseUrl do appsettings
	homolog_url nvarchar(200) null, --endereço da API de homologação da empresa (aba Parâmetros do WebApp), usado quando use_homolog_url = 1
	use_homolog_url bit not null default 0, --1: os jobs usam a homolog_url e as colunas hml_ (aba Parâmetros do WebApp); 0: a base_url e as de produção
	hml_client_id nvarchar(100) null, --client_id de homologação
	hml_client_secret nvarchar(200) null, --client_secret de homologação
	hml_username nvarchar(100) null, --username de homologação
	hml_password nvarchar(200) null --password de homologação
);
