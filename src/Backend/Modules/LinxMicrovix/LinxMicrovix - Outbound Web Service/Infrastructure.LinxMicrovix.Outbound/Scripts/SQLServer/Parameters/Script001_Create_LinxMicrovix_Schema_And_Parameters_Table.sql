/****** Schema do módulo (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.0) ******/

if not exists (select 1 from sys.schemas where name = 'linxmicrovix')
    exec('create schema linxmicrovix');

/****** Parâmetros: uma linha por cliente, com as credenciais de cada API; as lojas vêm de LinxGrupoLojas e B2CConsultaEmpresas (5.1) ******/

create table [linxmicrovix].LinxMicrovixParameters (
	integrated_or_updated_at	datetime,
	id							int not null identity primary key,
	[doc_company]				nvarchar(14) not null,   --CNPJ do cliente (chave em todas as tabelas do módulo)
	[grupo]						nvarchar(100),           --nome do grupo na ativação (parâmetro grupo do LinxGrupoLojas, opcional)
	erp_chave					uniqueidentifier,        --chave do WS de saída padrão (nula se o cliente não tiver)
	erp_user_name				nvarchar(50),            --linx_export pela v274
	erp_password				nvarchar(50),            --linx_export pela v274
	b2c_chave					uniqueidentifier,        --chave do WS B2C (nula se o cliente não tiver)
	b2c_user_name				nvarchar(50),            --linx_b2c pela V64
	b2c_password				nvarchar(50),            --linx_b2c pela V64
	base_url					nvarchar(200) null,      --endereço da API da empresa (aba Parâmetros do WebApp); nulo usa o LinxMicrovix:BaseUrl do appsettings
	homolog_url					nvarchar(200) null, --endereço da API de homologação da empresa (aba Parâmetros do WebApp), usado quando use_homolog_url = 1
	use_homolog_url bit not null default 0, --1: os jobs usam a homolog_url e as colunas hml_ (aba Parâmetros do WebApp); 0: a base_url e as de produção
	hml_erp_chave uniqueidentifier null, --erp_chave de homologação
	hml_erp_user_name nvarchar(50) null, --erp_user_name de homologação
	hml_erp_password nvarchar(50) null, --erp_password de homologação
	hml_b2c_chave uniqueidentifier null, --b2c_chave de homologação
	hml_b2c_user_name nvarchar(50) null, --b2c_user_name de homologação
	hml_b2c_password nvarchar(50) null --b2c_password de homologação
);
