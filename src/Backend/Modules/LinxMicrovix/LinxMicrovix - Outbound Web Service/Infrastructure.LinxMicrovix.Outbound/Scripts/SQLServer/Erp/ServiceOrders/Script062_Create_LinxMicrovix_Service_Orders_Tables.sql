/****** Ordens de serviço (LinxMicrovixServiceOrdersService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxOrdensServico: as ordens de serviço Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxOrdensServico (
	[integrated_or_updated_at]                 datetime,
	[doc_company]                              nvarchar(14),
	[cnpj_emp]                                 nvarchar(14), --loja consultada
	[numero_os]                                int,
	[empresa]                                  int,
	[data_os]                                  datetime,
	[data_envio_laboratorio]                   datetime,
	[cancelado]                                nvarchar(1),
	[id_laboratorio]                           int,
	[id_posicao_os_ramo_optico]                int,
	[compartilhar_hub_laboratorios]            bit,
	[cod_cliente_os]                           int,
	[cod_vendedor]                             int,
	[numero_sequencial_antecipacao_financeira] int,
	[chave_nfe_laboratorio]                    nvarchar(44),
	[timestamp]                                bigint,
	[portal]                                   int,
	[previsao_entrega_os]                      datetime
);

create table [linxmicrovix].LinxOrdensServico (
	[integrated_or_updated_at]                 datetime,
	[doc_company]                              nvarchar(14) not null,
	[cnpj_emp]                                 nvarchar(14) not null, --loja consultada
	[numero_os]                                int not null,
	[empresa]                                  int,
	[data_os]                                  datetime,
	[data_envio_laboratorio]                   datetime,
	[cancelado]                                nvarchar(1),
	[id_laboratorio]                           int,
	[id_posicao_os_ramo_optico]                int,
	[compartilhar_hub_laboratorios]            bit,
	[cod_cliente_os]                           int,
	[cod_vendedor]                             int,
	[numero_sequencial_antecipacao_financeira] int,
	[chave_nfe_laboratorio]                    nvarchar(44),
	[timestamp]                                bigint,
	[portal]                                   int,
	[previsao_entrega_os]                      datetime,
constraint [PK_LinxOrdensServico] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[numero_os] asc
));

/*LinxOrdensServicoProdutos: os produtos das ordens de serviço Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxOrdensServicoProdutos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[numero_os]                int,
	[cod_produto_serv]         bigint,
	[id_ordservprod]           int,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxOrdensServicoProdutos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[numero_os]                int,
	[cod_produto_serv]         bigint,
	[id_ordservprod]           int not null,
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_LinxOrdensServicoProdutos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_ordservprod] asc
));

/*LinxServicos: o cadastro de serviços de acordo com a loja pesquisada ou dados da estrutura mercadológica Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxServicos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_servico]              bigint,
	[nome]                     nvarchar(1000), --fora da documentação
	[id_setor]                 int,
	[desc_setor]               nvarchar(30),
	[id_linha]                 int,
	[desc_linha]               nvarchar(30),
	[id_marca]                 int,
	[desc_marca]               nvarchar(30),
	[dt_update]                datetime,
	[operacao_servico]         nvarchar(1),
	[servico_km]               nvarchar(1),
	[desativado]               nvarchar(1),
	[cod_lc11603]              nvarchar(4),
	[codigo_nbs]               nvarchar(20),
	[dt_inclusao]              datetime,
	[timestamp]                bigint,
	[codigo_ws]                nvarchar(50)
);

create table [linxmicrovix].LinxServicos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_servico]              bigint not null,
	[nome]                     nvarchar(1000), --fora da documentação
	[id_setor]                 int,
	[desc_setor]               nvarchar(30),
	[id_linha]                 int,
	[desc_linha]               nvarchar(30),
	[id_marca]                 int,
	[desc_marca]               nvarchar(30),
	[dt_update]                datetime,
	[operacao_servico]         nvarchar(1),
	[servico_km]               nvarchar(1),
	[desativado]               nvarchar(1),
	[cod_lc11603]              nvarchar(4),
	[codigo_nbs]               nvarchar(20),
	[dt_inclusao]              datetime,
	[timestamp]                bigint,
	[codigo_ws]                nvarchar(50),
constraint [PK_LinxServicos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_servico] asc
));

/*LinxServicosDetalhes: informações dos serviços de acordo com a loja pesquisada Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxServicosDetalhes (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[cnpj_emp]                   nvarchar(14), --loja consultada
	[portal]                     int,
	[cod_servico]                bigint,
	[preco_custo_hn]             decimal(19,4),
	[preco_primera_hn]           decimal(19,4),
	[preco_demais_hn]            decimal(19,4),
	[preco_custo_he]             decimal(19,4),
	[preco_primera_he]           decimal(19,4),
	[preco_demais_he]            decimal(19,4),
	[preco_custo_had]            decimal(19,4),
	[preco_primera_had]          decimal(19,4),
	[preco_demais_had]           decimal(19,4),
	[id_config_tributaria]       int,
	[desc_config_tributaria]     nvarchar(100),
	[cod_trib_servico_municipio] nvarchar(20),
	[timestamp]                  bigint
);

create table [linxmicrovix].LinxServicosDetalhes (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[cnpj_emp]                   nvarchar(14) not null, --loja consultada
	[portal]                     int,
	[cod_servico]                bigint not null,
	[preco_custo_hn]             decimal(19,4),
	[preco_primera_hn]           decimal(19,4),
	[preco_demais_hn]            decimal(19,4),
	[preco_custo_he]             decimal(19,4),
	[preco_primera_he]           decimal(19,4),
	[preco_demais_he]            decimal(19,4),
	[preco_custo_had]            decimal(19,4),
	[preco_primera_had]          decimal(19,4),
	[preco_demais_had]           decimal(19,4),
	[id_config_tributaria]       int,
	[desc_config_tributaria]     nvarchar(100),
	[cod_trib_servico_municipio] nvarchar(20),
	[timestamp]                  bigint,
constraint [PK_LinxServicosDetalhes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_servico] asc
));

/*LinxServicoClientes: os dados dos clientes do serviço. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxServicoClientes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_cliente]               int,
	[empresa]                  int,
	[id_servico]               bigint,
	[qtd_adquirido]            int,
	[qtd_agendado]             int,
	[qtd_utilizado]            int,
	[qtd_cancelado]            int,
	[dt_insercao]              datetime,
	[tipo_desconto]            int,
	[valor_desconto_por_item]  decimal(19,4),
	[id_dav_item]              bigint,
	[transacao_movimento]      int,
	[ficha]                    int,
	[timestamp]                bigint,
	[identificador]            nvarchar(1000) --fora da documentação
);

create table [linxmicrovix].LinxServicoClientes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_cliente]               int not null,
	[empresa]                  int,
	[id_servico]               bigint not null,
	[qtd_adquirido]            int,
	[qtd_agendado]             int,
	[qtd_utilizado]            int,
	[qtd_cancelado]            int,
	[dt_insercao]              datetime,
	[tipo_desconto]            int,
	[valor_desconto_por_item]  decimal(19,4),
	[id_dav_item]              bigint not null,
	[transacao_movimento]      int,
	[ficha]                    int,
	[timestamp]                bigint,
	[identificador]            nvarchar(1000), --fora da documentação
constraint [PK_LinxServicoClientes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_cliente] asc,
	[id_servico] asc,
	[id_dav_item] asc
));

/*LinxServicoAgendaComandas: a agenda das comandas dos serviços. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxServicoAgendaComandas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_comanda]               int,
	[empresa]                  int,
	[cliente]                  int,
	[data]                     datetime,
	[noiva]                    bit,
	[excluida]                 bit,
	[fechada]                  bit,
	[observacoes]              nvarchar(1000),
	[tipo_desconto]            int,
	[valor_desconto]           decimal(19,4),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxServicoAgendaComandas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_comanda]               int not null,
	[empresa]                  int,
	[cliente]                  int,
	[data]                     datetime,
	[noiva]                    bit,
	[excluida]                 bit,
	[fechada]                  bit,
	[observacoes]              nvarchar(1000),
	[tipo_desconto]            int,
	[valor_desconto]           decimal(19,4),
	[timestamp]                bigint,
constraint [PK_LinxServicoAgendaComandas] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_comanda] asc
));

/*LinxServicoAgendaEventos: a agenda dos eventos dos serviços. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxServicoAgendaEventos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_evento]                int,
	[id_espaco]                int,
	[data_evento]              datetime,
	[minutos_duracao]          int,
	[tipo_agendamento]         int,
	[id_cliente_servico]       int,
	[id_profissional]          int,
	[situacao]                 int,
	[data_situacao]            datetime,
	[observacao]               nvarchar(1000),
	[data_chegada]             datetime,
	[data_execucao]            datetime,
	[recorrente]               bit,
	[id_comanda]               int,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxServicoAgendaEventos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_evento]                int not null,
	[id_espaco]                int,
	[data_evento]              datetime,
	[minutos_duracao]          int,
	[tipo_agendamento]         int,
	[id_cliente_servico]       int,
	[id_profissional]          int,
	[situacao]                 int,
	[data_situacao]            datetime,
	[observacao]               nvarchar(1000),
	[data_chegada]             datetime,
	[data_execucao]            datetime,
	[recorrente]               bit,
	[id_comanda]               int,
	[timestamp]                bigint,
constraint [PK_LinxServicoAgendaEventos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_evento] asc
));

/*LinxOrdemServicoExternaDav: as ordens de serviço externas das DAVs Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxOrdemServicoExternaDav (
	[integrated_or_updated_at]                datetime,
	[doc_company]                             nvarchar(14),
	[cnpj_emp]                                nvarchar(14), --loja consultada
	[id_vendas_pos_tmp_ordem_servico_externa] int,
	[id_vendas_pos]                           bigint,
	[codigo_ordem_servico_externa]            nvarchar(20),
	[timestamp]                               bigint,
	[portal]                                  int
);

create table [linxmicrovix].LinxOrdemServicoExternaDav (
	[integrated_or_updated_at]                datetime,
	[doc_company]                             nvarchar(14) not null,
	[cnpj_emp]                                nvarchar(14) not null, --loja consultada
	[id_vendas_pos_tmp_ordem_servico_externa] int not null,
	[id_vendas_pos]                           bigint,
	[codigo_ordem_servico_externa]            nvarchar(20),
	[timestamp]                               bigint,
	[portal]                                  int,
constraint [PK_LinxOrdemServicoExternaDav] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_vendas_pos_tmp_ordem_servico_externa] asc
));

/*LinxMovimentoOrdemServicoExterna: a ordem de serviço externa da movimentação. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxMovimentoOrdemServicoExterna (
	[integrated_or_updated_at]           datetime,
	[doc_company]                        nvarchar(14),
	[cnpj_emp]                           nvarchar(14), --loja consultada
	[id_movimento_ordem_servico_externa] int,
	[identificador]                      uniqueidentifier,
	[codigo_ordem_servico]               nvarchar(20),
	[timestamp]                          bigint,
	[portal]                             int
);

create table [linxmicrovix].LinxMovimentoOrdemServicoExterna (
	[integrated_or_updated_at]           datetime,
	[doc_company]                        nvarchar(14) not null,
	[cnpj_emp]                           nvarchar(14) not null, --loja consultada
	[id_movimento_ordem_servico_externa] int not null,
	[identificador]                      uniqueidentifier,
	[codigo_ordem_servico]               nvarchar(20),
	[timestamp]                          bigint,
	[portal]                             int,
constraint [PK_LinxMovimentoOrdemServicoExterna] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_movimento_ordem_servico_externa] asc
));

/*LinxValeOrdemServicoExterna: os dados de vale ordem de serviço externa. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxValeOrdemServicoExterna (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14),
	[cnpj_emp]                      nvarchar(14), --loja consultada
	[portal]                        int,
	[id_vale_ordem_servico_externa] int,
	[numero_controle]               nvarchar(25),
	[timestamp]                     bigint
);

create table [linxmicrovix].LinxValeOrdemServicoExterna (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14) not null,
	[cnpj_emp]                      nvarchar(14) not null, --loja consultada
	[portal]                        int,
	[id_vale_ordem_servico_externa] int not null,
	[numero_controle]               nvarchar(25),
	[timestamp]                     bigint,
constraint [PK_LinxValeOrdemServicoExterna] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_vale_ordem_servico_externa] asc
));

/*LinxPagamentoParcialDav: os pagamentos parciais da DAVs*/

create table [untreated].LinxPagamentoParcialDav (
	[integrated_or_updated_at]       datetime,
	[doc_company]                    nvarchar(14),
	[id_vendas_pos]                  int,
	[valor]                          decimal(19,4),
	[ajuste_valor_desc_acresc_plano] decimal(19,4),
	[timestamp]                      bigint,
	[id_pagamento_parcial_tmp]       int,
	[plano]                          int,
	[forma_pgto]                     nvarchar(50),
	[valor_2]                        decimal(28,8), --fora da documentação
	[id_bandeira]                    int,
	[qtde_parcelas]                  int,
	[credito_debito]                 nvarchar(1),
	[portal]                         int
);

create table [linxmicrovix].LinxPagamentoParcialDav (
	[integrated_or_updated_at]       datetime,
	[doc_company]                    nvarchar(14) not null,
	[id_vendas_pos]                  int,
	[valor]                          decimal(19,4),
	[ajuste_valor_desc_acresc_plano] decimal(19,4),
	[timestamp]                      bigint,
	[id_pagamento_parcial_tmp]       int not null,
	[plano]                          int,
	[forma_pgto]                     nvarchar(50),
	[valor_2]                        decimal(28,8), --fora da documentação
	[id_bandeira]                    int,
	[qtde_parcelas]                  int,
	[credito_debito]                 nvarchar(1),
	[portal]                         int not null,
constraint [PK_LinxPagamentoParcialDav] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_pagamento_parcial_tmp] asc
));
