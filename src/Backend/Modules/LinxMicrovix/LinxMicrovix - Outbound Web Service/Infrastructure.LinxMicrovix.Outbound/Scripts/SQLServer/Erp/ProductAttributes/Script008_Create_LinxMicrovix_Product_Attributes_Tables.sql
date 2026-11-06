-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; .docs/MIGRATOR-CSX.md)
/****** Atributos de produto (LinxMicrovixProductAttributesService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxSetores: os setores cadastrados Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxSetores') = 1
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

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxSetores') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].LinxLinhas') = 1
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

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxLinhas') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].LinxMarcas') = 1
create table [untreated].LinxMarcas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_marca]                 int,
	[desc_marca]               nvarchar(30),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[portal]                   int
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxMarcas') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].LinxColecoes') = 1
create table [untreated].LinxColecoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_colecao]               int,
	[desc_colecao]             nvarchar(50),
	[timestamp]                bigint,
	[codigo_integracao_ws]     nvarchar(50),
	[portal]                   int
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxColecoes') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].LinxTamanhos') = 1
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

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxTamanhos') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].LinxCores') = 1
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

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxCores') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].LinxEspessuras') = 1
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

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxEspessuras') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].LinxUnidades') = 1
create table [untreated].LinxUnidades (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[unidade]                  nvarchar(50),
	[descricao]                nvarchar(200),
	[timestamp]                bigint,
	[portal]                   int
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxUnidades') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].LinxClassificacoes') = 1
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

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxClassificacoes') = 1
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

if dbo.fn_Migrator_Creates(N'[untreated].LinxSubClasses') = 1
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

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxSubClasses') = 1
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
