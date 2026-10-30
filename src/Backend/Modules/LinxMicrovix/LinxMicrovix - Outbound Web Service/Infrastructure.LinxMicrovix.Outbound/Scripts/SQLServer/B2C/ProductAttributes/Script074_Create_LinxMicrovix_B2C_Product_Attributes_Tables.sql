/****** Atributos de produto do B2C (LinxMicrovixB2CProductAttributesService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da V64; respostas planas: uma tabela por método, sem filhas

/*B2CConsultaClassificacao: */

create table [untreated].B2CConsultaClassificacao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigo_classificacao]     int,
	[nome_classificacao]       nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaClassificacao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigo_classificacao]     int not null,
	[nome_classificacao]       nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaClassificacao] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo_classificacao] asc
));

/*B2CConsultaColecoes: */

create table [untreated].B2CConsultaColecoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigo_colecao]           int, --fora da documentação
	[nome_colecao]             nvarchar(50), --fora da documentação
	[timestamp]                bigint,
	[marcas]                   nvarchar(50), --fora da documentação
	[portal]                   int --fora da documentação
);

create table [linxmicrovix].B2CConsultaColecoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigo_colecao]           int not null, --fora da documentação
	[nome_colecao]             nvarchar(50), --fora da documentação
	[timestamp]                bigint,
	[marcas]                   nvarchar(50), --fora da documentação
	[portal]                   int not null, --fora da documentação
constraint [PK_B2CConsultaColecoes] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo_colecao] asc
));

/*B2CConsultaEspessuras: */

create table [untreated].B2CConsultaEspessuras (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigo_espessura]         int,
	[nome_espessura]           nvarchar(100),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaEspessuras (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigo_espessura]         int not null,
	[nome_espessura]           nvarchar(100),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaEspessuras] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo_espessura] asc
));

/*B2CConsultaGrade1: */

create table [untreated].B2CConsultaGrade1 (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigo_grade1]            int,
	[nome_grade1]              nvarchar(100),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaGrade1 (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigo_grade1]            int not null,
	[nome_grade1]              nvarchar(100),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaGrade1] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo_grade1] asc
));

/*B2CConsultaGrade2: */

create table [untreated].B2CConsultaGrade2 (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigo_grade2]            int,
	[nome_grade2]              nvarchar(100),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaGrade2 (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigo_grade2]            int not null,
	[nome_grade2]              nvarchar(100),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaGrade2] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo_grade2] asc
));

/*B2CConsultaLinhas: */

create table [untreated].B2CConsultaLinhas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigo_linha]             int,
	[nome_linha]               nvarchar(30),
	[timestamp]                bigint,
	[setores]                  nvarchar(250),
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaLinhas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigo_linha]             int not null,
	[nome_linha]               nvarchar(30),
	[timestamp]                bigint,
	[setores]                  nvarchar(250),
	[portal]                   int not null,
constraint [PK_B2CConsultaLinhas] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo_linha] asc
));

/*B2CConsultaMarcas:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaMarcas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigo_marca]             int, --fora da documentação
	[nome_marca]               nvarchar(50), --fora da documentação
	[timestamp]                bigint,
	[linhas]                   nvarchar(10), --fora da documentação
	[portal]                   int --fora da documentação
);

create table [linxmicrovix].B2CConsultaMarcas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigo_marca]             int not null, --fora da documentação
	[nome_marca]               nvarchar(50), --fora da documentação
	[timestamp]                bigint,
	[linhas]                   nvarchar(10), --fora da documentação
	[portal]                   int not null, --fora da documentação
constraint [PK_B2CConsultaMarcas] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo_marca] asc
));

/*B2CConsultaSetores:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaSetores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigo_setor]             int,
	[nome_setor]               nvarchar(100),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaSetores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigo_setor]             int not null,
	[nome_setor]               nvarchar(100),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaSetores] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo_setor] asc
));

/*B2CConsultaUnidade:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaUnidade (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[unidade]                  nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaUnidade (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[unidade]                  nvarchar(50) not null,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaUnidade] primary key (
	[doc_company] asc,
	[portal] asc,
	[unidade] asc
));

/*B2CConsultaFornecedores: */

create table [untreated].B2CConsultaFornecedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cod_fornecedor]           int,
	[nome]                     nvarchar(250), --fora da documentação
	[nome_fantasia]            nvarchar(60),
	[tipo_pessoa]              nvarchar(1),
	[tipo_fornecedor]          nvarchar(1),
	[endereco]                 nvarchar(250),
	[numero_rua]               nvarchar(20),
	[bairro]                   nvarchar(60),
	[cep]                      nvarchar(9),
	[cidade]                   nvarchar(40),
	[uf]                       nvarchar(200),
	[documento]                nvarchar(14),
	[fone]                     nvarchar(20),
	[email]                    nvarchar(50),
	[pais]                     nvarchar(80),
	[obs]                      nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaFornecedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cod_fornecedor]           int not null,
	[nome]                     nvarchar(250), --fora da documentação
	[nome_fantasia]            nvarchar(60),
	[tipo_pessoa]              nvarchar(1),
	[tipo_fornecedor]          nvarchar(1),
	[endereco]                 nvarchar(250),
	[numero_rua]               nvarchar(20),
	[bairro]                   nvarchar(60),
	[cep]                      nvarchar(9),
	[cidade]                   nvarchar(40),
	[uf]                       nvarchar(200),
	[documento]                nvarchar(14),
	[fone]                     nvarchar(20),
	[email]                    nvarchar(50),
	[pais]                     nvarchar(80),
	[obs]                      nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaFornecedores] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_fornecedor] asc
));

/*B2CConsultaFlags:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaFlags (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_b2c_flags]             int,
	[descricao]                nvarchar(300),
	[timestamp]                bigint
);

create table [linxmicrovix].B2CConsultaFlags (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_b2c_flags]             int not null,
	[descricao]                nvarchar(300),
	[timestamp]                bigint,
constraint [PK_B2CConsultaFlags] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_flags] asc
));

/*B2CConsultaTags:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaTags (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_b2c_tags]              int,
	[descricao]                nvarchar(300),
	[timestamp]                bigint
);

create table [linxmicrovix].B2CConsultaTags (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_b2c_tags]              int not null,
	[descricao]                nvarchar(300),
	[timestamp]                bigint,
constraint [PK_B2CConsultaTags] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_tags] asc
));

/*B2CConsultaPalavrasChavePesquisa:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaPalavrasChavePesquisa (
	[integrated_or_updated_at]       datetime,
	[doc_company]                    nvarchar(14),
	[cnpj_emp]                       nvarchar(14), --loja consultada
	[portal]                         int,
	[id_b2c_palavras_chave_pesquisa] int,
	[descricao]                      nvarchar(max),
	[timestamp]                      bigint
);

create table [linxmicrovix].B2CConsultaPalavrasChavePesquisa (
	[integrated_or_updated_at]       datetime,
	[doc_company]                    nvarchar(14) not null,
	[cnpj_emp]                       nvarchar(14) not null, --loja consultada
	[portal]                         int,
	[id_b2c_palavras_chave_pesquisa] int not null,
	[descricao]                      nvarchar(max),
	[timestamp]                      bigint,
constraint [PK_B2CConsultaPalavrasChavePesquisa] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_palavras_chave_pesquisa] asc
));

/*B2CConsultaLegendasCadastrosAuxiliares:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaLegendasCadastrosAuxiliares (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[empresa]                  int,
	[legenda_setor]            nvarchar(20),
	[legenda_linha]            nvarchar(20),
	[legenda_marca]            nvarchar(20),
	[legenda_colecao]          nvarchar(20),
	[legenda_grade1]           nvarchar(20),
	[legenda_grade2]           nvarchar(20),
	[legenda_espessura]        nvarchar(20),
	[legenda_classificacao]    nvarchar(20),
	[timestamp]                bigint
);

create table [linxmicrovix].B2CConsultaLegendasCadastrosAuxiliares (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[empresa]                  int not null,
	[legenda_setor]            nvarchar(20),
	[legenda_linha]            nvarchar(20),
	[legenda_marca]            nvarchar(20),
	[legenda_colecao]          nvarchar(20),
	[legenda_grade1]           nvarchar(20),
	[legenda_grade2]           nvarchar(20),
	[legenda_espessura]        nvarchar(20),
	[legenda_classificacao]    nvarchar(20),
	[timestamp]                bigint,
constraint [PK_B2CConsultaLegendasCadastrosAuxiliares] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[empresa] asc
));
