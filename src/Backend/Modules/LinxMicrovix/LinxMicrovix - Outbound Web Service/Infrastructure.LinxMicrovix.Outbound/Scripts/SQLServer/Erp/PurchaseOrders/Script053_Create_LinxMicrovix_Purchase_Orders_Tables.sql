-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; docs/MIGRATOR-CSX.md)
/****** Pedidos de compra (LinxMicrovixPurchaseOrdersService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxPedidosCompra: informações de pedidos de compra de acordo com a loja pesquisada*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxPedidosCompra') = 1
create table [untreated].LinxPedidosCompra (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_pedido]               int,
	[data_pedido]              datetime,
	[transacao]                int,
	[usuario]                  int,
	[codigo_fornecedor]        int,
	[cod_produto]              bigint,
	[quantidade]               decimal(28,8),
	[valor_unitario]           decimal(19,4),
	[cod_comprador]            int,
	[valor_frete]              decimal(19,4),
	[valor_total]              decimal(19,4),
	[cod_plano_pagamento]      int,
	[plano_pagamento]          nvarchar(100),
	[obs]                      nvarchar(max),
	[aprovado]                 nvarchar(1),
	[cancelado]                nvarchar(1),
	[encerrado]                nvarchar(1),
	[data_aprovacao]           datetime,
	[numero_ped_fornec]        nvarchar(15),
	[tipo_frete]               nvarchar(1),
	[natureza_operacao]        nvarchar(73),
	[previsao_entrega]         datetime,
	[numero_projeto_officina]  nvarchar(50),
	[status_pedido]            nvarchar(1),
	[qtde_entregue]            decimal(28,8),
	[descricao_frete]          nvarchar(40),
	[integrado_linx]           bit,
	[nf_gerada]                int,
	[timestamp]                bigint,
	[empresa]                  int,
	[nf_origem_ws]             nvarchar(30),
	[id_compras_pedido_status] int,
	[qtde_desejada]            decimal(28,8),
	[motivo_ajuste]            nvarchar(500),
	[saldo_disponivel_api]     decimal(28,8)
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxPedidosCompra') = 1
create table [linxmicrovix].LinxPedidosCompra (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_pedido]               int,
	[data_pedido]              datetime,
	[transacao]                int not null,
	[usuario]                  int,
	[codigo_fornecedor]        int,
	[cod_produto]              bigint,
	[quantidade]               decimal(28,8),
	[valor_unitario]           decimal(19,4),
	[cod_comprador]            int,
	[valor_frete]              decimal(19,4),
	[valor_total]              decimal(19,4),
	[cod_plano_pagamento]      int,
	[plano_pagamento]          nvarchar(100),
	[obs]                      nvarchar(max),
	[aprovado]                 nvarchar(1),
	[cancelado]                nvarchar(1),
	[encerrado]                nvarchar(1),
	[data_aprovacao]           datetime,
	[numero_ped_fornec]        nvarchar(15),
	[tipo_frete]               nvarchar(1),
	[natureza_operacao]        nvarchar(73),
	[previsao_entrega]         datetime,
	[numero_projeto_officina]  nvarchar(50),
	[status_pedido]            nvarchar(1),
	[qtde_entregue]            decimal(28,8),
	[descricao_frete]          nvarchar(40),
	[integrado_linx]           bit,
	[nf_gerada]                int,
	[timestamp]                bigint,
	[empresa]                  int,
	[nf_origem_ws]             nvarchar(30),
	[id_compras_pedido_status] int,
	[qtde_desejada]            decimal(28,8),
	[motivo_ajuste]            nvarchar(500),
	[saldo_disponivel_api]     decimal(28,8),
constraint [PK_LinxPedidosCompra] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[transacao] asc
));

/*LinxPedidosCompraStatus: os status de pedidos de compra. Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxPedidosCompraStatus') = 1
create table [untreated].LinxPedidosCompraStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_compras_pedido_status] int,
	[descricao]                nvarchar(50),
	[ativo]                    bit,
	[timestamp]                bigint,
	[portal]                   int
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxPedidosCompraStatus') = 1
create table [linxmicrovix].LinxPedidosCompraStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_compras_pedido_status] int not null,
	[descricao]                nvarchar(50),
	[ativo]                    bit,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxPedidosCompraStatus] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_compras_pedido_status] asc
));

/*LinxPedidosCompraNumeroFornecedor: o numero do fornecedor do pedido de compra.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxPedidosCompraNumeroFornecedor') = 1
create table [untreated].LinxPedidosCompraNumeroFornecedor (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_pedido_fornecedor]     int,
	[numero_ped_erp]           int,
	[numero_ped_fornecedor]    nvarchar(20),
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxPedidosCompraNumeroFornecedor') = 1
create table [linxmicrovix].LinxPedidosCompraNumeroFornecedor (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_pedido_fornecedor]     int not null,
	[numero_ped_erp]           int,
	[numero_ped_fornecedor]    nvarchar(20),
	[timestamp]                bigint,
constraint [PK_LinxPedidosCompraNumeroFornecedor] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_pedido_fornecedor] asc
));
