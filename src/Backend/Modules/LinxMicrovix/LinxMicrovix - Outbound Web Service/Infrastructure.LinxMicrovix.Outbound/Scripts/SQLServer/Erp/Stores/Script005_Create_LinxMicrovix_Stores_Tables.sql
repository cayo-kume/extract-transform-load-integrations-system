/****** Lojas (LinxMicrovixStoresService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (26/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxGrupoLojas: lojas do grupo; sem timestamp: o grupo é relido inteiro a cada execução. Chave portal + empresa (uma loja vem sem CNPJ, testado)*/

create table [untreated].LinxGrupoLojas (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	portal                   int,
	empresa                  int, --id da empresa no portal
	CNPJ                     nvarchar(14), --vazio em uma das lojas da Newbloomers (testado)
	nome_empresa             nvarchar(50),
	id_empresas_rede         int,
	rede                     nvarchar(100),
	nome_portal              nvarchar(50),
	lojas_proprias           bit,
	classificacao_portal     nvarchar(50),
	emp_mod_linx_promo       bit --fora da v274
);

create table [linxmicrovix].LinxGrupoLojas (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	portal                   int not null,
	empresa                  int not null, --id da empresa no portal
	CNPJ                     nvarchar(14), --vazio em uma das lojas da Newbloomers (testado)
	nome_empresa             nvarchar(50),
	id_empresas_rede         int,
	rede                     nvarchar(100),
	nome_portal              nvarchar(50),
	lojas_proprias           bit,
	classificacao_portal     nvarchar(50),
	emp_mod_linx_promo       bit, --fora da v274
constraint [PK_LinxGrupoLojas] primary key (
	doc_company asc,
	portal asc,
	empresa asc
));

/*LinxLojas: cadastro de cada loja, consultada pelo CNPJ*/

create table [untreated].LinxLojas (
	integrated_or_updated_at   datetime,
	[doc_company]              nvarchar(14),
	cnpj_emp                   nvarchar(14), --loja consultada
	portal                     int,
	empresa                    int,
	nome_emp                   nvarchar(50),
	razao_emp                  nvarchar(200),
	inscricao_emp              nvarchar(20),
	endereco_emp               nvarchar(250),
	num_emp                    int,
	complement_emp             nvarchar(60),
	bairro_emp                 nvarchar(50),
	cep_emp                    nvarchar(9),
	cidade_emp                 nvarchar(50),
	estado_emp                 nvarchar(2),
	fone_emp                   nvarchar(70),
	email_emp                  nvarchar(50),
	cod_ibge_municipio         int,
	data_criacao_emp           datetime,
	data_criacao_portal        datetime,
	sistema_tributacao         nvarchar(1),
	regime_tributario          int,
	area_empresa               decimal(18,4), --área em m²
	[timestamp]                bigint,
	sigla_empresa              nvarchar(10),
	id_classe_fiscal           int,
	centro_distribuicao        bit,
	inscricao_municipal_emp    nvarchar(30), --10 na v274; 30 no código antigo
	cnae_emp                   nvarchar(7),
	cod_cliente_linx           nvarchar(6),
	tabela_preco_preferencial  int,
	subrede                    nvarchar(100),
	produto_disponivel_empresa bit,
	id_grupo                   int
);

create table [linxmicrovix].LinxLojas (
	integrated_or_updated_at   datetime,
	[doc_company]              nvarchar(14) not null,
	cnpj_emp                   nvarchar(14) not null, --loja consultada
	portal                     int,
	empresa                    int,
	nome_emp                   nvarchar(50),
	razao_emp                  nvarchar(200),
	inscricao_emp              nvarchar(20),
	endereco_emp               nvarchar(250),
	num_emp                    int,
	complement_emp             nvarchar(60),
	bairro_emp                 nvarchar(50),
	cep_emp                    nvarchar(9),
	cidade_emp                 nvarchar(50),
	estado_emp                 nvarchar(2),
	fone_emp                   nvarchar(70),
	email_emp                  nvarchar(50),
	cod_ibge_municipio         int,
	data_criacao_emp           datetime,
	data_criacao_portal        datetime,
	sistema_tributacao         nvarchar(1),
	regime_tributario          int,
	area_empresa               decimal(18,4), --área em m²
	[timestamp]                bigint,
	sigla_empresa              nvarchar(10),
	id_classe_fiscal           int,
	centro_distribuicao        bit,
	inscricao_municipal_emp    nvarchar(30), --10 na v274; 30 no código antigo
	cnae_emp                   nvarchar(7),
	cod_cliente_linx           nvarchar(6),
	tabela_preco_preferencial  int,
	subrede                    nvarchar(100),
	produto_disponivel_empresa bit,
	id_grupo                   int,
constraint [PK_LinxLojas] primary key (
	doc_company asc,
	cnpj_emp asc
));

/*LinxLojasParametros: parâmetros do portal para cada loja; a resposta não traz o CNPJ: cnpj_emp é a loja consultada*/

create table [untreated].LinxLojasParametros (
	integrated_or_updated_at      datetime,
	[doc_company]                 nvarchar(14),
	cnpj_emp                      nvarchar(14), --loja consultada
	portal                        int,
	empresa                       int,
	percentual_minimo_antecipacao decimal(19,4), --money na v274
	[timestamp]                   bigint
);

create table [linxmicrovix].LinxLojasParametros (
	integrated_or_updated_at      datetime,
	[doc_company]                 nvarchar(14) not null,
	cnpj_emp                      nvarchar(14) not null, --loja consultada
	portal                        int,
	empresa                       int,
	percentual_minimo_antecipacao decimal(19,4), --money na v274
	[timestamp]                   bigint,
constraint [PK_LinxLojasParametros] primary key (
	doc_company asc,
	cnpj_emp asc
));

/*LinxUsuarios: usuários do portal (método de portal, sem cnpjEmp; 100 por chamada, testado)*/

create table [untreated].LinxUsuarios (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	portal                   int,
	usuario_id               int,
	usuario_login            nvarchar(30),
	usuario_nome             nvarchar(50),
	usuario_email            nvarchar(60),
	usuario_grupo_id         int, --0 indica sem grupo
	grupo_usuarios           nvarchar(1), --S/N
	usuario_supervisor       nvarchar(1), --S/N
	usuario_doc              nvarchar(14),
	vendedor                 int,
	data_criacao             datetime,
	desativado               nvarchar(1), --S/N
	empresas                 nvarchar(max), --empresas com acesso, separadas por ;
	[timestamp]              bigint
);

create table [linxmicrovix].LinxUsuarios (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	portal                   int not null,
	usuario_id               int not null,
	usuario_login            nvarchar(30),
	usuario_nome             nvarchar(50),
	usuario_email            nvarchar(60),
	usuario_grupo_id         int, --0 indica sem grupo
	grupo_usuarios           nvarchar(1), --S/N
	usuario_supervisor       nvarchar(1), --S/N
	usuario_doc              nvarchar(14),
	vendedor                 int,
	data_criacao             datetime,
	desativado               nvarchar(1), --S/N
	empresas                 nvarchar(max), --empresas com acesso, separadas por ;
	[timestamp]              bigint,
constraint [PK_LinxUsuarios] primary key (
	doc_company asc,
	portal asc,
	usuario_id asc
));

/*LinxMetodos: catálogo de métodos da API (o mesmo para qualquer chave); sem timestamp: relido inteiro a cada execução*/

create table [untreated].LinxMetodos (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	metodo                   nvarchar(100),
	descricao                nvarchar(1000),
	parametros               nvarchar(1000) --parâmetros aceitos, separados por vírgula
);

create table [linxmicrovix].LinxMetodos (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	metodo                   nvarchar(100) not null,
	descricao                nvarchar(1000),
	parametros               nvarchar(1000), --parâmetros aceitos, separados por vírgula
constraint [PK_LinxMetodos] primary key (
	doc_company asc,
	metodo asc
));

/*LinxEntrantes: entrantes por período e loja, só dos últimos 2 anos (método de portal, sem cnpjEmp)*/

create table [untreated].LinxEntrantes (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	portal                   int,
	id_entrantes             bigint,
	data_inicio              datetime,
	data_fim                 datetime,
	cnpj_empresa             nvarchar(14),
	entrantes                int, --fora da v274: tipo a conferir quando vier preenchido
	[timestamp]              bigint
);

create table [linxmicrovix].LinxEntrantes (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	portal                   int not null,
	id_entrantes             bigint not null,
	data_inicio              datetime,
	data_fim                 datetime,
	cnpj_empresa             nvarchar(14),
	entrantes                int, --fora da v274: tipo a conferir quando vier preenchido
	[timestamp]              bigint,
constraint [PK_LinxEntrantes] primary key (
	doc_company asc,
	portal asc,
	id_entrantes asc
));
