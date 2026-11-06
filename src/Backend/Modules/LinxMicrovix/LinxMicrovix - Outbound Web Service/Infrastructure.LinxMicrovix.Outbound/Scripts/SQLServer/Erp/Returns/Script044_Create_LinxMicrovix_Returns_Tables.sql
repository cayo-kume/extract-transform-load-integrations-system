-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; .docs/MIGRATOR-CSX.md)
/****** Trocas e devoluções (LinxMicrovixReturnsService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxMovimentoTrocas: os valores e números das trocas realizadas nas vendas*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxMovimentoTrocas') = 1
create table [untreated].LinxMovimentoTrocas (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14),
	[cnpj_emp]                      nvarchar(14), --loja consultada
	[portal]                        int,
	[identificador]                 uniqueidentifier,
	[num_vale]                      bigint,
	[valor_vale]                    decimal(19,4),
	[motivo]                        nvarchar(50),
	[doc_origem]                    int,
	[serie_origem]                  nvarchar(10),
	[doc_venda]                     int,
	[serie_venda]                   nvarchar(10),
	[excluido]                      bit,
	[timestamp]                     bigint,
	[desfazimento]                  bit,
	[empresa]                       int,
	[vale_cod_cliente]              int,
	[vale_codigoproduto]            bigint,
	[id_vale_ordem_servico_externa] int,
	[doc_venda_origem]              int,
	[serie_venda_origem]            nvarchar(10),
	[cod_cliente]                   int,
	[data_vencimento]               datetime,
	[valor_original]                decimal(19,4),
	[id_antecipacoes_financeiras]   int,
	[obs_troca]                     nvarchar(180)
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxMovimentoTrocas') = 1
create table [linxmicrovix].LinxMovimentoTrocas (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14) not null,
	[cnpj_emp]                      nvarchar(14) not null, --loja consultada
	[portal]                        int,
	[identificador]                 uniqueidentifier not null,
	[num_vale]                      bigint not null,
	[valor_vale]                    decimal(19,4),
	[motivo]                        nvarchar(50),
	[doc_origem]                    int,
	[serie_origem]                  nvarchar(10),
	[doc_venda]                     int,
	[serie_venda]                   nvarchar(10),
	[excluido]                      bit,
	[timestamp]                     bigint,
	[desfazimento]                  bit,
	[empresa]                       int,
	[vale_cod_cliente]              int,
	[vale_codigoproduto]            bigint,
	[id_vale_ordem_servico_externa] int,
	[doc_venda_origem]              int,
	[serie_venda_origem]            nvarchar(10),
	[cod_cliente]                   int,
	[data_vencimento]               datetime,
	[valor_original]                decimal(19,4),
	[id_antecipacoes_financeiras]   int,
	[obs_troca]                     nvarchar(180),
constraint [PK_LinxMovimentoTrocas] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[identificador] asc,
	[num_vale] asc
));

/*LinxMovimentoTrocafone: os valores e números dos vouchers Trocafone realizadas nas vendas Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxMovimentoTrocafone') = 1
create table [untreated].LinxMovimentoTrocafone (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[identificador]            uniqueidentifier,
	[num_vale]                 bigint,
	[valor_vale]               decimal(19,4),
	[voucher]                  nvarchar(100),
	[nome_produto]             nvarchar(250),
	[condicao]                 nvarchar(250),
	[id_tradein_parceiro]      int,
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxMovimentoTrocafone') = 1
create table [linxmicrovix].LinxMovimentoTrocafone (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[identificador]            uniqueidentifier not null,
	[num_vale]                 bigint not null,
	[valor_vale]               decimal(19,4),
	[voucher]                  nvarchar(100),
	[nome_produto]             nvarchar(250),
	[condicao]                 nvarchar(250),
	[id_tradein_parceiro]      int,
	[timestamp]                bigint,
constraint [PK_LinxMovimentoTrocafone] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[identificador] asc,
	[num_vale] asc
));

/*LinxMovimentoDevolucoesItens: os dados da tabela movimento_devolucoes_itens cadastrados no portal.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxMovimentoDevolucoesItens') = 1
create table [untreated].LinxMovimentoDevolucoesItens (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14),
	[cnpj_emp]                      nvarchar(14), --loja consultada
	[portal]                        int,
	[identificador_venda]           uniqueidentifier,
	[identificador_devolucao]       uniqueidentifier,
	[codigoproduto]                 bigint,
	[id_movimento_devolucoes_itens] int,
	[transacao_origem]              int,
	[transacao_devolucao]           int,
	[qtde_devolvida]                decimal(28,8),
	[timestamp]                     bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxMovimentoDevolucoesItens') = 1
create table [linxmicrovix].LinxMovimentoDevolucoesItens (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14) not null,
	[cnpj_emp]                      nvarchar(14) not null, --loja consultada
	[portal]                        int,
	[identificador_venda]           uniqueidentifier,
	[identificador_devolucao]       uniqueidentifier,
	[codigoproduto]                 bigint,
	[id_movimento_devolucoes_itens] int not null,
	[transacao_origem]              int,
	[transacao_devolucao]           int,
	[qtde_devolvida]                decimal(28,8),
	[timestamp]                     bigint,
constraint [PK_LinxMovimentoDevolucoesItens] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_movimento_devolucoes_itens] asc
));

/*LinxMovimentoOrigemDevolucoes: os documentos de origem relacionados as devoluções*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxMovimentoOrigemDevolucoes') = 1
create table [untreated].LinxMovimentoOrigemDevolucoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[identificador]            uniqueidentifier,
	[nota_origem]              int,
	[ecf_origem]               int,
	[data_origem]              datetime,
	[serie_origem]             nvarchar(10),
	[empresa]                  int,
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxMovimentoOrigemDevolucoes') = 1
create table [linxmicrovix].LinxMovimentoOrigemDevolucoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[identificador]            uniqueidentifier not null,
	[nota_origem]              int not null,
	[ecf_origem]               int,
	[data_origem]              datetime,
	[serie_origem]             nvarchar(10),
	[empresa]                  int,
	[timestamp]                bigint,
constraint [PK_LinxMovimentoOrigemDevolucoes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[identificador] asc,
	[nota_origem] asc
));

/*LinxMotivoDevolucao: os motivos de devoluções cadastrados no ERP. Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxMotivoDevolucao') = 1
create table [untreated].LinxMotivoDevolucao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[cod_motivo]               int,
	[descricao_motivo]         nvarchar(50),
	[cod_deposito]             int,
	[ativo]                    bit,
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxMotivoDevolucao') = 1
create table [linxmicrovix].LinxMotivoDevolucao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[cod_motivo]               int not null,
	[descricao_motivo]         nvarchar(50),
	[cod_deposito]             int,
	[ativo]                    bit,
	[timestamp]                bigint,
constraint [PK_LinxMotivoDevolucao] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_motivo] asc
));

/*LinxTrocaUnificadaResumoVendas: as vendas do troca unificada.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxTrocaUnificadaResumoVendas') = 1
create table [untreated].LinxTrocaUnificadaResumoVendas (
	[integrated_or_updated_at]         datetime,
	[doc_company]                      nvarchar(14),
	[cnpj_emp]                         nvarchar(14), --loja consultada
	[portal]                           int,
	[empresa]                          int,
	[id_troca_unificada_resumo_vendas] bigint,
	[token]                            nvarchar(30),
	[identificador]                    uniqueidentifier,
	[documento]                        int,
	[serie]                            nvarchar(10),
	[data_venda]                       datetime,
	[documento_cliente]                nvarchar(14),
	[venda_cancelada]                  bit,
	[timestamp]                        bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxTrocaUnificadaResumoVendas') = 1
create table [linxmicrovix].LinxTrocaUnificadaResumoVendas (
	[integrated_or_updated_at]         datetime,
	[doc_company]                      nvarchar(14) not null,
	[cnpj_emp]                         nvarchar(14) not null, --loja consultada
	[portal]                           int,
	[empresa]                          int,
	[id_troca_unificada_resumo_vendas] bigint not null,
	[token]                            nvarchar(30),
	[identificador]                    uniqueidentifier,
	[documento]                        int,
	[serie]                            nvarchar(10),
	[data_venda]                       datetime,
	[documento_cliente]                nvarchar(14),
	[venda_cancelada]                  bit,
	[timestamp]                        bigint,
constraint [PK_LinxTrocaUnificadaResumoVendas] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_troca_unificada_resumo_vendas] asc
));

/*LinxTrocaUnificadaResumoVendasItens: */

if dbo.fn_Migrator_Creates(N'[untreated].LinxTrocaUnificadaResumoVendasItens') = 1
create table [untreated].LinxTrocaUnificadaResumoVendasItens (
	[integrated_or_updated_at]               datetime,
	[doc_company]                            nvarchar(14),
	[cnpj_emp]                               nvarchar(14), --loja consultada
	[id_troca_unificada_resumo_vendas_itens] bigint,
	[id_troca_unificada_resumo_vendas]       bigint,
	[codigoproduto]                          bigint,
	[transacao]                              int,
	[serial]                                 nvarchar(50),
	[valor_liquido]                          decimal(19,4),
	[data_validade]                          datetime,
	[venda_referenciada]                     bit,
	[token_utilizado]                        bit,
	[quantidade]                             decimal(28,8),
	[timestamp]                              bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxTrocaUnificadaResumoVendasItens') = 1
create table [linxmicrovix].LinxTrocaUnificadaResumoVendasItens (
	[integrated_or_updated_at]               datetime,
	[doc_company]                            nvarchar(14) not null,
	[cnpj_emp]                               nvarchar(14) not null, --loja consultada
	[id_troca_unificada_resumo_vendas_itens] bigint not null,
	[id_troca_unificada_resumo_vendas]       bigint,
	[codigoproduto]                          bigint,
	[transacao]                              int,
	[serial]                                 nvarchar(50),
	[valor_liquido]                          decimal(19,4),
	[data_validade]                          datetime,
	[venda_referenciada]                     bit,
	[token_utilizado]                        bit,
	[quantidade]                             decimal(28,8),
	[timestamp]                              bigint,
constraint [PK_LinxTrocaUnificadaResumoVendasItens] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_troca_unificada_resumo_vendas_itens] asc
));

/*LinxTrocaUnificadaResumoBaixa: os dados do resumo baixa do troca unificada*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxTrocaUnificadaResumoBaixa') = 1
create table [untreated].LinxTrocaUnificadaResumoBaixa (
	[integrated_or_updated_at]               datetime,
	[doc_company]                            nvarchar(14),
	[cnpj_emp]                               nvarchar(14), --loja consultada
	[portal_baixa]                           int,
	[empresa_baixa]                          int,
	[cnpj_empresa_baixa]                     nvarchar(14),
	[id_troca_baixa]                         int,
	[id_troca_unificada_resumo_vendas_itens] bigint,
	[data_troca_baixa]                       datetime,
	[transacao_baixa]                        int,
	[descricao_empresa_baixa]                nvarchar(50),
	[timestamp]                              bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxTrocaUnificadaResumoBaixa') = 1
create table [linxmicrovix].LinxTrocaUnificadaResumoBaixa (
	[integrated_or_updated_at]               datetime,
	[doc_company]                            nvarchar(14) not null,
	[cnpj_emp]                               nvarchar(14) not null, --loja consultada
	[portal_baixa]                           int,
	[empresa_baixa]                          int,
	[cnpj_empresa_baixa]                     nvarchar(14),
	[id_troca_baixa]                         int not null,
	[id_troca_unificada_resumo_vendas_itens] bigint,
	[data_troca_baixa]                       datetime,
	[transacao_baixa]                        int,
	[descricao_empresa_baixa]                nvarchar(50),
	[timestamp]                              bigint,
constraint [PK_LinxTrocaUnificadaResumoBaixa] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_troca_baixa] asc
));

/*LinxTradeinParceiro: os registros cadastrados na tabela tradein_parceiro do portal Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxTradeinParceiro') = 1
create table [untreated].LinxTradeinParceiro (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_tradein_parceiro]      int,
	[nome_parceiro]            nvarchar(100),
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxTradeinParceiro') = 1
create table [linxmicrovix].LinxTradeinParceiro (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_tradein_parceiro]      int not null,
	[nome_parceiro]            nvarchar(100),
	[timestamp]                bigint,
constraint [PK_LinxTradeinParceiro] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_tradein_parceiro] asc
));

/*LinxDevolucaoRemanejoFabrica: as devoluções e remanejos de fábrica. Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxDevolucaoRemanejoFabrica') = 1
create table [untreated].LinxDevolucaoRemanejoFabrica (
	[integrated_or_updated_at]             datetime,
	[doc_company]                          nvarchar(14),
	[cnpj_emp]                             nvarchar(14), --loja consultada
	[id_devolucao_remanejo_fabrica]        int,
	[id_devolucao_remanejo_fabrica_tipo]   int,
	[id_motivo_devolucao_fabrica]          int,
	[id_deposito]                          int,
	[id_devolucao_remanejo_fabrica_status] int,
	[empresa]                              int,
	[fornecedor]                           int,
	[cfop]                                 nvarchar(10),
	[serie]                                nvarchar(10),
	[codigo_solicitacao]                   nvarchar(50),
	[portal]                               int,
	[data_solicitacao]                     datetime,
	[timestamp]                            bigint,
	[portal_2]                             nvarchar(1000) --fora da documentação
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxDevolucaoRemanejoFabrica') = 1
create table [linxmicrovix].LinxDevolucaoRemanejoFabrica (
	[integrated_or_updated_at]             datetime,
	[doc_company]                          nvarchar(14) not null,
	[cnpj_emp]                             nvarchar(14) not null, --loja consultada
	[id_devolucao_remanejo_fabrica]        int not null,
	[id_devolucao_remanejo_fabrica_tipo]   int,
	[id_motivo_devolucao_fabrica]          int,
	[id_deposito]                          int,
	[id_devolucao_remanejo_fabrica_status] int,
	[empresa]                              int,
	[fornecedor]                           int,
	[cfop]                                 nvarchar(10),
	[serie]                                nvarchar(10),
	[codigo_solicitacao]                   nvarchar(50),
	[portal]                               int,
	[data_solicitacao]                     datetime,
	[timestamp]                            bigint,
	[portal_2]                             nvarchar(1000), --fora da documentação
constraint [PK_LinxDevolucaoRemanejoFabrica] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_devolucao_remanejo_fabrica] asc
));

/*LinxDevolucaoRemanejoFabricaItem: os itens das devoluções e remanejos de fábrica. Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxDevolucaoRemanejoFabricaItem') = 1
create table [untreated].LinxDevolucaoRemanejoFabricaItem (
	[integrated_or_updated_at]           datetime,
	[doc_company]                        nvarchar(14),
	[cnpj_emp]                           nvarchar(14), --loja consultada
	[id_devolucao_remanejo_fabrica_item] int,
	[id_devolucao_remanejo_fabrica]      int,
	[codigoproduto]                      bigint,
	[codigo_produto_franqueadora]        nvarchar(50),
	[quantidade]                         int,
	[codebar]                            nvarchar(20),
	[serial]                             nvarchar(50),
	[timestamp]                          bigint,
	[portal]                             int
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxDevolucaoRemanejoFabricaItem') = 1
create table [linxmicrovix].LinxDevolucaoRemanejoFabricaItem (
	[integrated_or_updated_at]           datetime,
	[doc_company]                        nvarchar(14) not null,
	[cnpj_emp]                           nvarchar(14) not null, --loja consultada
	[id_devolucao_remanejo_fabrica_item] int not null,
	[id_devolucao_remanejo_fabrica]      int,
	[codigoproduto]                      bigint,
	[codigo_produto_franqueadora]        nvarchar(50),
	[quantidade]                         int,
	[codebar]                            nvarchar(20),
	[serial]                             nvarchar(50),
	[timestamp]                          bigint,
	[portal]                             int,
constraint [PK_LinxDevolucaoRemanejoFabricaItem] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_devolucao_remanejo_fabrica_item] asc
));

/*LinxDevolucaoRemanejoFabricaStatus: o cadastro de status da rotina devoluções e remanejos de fábrica. Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxDevolucaoRemanejoFabricaStatus') = 1
create table [untreated].LinxDevolucaoRemanejoFabricaStatus (
	[integrated_or_updated_at]             datetime,
	[doc_company]                          nvarchar(14),
	[id_devolucao_remanejo_fabrica_status] int,
	[id_devolucao_remanejo_fabrica_tipo]   int,
	[descricao]                            nvarchar(100),
	[timestamp]                            bigint,
	[portal]                               int
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxDevolucaoRemanejoFabricaStatus') = 1
create table [linxmicrovix].LinxDevolucaoRemanejoFabricaStatus (
	[integrated_or_updated_at]             datetime,
	[doc_company]                          nvarchar(14) not null,
	[id_devolucao_remanejo_fabrica_status] int not null,
	[id_devolucao_remanejo_fabrica_tipo]   int,
	[descricao]                            nvarchar(100),
	[timestamp]                            bigint,
	[portal]                               int not null,
constraint [PK_LinxDevolucaoRemanejoFabricaStatus] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_devolucao_remanejo_fabrica_status] asc
));

/*LinxDevolucaoRemanejoFabricaTipo: o cadastro de tipos da rotina devoluções e remanejos de fábrica. Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxDevolucaoRemanejoFabricaTipo') = 1
create table [untreated].LinxDevolucaoRemanejoFabricaTipo (
	[integrated_or_updated_at]           datetime,
	[doc_company]                        nvarchar(14),
	[id_devolucao_remanejo_fabrica_tipo] int,
	[descricao]                          nvarchar(100),
	[timestamp]                          bigint,
	[portal]                             int
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxDevolucaoRemanejoFabricaTipo') = 1
create table [linxmicrovix].LinxDevolucaoRemanejoFabricaTipo (
	[integrated_or_updated_at]           datetime,
	[doc_company]                        nvarchar(14) not null,
	[id_devolucao_remanejo_fabrica_tipo] int not null,
	[descricao]                          nvarchar(100),
	[timestamp]                          bigint,
	[portal]                             int not null,
constraint [PK_LinxDevolucaoRemanejoFabricaTipo] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_devolucao_remanejo_fabrica_tipo] asc
));
