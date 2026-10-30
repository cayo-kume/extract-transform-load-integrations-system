/****** Documentos fiscais (LinxMicrovixFiscalDocumentsService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxXMLDocumentos: o XML dos documentos.*/

create table [untreated].LinxXMLDocumentos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[documento]                int,
	[serie]                    nvarchar(10),
	[data_emissao]             datetime,
	[chave_nfe]                nvarchar(44),
	[situacao]                 int,
	[xml]                      nvarchar(max),
	[excluido]                 bit,
	[identificador_microvix]   uniqueidentifier,
	[dt_insert]                datetime,
	[timestamp]                bigint,
	[nProtCanc]                nvarchar(20),
	[nProtInut]                nvarchar(20),
	[xmlDistribuicao]          nvarchar(max),
	[nProtDeneg]               nvarchar(15),
	[cStat]                    nvarchar(5),
	[id_nfe]                   int,
	[justificativa]            nvarchar(255),
	[cod_cliente]              int
);

create table [linxmicrovix].LinxXMLDocumentos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[documento]                int,
	[serie]                    nvarchar(10),
	[data_emissao]             datetime,
	[chave_nfe]                nvarchar(44),
	[situacao]                 int,
	[xml]                      nvarchar(max),
	[excluido]                 bit,
	[identificador_microvix]   uniqueidentifier not null,
	[dt_insert]                datetime,
	[timestamp]                bigint,
	[nProtCanc]                nvarchar(20),
	[nProtInut]                nvarchar(20),
	[xmlDistribuicao]          nvarchar(max),
	[nProtDeneg]               nvarchar(15),
	[cStat]                    nvarchar(5),
	[id_nfe]                   int,
	[justificativa]            nvarchar(255),
	[cod_cliente]              int,
constraint [PK_LinxXMLDocumentos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[identificador_microvix] asc
));

/*LinxNFeEvento: os dados eventos da NFe.*/

create table [untreated].LinxNFeEvento (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_nfe_evento]            int,
	[id_nfe]                   int,
	[codigo_tipo]              nvarchar(6),
	[xml]                      nvarchar(max),
	[timestamp]                bigint,
	[data_emissao]             datetime,
	[hora_emissao]             nvarchar(20) --5 na documentação; a resposta real tem até 8
);

create table [linxmicrovix].LinxNFeEvento (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_nfe_evento]            int not null,
	[id_nfe]                   int,
	[codigo_tipo]              nvarchar(6),
	[xml]                      nvarchar(max),
	[timestamp]                bigint,
	[data_emissao]             datetime,
	[hora_emissao]             nvarchar(20), --5 na documentação; a resposta real tem até 8
constraint [PK_LinxNFeEvento] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_nfe_evento] asc
));

/*LinxNfse: nota Fiscal de Serviços Eletrônica Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxNfse (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   nvarchar(20),
	[documento]                nvarchar(20),
	[id_nfse]                  int,
	[codigo_verificacao]       nvarchar(50),
	[id_nfse_situacao]         int,
	[identificador]            uniqueidentifier,
	[dt_insert]                datetime,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxNfse (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   nvarchar(20),
	[documento]                nvarchar(20),
	[id_nfse]                  int not null,
	[codigo_verificacao]       nvarchar(50),
	[id_nfse_situacao]         int,
	[identificador]            uniqueidentifier,
	[dt_insert]                datetime,
	[timestamp]                bigint,
constraint [PK_LinxNfse] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_nfse] asc
));

/*LinxEntradaNFe: dados de NFes de entrada.*/

create table [untreated].LinxEntradaNFe (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id_entrada_nfe]           int,
	[numero_nfe]               int,
	[lancada_no_microvix]      bit,
	[empresa]                  int,
	[documento_emitente]       nvarchar(14),
	[razao_social_emitente]    nvarchar(60),
	[serie_nfe]                int,
	[data_emissao_nfe]         datetime,
	[chave_nfe]                nvarchar(44),
	[excluida]                 bit,
	[dt_insert]                datetime,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxEntradaNFe (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id_entrada_nfe]           int not null,
	[numero_nfe]               int,
	[lancada_no_microvix]      bit,
	[empresa]                  int,
	[documento_emitente]       nvarchar(14),
	[razao_social_emitente]    nvarchar(60),
	[serie_nfe]                int,
	[data_emissao_nfe]         datetime,
	[chave_nfe]                nvarchar(44),
	[excluida]                 bit,
	[dt_insert]                datetime,
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_LinxEntradaNFe] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_entrada_nfe] asc
));

/*LinxEntradaNFeProdutos: dados de itens de NFes de entrada.*/

create table [untreated].LinxEntradaNFeProdutos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_entrada_nfe_produtos]  int,
	[id_entrada_nfe]           int,
	[numero_item]              int,
	[codigo_produto]           nvarchar(60),
	[descricao_produto]        nvarchar(250),
	[gtin]                     nvarchar(25),
	[qtde_tributavel]          decimal(19,4),
	[qtde_comercial]           decimal(19,4),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxEntradaNFeProdutos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_entrada_nfe_produtos]  int not null,
	[id_entrada_nfe]           int,
	[numero_item]              int,
	[codigo_produto]           nvarchar(60),
	[descricao_produto]        nvarchar(250),
	[gtin]                     nvarchar(25),
	[qtde_tributavel]          decimal(19,4),
	[qtde_comercial]           decimal(19,4),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxEntradaNFeProdutos] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_entrada_nfe_produtos] asc
));

/*LinxConhecimentoFretes: dados de conhecimento de fretes. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxConhecimentoFretes (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14),
	[cnpj_emp]                      nvarchar(14), --loja consultada
	[id_conhecimento]               int,
	[transportador]                 int,
	[valor_frete]                   decimal(19,4),
	[conhecimento]                  int,
	[serie_frete]                   nvarchar(10),
	[tipo_frete]                    nvarchar(max),
	[vencto_frete]                  datetime,
	[valor_total_notas]             decimal(19,4),
	[valor_lancado]                 decimal(19,4),
	[data_emissao]                  datetime,
	[data_entrada]                  datetime,
	[cfop]                          nvarchar(max),
	[aliq_icms]                     decimal(19,4),
	[empresa]                       int,
	[base_calculo_icms]             decimal(19,4),
	[id_cfop_fiscal]                int,
	[id_modelo_nf]                  int,
	[id_cte_tipo]                   int,
	[valor_icms]                    decimal(19,4),
	[valor_pis]                     decimal(19,4),
	[valor_cofins]                  decimal(19,4),
	[chave_cte]                     nvarchar(44),
	[regime_pis_cofins]             nvarchar(max),
	[base_pis]                      decimal(19,4),
	[base_cofins]                   decimal(19,4),
	[id_cst_icms_fiscal]            int,
	[id_nfe_ibge_municipio_origem]  int,
	[id_nfe_ibge_municipio_destino] int,
	[timestamp]                     bigint,
	[portal]                        int
);

create table [linxmicrovix].LinxConhecimentoFretes (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14) not null,
	[cnpj_emp]                      nvarchar(14) not null, --loja consultada
	[id_conhecimento]               int not null,
	[transportador]                 int,
	[valor_frete]                   decimal(19,4),
	[conhecimento]                  int,
	[serie_frete]                   nvarchar(10),
	[tipo_frete]                    nvarchar(max),
	[vencto_frete]                  datetime,
	[valor_total_notas]             decimal(19,4),
	[valor_lancado]                 decimal(19,4),
	[data_emissao]                  datetime,
	[data_entrada]                  datetime,
	[cfop]                          nvarchar(max),
	[aliq_icms]                     decimal(19,4),
	[empresa]                       int,
	[base_calculo_icms]             decimal(19,4),
	[id_cfop_fiscal]                int,
	[id_modelo_nf]                  int,
	[id_cte_tipo]                   int,
	[valor_icms]                    decimal(19,4),
	[valor_pis]                     decimal(19,4),
	[valor_cofins]                  decimal(19,4),
	[chave_cte]                     nvarchar(44),
	[regime_pis_cofins]             nvarchar(max),
	[base_pis]                      decimal(19,4),
	[base_cofins]                   decimal(19,4),
	[id_cst_icms_fiscal]            int,
	[id_nfe_ibge_municipio_origem]  int,
	[id_nfe_ibge_municipio_destino] int,
	[timestamp]                     bigint,
	[portal]                        int,
constraint [PK_LinxConhecimentoFretes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_conhecimento] asc
));

/*LinxFreteTipos: tipos de frete. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxFreteTipos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_frete_tipos]           int,
	[descricao]                nvarchar(40),
	[codigo_sped]              int,
	[codigo_nfe]               int,
	[codigo_sintegra]          int,
	[permite_valor]            bit,
	[ativo]                    bit,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxFreteTipos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_frete_tipos]           int not null,
	[descricao]                nvarchar(40),
	[codigo_sped]              int,
	[codigo_nfe]               int,
	[codigo_sintegra]          int,
	[permite_valor]            bit,
	[ativo]                    bit,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxFreteTipos] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_frete_tipos] asc
));

/*LinxReducoesZ: as ReduçõesZ de acordo com a loja pesquisada, somente para clientes que ainda utilizam a impressora fiscal Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxReducoesZ (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[cnpj]                     nvarchar(14),
	[data_fechamento]          datetime,
	[numero_ecf]               int,
	[numeroserie]              nvarchar(30),
	[numero_reducoes]          int,
	[cupom_inicial]            int,
	[cupom_final]              int,
	[gt_inicial]               decimal(19,4),
	[gt_final]                 decimal(19,4),
	[qtde_cupons]              int,
	[venda_bruta]              decimal(19,4),
	[descontos]                decimal(19,4),
	[acrescimos]               decimal(19,4),
	[cancelamentos]            decimal(19,4),
	[cro]                      int,
	[usuario]                  int,
	[aliquota]                 nvarchar(4),
	[valor]                    decimal(19,4),
	[data_mov]                 datetime,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxReducoesZ (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[cnpj]                     nvarchar(14),
	[data_fechamento]          datetime,
	[numero_ecf]               int,
	[numeroserie]              nvarchar(30) not null,
	[numero_reducoes]          int not null,
	[cupom_inicial]            int,
	[cupom_final]              int,
	[gt_inicial]               decimal(19,4),
	[gt_final]                 decimal(19,4),
	[qtde_cupons]              int,
	[venda_bruta]              decimal(19,4),
	[descontos]                decimal(19,4),
	[acrescimos]               decimal(19,4),
	[cancelamentos]            decimal(19,4),
	[cro]                      int,
	[usuario]                  int,
	[aliquota]                 nvarchar(4) not null,
	[valor]                    decimal(19,4),
	[data_mov]                 datetime,
	[timestamp]                bigint,
constraint [PK_LinxReducoesZ] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[numeroserie] asc,
	[numero_reducoes] asc,
	[aliquota] asc
));

/*LinxECF: as ECFs cadastradas para a empresa. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxECF (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id_ecf]                   int,
	[empresa]                  int,
	[ecf]                      int,
	[numeroserie]              nvarchar(30),
	[indice_sangria]           nvarchar(4),
	[modelo]                   int,
	[nome]                     nvarchar(50), --fora da documentação
	[indice_suprimento]        nvarchar(4),
	[ativo]                    bit,
	[indice_sinal]             nvarchar(53),
	[indice_antecipacao]       nvarchar(50),
	[indice_cartao_credito]    nvarchar(50),
	[empresa_ecf]              nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxECF (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id_ecf]                   int not null,
	[empresa]                  int,
	[ecf]                      int,
	[numeroserie]              nvarchar(30),
	[indice_sangria]           nvarchar(4),
	[modelo]                   int,
	[nome]                     nvarchar(50), --fora da documentação
	[indice_suprimento]        nvarchar(4),
	[ativo]                    bit,
	[indice_sinal]             nvarchar(53),
	[indice_antecipacao]       nvarchar(50),
	[indice_cartao_credito]    nvarchar(50),
	[empresa_ecf]              nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_LinxECF] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_ecf] asc
));

/*LinxECFFormasPagamento: as formas de pagamento da ECF. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxECFFormasPagamento (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[cnpj_emp]                   nvarchar(14), --loja consultada
	[id_empresa_ecf_formas_pgto] int,
	[id_ecf]                     int,
	[cod_forma_pgto]             int,
	[indice_forma]               nvarchar(53),
	[timestamp]                  bigint,
	[portal]                     int
);

create table [linxmicrovix].LinxECFFormasPagamento (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[cnpj_emp]                   nvarchar(14) not null, --loja consultada
	[id_empresa_ecf_formas_pgto] int not null,
	[id_ecf]                     int,
	[cod_forma_pgto]             int,
	[indice_forma]               nvarchar(53),
	[timestamp]                  bigint,
	[portal]                     int,
constraint [PK_LinxECFFormasPagamento] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_empresa_ecf_formas_pgto] asc
));

/*LinxNfceEstacao: cadastro de estações. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxNfceEstacao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id_nfce_estacao]          int,
	[empresa]                  int,
	[descricao]                nvarchar(50),
	[numero_pdv_tef]           nvarchar(20),
	[ativo]                    bit,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxNfceEstacao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id_nfce_estacao]          int not null,
	[empresa]                  int,
	[descricao]                nvarchar(50),
	[numero_pdv_tef]           nvarchar(20),
	[ativo]                    bit,
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_LinxNfceEstacao] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_nfce_estacao] asc
));

/*LinxSeries: as séries cadastradas no portal. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxSeries (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[serie]                    nvarchar(10),
	[id_modelo_nf]             int,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxSeries (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[serie]                    nvarchar(10) not null,
	[id_modelo_nf]             int not null,
	[timestamp]                bigint,
constraint [PK_LinxSeries] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[serie] asc,
	[id_modelo_nf] asc
));
