/****** Preços, promoções e cupons (LinxMicrovixPricesService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxProdutosTabelas: o cadastro de Tabelas de Preços Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxProdutosTabelas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_tabela]                int,
	[nome_tabela]              nvarchar(50),
	[ativa]                    nvarchar(1),
	[timestamp]                bigint,
	[tipo_tabela]              nvarchar(1),
	[codigo_integracao_ws]     nvarchar(50)
);

create table [linxmicrovix].LinxProdutosTabelas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_tabela]                int not null,
	[nome_tabela]              nvarchar(50),
	[ativa]                    nvarchar(1),
	[timestamp]                bigint,
	[tipo_tabela]              nvarchar(1),
	[codigo_integracao_ws]     nvarchar(50),
constraint [PK_LinxProdutosTabelas] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_tabela] asc
));

/*LinxProdutosTabelasPrecos: a relação dos produtos por tabela de preço*/

create table [untreated].LinxProdutosTabelasPrecos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_tabela]                int,
	[cod_produto]              bigint,
	[precovenda]               decimal(19,4),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxProdutosTabelasPrecos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_tabela]                int not null,
	[cod_produto]              bigint not null,
	[precovenda]               decimal(19,4),
	[timestamp]                bigint,
constraint [PK_LinxProdutosTabelasPrecos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_tabela] asc,
	[cod_produto] asc
));

/*LinxProdutosPromocoes: informações das promoções dos produtos de acordo com a loja pesquisada*/

create table [untreated].LinxProdutosPromocoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_produto]              bigint,
	[preco_promocao]           decimal(19,4),
	[data_inicio_promocao]     datetime,
	[data_termino_promocao]    datetime,
	[data_cadastro_promocao]   datetime,
	[promocao_ativa]           nvarchar(1),
	[id_campanha]              bigint,
	[nome_campanha]            nvarchar(60),
	[promocao_opcional]        bit,
	[custo_total_campanha]     decimal(19,4),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxProdutosPromocoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_produto]              bigint not null,
	[preco_promocao]           decimal(19,4),
	[data_inicio_promocao]     datetime not null,
	[data_termino_promocao]    datetime,
	[data_cadastro_promocao]   datetime,
	[promocao_ativa]           nvarchar(1),
	[id_campanha]              bigint not null,
	[nome_campanha]            nvarchar(60),
	[promocao_opcional]        bit,
	[custo_total_campanha]     decimal(19,4),
	[timestamp]                bigint,
constraint [PK_LinxProdutosPromocoes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_produto] asc,
	[data_inicio_promocao] asc,
	[id_campanha] asc
));

/*LinxAcoesPromocionais: as Ações Promocionais cadastradas no Microvix Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxAcoesPromocionais (
	[integrated_or_updated_at]     datetime,
	[doc_company]                  nvarchar(14),
	[cnpj_emp]                     nvarchar(14), --loja consultada
	[portal]                       int,
	[id_acoes_promocionais]        int,
	[descricao]                    nvarchar(100),
	[vigencia_inicio]              datetime,
	[vigencia_fim]                 datetime,
	[observacao]                   nvarchar(max),
	[ativa]                        bit,
	[excluida]                     bit,
	[integrada]                    bit,
	[qtde_integrada]               int,
	[valor_pago_franqueadora]      decimal(19,4),
	[id_acoes_promocionais_origem] int,
	[timestamp]                    bigint
);

create table [linxmicrovix].LinxAcoesPromocionais (
	[integrated_or_updated_at]     datetime,
	[doc_company]                  nvarchar(14) not null,
	[cnpj_emp]                     nvarchar(14) not null, --loja consultada
	[portal]                       int,
	[id_acoes_promocionais]        int not null,
	[descricao]                    nvarchar(100),
	[vigencia_inicio]              datetime,
	[vigencia_fim]                 datetime,
	[observacao]                   nvarchar(max),
	[ativa]                        bit,
	[excluida]                     bit,
	[integrada]                    bit,
	[qtde_integrada]               int,
	[valor_pago_franqueadora]      decimal(19,4),
	[id_acoes_promocionais_origem] int,
	[timestamp]                    bigint,
constraint [PK_LinxAcoesPromocionais] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_acoes_promocionais] asc
));

/*LinxAcoesPromocionaisCombinacaoProdutosItens: os itens da combinação de produtos de ações promocionais.*/

create table [untreated].LinxAcoesPromocionaisCombinacaoProdutosItens (
	[integrated_or_updated_at]                        datetime,
	[doc_company]                                     nvarchar(14),
	[id_acoes_promocionais_combinacao_produtos_itens] int,
	[id_combinacao_produto]                           int,
	[codigoproduto]                                   bigint,
	[timestamp]                                       bigint,
	[portal]                                          int
);

create table [linxmicrovix].LinxAcoesPromocionaisCombinacaoProdutosItens (
	[integrated_or_updated_at]                        datetime,
	[doc_company]                                     nvarchar(14) not null,
	[id_acoes_promocionais_combinacao_produtos_itens] int not null,
	[id_combinacao_produto]                           int,
	[codigoproduto]                                   bigint,
	[timestamp]                                       bigint,
	[portal]                                          int not null,
constraint [PK_LinxAcoesPromocionaisCombinacaoProdutosItens] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_acoes_promocionais_combinacao_produtos_itens] asc
));

/*LinxAcoesPromocionaisProdutosCortesia: os produtos de cortesia de ações promocionais.*/

create table [untreated].LinxAcoesPromocionaisProdutosCortesia (
	[integrated_or_updated_at]                datetime,
	[doc_company]                             nvarchar(14),
	[id_acoes_promocionais_produtos_cortesia] int,
	[id_acoes_promocionais]                   int,
	[codigoproduto]                           bigint,
	[id_combinacao_produto]                   int,
	[timestamp]                               bigint,
	[portal]                                  int
);

create table [linxmicrovix].LinxAcoesPromocionaisProdutosCortesia (
	[integrated_or_updated_at]                datetime,
	[doc_company]                             nvarchar(14) not null,
	[id_acoes_promocionais_produtos_cortesia] int not null,
	[id_acoes_promocionais]                   int,
	[codigoproduto]                           bigint,
	[id_combinacao_produto]                   int,
	[timestamp]                               bigint,
	[portal]                                  int not null,
constraint [PK_LinxAcoesPromocionaisProdutosCortesia] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_acoes_promocionais_produtos_cortesia] asc
));

/*LinxMovimentoAcoesPromocionais: as Ações Promocionais realizadas nas movimentações*/

create table [untreated].LinxMovimentoAcoesPromocionais (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[identificador]            uniqueidentifier,
	[transacao]                int,
	[id_acoes_promocionais]    int,
	[desconto_item]            decimal(19,4),
	[quantidade]               int,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxMovimentoAcoesPromocionais (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[identificador]            uniqueidentifier,
	[transacao]                int not null,
	[id_acoes_promocionais]    int not null,
	[desconto_item]            decimal(19,4),
	[quantidade]               int,
	[timestamp]                bigint not null,
constraint [PK_LinxMovimentoAcoesPromocionais] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[transacao] asc,
	[id_acoes_promocionais] asc,
	[timestamp] asc
));

/*LinxCupomDesconto: os registros do cadastro de cupons de desconto.*/

create table [untreated].LinxCupomDesconto (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_cupom_desconto]        int,
	[cupom]                    nvarchar(50),
	[descricao]                nvarchar(255),
	[percentual_desconto]      decimal(28,8),
	[data_inicial_vigencia]    datetime,
	[data_final_vigencia]      datetime,
	[qtde_original]            int,
	[qtde_disponivel]          int,
	[cod_vendedor]             int,
	[disponivel]               bit,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxCupomDesconto (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_cupom_desconto]        int,
	[cupom]                    nvarchar(50) not null,
	[descricao]                nvarchar(255),
	[percentual_desconto]      decimal(28,8),
	[data_inicial_vigencia]    datetime,
	[data_final_vigencia]      datetime,
	[qtde_original]            int,
	[qtde_disponivel]          int,
	[cod_vendedor]             int,
	[disponivel]               bit,
	[timestamp]                bigint,
constraint [PK_LinxCupomDesconto] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cupom] asc
));

/*LinxCupomDescontoVendas: o id do cupom de desconto, o valor do cupom e identificador das vendas que utilizaram cupons de desconto. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxCupomDescontoVendas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_cupom_desconto_vendas] int,
	[id_cupom_desconto]        int,
	[identificador]            uniqueidentifier,
	[valor]                    decimal(28,8),
	[timestamp]                bigint,
	[id_vendas_pos]            int
);

create table [linxmicrovix].LinxCupomDescontoVendas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_cupom_desconto_vendas] int,
	[id_cupom_desconto]        int,
	[identificador]            uniqueidentifier not null,
	[valor]                    decimal(28,8),
	[timestamp]                bigint,
	[id_vendas_pos]            int,
constraint [PK_LinxCupomDescontoVendas] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[identificador] asc
));

/*LinxMotivoDesconto: os motivos de desconto cadastrados no portal Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxMotivoDesconto (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_motivo_desconto]       int,
	[desc_motivo_desconto]     nvarchar(60),
	[ativo]                    bit,
	[excluido]                 bit,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxMotivoDesconto (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_motivo_desconto]       int not null,
	[desc_motivo_desconto]     nvarchar(60),
	[ativo]                    bit,
	[excluido]                 bit,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxMotivoDesconto] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_motivo_desconto] asc
));
