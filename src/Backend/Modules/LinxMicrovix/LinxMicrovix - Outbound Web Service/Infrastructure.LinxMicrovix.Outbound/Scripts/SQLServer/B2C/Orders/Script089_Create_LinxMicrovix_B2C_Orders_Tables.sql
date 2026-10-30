/****** Pedidos e notas do B2C (LinxMicrovixB2COrdersService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da V64; respostas planas: uma tabela por método, sem filhas

/*B2CConsultaPedidos: */

create table [untreated].B2CConsultaPedidos (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[id_pedido]                  int,
	[dt_pedido]                  datetime, --fora da documentação
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
	[dt_insert]                  datetime, --fora da documentação
	[dt_disponivel_faturamento]  datetime, --fora da documentação
	[mensagem_falha_faturamento] nvarchar(max),
	[portal]                     int,
	[order_id]                   nvarchar(40),
	[fulfillment_id]             nvarchar(40),
	[id_tipo_b2c]                int,
	[ecommerce_origem]           nvarchar(200),
	[id_endereco_entrega]        int,
	[marketplace_loja]           nvarchar(50), --fora da documentação
	[desconto]                   decimal(19,4),
	[order_id_alternativo]       nvarchar(50), --fora da documentação
	[cnpj_origem_venda]          nvarchar(1000), --fora da documentação
	[data_previsao_entrega]      datetime,
	[marketplace_full]           bit
);

create table [linxmicrovix].B2CConsultaPedidos (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[id_pedido]                  int not null,
	[dt_pedido]                  datetime, --fora da documentação
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
	[dt_insert]                  datetime, --fora da documentação
	[dt_disponivel_faturamento]  datetime, --fora da documentação
	[mensagem_falha_faturamento] nvarchar(max),
	[portal]                     int not null,
	[order_id]                   nvarchar(40),
	[fulfillment_id]             nvarchar(40),
	[id_tipo_b2c]                int,
	[ecommerce_origem]           nvarchar(200),
	[id_endereco_entrega]        int,
	[marketplace_loja]           nvarchar(50), --fora da documentação
	[desconto]                   decimal(19,4),
	[order_id_alternativo]       nvarchar(50), --fora da documentação
	[cnpj_origem_venda]          nvarchar(1000), --fora da documentação
	[data_previsao_entrega]      datetime,
	[marketplace_full]           bit,
constraint [PK_B2CConsultaPedidos] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_pedido] asc
));

/*B2CConsultaPedidosItens: */

create table [untreated].B2CConsultaPedidosItens (
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

create table [linxmicrovix].B2CConsultaPedidosItens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_pedido_item]           int not null,
	[id_pedido]                int,
	[codigoproduto]            bigint,
	[quantidade]               int,
	[vl_unitario]              decimal(28,8),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaPedidosItens] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_pedido_item] asc
));

/*B2CConsultaPedidosPlanos: */

create table [untreated].B2CConsultaPedidosPlanos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_pedido_planos]         bigint,
	[id_pedido]                int,
	[plano_pagamento]          int,
	[valor_plano]              decimal(19,4),
	[nsu_sitef]                nvarchar(20),
	[cod_autorizacao]          nvarchar(50),
	[texto_comprovante]        nvarchar(max),
	[cod_loja_sitef]           nvarchar(10),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaPedidosPlanos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_pedido_planos]         bigint not null,
	[id_pedido]                int,
	[plano_pagamento]          int,
	[valor_plano]              decimal(19,4),
	[nsu_sitef]                nvarchar(20),
	[cod_autorizacao]          nvarchar(50),
	[texto_comprovante]        nvarchar(max),
	[cod_loja_sitef]           nvarchar(10),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaPedidosPlanos] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_pedido_planos] asc
));

/*B2CConsultaPedidosStatus: */

create table [untreated].B2CConsultaPedidosStatus (
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

create table [linxmicrovix].B2CConsultaPedidosStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id]                       int not null,
	[id_status]                int,
	[id_pedido]                int,
	[data_hora]                datetime,
	[anotacao]                 nvarchar(80),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaPedidosStatus] primary key (
	[doc_company] asc,
	[portal] asc,
	[id] asc
));

/*B2CConsultaPedidosIdentificador: os registro da tabela B2C_vendas*/

create table [untreated].B2CConsultaPedidosIdentificador (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[empresa]                  int,
	[identificador]            uniqueidentifier,
	[id_venda]                 int,
	[order_id]                 nvarchar(40),
	[id_cliente]               int,
	[valor_frete]              decimal(19,4),
	[data_origem]              datetime,
	[timestamp]                bigint
);

create table [linxmicrovix].B2CConsultaPedidosIdentificador (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[empresa]                  int,
	[identificador]            uniqueidentifier not null,
	[id_venda]                 int,
	[order_id]                 nvarchar(40),
	[id_cliente]               int,
	[valor_frete]              decimal(19,4),
	[data_origem]              datetime,
	[timestamp]                bigint,
constraint [PK_B2CConsultaPedidosIdentificador] primary key (
	[doc_company] asc,
	[portal] asc,
	[identificador] asc
));

/*B2CConsultaPedidosTipos:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaPedidosTipos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_tipo_b2c]              int,
	[descricao]                nvarchar(200),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaPedidosTipos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_tipo_b2c]              int not null,
	[descricao]                nvarchar(200),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaPedidosTipos] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_tipo_b2c] asc
));

/*B2CConsultaStatus:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_status]                int,
	[descricao_status]         nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaStatus (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_status]                int not null,
	[descricao_status]         nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaStatus] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_status] asc
));

/*B2CConsultaCodigoRastreio: */

create table [untreated].B2CConsultaCodigoRastreio (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_pedido]                int,
	[documento]                int,
	[serie]                    nvarchar(10),
	[codigo_rastreio]          nvarchar(20),
	[sequencia_volume]         nvarchar(20),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaCodigoRastreio (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_pedido]                int not null,
	[documento]                int,
	[serie]                    nvarchar(10),
	[codigo_rastreio]          nvarchar(20) not null,
	[sequencia_volume]         nvarchar(20),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaCodigoRastreio] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_pedido] asc,
	[codigo_rastreio] asc
));

/*B2CConsultaNFe:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaNFe (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id_nfe]                   int,
	[id_pedido]                int,
	[documento]                int,
	[data_emissao]             datetime,
	[chave_nfe]                nvarchar(44),
	[situacao]                 int,
	[xml]                      nvarchar(max),
	[excluido]                 bit,
	[identificador_microvix]   nvarchar(1000), --fora da documentação
	[dt_insert]                datetime,
	[valor_nota]               decimal(19,4),
	[serie]                    nvarchar(10),
	[frete]                    decimal(19,4),
	[timestamp]                bigint,
	[portal]                   int,
	[nProt]                    nvarchar(15),
	[codigo_modelo_nf]         nvarchar(3),
	[justificativa]            nvarchar(255),
	[tpAmb]                    int
);

create table [linxmicrovix].B2CConsultaNFe (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id_nfe]                   int not null,
	[id_pedido]                int,
	[documento]                int,
	[data_emissao]             datetime,
	[chave_nfe]                nvarchar(44),
	[situacao]                 int,
	[xml]                      nvarchar(max),
	[excluido]                 bit,
	[identificador_microvix]   nvarchar(1000), --fora da documentação
	[dt_insert]                datetime,
	[valor_nota]               decimal(19,4),
	[serie]                    nvarchar(10),
	[frete]                    decimal(19,4),
	[timestamp]                bigint,
	[portal]                   int,
	[nProt]                    nvarchar(15),
	[codigo_modelo_nf]         nvarchar(3),
	[justificativa]            nvarchar(255),
	[tpAmb]                    int,
constraint [PK_B2CConsultaNFe] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_nfe] asc
));

/*B2CConsultaNFeSituacao:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaNFeSituacao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_nfe_situacao]          int,
	[descricao]                nvarchar(30),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaNFeSituacao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_nfe_situacao]          int not null,
	[descricao]                nvarchar(30),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaNFeSituacao] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_nfe_situacao] asc
));
