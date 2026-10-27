/****** Pagamentos e caixa (LinxMicrovixPaymentsService): tabelas untreated e linxmicrovix (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxMovimentoCartoes: informações dos cartões de acordo com a loja pesquisada*/

create table [untreated].LinxMovimentoCartoes (
	[integrated_or_updated_at]                  datetime,
	[doc_company]                               nvarchar(14),
	[cnpj_emp]                                  nvarchar(14), --loja consultada
	[portal]                                    int,
	[codlojasitef]                              nvarchar(10),
	[data_lancamento]                           datetime,
	[identificador]                             uniqueidentifier,
	[cupomfiscal]                               nvarchar(20),
	[credito_debito]                            nvarchar(1),
	[id_cartao_bandeira]                        int,
	[descricao_bandeira]                        nvarchar(100),
	[valor]                                     decimal(19,4),
	[ordem_cartao]                              int,
	[nsu_host]                                  nvarchar(50),
	[nsu_sitef]                                 nvarchar(50),
	[cod_autorizacao]                           nvarchar(50),
	[id_antecipacoes_financeiras]               int,
	[transacao_servico_terceiro]                bit, --fora da documentação
	[texto_comprovante]                         nvarchar(max),
	[id_maquineta_pos]                          int,
	[descricao_maquineta]                       nvarchar(50),
	[serie_maquineta]                           nvarchar(50),
	[timestamp]                                 bigint,
	[cartao_prepago]                            nvarchar(1),
	[transacao_cancelada_smartpos]              bit,
	[usuario_cancelamento_transacao_smartpos]   int,
	[data_hora_cancelamento_transacao_smartpos] datetime
);

create table [linxmicrovix].LinxMovimentoCartoes (
	[integrated_or_updated_at]                  datetime,
	[doc_company]                               nvarchar(14) not null,
	[cnpj_emp]                                  nvarchar(14) not null, --loja consultada
	[portal]                                    int,
	[codlojasitef]                              nvarchar(10),
	[data_lancamento]                           datetime,
	[identificador]                             uniqueidentifier not null,
	[cupomfiscal]                               nvarchar(20),
	[credito_debito]                            nvarchar(1),
	[id_cartao_bandeira]                        int,
	[descricao_bandeira]                        nvarchar(100),
	[valor]                                     decimal(19,4),
	[ordem_cartao]                              int not null,
	[nsu_host]                                  nvarchar(50),
	[nsu_sitef]                                 nvarchar(50),
	[cod_autorizacao]                           nvarchar(50),
	[id_antecipacoes_financeiras]               int,
	[transacao_servico_terceiro]                bit, --fora da documentação
	[texto_comprovante]                         nvarchar(max),
	[id_maquineta_pos]                          int,
	[descricao_maquineta]                       nvarchar(50),
	[serie_maquineta]                           nvarchar(50),
	[timestamp]                                 bigint,
	[cartao_prepago]                            nvarchar(1),
	[transacao_cancelada_smartpos]              bit,
	[usuario_cancelamento_transacao_smartpos]   int,
	[data_hora_cancelamento_transacao_smartpos] datetime,
constraint [PK_LinxMovimentoCartoes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[identificador] asc,
	[ordem_cartao] asc
));

/*LinxMovimentoPlanos: os Planos de Pagamentos realizados nas movimentações*/

create table [untreated].LinxMovimentoPlanos (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[cnpj_emp]                   nvarchar(14), --loja consultada
	[portal]                     int,
	[identificador]              uniqueidentifier,
	[plano]                      int,
	[desc_plano]                 nvarchar(100),
	[total]                      decimal(19,4),
	[qtde_parcelas]              int,
	[indice_plano]               decimal(19,4),
	[cod_forma_pgto]             int,
	[forma_pgto]                 nvarchar(50),
	[tipo_transacao]             nvarchar(1),
	[taxa_financeira]            decimal(19,4),
	[ordem_cartao]               int,
	[timestamp]                  bigint,
	[empresa]                    int,
	[gerado_config_receb_vendas] bit
);

create table [linxmicrovix].LinxMovimentoPlanos (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[cnpj_emp]                   nvarchar(14) not null, --loja consultada
	[portal]                     int,
	[identificador]              uniqueidentifier not null,
	[plano]                      int not null,
	[desc_plano]                 nvarchar(100),
	[total]                      decimal(19,4),
	[qtde_parcelas]              int,
	[indice_plano]               decimal(19,4),
	[cod_forma_pgto]             int,
	[forma_pgto]                 nvarchar(50),
	[tipo_transacao]             nvarchar(1),
	[taxa_financeira]            decimal(19,4),
	[ordem_cartao]               int not null,
	[timestamp]                  bigint,
	[empresa]                    int,
	[gerado_config_receb_vendas] bit,
constraint [PK_LinxMovimentoPlanos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[identificador] asc,
	[plano] asc,
	[ordem_cartao] asc
));

/*LinxPlanos: o cadastro dos Planos de Pagamentos do portal*/

create table [untreated].LinxPlanos (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[portal]                     int,
	[plano]                      int,
	[desc_plano]                 nvarchar(100),
	[qtde_parcelas]              int,
	[prazo_entre_parcelas]       int,
	[tipo_plano]                 nvarchar(1),
	[indice_plano]               decimal(19,4),
	[cod_forma_pgto]             int,
	[forma_pgto]                 nvarchar(50),
	[conta_central]              int,
	[tipo_transacao]             nvarchar(1),
	[taxa_financeira]            decimal(19,4),
	[dt_upd]                     datetime,
	[desativado]                 nvarchar(1),
	[usa_tef]                    nvarchar(1),
	[timestamp]                  bigint,
	[codigo_ws]                  nvarchar(50),
	[valor_minimo_parcela]       decimal(28,8), --fora da documentação
	[gerado_config_receb_vendas] bit
);

create table [linxmicrovix].LinxPlanos (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[portal]                     int not null,
	[plano]                      int not null,
	[desc_plano]                 nvarchar(100),
	[qtde_parcelas]              int,
	[prazo_entre_parcelas]       int,
	[tipo_plano]                 nvarchar(1),
	[indice_plano]               decimal(19,4),
	[cod_forma_pgto]             int,
	[forma_pgto]                 nvarchar(50),
	[conta_central]              int,
	[tipo_transacao]             nvarchar(1),
	[taxa_financeira]            decimal(19,4),
	[dt_upd]                     datetime,
	[desativado]                 nvarchar(1),
	[usa_tef]                    nvarchar(1),
	[timestamp]                  bigint,
	[codigo_ws]                  nvarchar(50),
	[valor_minimo_parcela]       decimal(28,8), --fora da documentação
	[gerado_config_receb_vendas] bit,
constraint [PK_LinxPlanos] primary key (
	[doc_company] asc,
	[portal] asc,
	[plano] asc
));

/*LinxPlanosParcelas: */

create table [untreated].LinxPlanosParcelas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[plano]                    int,
	[ordem_parcela]            int,
	[prazo_parc]               int,
	[id_planos_parcelas]       int,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxPlanosParcelas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[plano]                    int,
	[ordem_parcela]            int,
	[prazo_parc]               int,
	[id_planos_parcelas]       int not null,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxPlanosParcelas] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_planos_parcelas] asc
));

/*LinxPlanosBandeiras: o cadastro de bandeiras de cartões vinculadas aos Planos de Pagamento do Portal*/

create table [untreated].LinxPlanosBandeiras (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[plano]                    int,
	[bandeira]                 nvarchar(30),
	[tipo_bandeira]            nvarchar(100),
	[adquirente]               int,
	[nome_adquirente]          nvarchar(60),
	[codigo_bandeira_sitef]    nvarchar(10),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxPlanosBandeiras (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[plano]                    int not null,
	[bandeira]                 nvarchar(30) not null,
	[tipo_bandeira]            nvarchar(100),
	[adquirente]               int,
	[nome_adquirente]          nvarchar(60),
	[codigo_bandeira_sitef]    nvarchar(10),
	[timestamp]                bigint,
constraint [PK_LinxPlanosBandeiras] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[plano] asc,
	[bandeira] asc
));

/*LinxMovimentoGiftCard: os registro de giftcard Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxMovimentoGiftCard (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[empresa]                  int,
	[data_transacao]           datetime,
	[operacao]                 int,
	[nsu_cliente]              nvarchar(100),
	[nsu_host]                 nvarchar(100),
	[valor]                    decimal(19,4),
	[json_envio]               nvarchar(max),
	[json_retorno]             nvarchar(max),
	[qtde_tentativa]           int,
	[aprovado_barramento]      nvarchar(4),
	[estornada]                bit,
	[codigo_loja]              nvarchar(100),
	[identificador_movimento]  uniqueidentifier,
	[numero_cartao]            nvarchar(100),
	[plano]                    int,
	[ambiente_producao]        bit,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxMovimentoGiftCard (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[empresa]                  int,
	[data_transacao]           datetime,
	[operacao]                 int,
	[nsu_cliente]              nvarchar(100) not null,
	[nsu_host]                 nvarchar(100),
	[valor]                    decimal(19,4),
	[json_envio]               nvarchar(max),
	[json_retorno]             nvarchar(max),
	[qtde_tentativa]           int,
	[aprovado_barramento]      nvarchar(4),
	[estornada]                bit,
	[codigo_loja]              nvarchar(100),
	[identificador_movimento]  uniqueidentifier not null,
	[numero_cartao]            nvarchar(100),
	[plano]                    int,
	[ambiente_producao]        bit,
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_LinxMovimentoGiftCard] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[identificador_movimento] asc,
	[nsu_cliente] asc
));

/*LinxValesComprasEnviadosAPI: vale Compras importado via API. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxValesComprasEnviadosAPI (
	[integrated_or_updated_at]    datetime,
	[doc_company]                 nvarchar(14),
	[cnpj_emp]                    nvarchar(14), --loja consultada
	[numero_controle]             int,
	[valor_vale]                  decimal(19,4),
	[doc_cliente]                 nvarchar(14),
	[status_vale]                 int,
	[codigo_portal_resgate]       int,
	[codigo_empresa_resgate]      int,
	[codigo_usuario_resgate]      int,
	[codigo_vale_empresa_resgate] int,
	[data_criacao]                datetime,
	[timestamp]                   bigint,
	[cnpj_empresa_resgate]        nvarchar(14),
	[data_resgate]                datetime,
	[portal]                      int
);

create table [linxmicrovix].LinxValesComprasEnviadosAPI (
	[integrated_or_updated_at]    datetime,
	[doc_company]                 nvarchar(14) not null,
	[cnpj_emp]                    nvarchar(14) not null, --loja consultada
	[numero_controle]             int not null,
	[valor_vale]                  decimal(19,4),
	[doc_cliente]                 nvarchar(14),
	[status_vale]                 int,
	[codigo_portal_resgate]       int,
	[codigo_empresa_resgate]      int,
	[codigo_usuario_resgate]      int,
	[codigo_vale_empresa_resgate] int,
	[data_criacao]                datetime,
	[timestamp]                   bigint,
	[cnpj_empresa_resgate]        nvarchar(14),
	[data_resgate]                datetime,
	[portal]                      int,
constraint [PK_LinxValesComprasEnviadosAPI] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[numero_controle] asc
));

/*LinxSangriaSuprimentos: as informações das Sangrias ou Suprimentos feitos no caixa Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxSangriaSuprimentos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[usuario]                  int,
	[data]                     nvarchar(1000), --fora da documentação
	[valor]                    decimal(19,4),
	[obs]                      nvarchar(max),
	[cancelado]                nvarchar(1),
	[conferido]                bit,
	[cod_historico]            bigint,
	[desc_historico]           nvarchar(50),
	[id_sangria_suprimentos]   int,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxSangriaSuprimentos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[usuario]                  int,
	[data]                     nvarchar(1000), --fora da documentação
	[valor]                    decimal(19,4),
	[obs]                      nvarchar(max),
	[cancelado]                nvarchar(1),
	[conferido]                bit,
	[cod_historico]            bigint,
	[desc_historico]           nvarchar(50),
	[id_sangria_suprimentos]   int not null,
	[timestamp]                bigint,
constraint [PK_LinxSangriaSuprimentos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_sangria_suprimentos] asc
));

/*LinxFechamentoCaixa:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxFechamentoCaixa (
	[integrated_or_updated_at]     datetime,
	[doc_company]                  nvarchar(14),
	[cnpj_emp]                     nvarchar(14), --loja consultada
	[empresa]                      int,
	[data]                         datetime,
	[usuario]                      nvarchar(10),
	[qtd_001]                      int,
	[qtd_005]                      int,
	[qtd_010]                      int,
	[qtd_025]                      int,
	[qtd_050]                      int,
	[qtd_1]                        int,
	[qtd_2]                        int,
	[qtd_5]                        int,
	[qtd_10]                       int,
	[qtd_20]                       int,
	[qtd_50]                       int,
	[qtd_100]                      int,
	[qtd_200]                      int,
	[total_geral]                  decimal(19,4),
	[obs]                          nvarchar(max),
	[total_c_vista]                decimal(19,4),
	[total_c_prazo]                decimal(19,4),
	[total_cartao]                 decimal(19,4),
	[total_convenio]               decimal(19,4),
	[total_crediario]              decimal(19,4),
	[total_giftcard]               decimal(19,4),
	[total_qr_linx]                decimal(19,4),
	[total_cartao_credito]         decimal(19,4),
	[total_cartao_debito]          decimal(19,4),
	[total_pix]                    decimal(19,4),
	[total_link_pagamento]         decimal(19,4),
	[total_link_pagamento_credito] decimal(19,4),
	[total_link_pagamento_debito]  decimal(19,4),
	[total_vale_compra]            decimal(19,4),
	[vale_compras_dev]             decimal(19,4),
	[sangria]                      decimal(19,4),
	[suprimentos]                  decimal(19,4),
	[ativo]                        bit,
	[conferencia_efetuada]         bit,
	[timestamp]                    bigint,
	[portal]                       int
);

create table [linxmicrovix].LinxFechamentoCaixa (
	[integrated_or_updated_at]     datetime,
	[doc_company]                  nvarchar(14) not null,
	[cnpj_emp]                     nvarchar(14) not null, --loja consultada
	[empresa]                      int not null,
	[data]                         datetime not null,
	[usuario]                      nvarchar(10) not null,
	[qtd_001]                      int,
	[qtd_005]                      int,
	[qtd_010]                      int,
	[qtd_025]                      int,
	[qtd_050]                      int,
	[qtd_1]                        int,
	[qtd_2]                        int,
	[qtd_5]                        int,
	[qtd_10]                       int,
	[qtd_20]                       int,
	[qtd_50]                       int,
	[qtd_100]                      int,
	[qtd_200]                      int,
	[total_geral]                  decimal(19,4),
	[obs]                          nvarchar(max),
	[total_c_vista]                decimal(19,4),
	[total_c_prazo]                decimal(19,4),
	[total_cartao]                 decimal(19,4),
	[total_convenio]               decimal(19,4),
	[total_crediario]              decimal(19,4),
	[total_giftcard]               decimal(19,4),
	[total_qr_linx]                decimal(19,4),
	[total_cartao_credito]         decimal(19,4),
	[total_cartao_debito]          decimal(19,4),
	[total_pix]                    decimal(19,4),
	[total_link_pagamento]         decimal(19,4),
	[total_link_pagamento_credito] decimal(19,4),
	[total_link_pagamento_debito]  decimal(19,4),
	[total_vale_compra]            decimal(19,4),
	[vale_compras_dev]             decimal(19,4),
	[sangria]                      decimal(19,4),
	[suprimentos]                  decimal(19,4),
	[ativo]                        bit,
	[conferencia_efetuada]         bit,
	[timestamp]                    bigint,
	[portal]                       int,
constraint [PK_LinxFechamentoCaixa] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[empresa] asc,
	[data] asc,
	[usuario] asc
));
