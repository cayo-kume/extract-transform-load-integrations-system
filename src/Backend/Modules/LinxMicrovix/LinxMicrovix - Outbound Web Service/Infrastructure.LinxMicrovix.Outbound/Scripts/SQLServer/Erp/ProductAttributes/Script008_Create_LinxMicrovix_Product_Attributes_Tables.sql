/****** Atributos de produto (LinxMicrovixProductAttributesService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxSetores: os setores cadastrados Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxSetores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_setor]                 int,
	[desc_setor]               nvarchar(30),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[ativo]                    bit,
	[portal]                   int
);

create table [linxmicrovix].LinxSetores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_setor]                 int not null,
	[desc_setor]               nvarchar(30),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[ativo]                    bit,
	[portal]                   int not null,
constraint [PK_LinxSetores] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_setor] asc
));

/*LinxLinhas: todos os registros do cadastro auxiliar linha*/

create table [untreated].LinxLinhas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_linha]                 int,
	[desc_linha]               nvarchar(30),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[portal]                   int,
	[coeficiente_comissao]     decimal(28,8)
);

create table [linxmicrovix].LinxLinhas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_linha]                 int not null,
	[desc_linha]               nvarchar(30),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[portal]                   int not null,
	[coeficiente_comissao]     decimal(28,8),
constraint [PK_LinxLinhas] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_linha] asc
));

/*LinxMarcas: as marcas cadastradas.*/

create table [untreated].LinxMarcas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_marca]                 int,
	[desc_marca]               nvarchar(30),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[portal]                   int
);

create table [linxmicrovix].LinxMarcas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_marca]                 int not null,
	[desc_marca]               nvarchar(30),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[portal]                   int not null,
constraint [PK_LinxMarcas] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_marca] asc
));

/*LinxColecoes: as coleções cadastradas.*/

create table [untreated].LinxColecoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_colecao]               int,
	[desc_colecao]             nvarchar(50),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[portal]                   int
);

create table [linxmicrovix].LinxColecoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_colecao]               int not null,
	[desc_colecao]             nvarchar(50),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[portal]                   int not null,
constraint [PK_LinxColecoes] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_colecao] asc
));

/*LinxTamanhos: os tamanhos cadastrados.*/

create table [untreated].LinxTamanhos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_tamanho]               nvarchar(5),
	[desc_tamanho]             nvarchar(50),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[ativo]                    bit,
	[portal]                   int
);

create table [linxmicrovix].LinxTamanhos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_tamanho]               nvarchar(5) not null,
	[desc_tamanho]             nvarchar(50),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[ativo]                    bit,
	[portal]                   int not null,
constraint [PK_LinxTamanhos] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_tamanho] asc
));

/*LinxCores: as cores cadastradas.*/

create table [untreated].LinxCores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_cor]                   int,
	[desc_cor]                 nvarchar(30),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[ativo]                    bit,
	[portal]                   int
);

create table [linxmicrovix].LinxCores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_cor]                   int not null,
	[desc_cor]                 nvarchar(30),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[ativo]                    bit,
	[portal]                   int not null,
constraint [PK_LinxCores] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_cor] asc
));

/*LinxEspessuras: as espessuras cadastradas.*/

create table [untreated].LinxEspessuras (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_espessura]             int,
	[desc_espessura]           nvarchar(50),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[ativo]                    bit,
	[portal]                   int
);

create table [linxmicrovix].LinxEspessuras (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_espessura]             int not null,
	[desc_espessura]           nvarchar(50),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[ativo]                    bit,
	[portal]                   int not null,
constraint [PK_LinxEspessuras] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_espessura] asc
));

/*LinxUnidades: as unidades cadastradas.*/

create table [untreated].LinxUnidades (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[unidade]                  nvarchar(50),
	[descricao]                nvarchar(200),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxUnidades (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[unidade]                  nvarchar(50) not null,
	[descricao]                nvarchar(200),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxUnidades] primary key (
	[doc_company] asc,
	[portal] asc,
	[unidade] asc
));

/*LinxClassificacoes: as classificacoes cadastradas.*/

create table [untreated].LinxClassificacoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_classificacao]         int,
	[desc_classificacao]       nvarchar(50),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[ativo]                    bit,
	[portal]                   int
);

create table [linxmicrovix].LinxClassificacoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_classificacao]         int not null,
	[desc_classificacao]       nvarchar(50),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[ativo]                    bit,
	[portal]                   int not null,
constraint [PK_LinxClassificacoes] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_classificacao] asc
));

/*LinxSubClasses: o cadastro das Sub Classes dos clientes e fornecedores. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxSubClasses (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id_subclasses]            int,
	[classe]                   int,
	[descricao]                nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxSubClasses (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id_subclasses]            int not null,
	[classe]                   int,
	[descricao]                nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_LinxSubClasses] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_subclasses] asc
));
