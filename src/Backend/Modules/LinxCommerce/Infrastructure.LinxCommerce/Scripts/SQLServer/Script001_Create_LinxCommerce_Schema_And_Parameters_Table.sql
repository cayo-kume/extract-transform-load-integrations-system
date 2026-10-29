/****** Schema do módulo (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.0) ******/
--o schema legado linx_commerce (lido pelo Oto e pela Stone) não é tocado

if not exists (select 1 from sys.schemas where name = 'linxcommerce')
    exec('create schema linxcommerce');

/****** Parâmetros: uma linha por cliente, com o endereço do ambiente dele no Linx Commerce (5.1) ******/

create table [linxcommerce].LinxCommerceParameters (
	integrated_or_updated_at datetime,
	id int not null identity primary key,
	[doc_company] nvarchar(14) not null,   --CNPJ do cliente (chave em todas as tabelas do módulo)
	base_url nvarchar(200) not null,       --endereço do ambiente do cliente (https://<loja>.layer.core.dcg.com.br)
	user_name nvarchar(100) not null,      --usuário da API (primeira parte do Basic)
	api_key nvarchar(200) not null         --chave da API (segunda parte do Basic)
);
