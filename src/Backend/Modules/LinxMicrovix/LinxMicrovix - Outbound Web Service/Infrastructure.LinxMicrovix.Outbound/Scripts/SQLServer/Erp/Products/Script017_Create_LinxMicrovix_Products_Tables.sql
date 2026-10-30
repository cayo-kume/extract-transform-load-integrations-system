/****** Produtos (LinxMicrovixProductsService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxProdutos: o cadastro de produtos de acordo com a loja pesquisada ou dados da estrutura mercadológica*/

create table [untreated].LinxProdutos (
	[integrated_or_updated_at]          datetime,
	[doc_company]                       nvarchar(14),
	[portal]                            int,
	[cod_produto]                       bigint,
	[cod_barra]                         nvarchar(20),
	[nome]                              nvarchar(250), --fora da documentação
	[ncm]                               nvarchar(20),
	[cest]                              nvarchar(10),
	[referencia]                        nvarchar(20),
	[cod_auxiliar]                      nvarchar(40),
	[unidade]                           nvarchar(50),
	[desc_cor]                          nvarchar(30),
	[desc_tamanho]                      nvarchar(50),
	[desc_setor]                        nvarchar(30),
	[desc_linha]                        nvarchar(30),
	[desc_marca]                        nvarchar(30),
	[desc_colecao]                      nvarchar(50),
	[dt_update]                         datetime,
	[cod_fornecedor]                    int,
	[desativado]                        nvarchar(1),
	[desc_espessura]                    nvarchar(50),
	[id_espessura]                      int,
	[desc_classificacao]                nvarchar(50),
	[id_classificacao]                  int,
	[origem_mercadoria]                 int,
	[peso_liquido]                      decimal(28,8),
	[peso_bruto]                        decimal(28,8),
	[id_cor]                            int,
	[id_tamanho]                        nvarchar(5),
	[id_setor]                          int,
	[id_linha]                          int,
	[id_marca]                          int,
	[id_colecao]                        int,
	[dt_inclusao]                       datetime,
	[timestamp]                         bigint,
	[fator_conversao]                   decimal(28,8),
	[codigo_integracao_ws]              nvarchar(50),
	[codigo_integracao_reshop]          nvarchar(50),
	[id_produtos_opticos_tipo]          int,
	[id_sped_tipo_item]                 int,
	[componente]                        nvarchar(1),
	[altura_para_frete]                 decimal(19,4),
	[largura_para_frete]                decimal(19,4),
	[comprimento_para_frete]            decimal(19,4),
	[loja_virtual]                      nvarchar(max),
	[codigoproduto_io]                  nvarchar(50),
	[cod_comprador]                     int,
	[codigo_integracao_oms]             nvarchar(50),
	[obrigatorio_identificacao_cliente] bit,
	[descricao_basica]                  nvarchar(100),
	[curva]                             nvarchar(1),
	[tributa_fcp]                       bit,
	[observacao]                        nvarchar(1000),
	[codigo_adicional_reshop]           nvarchar(100),
	[brinde]                            bit,
	[exige_controle]                    nvarchar(1),
	[multiplo_venda]                    decimal(28,8),
	[unidade_compra]                    nvarchar(50)
);

create table [linxmicrovix].LinxProdutos (
	[integrated_or_updated_at]          datetime,
	[doc_company]                       nvarchar(14) not null,
	[portal]                            int not null,
	[cod_produto]                       bigint not null,
	[cod_barra]                         nvarchar(20),
	[nome]                              nvarchar(250), --fora da documentação
	[ncm]                               nvarchar(20),
	[cest]                              nvarchar(10),
	[referencia]                        nvarchar(20),
	[cod_auxiliar]                      nvarchar(40),
	[unidade]                           nvarchar(50),
	[desc_cor]                          nvarchar(30),
	[desc_tamanho]                      nvarchar(50),
	[desc_setor]                        nvarchar(30),
	[desc_linha]                        nvarchar(30),
	[desc_marca]                        nvarchar(30),
	[desc_colecao]                      nvarchar(50),
	[dt_update]                         datetime,
	[cod_fornecedor]                    int,
	[desativado]                        nvarchar(1),
	[desc_espessura]                    nvarchar(50),
	[id_espessura]                      int,
	[desc_classificacao]                nvarchar(50),
	[id_classificacao]                  int,
	[origem_mercadoria]                 int,
	[peso_liquido]                      decimal(28,8),
	[peso_bruto]                        decimal(28,8),
	[id_cor]                            int,
	[id_tamanho]                        nvarchar(5),
	[id_setor]                          int,
	[id_linha]                          int,
	[id_marca]                          int,
	[id_colecao]                        int,
	[dt_inclusao]                       datetime,
	[timestamp]                         bigint,
	[fator_conversao]                   decimal(28,8),
	[codigo_integracao_ws]              nvarchar(50),
	[codigo_integracao_reshop]          nvarchar(50),
	[id_produtos_opticos_tipo]          int,
	[id_sped_tipo_item]                 int,
	[componente]                        nvarchar(1),
	[altura_para_frete]                 decimal(19,4),
	[largura_para_frete]                decimal(19,4),
	[comprimento_para_frete]            decimal(19,4),
	[loja_virtual]                      nvarchar(max),
	[codigoproduto_io]                  nvarchar(50),
	[cod_comprador]                     int,
	[codigo_integracao_oms]             nvarchar(50),
	[obrigatorio_identificacao_cliente] bit,
	[descricao_basica]                  nvarchar(100),
	[curva]                             nvarchar(1),
	[tributa_fcp]                       bit,
	[observacao]                        nvarchar(1000),
	[codigo_adicional_reshop]           nvarchar(100),
	[brinde]                            bit,
	[exige_controle]                    nvarchar(1),
	[multiplo_venda]                    decimal(28,8),
	[unidade_compra]                    nvarchar(50),
constraint [PK_LinxProdutos] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_produto] asc
));

/*LinxProdutosAssociados: a associação entre produtos. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxProdutosAssociados (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   nvarchar(50),
	[codigoproduto_origem]     bigint,
	[codigoproduto_associado]  bigint,
	[coeficiente_desconto]     decimal(28,8),
	[timestamp]                bigint,
	[qtde_item]                decimal(28,8),
	[item_obrigatorio]         bit
);

create table [linxmicrovix].LinxProdutosAssociados (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   nvarchar(50),
	[codigoproduto_origem]     bigint not null,
	[codigoproduto_associado]  bigint not null,
	[coeficiente_desconto]     decimal(28,8),
	[timestamp]                bigint,
	[qtde_item]                decimal(28,8),
	[item_obrigatorio]         bit,
constraint [PK_LinxProdutosAssociados] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[codigoproduto_origem] asc,
	[codigoproduto_associado] asc
));

/*LinxProdutosCamposAdicionais: os campos adicionais preenchidos dos Produtos cadastrados no portal*/

create table [untreated].LinxProdutosCamposAdicionais (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_produto]              bigint,
	[campo]                    nvarchar(50), --fora da documentação
	[valor]                    nvarchar(30),
	[timestamp]                bigint,
	[id_campo_valor]           int
);

create table [linxmicrovix].LinxProdutosCamposAdicionais (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_produto]              bigint not null,
	[campo]                    nvarchar(50) not null, --fora da documentação
	[valor]                    nvarchar(30),
	[timestamp]                bigint,
	[id_campo_valor]           int,
constraint [PK_LinxProdutosCamposAdicionais] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_produto] asc,
	[campo] asc
));

/*LinxProdutosCodBar: o cadastro de códigos de barras dos produtos*/

create table [untreated].LinxProdutosCodBar (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[cod_produto]              bigint,
	[cod_barra]                nvarchar(20),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxProdutosCodBar (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[cod_produto]              bigint not null,
	[cod_barra]                nvarchar(20) not null,
	[timestamp]                bigint,
constraint [PK_LinxProdutosCodBar] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_produto] asc,
	[cod_barra] asc
));

/*LinxProdutosFornec: os registros de produtos x fornecedores*/

create table [untreated].LinxProdutosFornec (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id_prod_forn]             int,
	[codigoproduto]            bigint,
	[cod_fornecedor]           int,
	[custo]                    decimal(19,4),
	[moeda]                    nvarchar(10),
	[unidade_compra]           nvarchar(50),
	[fator_conversao_compra]   decimal(19,4),
	[codauxiliar]              nvarchar(40),
	[qtde_embalagem]           decimal(28,8),
	[prazo_entrega_padrao]     int,
	[empresa]                  int,
	[desconto1]                decimal(28,8),
	[desconto2]                decimal(28,8),
	[desconto3]                decimal(28,8),
	[desconto]                 decimal(28,8),
	[ipi1]                     decimal(19,4),
	[diferencial_icms]         decimal(28,8),
	[despesas1]                decimal(19,4),
	[acrescimo]                decimal(19,4),
	[valor_custo_substituicao] decimal(19,4),
	[frete1]                   decimal(19,4),
	[fornecedor_principal]     nvarchar(1),
	[excluido]                 nvarchar(1),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxProdutosFornec (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id_prod_forn]             int not null,
	[codigoproduto]            bigint,
	[cod_fornecedor]           int,
	[custo]                    decimal(19,4),
	[moeda]                    nvarchar(10),
	[unidade_compra]           nvarchar(50),
	[fator_conversao_compra]   decimal(19,4),
	[codauxiliar]              nvarchar(40),
	[qtde_embalagem]           decimal(28,8),
	[prazo_entrega_padrao]     int,
	[empresa]                  int,
	[desconto1]                decimal(28,8),
	[desconto2]                decimal(28,8),
	[desconto3]                decimal(28,8),
	[desconto]                 decimal(28,8),
	[ipi1]                     decimal(19,4),
	[diferencial_icms]         decimal(28,8),
	[despesas1]                decimal(19,4),
	[acrescimo]                decimal(19,4),
	[valor_custo_substituicao] decimal(19,4),
	[frete1]                   decimal(19,4),
	[fornecedor_principal]     nvarchar(1),
	[excluido]                 nvarchar(1),
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_LinxProdutosFornec] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_prod_forn] asc
));

/*LinxProdutosInformacoes: informações dos produtos (características). Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxProdutosInformacoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigoproduto]            bigint,
	[informacoes_produto]      nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxProdutosInformacoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigoproduto]            bigint not null,
	[informacoes_produto]      nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxProdutosInformacoes] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigoproduto] asc
));

/*LinxProdutosImagensURL: as URLs das imagens dos produtos.*/

create table [untreated].LinxProdutosImagensURL (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[codigoproduto]            bigint,
	[url_imagem_blob]          nvarchar(200),
	[timestamp]                bigint,
	[portal]                   int,
	[id_imagem]                int
);

create table [linxmicrovix].LinxProdutosImagensURL (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[codigoproduto]            bigint not null,
	[url_imagem_blob]          nvarchar(200),
	[timestamp]                bigint,
	[portal]                   int not null,
	[id_imagem]                int not null,
constraint [PK_LinxProdutosImagensURL] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_imagem] asc,
	[codigoproduto] asc
));

/*LinxProdutosLotes: o saldo do lotes de produtos. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxProdutosLotes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[codigoproduto]            bigint,
	[empresa]                  int,
	[lote]                     nvarchar(60),
	[deposito]                 int,
	[saldo_disponivel]         decimal(19,4),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxProdutosLotes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[codigoproduto]            bigint not null,
	[empresa]                  int not null,
	[lote]                     nvarchar(60) not null,
	[deposito]                 int not null,
	[saldo_disponivel]         decimal(19,4),
	[timestamp]                bigint,
constraint [PK_LinxProdutosLotes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[codigoproduto] asc,
	[empresa] asc,
	[lote] asc,
	[deposito] asc
));

/*LinxLotesProdutos: os dados dos lotes dos produtos. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxLotesProdutos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id_lote]                  int,
	[codigo_produto]           bigint,
	[lote]                     nvarchar(60),
	[identificador]            nvarchar(1000), --fora da documentação
	[transacao]                int,
	[data_fabricacao]          datetime,
	[data_vencimento]          datetime,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxLotesProdutos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id_lote]                  int not null,
	[codigo_produto]           bigint,
	[lote]                     nvarchar(60),
	[identificador]            nvarchar(1000), --fora da documentação
	[transacao]                int,
	[data_fabricacao]          datetime,
	[data_vencimento]          datetime,
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_LinxLotesProdutos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_lote] asc
));

/*LinxProdutosSerial: o saldo atual dos seriais cadastrados no portal. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxProdutosSerial (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[codigoproduto]            bigint,
	[serial]                   nvarchar(50),
	[id_deposito]              int,
	[saldo]                    bit,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxProdutosSerial (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int not null,
	[codigoproduto]            bigint not null,
	[serial]                   nvarchar(50) not null,
	[id_deposito]              int not null,
	[saldo]                    bit,
	[timestamp]                bigint,
constraint [PK_LinxProdutosSerial] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[codigoproduto] asc,
	[empresa] asc,
	[serial] asc,
	[id_deposito] asc
));
