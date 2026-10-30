/****** Financeiro (LinxMicrovixFinancialService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxFaturas: as faturas a receber ou a pagar de uma loja*/

create table [untreated].LinxFaturas (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14),
	[cnpj_emp]                      nvarchar(14), --loja consultada
	[portal]                        int,
	[codigo_fatura]                 bigint,
	[data_emissao]                  datetime,
	[cod_cliente]                   int,
	[nome_cliente]                  nvarchar(60),
	[data_vencimento]               datetime,
	[data_baixa]                    datetime,
	[valor_fatura]                  decimal(19,4),
	[valor_pago]                    decimal(19,4),
	[valor_desconto]                decimal(19,4),
	[valor_juros]                   decimal(19,4),
	[documento]                     int,
	[serie]                         nvarchar(10),
	[ecf]                           int,
	[observacao]                    nvarchar(max),
	[qtde_parcelas]                 int,
	[ordem_parcela]                 int,
	[receber_pagar]                 nvarchar(1),
	[vendedor]                      int,
	[excluido]                      nvarchar(1),
	[cancelado]                     nvarchar(1),
	[identificador]                 uniqueidentifier,
	[NSU]                           nvarchar(50),
	[cod_autorizacao]               nvarchar(50),
	[documento_sem_tef]             nvarchar(350),
	[autorizacao_sem_tef]           nvarchar(30),
	[plano]                         int,
	[conta_credito]                 bigint,
	[conta_debito]                  bigint,
	[conta_fluxo]                   bigint,
	[cod_historico]                 bigint,
	[forma_pgto]                    nvarchar(50),
	[ordem_cartao]                  int,
	[banco_codigo]                  nvarchar(10),
	[banco_agencia]                 nvarchar(30),
	[banco_conta]                   nvarchar(30),
	[banco_autorizacao_garantidora] nvarchar(30),
	[numero_bilhete_seguro]         nvarchar(30),
	[timestamp]                     bigint,
	[empresa]                       int,
	[id_categorias_financeiras]     int,
	[taxa_financeira]               decimal(19,4),
	[valor_abatimento]              decimal(28,8),
	[valor_multa]                   decimal(19,4),
	[centrocusto]                   int,
	[perc_taxa_adquirente]          decimal(28,8),
	[fatura_origem_importacao_erp]  nvarchar(50),
	[cod_portador]                  int,
	[nsu_host]                      nvarchar(50),
	[nsu_sitef]                     nvarchar(50),
	[previsao]                      nvarchar(1),
	[data_lanc]                     datetime,
	[dt_insert]                     datetime,
	[plano_pagamento_baixa]         int
);

create table [linxmicrovix].LinxFaturas (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14) not null,
	[cnpj_emp]                      nvarchar(14) not null, --loja consultada
	[portal]                        int,
	[codigo_fatura]                 bigint not null,
	[data_emissao]                  datetime,
	[cod_cliente]                   int,
	[nome_cliente]                  nvarchar(60),
	[data_vencimento]               datetime,
	[data_baixa]                    datetime,
	[valor_fatura]                  decimal(19,4),
	[valor_pago]                    decimal(19,4),
	[valor_desconto]                decimal(19,4),
	[valor_juros]                   decimal(19,4),
	[documento]                     int,
	[serie]                         nvarchar(10),
	[ecf]                           int,
	[observacao]                    nvarchar(max),
	[qtde_parcelas]                 int,
	[ordem_parcela]                 int not null,
	[receber_pagar]                 nvarchar(1),
	[vendedor]                      int,
	[excluido]                      nvarchar(1),
	[cancelado]                     nvarchar(1),
	[identificador]                 uniqueidentifier,
	[NSU]                           nvarchar(50),
	[cod_autorizacao]               nvarchar(50),
	[documento_sem_tef]             nvarchar(350),
	[autorizacao_sem_tef]           nvarchar(30),
	[plano]                         int,
	[conta_credito]                 bigint,
	[conta_debito]                  bigint,
	[conta_fluxo]                   bigint,
	[cod_historico]                 bigint,
	[forma_pgto]                    nvarchar(50),
	[ordem_cartao]                  int,
	[banco_codigo]                  nvarchar(10),
	[banco_agencia]                 nvarchar(30),
	[banco_conta]                   nvarchar(30),
	[banco_autorizacao_garantidora] nvarchar(30),
	[numero_bilhete_seguro]         nvarchar(30),
	[timestamp]                     bigint,
	[empresa]                       int,
	[id_categorias_financeiras]     int,
	[taxa_financeira]               decimal(19,4),
	[valor_abatimento]              decimal(28,8),
	[valor_multa]                   decimal(19,4),
	[centrocusto]                   int,
	[perc_taxa_adquirente]          decimal(28,8),
	[fatura_origem_importacao_erp]  nvarchar(50),
	[cod_portador]                  int,
	[nsu_host]                      nvarchar(50),
	[nsu_sitef]                     nvarchar(50),
	[previsao]                      nvarchar(1),
	[data_lanc]                     datetime,
	[dt_insert]                     datetime,
	[plano_pagamento_baixa]         int,
constraint [PK_LinxFaturas] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[codigo_fatura] asc,
	[ordem_parcela] asc
));

/*LinxFaturasOrigem: as faturas de origem Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxFaturasOrigem (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[codigo_fatura]            bigint,
	[codigo_fatura_origem]     bigint,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxFaturasOrigem (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[codigo_fatura]            bigint not null,
	[codigo_fatura_origem]     bigint not null,
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_LinxFaturasOrigem] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[codigo_fatura] asc,
	[codigo_fatura_origem] asc
));

/*LinxPortadores: os dados dos portadores. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxPortadores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cod_portador]             int,
	[nome_portador]            nvarchar(50),
	[taxa_acrescimo]           decimal(19,4),
	[aviso_vencimento]         bit,
	[reaviso_vencimento]       bit,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxPortadores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cod_portador]             int not null,
	[nome_portador]            nvarchar(50),
	[taxa_acrescimo]           decimal(19,4),
	[aviso_vencimento]         bit,
	[reaviso_vencimento]       bit,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxPortadores] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_portador] asc
));

/*LinxAntecipacoesFinanceiras: informações das antecipações financeiras do portal Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxAntecipacoesFinanceiras (
	[integrated_or_updated_at]             datetime,
	[doc_company]                          nvarchar(14),
	[cnpj_emp]                             nvarchar(14), --loja consultada
	[portal]                               int,
	[empresa]                              int,
	[cod_cliente]                          int,
	[numero_antecipacao]                   int,
	[data_antecipacao]                     datetime,
	[previsao_entrega]                     datetime,
	[dav_pv]                               nvarchar(3),
	[numero_origem]                        int,
	[dav_remessa]                          int,
	[codigoproduto]                        bigint,
	[quantidade]                           decimal(19,4),
	[preco_unitario_produto]               decimal(19,4),
	[valor_pago_antecipacao]               decimal(19,4),
	[entregue]                             nvarchar(1),
	[identificador]                        uniqueidentifier,
	[timestamp]                            bigint,
	[cancelado]                            bit,
	[id_antecipacoes_financeiras]          int,
	[id_antecipacoes_financeiras_detalhes] int,
	[id_vendas_pos_produtos]               int,
	[valor_pagar]                          decimal(19,4),
	[identificador_faturamento_antecipado] uniqueidentifier
);

create table [linxmicrovix].LinxAntecipacoesFinanceiras (
	[integrated_or_updated_at]             datetime,
	[doc_company]                          nvarchar(14) not null,
	[cnpj_emp]                             nvarchar(14) not null, --loja consultada
	[portal]                               int,
	[empresa]                              int,
	[cod_cliente]                          int,
	[numero_antecipacao]                   int not null,
	[data_antecipacao]                     datetime,
	[previsao_entrega]                     datetime,
	[dav_pv]                               nvarchar(3),
	[numero_origem]                        int,
	[dav_remessa]                          int,
	[codigoproduto]                        bigint not null,
	[quantidade]                           decimal(19,4),
	[preco_unitario_produto]               decimal(19,4),
	[valor_pago_antecipacao]               decimal(19,4),
	[entregue]                             nvarchar(1),
	[identificador]                        uniqueidentifier,
	[timestamp]                            bigint,
	[cancelado]                            bit,
	[id_antecipacoes_financeiras]          int,
	[id_antecipacoes_financeiras_detalhes] int,
	[id_vendas_pos_produtos]               int,
	[valor_pagar]                          decimal(19,4),
	[identificador_faturamento_antecipado] uniqueidentifier,
constraint [PK_LinxAntecipacoesFinanceiras] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[numero_antecipacao] asc,
	[codigoproduto] asc
));

/*LinxAntecipacoesFinanceirasPlanos: os dados da tabela antecipacoes_financeiras  e antecipacoes_financeiras_planos no portal. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxAntecipacoesFinanceirasPlanos (
	[integrated_or_updated_at]       datetime,
	[doc_company]                    nvarchar(14),
	[cnpj_emp]                       nvarchar(14), --loja consultada
	[portal]                         int,
	[empresa]                        int,
	[id_antecipacoes_financeiras]    int,
	[numero_antecipacao]             int,
	[forma_pgto]                     nvarchar(50),
	[plano]                          int,
	[nome_plano]                     nvarchar(100),
	[valor]                          decimal(19,4),
	[cancelado]                      bit,
	[timestamp]                      bigint,
	[data_criacao_antecipacao_plano] datetime
);

create table [linxmicrovix].LinxAntecipacoesFinanceirasPlanos (
	[integrated_or_updated_at]       datetime,
	[doc_company]                    nvarchar(14) not null,
	[cnpj_emp]                       nvarchar(14) not null, --loja consultada
	[portal]                         int,
	[empresa]                        int,
	[id_antecipacoes_financeiras]    int not null,
	[numero_antecipacao]             int,
	[forma_pgto]                     nvarchar(50) not null,
	[plano]                          int not null,
	[nome_plano]                     nvarchar(100),
	[valor]                          decimal(19,4),
	[cancelado]                      bit,
	[timestamp]                      bigint,
	[data_criacao_antecipacao_plano] datetime,
constraint [PK_LinxAntecipacoesFinanceirasPlanos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_antecipacoes_financeiras] asc,
	[plano] asc,
	[forma_pgto] asc
));

/*LinxDesfazimentosAntecipacoes: os valores registrados na tabela desfazimentos_antecipacoes Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxDesfazimentosAntecipacoes (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14),
	[cnpj_emp]                      nvarchar(14), --loja consultada
	[portal]                        int,
	[id_desfazimentos_antecipacoes] int,
	[id_antecipacoes_financeiras]   int,
	[data_desfazimento]             datetime,
	[valor_desfazimento]            nvarchar(1000), --fora da documentação
	[valor_acerto]                  decimal(19,4),
	[tipo_acerto]                   int,
	[observacoes]                   nvarchar(max),
	[usuario]                       int,
	[timestamp]                     bigint
);

create table [linxmicrovix].LinxDesfazimentosAntecipacoes (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14) not null,
	[cnpj_emp]                      nvarchar(14) not null, --loja consultada
	[portal]                        int,
	[id_desfazimentos_antecipacoes] int not null,
	[id_antecipacoes_financeiras]   int,
	[data_desfazimento]             datetime,
	[valor_desfazimento]            nvarchar(1000), --fora da documentação
	[valor_acerto]                  decimal(19,4),
	[tipo_acerto]                   int,
	[observacoes]                   nvarchar(max),
	[usuario]                       int,
	[timestamp]                     bigint,
constraint [PK_LinxDesfazimentosAntecipacoes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_desfazimentos_antecipacoes] asc
));

/*LinxDesfazimentosAntecipacoesDocumento: os valores registrados na tabela desfazimentos_antecipacoes_documento Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxDesfazimentosAntecipacoesDocumento (
	[integrated_or_updated_at]         datetime,
	[doc_company]                      nvarchar(14),
	[cnpj_emp]                         nvarchar(14), --loja consultada
	[portal]                           int,
	[id_documento_desfazimento_optico] int,
	[empresa]                          int,
	[id_desfazimentos_antecipacoes]    nvarchar(1000), --fora da documentação
	[id_antecipacoes_financeiras]      int,
	[cfop]                             nvarchar(10),
	[cod_cliente]                      int,
	[valor_operacao_nao_desfeita]      decimal(19,4),
	[texto_descricao_desfazimento]     nvarchar(max),
	[timestamp]                        bigint
);

create table [linxmicrovix].LinxDesfazimentosAntecipacoesDocumento (
	[integrated_or_updated_at]         datetime,
	[doc_company]                      nvarchar(14) not null,
	[cnpj_emp]                         nvarchar(14) not null, --loja consultada
	[portal]                           int,
	[id_documento_desfazimento_optico] int not null,
	[empresa]                          int,
	[id_desfazimentos_antecipacoes]    nvarchar(1000), --fora da documentação
	[id_antecipacoes_financeiras]      int,
	[cfop]                             nvarchar(10),
	[cod_cliente]                      int,
	[valor_operacao_nao_desfeita]      decimal(19,4),
	[texto_descricao_desfazimento]     nvarchar(max),
	[timestamp]                        bigint,
constraint [PK_LinxDesfazimentosAntecipacoesDocumento] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_documento_desfazimento_optico] asc
));

/*LinxDesfazimentosAntecipacoesItens: os valores registrados na tabela desfazimentos_antecipacoes_itens Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxDesfazimentosAntecipacoesItens (
	[integrated_or_updated_at]            datetime,
	[doc_company]                         nvarchar(14),
	[cnpj_emp]                            nvarchar(14), --loja consultada
	[portal]                              int,
	[id_desfazimentos_antecipacoes_itens] int,
	[id_desfazimentos_antecipacoes]       nvarchar(1000), --fora da documentação
	[numero_sequencial]                   nvarchar(1000), --fora da documentação
	[ordem]                               int,
	[identificador]                       uniqueidentifier,
	[preco]                               decimal(19,4),
	[quantidade]                          decimal(19,4),
	[desconto]                            decimal(19,4),
	[valor]                               decimal(19,4),
	[timestamp]                           bigint
);

create table [linxmicrovix].LinxDesfazimentosAntecipacoesItens (
	[integrated_or_updated_at]            datetime,
	[doc_company]                         nvarchar(14) not null,
	[cnpj_emp]                            nvarchar(14) not null, --loja consultada
	[portal]                              int,
	[id_desfazimentos_antecipacoes_itens] int not null,
	[id_desfazimentos_antecipacoes]       nvarchar(1000), --fora da documentação
	[numero_sequencial]                   nvarchar(1000), --fora da documentação
	[ordem]                               int,
	[identificador]                       uniqueidentifier,
	[preco]                               decimal(19,4),
	[quantidade]                          decimal(19,4),
	[desconto]                            decimal(19,4),
	[valor]                               decimal(19,4),
	[timestamp]                           bigint,
constraint [PK_LinxDesfazimentosAntecipacoesItens] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_desfazimentos_antecipacoes_itens] asc
));

/*LinxCategoriasFinanceiras: categoria financeira*/

create table [untreated].LinxCategoriasFinanceiras (
	[integrated_or_updated_at]  datetime,
	[doc_company]               nvarchar(14),
	[id_categorias_financeiras] int,
	[descricao]                 nvarchar(50),
	[ativo]                     bit,
	[historico]                 bigint,
	[timestamp]                 bigint,
	[portal]                    int
);

create table [linxmicrovix].LinxCategoriasFinanceiras (
	[integrated_or_updated_at]  datetime,
	[doc_company]               nvarchar(14) not null,
	[id_categorias_financeiras] int not null,
	[descricao]                 nvarchar(50),
	[ativo]                     bit,
	[historico]                 bigint,
	[timestamp]                 bigint,
	[portal]                    int not null,
constraint [PK_LinxCategoriasFinanceiras] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_categorias_financeiras] asc
));

/*LinxCentroCusto: as informações dos centros de custos cadastrados no portal*/

create table [untreated].LinxCentroCusto (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[CNPJ]                     nvarchar(14),
	[id_centrocusto]           int,
	[nome_centrocusto]         nvarchar(50),
	[ativo]                    bit,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxCentroCusto (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int not null,
	[CNPJ]                     nvarchar(14),
	[id_centrocusto]           int not null,
	[nome_centrocusto]         nvarchar(50),
	[ativo]                    bit,
	[timestamp]                bigint,
constraint [PK_LinxCentroCusto] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[empresa] asc,
	[id_centrocusto] asc
));

/*LinxPlanoContas: as informações do plano de contas cadastrado no portal*/

create table [untreated].LinxPlanoContas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[CNPJ]                     nvarchar(14),
	[id_conta]                 int,
	[nome_conta]               nvarchar(50),
	[sintetica]                nvarchar(1),
	[indice]                   nvarchar(150),
	[ativa]                    nvarchar(1),
	[fluxo_caixa]              nvarchar(1),
	[CONTA_CONTABIL]           nvarchar(20),
	[id_natureza_conta]        int,
	[conta_bancaria]           bit,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxPlanoContas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int not null,
	[CNPJ]                     nvarchar(14),
	[id_conta]                 int not null,
	[nome_conta]               nvarchar(50),
	[sintetica]                nvarchar(1),
	[indice]                   nvarchar(150),
	[ativa]                    nvarchar(1),
	[fluxo_caixa]              nvarchar(1),
	[CONTA_CONTABIL]           nvarchar(20),
	[id_natureza_conta]        int,
	[conta_bancaria]           bit,
	[timestamp]                bigint,
constraint [PK_LinxPlanoContas] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[empresa] asc,
	[id_conta] asc
));

/*LinxLancContabil: informações dos Lançamentos Contábeis por conta e empresa Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxLancContabil (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_lanc]                 bigint,
	[centro_custo]             nvarchar(50),
	[ind_conta]                nvarchar(150),
	[cod_conta]                bigint,
	[nome_conta]               nvarchar(50),
	[valor_conta]              decimal(19,4),
	[cred_deb]                 nvarchar(1),
	[data_lanc]                datetime,
	[compl_conta]              nvarchar(500),
	[identificador]            uniqueidentifier,
	[cod_historico]            bigint,
	[desc_historico]           nvarchar(50),
	[data_compensacao]         datetime,
	[fatura_origem]            int,
	[efetivado]                nvarchar(1),
	[timestamp]                bigint,
	[empresa]                  int,
	[id_lanc]                  bigint,
	[cancelado]                nvarchar(1)
);

create table [linxmicrovix].LinxLancContabil (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_lanc]                 bigint,
	[centro_custo]             nvarchar(50),
	[ind_conta]                nvarchar(150),
	[cod_conta]                bigint,
	[nome_conta]               nvarchar(50),
	[valor_conta]              decimal(19,4),
	[cred_deb]                 nvarchar(1),
	[data_lanc]                datetime,
	[compl_conta]              nvarchar(500),
	[identificador]            uniqueidentifier,
	[cod_historico]            bigint,
	[desc_historico]           nvarchar(50),
	[data_compensacao]         datetime,
	[fatura_origem]            int,
	[efetivado]                nvarchar(1),
	[timestamp]                bigint,
	[empresa]                  int,
	[id_lanc]                  bigint not null,
	[cancelado]                nvarchar(1),
constraint [PK_LinxLancContabil] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_lanc] asc
));

/*LinxHistoricosContabeis: os dados dos históricos contábeis.*/

create table [untreated].LinxHistoricosContabeis (
	[integrated_or_updated_at]           datetime,
	[doc_company]                        nvarchar(14),
	[id_historico]                       bigint,
	[nome_historico]                     nvarchar(50),
	[conta_credito]                      bigint,
	[conta_debito]                       bigint,
	[tipo_rateio]                        nvarchar(1),
	[inativo]                            nvarchar(1),
	[operacao]                           nvarchar(1),
	[historico_contabil]                 int,
	[somente_gerencial]                  bit,
	[obrigatorio_insercao_anexo_sangria] bit,
	[timestamp]                          bigint,
	[portal]                             int
);

create table [linxmicrovix].LinxHistoricosContabeis (
	[integrated_or_updated_at]           datetime,
	[doc_company]                        nvarchar(14) not null,
	[id_historico]                       bigint not null,
	[nome_historico]                     nvarchar(50),
	[conta_credito]                      bigint,
	[conta_debito]                       bigint,
	[tipo_rateio]                        nvarchar(1),
	[inativo]                            nvarchar(1),
	[operacao]                           nvarchar(1),
	[historico_contabil]                 int,
	[somente_gerencial]                  bit,
	[obrigatorio_insercao_anexo_sangria] bit,
	[timestamp]                          bigint,
	[portal]                             int not null,
constraint [PK_LinxHistoricosContabeis] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_historico] asc
));

/*LinxExtratoBancarioLancamentos: os registros da tabela extrato_bancario_lancamentos.*/

create table [untreated].LinxExtratoBancarioLancamentos (
	[integrated_or_updated_at]        datetime,
	[doc_company]                     nvarchar(14),
	[portal]                          int,
	[id_extrato_bancario_lancamentos] int,
	[codigo_banco]                    nvarchar(4),
	[numero_conta]                    nvarchar(20),
	[id_conta]                        bigint,
	[valor]                           decimal(19,4),
	[descricao]                       nvarchar(100),
	[data_lancamento]                 datetime,
	[timestamp]                       bigint
);

create table [linxmicrovix].LinxExtratoBancarioLancamentos (
	[integrated_or_updated_at]        datetime,
	[doc_company]                     nvarchar(14) not null,
	[portal]                          int not null,
	[id_extrato_bancario_lancamentos] int not null,
	[codigo_banco]                    nvarchar(4),
	[numero_conta]                    nvarchar(20),
	[id_conta]                        bigint,
	[valor]                           decimal(19,4),
	[descricao]                       nvarchar(100),
	[data_lancamento]                 datetime,
	[timestamp]                       bigint,
constraint [PK_LinxExtratoBancarioLancamentos] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_extrato_bancario_lancamentos] asc
));

/*LinxExtratoBancarioConciliacoes: os registros da tabela extrato_bancario_conciliacoes.*/

create table [untreated].LinxExtratoBancarioConciliacoes (
	[integrated_or_updated_at]         datetime,
	[doc_company]                      nvarchar(14),
	[portal]                           int,
	[id_extrato_bancario_conciliacoes] int,
	[id_extrato_bancario_lancamentos]  int,
	[codigo_fatura]                    bigint,
	[dados_anteriores_baixa_fatura]    nvarchar(max),
	[timestamp]                        bigint
);

create table [linxmicrovix].LinxExtratoBancarioConciliacoes (
	[integrated_or_updated_at]         datetime,
	[doc_company]                      nvarchar(14) not null,
	[portal]                           int not null,
	[id_extrato_bancario_conciliacoes] int not null,
	[id_extrato_bancario_lancamentos]  int,
	[codigo_fatura]                    bigint,
	[dados_anteriores_baixa_fatura]    nvarchar(max),
	[timestamp]                        bigint,
constraint [PK_LinxExtratoBancarioConciliacoes] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_extrato_bancario_conciliacoes] asc
));
