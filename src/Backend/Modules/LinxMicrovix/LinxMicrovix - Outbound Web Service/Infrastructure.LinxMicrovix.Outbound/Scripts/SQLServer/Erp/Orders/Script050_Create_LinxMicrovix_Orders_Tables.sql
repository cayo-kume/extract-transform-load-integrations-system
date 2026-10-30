/****** Pedidos de venda (LinxMicrovixOrdersService): tabelas untreated e linxmicrovix (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxPedidosVenda: informações de pedidos de venda de acordo com a loja pesquisada*/

create table [untreated].LinxPedidosVenda (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_pedido]               int,
	[data_lancamento]          datetime,
	[hora_lancamento]          nvarchar(5),
	[transacao]                int,
	[usuario]                  int,
	[codigo_cliente]           int,
	[cod_produto]              bigint,
	[quantidade]               decimal(28,8),
	[valor_unitario]           decimal(19,4),
	[cod_vendedor]             int,
	[valor_frete]              decimal(19,4),
	[valor_total]              decimal(19,4),
	[desconto_item]            decimal(19,4),
	[cod_plano_pagamento]      int,
	[plano_pagamento]          nvarchar(100),
	[obs]                      nvarchar(max),
	[aprovado]                 nvarchar(1),
	[cancelado]                nvarchar(1),
	[data_aprovacao]           datetime,
	[data_alteracao]           datetime,
	[tipo_frete]               int,
	[natureza_operacao]        nvarchar(60),
	[tabela_preco]             int,
	[nome_tabela_preco]        nvarchar(50),
	[previsao_entrega]         datetime,
	[realizado_por]            int,
	[pontuacao_ser]            int,
	[venda_externa]            nvarchar(1),
	[nf_gerada]                nvarchar(max),
	[status]                   nvarchar(1),
	[numero_projeto_officina]  nvarchar(50),
	[cod_natureza_operacao]    nvarchar(10),
	[margem_contribuicao]      decimal(19,4),
	[doc_origem]               int,
	[posicao_item]             int,
	[orcamento_origem]         int,
	[transacao_origem]         int,
	[timestamp]                bigint,
	[desconto]                 decimal(19,4),
	[transacao_ws]             int,
	[empresa]                  int,
	[transportador]            bigint,
	[deposito]                 int,
	[texto_agrupamento]        nvarchar(60),
	[posicao]                  int,
	[encerrado]                bit
);

create table [linxmicrovix].LinxPedidosVenda (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_pedido]               int,
	[data_lancamento]          datetime,
	[hora_lancamento]          nvarchar(5),
	[transacao]                int not null,
	[usuario]                  int,
	[codigo_cliente]           int,
	[cod_produto]              bigint,
	[quantidade]               decimal(28,8),
	[valor_unitario]           decimal(19,4),
	[cod_vendedor]             int,
	[valor_frete]              decimal(19,4),
	[valor_total]              decimal(19,4),
	[desconto_item]            decimal(19,4),
	[cod_plano_pagamento]      int,
	[plano_pagamento]          nvarchar(100),
	[obs]                      nvarchar(max),
	[aprovado]                 nvarchar(1),
	[cancelado]                nvarchar(1),
	[data_aprovacao]           datetime,
	[data_alteracao]           datetime,
	[tipo_frete]               int,
	[natureza_operacao]        nvarchar(60),
	[tabela_preco]             int,
	[nome_tabela_preco]        nvarchar(50),
	[previsao_entrega]         datetime,
	[realizado_por]            int,
	[pontuacao_ser]            int,
	[venda_externa]            nvarchar(1),
	[nf_gerada]                nvarchar(max),
	[status]                   nvarchar(1),
	[numero_projeto_officina]  nvarchar(50),
	[cod_natureza_operacao]    nvarchar(10),
	[margem_contribuicao]      decimal(19,4),
	[doc_origem]               int,
	[posicao_item]             int,
	[orcamento_origem]         int,
	[transacao_origem]         int,
	[timestamp]                bigint,
	[desconto]                 decimal(19,4),
	[transacao_ws]             int,
	[empresa]                  int,
	[transportador]            bigint,
	[deposito]                 int,
	[texto_agrupamento]        nvarchar(60),
	[posicao]                  int,
	[encerrado]                bit,
constraint [PK_LinxPedidosVenda] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[transacao] asc
));

/*LinxPlanosPedidoVenda: os Planos de Pagamentos realizados nos pedidos de vendas Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxPlanosPedidoVenda (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_pedido]               int,
	[plano]                    int,
	[desc_plano]               nvarchar(100),
	[total]                    decimal(19,4),
	[qtde_parcelas]            int,
	[indice_plano]             decimal(19,4),
	[valor_desc_acresc_plano]  decimal(19,4),
	[cod_forma_pgto]           int,
	[forma_pgto]               nvarchar(50),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxPlanosPedidoVenda (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_pedido]               int not null,
	[plano]                    int not null,
	[desc_plano]               nvarchar(100),
	[total]                    decimal(19,4),
	[qtde_parcelas]            int,
	[indice_plano]             decimal(19,4),
	[valor_desc_acresc_plano]  decimal(19,4),
	[cod_forma_pgto]           int,
	[forma_pgto]               nvarchar(50),
	[timestamp]                bigint,
constraint [PK_LinxPlanosPedidoVenda] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_pedido] asc,
	[plano] asc
));

/*LinxPedidosVendaChecklistEntregaLocal: os tipos de locais cadastrados no checklist de entrega do pedido de venda. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxPedidosVendaChecklistEntregaLocal (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[cnpj_emp]                   nvarchar(14), --loja consultada
	[id_checklist_entrega_local] int,
	[descricao]                  nvarchar(100),
	[timestamp]                  bigint,
	[portal]                     int
);

create table [linxmicrovix].LinxPedidosVendaChecklistEntregaLocal (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[cnpj_emp]                   nvarchar(14) not null, --loja consultada
	[id_checklist_entrega_local] int not null,
	[descricao]                  nvarchar(100),
	[timestamp]                  bigint,
	[portal]                     int,
constraint [PK_LinxPedidosVendaChecklistEntregaLocal] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_checklist_entrega_local] asc
));

/*LinxPedidosVendaChecklistEntregaArmazenamento: os tipos de armazenamentos cadastrados no checklist de entrega do pedido de venda. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxPedidosVendaChecklistEntregaArmazenamento (
	[integrated_or_updated_at]           datetime,
	[doc_company]                        nvarchar(14),
	[cnpj_emp]                           nvarchar(14), --loja consultada
	[id_checklist_entrega_armazenamento] int,
	[descricao]                          nvarchar(100),
	[timestamp]                          bigint,
	[portal]                             int
);

create table [linxmicrovix].LinxPedidosVendaChecklistEntregaArmazenamento (
	[integrated_or_updated_at]           datetime,
	[doc_company]                        nvarchar(14) not null,
	[cnpj_emp]                           nvarchar(14) not null, --loja consultada
	[id_checklist_entrega_armazenamento] int not null,
	[descricao]                          nvarchar(100),
	[timestamp]                          bigint,
	[portal]                             int,
constraint [PK_LinxPedidosVendaChecklistEntregaArmazenamento] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_checklist_entrega_armazenamento] asc
));

/*LinxPedidosVendaChecklistEntregaDificuldade: os tipos de dificuldades cadastradas no checklist de entrega do pedido de venda. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxPedidosVendaChecklistEntregaDificuldade (
	[integrated_or_updated_at]         datetime,
	[doc_company]                      nvarchar(14),
	[cnpj_emp]                         nvarchar(14), --loja consultada
	[id_checklist_entrega_dificuldade] nvarchar(1000), --fora da documentação
	[descricao]                        nvarchar(100),
	[timestamp]                        bigint,
	[portal]                           int
);

create table [linxmicrovix].LinxPedidosVendaChecklistEntregaDificuldade (
	[integrated_or_updated_at]         datetime,
	[doc_company]                      nvarchar(14) not null,
	[cnpj_emp]                         nvarchar(14) not null, --loja consultada
	[id_checklist_entrega_dificuldade] nvarchar(1000) not null, --fora da documentação
	[descricao]                        nvarchar(100),
	[timestamp]                        bigint,
	[portal]                           int,
constraint [PK_LinxPedidosVendaChecklistEntregaDificuldade] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_checklist_entrega_dificuldade] asc
));

/*LinxOrcamentoPosicao: os dados das posições cadastradas para os orçamentos. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxOrcamentoPosicao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_posicao]               int,
	[descricao_posicao]        nvarchar(50),
	[ativo_posicao]            nvarchar(1),
	[timestamp]                bigint,
	[portal]                   int --fora da documentação
);

create table [linxmicrovix].LinxOrcamentoPosicao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_posicao]               int not null,
	[descricao_posicao]        nvarchar(50),
	[ativo_posicao]            nvarchar(1),
	[timestamp]                bigint,
	[portal]                   int not null, --fora da documentação
constraint [PK_LinxOrcamentoPosicao] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_posicao] asc
));

/*LinxB2CPedidos: os pedidos de B2C*/

create table [untreated].LinxB2CPedidos (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[cnpj_emp]                   nvarchar(14), --loja consultada
	[id_pedido]                  int,
	[dt_pedido]                  datetime,
	[cod_cliente_erp]            int,
	[cod_cliente_b2c]            int,
	[vl_frete]                   decimal(28,8),
	[forma_pgto]                 int,
	[plano_pagamento]            int,
	[anotacao]                   nvarchar(400),
	[taxa_impressao]             decimal(28,8),
	[finalizado]                 bit,
	[valor_frete_gratis]         decimal(28,8),
	[tipo_frete]                 int,
	[id_status]                  int,
	[cod_transportador]          int,
	[tipo_cobranca_frete]        int,
	[ativo]                      bit,
	[empresa]                    int,
	[id_tabela_preco]            int,
	[valor_credito]              decimal(19,4),
	[cod_vendedor]               int,
	[timestamp]                  bigint,
	[dt_insert]                  datetime,
	[dt_disponivel_faturamento]  datetime,
	[mensagem_falha_faturamento] nvarchar(max),
	[portal]                     int,
	[order_id]                   nvarchar(40),
	[fulfillment_id]             nvarchar(40),
	[id_tipo_b2c]                int,
	[ecommerce_origem]           nvarchar(200),
	[marketplace_loja]           nvarchar(60),
	[cnpj_origem_venda]          nvarchar(14),
	[identificador]              uniqueidentifier
);

create table [linxmicrovix].LinxB2CPedidos (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[cnpj_emp]                   nvarchar(14) not null, --loja consultada
	[id_pedido]                  int not null,
	[dt_pedido]                  datetime,
	[cod_cliente_erp]            int,
	[cod_cliente_b2c]            int,
	[vl_frete]                   decimal(28,8),
	[forma_pgto]                 int,
	[plano_pagamento]            int,
	[anotacao]                   nvarchar(400),
	[taxa_impressao]             decimal(28,8),
	[finalizado]                 bit,
	[valor_frete_gratis]         decimal(28,8),
	[tipo_frete]                 int,
	[id_status]                  int,
	[cod_transportador]          int,
	[tipo_cobranca_frete]        int,
	[ativo]                      bit,
	[empresa]                    int,
	[id_tabela_preco]            int,
	[valor_credito]              decimal(19,4),
	[cod_vendedor]               int,
	[timestamp]                  bigint,
	[dt_insert]                  datetime,
	[dt_disponivel_faturamento]  datetime,
	[mensagem_falha_faturamento] nvarchar(max),
	[portal]                     int,
	[order_id]                   nvarchar(40),
	[fulfillment_id]             nvarchar(40),
	[id_tipo_b2c]                int,
	[ecommerce_origem]           nvarchar(200),
	[marketplace_loja]           nvarchar(60),
	[cnpj_origem_venda]          nvarchar(14),
	[identificador]              uniqueidentifier,
constraint [PK_LinxB2CPedidos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_pedido] asc
));

/*LinxB2CPedidosItens: os itens dos pedidos de B2C*/

create table [untreated].LinxB2CPedidosItens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_pedido_item]           int,
	[id_pedido]                int,
	[codigoproduto]            bigint,
	[quantidade]               int,
	[vl_unitario]              decimal(28,8),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxB2CPedidosItens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_pedido_item]           int not null,
	[id_pedido]                int,
	[codigoproduto]            bigint,
	[quantidade]               int,
	[vl_unitario]              decimal(28,8),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxB2CPedidosItens] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_pedido_item] asc
));

/*LinxB2CPedidosStatus: o status dos pedidos de B2C*/

create table [untreated].LinxB2CPedidosStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id]                       int,
	[id_status]                int,
	[id_pedido]                int,
	[data_hora]                datetime,
	[anotacao]                 nvarchar(80),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxB2CPedidosStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id]                       int not null,
	[id_status]                int,
	[id_pedido]                int,
	[data_hora]                datetime,
	[anotacao]                 nvarchar(80),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxB2CPedidosStatus] primary key (
	[doc_company] asc,
	[portal] asc,
	[id] asc
));

/*LinxB2CStatus: o cadastro dos status de B2C Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxB2CStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_status]                int,
	[descricao_status]         nvarchar(30),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxB2CStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_status]                int not null,
	[descricao_status]         nvarchar(30),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxB2CStatus] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_status] asc
));
