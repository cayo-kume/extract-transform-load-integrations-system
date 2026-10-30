/****** Produtos do B2C (LinxMicrovixB2CProductsService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da V64; respostas planas: uma tabela por método, sem filhas

/*B2CConsultaProdutos: */

create table [untreated].B2CConsultaProdutos (
	[integrated_or_updated_at]    datetime,
	[doc_company]                 nvarchar(14),
	[codigoproduto]               bigint,
	[referencia]                  nvarchar(20),
	[codauxiliar1]                nvarchar(40),
	[descricao_basica]            nvarchar(100),
	[nome_produto]                nvarchar(250),
	[peso_liquido]                decimal(28,8),
	[codigo_setor]                int,
	[codigo_linha]                int,
	[codigo_marca]                int,
	[codigo_colecao]              int,
	[codigo_espessura]            int,
	[codigo_grade1]               int,
	[codigo_grade2]               int,
	[unidade]                     nvarchar(50),
	[ativo]                       bit,
	[codigo_classificacao]        int,
	[dt_cadastro]                 datetime,
	[observacao]                  nvarchar(max),
	[cod_fornecedor]              int,
	[dt_update]                   datetime,
	[timestamp]                   bigint,
	[altura_para_frete]           decimal(28,8),
	[largura_para_frete]          decimal(28,8),
	[comprimento_para_frete]      decimal(28,8),
	[peso_bruto]                  decimal(28,8),
	[portal]                      int,
	[descricao_completa_commerce] nvarchar(max), --até 8000 caracteres (validador); nvarchar só vai até 4000 ou max
	[canais_ecommerce_publicados] int,
	[inicio_publicacao_produto]   datetime,
	[fim_publicacao_produto]      datetime,
	[codigo_integracao_oms]       nvarchar(50),
	[descricao_garantia]          nvarchar(max),
	[meses_garantia_fabrica]      int,
	[ncm]                         nvarchar(20),
	[cest]                        nvarchar(10),
	[codigo_adicional_reshop]     nvarchar(100)
);

create table [linxmicrovix].B2CConsultaProdutos (
	[integrated_or_updated_at]    datetime,
	[doc_company]                 nvarchar(14) not null,
	[codigoproduto]               bigint not null,
	[referencia]                  nvarchar(20),
	[codauxiliar1]                nvarchar(40),
	[descricao_basica]            nvarchar(100),
	[nome_produto]                nvarchar(250),
	[peso_liquido]                decimal(28,8),
	[codigo_setor]                int,
	[codigo_linha]                int,
	[codigo_marca]                int,
	[codigo_colecao]              int,
	[codigo_espessura]            int,
	[codigo_grade1]               int,
	[codigo_grade2]               int,
	[unidade]                     nvarchar(50),
	[ativo]                       bit,
	[codigo_classificacao]        int,
	[dt_cadastro]                 datetime,
	[observacao]                  nvarchar(max),
	[cod_fornecedor]              int,
	[dt_update]                   datetime,
	[timestamp]                   bigint,
	[altura_para_frete]           decimal(28,8),
	[largura_para_frete]          decimal(28,8),
	[comprimento_para_frete]      decimal(28,8),
	[peso_bruto]                  decimal(28,8),
	[portal]                      int not null,
	[descricao_completa_commerce] nvarchar(max), --até 8000 caracteres (validador); nvarchar só vai até 4000 ou max
	[canais_ecommerce_publicados] int,
	[inicio_publicacao_produto]   datetime,
	[fim_publicacao_produto]      datetime,
	[codigo_integracao_oms]       nvarchar(50),
	[descricao_garantia]          nvarchar(max),
	[meses_garantia_fabrica]      int,
	[ncm]                         nvarchar(20),
	[cest]                        nvarchar(10),
	[codigo_adicional_reshop]     nvarchar(100),
constraint [PK_B2CConsultaProdutos] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigoproduto] asc
));

/*B2CConsultaProdutosAssociados:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaProdutosAssociados (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id]                       int,
	[codigoproduto]            bigint,
	[codigoproduto_associado]  bigint,
	[coeficiente_desconto]     decimal(28,8),
	[timestamp]                bigint,
	[portal]                   int,
	[qtde_item]                decimal(28,8),
	[item_obrigatorio]         bit
);

create table [linxmicrovix].B2CConsultaProdutosAssociados (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id]                       int not null,
	[codigoproduto]            bigint,
	[codigoproduto_associado]  bigint,
	[coeficiente_desconto]     decimal(28,8),
	[timestamp]                bigint,
	[portal]                   int,
	[qtde_item]                decimal(28,8),
	[item_obrigatorio]         bit,
constraint [PK_B2CConsultaProdutosAssociados] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id] asc
));

/*B2CConsultaProdutosCampanhas:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaProdutosCampanhas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[codigo_campanha]          int,
	[nome_campanha]            nvarchar(60),
	[vigencia_inicio]          datetime,
	[vigencia_fim]             datetime,
	[observacao]               nvarchar(max),
	[ativo]                    bit,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosCampanhas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[codigo_campanha]          int not null,
	[nome_campanha]            nvarchar(60),
	[vigencia_inicio]          datetime,
	[vigencia_fim]             datetime,
	[observacao]               nvarchar(max),
	[ativo]                    bit,
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_B2CConsultaProdutosCampanhas] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[codigo_campanha] asc
));

/*B2CConsultaProdutosCodebar: */

create table [untreated].B2CConsultaProdutosCodebar (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigoproduto]            bigint,
	[codebar]                  nvarchar(max),
	[id_produtos_codebar]      int,
	[principal]                bit,
	[empresa]                  int,
	[timestamp]                bigint,
	[tipo_codebar]             nvarchar(max),
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosCodebar (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigoproduto]            bigint,
	[codebar]                  nvarchar(max),
	[id_produtos_codebar]      int not null,
	[principal]                bit,
	[empresa]                  int,
	[timestamp]                bigint,
	[tipo_codebar]             nvarchar(max),
	[portal]                   int not null,
constraint [PK_B2CConsultaProdutosCodebar] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_produtos_codebar] asc
));

/*B2CConsultaProdutosDimensoes: */

create table [untreated].B2CConsultaProdutosDimensoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigoproduto]            bigint,
	[altura]                   decimal(28,8), --fora da documentação
	[comprimento]              decimal(28,8), --fora da documentação
	[timestamp]                bigint,
	[largura]                  decimal(28,8), --fora da documentação
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosDimensoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigoproduto]            bigint not null,
	[altura]                   decimal(28,8), --fora da documentação
	[comprimento]              decimal(28,8), --fora da documentação
	[timestamp]                bigint,
	[largura]                  decimal(28,8), --fora da documentação
	[portal]                   int not null,
constraint [PK_B2CConsultaProdutosDimensoes] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigoproduto] asc
));

/*B2CConsultaProdutosInformacoes:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaProdutosInformacoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_produtos_informacoes]  int,
	[codigoproduto]            bigint,
	[informacoes_produto]      nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosInformacoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_produtos_informacoes]  int not null,
	[codigoproduto]            bigint,
	[informacoes_produto]      nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaProdutosInformacoes] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_produtos_informacoes] asc
));

/*B2CConsultaProdutosStatus: */

create table [untreated].B2CConsultaProdutosStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigoproduto]            bigint,
	[referencia]               nvarchar(20),
	[ativo]                    bit,
	[b2c]                      bit,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigoproduto]            bigint not null,
	[referencia]               nvarchar(20),
	[ativo]                    bit,
	[b2c]                      bit,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaProdutosStatus] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigoproduto] asc
));

/*B2CConsultaProdutosFlags:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaProdutosFlags (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_b2c_flags_produtos]    int,
	[id_b2c_flags]             int,
	[codigoproduto]            bigint,
	[timestamp]                bigint,
	[descricao_b2c_flags]      nvarchar(300)
);

create table [linxmicrovix].B2CConsultaProdutosFlags (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_b2c_flags_produtos]    int not null,
	[id_b2c_flags]             int,
	[codigoproduto]            bigint,
	[timestamp]                bigint,
	[descricao_b2c_flags]      nvarchar(300),
constraint [PK_B2CConsultaProdutosFlags] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_flags_produtos] asc
));

/*B2CConsultaProdutosTags:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaProdutosTags (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_b2c_tags_produtos]     int,
	[id_b2c_tags]              int,
	[codigoproduto]            bigint,
	[timestamp]                bigint,
	[descricao_b2c_tags]       nvarchar(300)
);

create table [linxmicrovix].B2CConsultaProdutosTags (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_b2c_tags_produtos]     int not null,
	[id_b2c_tags]              int,
	[codigoproduto]            bigint,
	[timestamp]                bigint,
	[descricao_b2c_tags]       nvarchar(300),
constraint [PK_B2CConsultaProdutosTags] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_tags_produtos] asc
));

/*B2CConsultaProdutosPalavrasChavePesquisa:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaProdutosPalavrasChavePesquisa (
	[integrated_or_updated_at]                datetime,
	[doc_company]                             nvarchar(14),
	[cnpj_emp]                                nvarchar(14), --loja consultada
	[portal]                                  int,
	[id_b2c_palavras_chave_pesquisa_produtos] int,
	[id_b2c_palavras_chave_pesquisa]          int,
	[codigoproduto]                           bigint,
	[timestamp]                               bigint,
	[descricao_b2c_palavras_chave_pesquisa]   nvarchar(max)
);

create table [linxmicrovix].B2CConsultaProdutosPalavrasChavePesquisa (
	[integrated_or_updated_at]                datetime,
	[doc_company]                             nvarchar(14) not null,
	[cnpj_emp]                                nvarchar(14) not null, --loja consultada
	[portal]                                  int,
	[id_b2c_palavras_chave_pesquisa_produtos] int not null,
	[id_b2c_palavras_chave_pesquisa]          int,
	[codigoproduto]                           bigint,
	[timestamp]                               bigint,
	[descricao_b2c_palavras_chave_pesquisa]   nvarchar(max),
constraint [PK_B2CConsultaProdutosPalavrasChavePesquisa] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_palavras_chave_pesquisa_produtos] asc
));
