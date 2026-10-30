/****** Vendas POS temporárias (LinxMicrovixPosSalesService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxVendasPosTmp: os registros da tabela vendas_pos_tmp*/

create table [untreated].LinxVendasPosTmp (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_vendas_pos]            bigint,
	[numero_ficha]             bigint,
	[numero_sequencial]        int,
	[cod_cliente]              int,
	[data_lcto]                datetime,
	[empresa]                  int,
	[cliente_indicador]        int,
	[observacao_dav]           nvarchar(4000),
	[observacao]               nvarchar(500),
	[codigo_sistema_terceiro]  int,
	[cancelado]                bit,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxVendasPosTmp (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_vendas_pos]            bigint not null,
	[numero_ficha]             bigint,
	[numero_sequencial]        int,
	[cod_cliente]              int,
	[data_lcto]                datetime,
	[empresa]                  int,
	[cliente_indicador]        int,
	[observacao_dav]           nvarchar(4000),
	[observacao]               nvarchar(500),
	[codigo_sistema_terceiro]  int,
	[cancelado]                bit,
	[timestamp]                bigint,
constraint [PK_LinxVendasPosTmp] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_vendas_pos] asc
));

/*LinxVendasPosProdutosTmp: os registros da tabela vendas_pos_produtos_tmp.*/

create table [untreated].LinxVendasPosProdutosTmp (
	[integrated_or_updated_at]    datetime,
	[doc_company]                 nvarchar(14),
	[cnpj_emp]                    nvarchar(14), --loja consultada
	[portal]                      int,
	[id_vendas_pos_produtos]      bigint,
	[numero_ficha]                bigint,
	[empresa]                     int,
	[codigoproduto]               bigint,
	[preco_unitario]              decimal(19,4),
	[preco_tabela]                decimal(19,4),
	[quantidade]                  decimal(19,4),
	[vendedor]                    int,
	[desconto_item]               decimal(19,4),
	[id_tabela_preco]             int,
	[altura]                      decimal(19,4),
	[comprimento]                 decimal(19,4),
	[largura]                     decimal(19,4),
	[cfop_item]                   nvarchar(10),
	[excluido]                    bit,
	[aliq_icms]                   decimal(19,4),
	[cst_fiscal]                  nvarchar(4),
	[indice_ordenacao_espelho]    int,
	[id_vendas_pos]               int,
	[id_cst_icms_fiscal]          int,
	[id_cst_ipi_fiscal]           int,
	[id_cst_pis_fiscal]           int,
	[id_cst_cofins_fiscal]        int,
	[aliq_iss]                    decimal(19,4),
	[id_cfop_fiscal]              int,
	[aliq_pis]                    decimal(19,4),
	[aliq_cofins]                 decimal(19,4),
	[desconto_item_valor]         decimal(19,4),
	[lote]                        nvarchar(60),
	[deposito]                    int,
	[previsao_entrega]            datetime,
	[transacao_movimento_reserva] int,
	[data_exclusao]               datetime,
	[desconto_total_item]         decimal(19,4),
	[id_motivo_desconto]          int,
	[motivo_desconto_observacao]  nvarchar(500),
	[crm_cliente_indicador]       nvarchar(18),
	[cod_cliente_indicador]       int,
	[timestamp]                   bigint
);

create table [linxmicrovix].LinxVendasPosProdutosTmp (
	[integrated_or_updated_at]    datetime,
	[doc_company]                 nvarchar(14) not null,
	[cnpj_emp]                    nvarchar(14) not null, --loja consultada
	[portal]                      int,
	[id_vendas_pos_produtos]      bigint not null,
	[numero_ficha]                bigint,
	[empresa]                     int,
	[codigoproduto]               bigint,
	[preco_unitario]              decimal(19,4),
	[preco_tabela]                decimal(19,4),
	[quantidade]                  decimal(19,4),
	[vendedor]                    int,
	[desconto_item]               decimal(19,4),
	[id_tabela_preco]             int,
	[altura]                      decimal(19,4),
	[comprimento]                 decimal(19,4),
	[largura]                     decimal(19,4),
	[cfop_item]                   nvarchar(10),
	[excluido]                    bit,
	[aliq_icms]                   decimal(19,4),
	[cst_fiscal]                  nvarchar(4),
	[indice_ordenacao_espelho]    int,
	[id_vendas_pos]               int,
	[id_cst_icms_fiscal]          int,
	[id_cst_ipi_fiscal]           int,
	[id_cst_pis_fiscal]           int,
	[id_cst_cofins_fiscal]        int,
	[aliq_iss]                    decimal(19,4),
	[id_cfop_fiscal]              int,
	[aliq_pis]                    decimal(19,4),
	[aliq_cofins]                 decimal(19,4),
	[desconto_item_valor]         decimal(19,4),
	[lote]                        nvarchar(60),
	[deposito]                    int,
	[previsao_entrega]            datetime,
	[transacao_movimento_reserva] int,
	[data_exclusao]               datetime,
	[desconto_total_item]         decimal(19,4),
	[id_motivo_desconto]          int,
	[motivo_desconto_observacao]  nvarchar(500),
	[crm_cliente_indicador]       nvarchar(18),
	[cod_cliente_indicador]       int,
	[timestamp]                   bigint,
constraint [PK_LinxVendasPosProdutosTmp] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_vendas_pos_produtos] asc
));

/*LinxVendasPosAgrupamentoTmp: os registros da tabela vendas_pos_agrupamento_tmp Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxVendasPosAgrupamentoTmp (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14),
	[portal]                        int,
	[id_vendas_pos_agrupamento_tmp] int,
	[id_vendas_pos]                 bigint,
	[id_vendas_pos_origem]          bigint,
	[timestamp]                     bigint
);

create table [linxmicrovix].LinxVendasPosAgrupamentoTmp (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14) not null,
	[portal]                        int not null,
	[id_vendas_pos_agrupamento_tmp] int not null,
	[id_vendas_pos]                 bigint,
	[id_vendas_pos_origem]          bigint,
	[timestamp]                     bigint,
constraint [PK_LinxVendasPosAgrupamentoTmp] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_vendas_pos_agrupamento_tmp] asc
));
