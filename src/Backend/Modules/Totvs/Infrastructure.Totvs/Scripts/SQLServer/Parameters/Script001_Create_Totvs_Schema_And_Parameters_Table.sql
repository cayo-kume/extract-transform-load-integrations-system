/****** Schema do módulo (src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.0) ******/

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
	[password] nvarchar(200) not null
);
