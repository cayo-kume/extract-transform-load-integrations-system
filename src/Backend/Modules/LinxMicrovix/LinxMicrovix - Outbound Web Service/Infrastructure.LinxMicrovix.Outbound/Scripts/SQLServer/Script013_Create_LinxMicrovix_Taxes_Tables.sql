/****** Tabelas fiscais (LinxMicrovixTaxesService): tabelas untreated e linxmicrovix (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxNaturezaOperacao: as naturezas de operação.*/

create table [untreated].LinxNaturezaOperacao (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[portal]                     int,
	[cod_natureza_operacao]      nvarchar(10),
	[descricao]                  nvarchar(60),
	[soma_relatorios]            nvarchar(1),
	[operacao]                   nvarchar(2),
	[inativa]                    nvarchar(1),
	[timestamp]                  bigint,
	[calcula_ipi]                bit,
	[calcula_iss]                bit,
	[calcula_irrf]               bit,
	[tipo_preco]                 nvarchar(2),
	[atualiza_custo]             nvarchar(1),
	[transferencia]              bit,
	[baixar_estoque]             bit,
	[consumo_proprio]            bit,
	[contabiliza_cmv]            bit,
	[despesa]                    bit,
	[atualiza_custo_medio]       bit,
	[exige_nf_origem]            bit,
	[integra_contabilidade]      bit,
	[id_obs]                     int,
	[venda_futura]               bit,
	[base_icms_considera_ipi]    bit,
	[permite_escolha_historico]  bit,
	[import_produtos]            bit,
	[deposito_reserva_venda]     int,
	[exibe_nfe]                  bit,
	[faturamento_antecipado]     bit,
	[exibir_informacoes_imposto] bit,
	[gera_garantia_nacional]     bit,
	[transferencia_deposito]     bit,
	[venda_diferencial_aliquota] bit,
	[insere_obs_pis_cofins]      bit,
	[diferencial_ativo_consumo]  bit,
	[recusa_de]                  bit,
	[codigo_ws]                  nvarchar(50)
);

create table [linxmicrovix].LinxNaturezaOperacao (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[portal]                     int not null,
	[cod_natureza_operacao]      nvarchar(10) not null,
	[descricao]                  nvarchar(60),
	[soma_relatorios]            nvarchar(1),
	[operacao]                   nvarchar(2),
	[inativa]                    nvarchar(1),
	[timestamp]                  bigint,
	[calcula_ipi]                bit,
	[calcula_iss]                bit,
	[calcula_irrf]               bit,
	[tipo_preco]                 nvarchar(2),
	[atualiza_custo]             nvarchar(1),
	[transferencia]              bit,
	[baixar_estoque]             bit,
	[consumo_proprio]            bit,
	[contabiliza_cmv]            bit,
	[despesa]                    bit,
	[atualiza_custo_medio]       bit,
	[exige_nf_origem]            bit,
	[integra_contabilidade]      bit,
	[id_obs]                     int,
	[venda_futura]               bit,
	[base_icms_considera_ipi]    bit,
	[permite_escolha_historico]  bit,
	[import_produtos]            bit,
	[deposito_reserva_venda]     int,
	[exibe_nfe]                  bit,
	[faturamento_antecipado]     bit,
	[exibir_informacoes_imposto] bit,
	[gera_garantia_nacional]     bit,
	[transferencia_deposito]     bit,
	[venda_diferencial_aliquota] bit,
	[insere_obs_pis_cofins]      bit,
	[diferencial_ativo_consumo]  bit,
	[recusa_de]                  bit,
	[codigo_ws]                  nvarchar(50),
constraint [PK_LinxNaturezaOperacao] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_natureza_operacao] asc
));

/*LinxCfopFiscal: o cadastro da CFOP Fiscal*/

create table [untreated].LinxCfopFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_cfop_fiscal]           int,
	[cfop_fiscal]              nvarchar(5),
	[descricao]                nvarchar(300),
	[excluido]                 bit,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxCfopFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_cfop_fiscal]           int not null,
	[cfop_fiscal]              nvarchar(5),
	[descricao]                nvarchar(300),
	[excluido]                 bit,
	[timestamp]                bigint,
constraint [PK_LinxCfopFiscal] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_cfop_fiscal] asc
));

/*LinxCstIcmsFiscal: o cadastro de cst icms fiscal*/

create table [untreated].LinxCstIcmsFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_cst_icms_fiscal]       int,
	[cst_icms_fiscal]          nvarchar(4),
	[descricao]                nvarchar(150),
	[substituicao_tributaria]  bit,
	[excluido]                 bit,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxCstIcmsFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_cst_icms_fiscal]       int not null,
	[cst_icms_fiscal]          nvarchar(4),
	[descricao]                nvarchar(150),
	[substituicao_tributaria]  bit,
	[excluido]                 bit,
	[timestamp]                bigint,
constraint [PK_LinxCstIcmsFiscal] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_cst_icms_fiscal] asc
));

/*LinxCstIpiFiscal: o conteúdo da tabela cst_ipi_fiscal Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxCstIpiFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_cst_ipi_fiscal]        int,
	[cst_ipi_fiscal]           nvarchar(4),
	[descricao]                nvarchar(150),
	[excluido]                 bit,
	[inicio_vigencia]          datetime,
	[termino_vigencia]         datetime,
	[cst_ipi_fiscal_inverso]   nvarchar(4),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxCstIpiFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_cst_ipi_fiscal]        int not null,
	[cst_ipi_fiscal]           nvarchar(4),
	[descricao]                nvarchar(150),
	[excluido]                 bit,
	[inicio_vigencia]          datetime,
	[termino_vigencia]         datetime,
	[cst_ipi_fiscal_inverso]   nvarchar(4),
	[timestamp]                bigint,
constraint [PK_LinxCstIpiFiscal] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_cst_ipi_fiscal] asc
));

/*LinxCstPisFiscal: o conteúdo da tabela cst_pis_fiscal*/

create table [untreated].LinxCstPisFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_cst_pis_fiscal]        int,
	[cst_pis_fiscal]           nvarchar(4),
	[descricao]                nvarchar(150),
	[excluido]                 bit,
	[inicio_vigencia]          datetime,
	[termino_vigencia]         datetime,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxCstPisFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_cst_pis_fiscal]        int not null,
	[cst_pis_fiscal]           nvarchar(4),
	[descricao]                nvarchar(150),
	[excluido]                 bit,
	[inicio_vigencia]          datetime,
	[termino_vigencia]         datetime,
	[timestamp]                bigint,
constraint [PK_LinxCstPisFiscal] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_cst_pis_fiscal] asc
));

/*LinxCstCofinsFiscal: o conteúdo da tabela cst_cofins_fiscal*/

create table [untreated].LinxCstCofinsFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_cst_cofins_fiscal]     int,
	[cst_cofins_fiscal]        nvarchar(4),
	[descricao]                nvarchar(150),
	[excluido]                 bit,
	[inicio_vigencia]          datetime,
	[termino_vigencia]         datetime,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxCstCofinsFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_cst_cofins_fiscal]     int not null,
	[cst_cofins_fiscal]        nvarchar(4),
	[descricao]                nvarchar(150),
	[excluido]                 bit,
	[inicio_vigencia]          datetime,
	[termino_vigencia]         datetime,
	[timestamp]                bigint,
constraint [PK_LinxCstCofinsFiscal] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_cst_cofins_fiscal] asc
));

/*LinxCsosnFiscal: o cadastro de csosn fiscal*/

create table [untreated].LinxCsosnFiscal (
	[integrated_or_updated_at]     datetime,
	[doc_company]                  nvarchar(14),
	[portal]                       int,
	[id_csosn_fiscal]              int,
	[csosn_fiscal]                 nvarchar(5),
	[descricao]                    nvarchar(200),
	[id_csosn_fiscal_substitutiva] int,
	[timestamp]                    bigint
);

create table [linxmicrovix].LinxCsosnFiscal (
	[integrated_or_updated_at]     datetime,
	[doc_company]                  nvarchar(14) not null,
	[portal]                       int not null,
	[id_csosn_fiscal]              int not null,
	[csosn_fiscal]                 nvarchar(5),
	[descricao]                    nvarchar(200),
	[id_csosn_fiscal_substitutiva] int,
	[timestamp]                    bigint,
constraint [PK_LinxCsosnFiscal] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_csosn_fiscal] asc
));

/*LinxClasseFiscal: o cadastro da classe fiscal.*/

create table [untreated].LinxClasseFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_classe_fiscal]         int,
	[descricao]                nvarchar(150),
	[excluido]                 bit,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxClasseFiscal (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_classe_fiscal]         int not null,
	[descricao]                nvarchar(150),
	[excluido]                 bit,
	[timestamp]                bigint,
constraint [PK_LinxClasseFiscal] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_classe_fiscal] asc
));

/*LinxCest: o conteúdo da tabela classificacao_cest_produto*/

create table [untreated].LinxCest (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[portal]                     int,
	[id_cest]                    int,
	[descricao]                  nvarchar(700),
	[cest]                       nvarchar(10),
	[id_segmento_mercadoria_bem] int,
	[ativo]                      bit,
	[timestamp]                  bigint
);

create table [linxmicrovix].LinxCest (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[portal]                     int not null,
	[id_cest]                    int not null,
	[descricao]                  nvarchar(700),
	[cest]                       nvarchar(10),
	[id_segmento_mercadoria_bem] int,
	[ativo]                      bit,
	[timestamp]                  bigint,
constraint [PK_LinxCest] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_cest] asc
));

/*LinxNcm: o conteúdo da tabela classificacao_ncm_produto.*/

create table [untreated].LinxNcm (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[codigo]                   nvarchar(20),
	[descricao]                nvarchar(60),
	[codigo_genero]            int,
	[ativo]                    bit,
	[id_ncm]                   int,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxNcm (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[codigo]                   nvarchar(20) not null,
	[descricao]                nvarchar(60),
	[codigo_genero]            int,
	[ativo]                    bit,
	[id_ncm]                   int,
	[timestamp]                bigint,
constraint [PK_LinxNcm] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo] asc
));

/*LinxEnquadramentoIpi: o conteúdo da tabela enquadramento_ipi*/

create table [untreated].LinxEnquadramentoIpi (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_enquadramento_ipi]     int,
	[codigo]                   nvarchar(3),
	[descricao]                nvarchar(600),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxEnquadramentoIpi (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_enquadramento_ipi]     int not null,
	[codigo]                   nvarchar(3),
	[descricao]                nvarchar(600),
	[timestamp]                bigint,
constraint [PK_LinxEnquadramentoIpi] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_enquadramento_ipi] asc
));

/*LinxMotivosDesoneracaoIcms: o conteúdo da tabela motivos_desoneracao_icms Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxMotivosDesoneracaoIcms (
	[integrated_or_updated_at]    datetime,
	[doc_company]                 nvarchar(14),
	[portal]                      int,
	[id_motivos_desoneracao_icms] int,
	[descricao]                   nvarchar(200),
	[timestamp]                   bigint
);

create table [linxmicrovix].LinxMotivosDesoneracaoIcms (
	[integrated_or_updated_at]    datetime,
	[doc_company]                 nvarchar(14) not null,
	[portal]                      int not null,
	[id_motivos_desoneracao_icms] int not null,
	[descricao]                   nvarchar(200),
	[timestamp]                   bigint,
constraint [PK_LinxMotivosDesoneracaoIcms] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_motivos_desoneracao_icms] asc
));

/*LinxSpedTipoItem: dados do Sped tipo item. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxSpedTipoItem (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_sped_tipo_item]        int,
	[codigo]                   nvarchar(2),
	[descricao]                nvarchar(60),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxSpedTipoItem (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_sped_tipo_item]        int not null,
	[codigo]                   nvarchar(2),
	[descricao]                nvarchar(60),
	[timestamp]                bigint,
constraint [PK_LinxSpedTipoItem] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_sped_tipo_item] asc
));

/*LinxSpedTipoBaseCredito: o conteúdo da tabela sped_tipo_base_credito. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxSpedTipoBaseCredito (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14),
	[portal]                        int,
	[id_sped_tipo_base_credito]     int,
	[codigo_sped_tipo_base_credito] nvarchar(2),
	[descricao]                     nvarchar(150),
	[timestamp]                     bigint
);

create table [linxmicrovix].LinxSpedTipoBaseCredito (
	[integrated_or_updated_at]      datetime,
	[doc_company]                   nvarchar(14) not null,
	[portal]                        int not null,
	[id_sped_tipo_base_credito]     int not null,
	[codigo_sped_tipo_base_credito] nvarchar(2),
	[descricao]                     nvarchar(150),
	[timestamp]                     bigint,
constraint [PK_LinxSpedTipoBaseCredito] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_sped_tipo_base_credito] asc
));

/*LinxRotinaOrigem: retornas as rotinas origem cadastradas.*/

create table [untreated].LinxRotinaOrigem (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[codigo_rotina]            int,
	[descricao_rotina]         nvarchar(150),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxRotinaOrigem (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[codigo_rotina]            int not null,
	[descricao_rotina]         nvarchar(150),
	[timestamp]                bigint,
constraint [PK_LinxRotinaOrigem] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo_rotina] asc
));

/*LinxNfeIbgeMunicipio: dados do IBGE sobre municípios.*/

create table [untreated].LinxNfeIbgeMunicipio (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_nfe_ibge_municipio]    int,
	[cod_ibge_municipio]       int,
	[nome]                     nvarchar(100), --fora da documentação
	[cod_ibge_uf]              int,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxNfeIbgeMunicipio (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_nfe_ibge_municipio]    int not null,
	[cod_ibge_municipio]       int,
	[nome]                     nvarchar(100), --fora da documentação
	[cod_ibge_uf]              int,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxNfeIbgeMunicipio] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_nfe_ibge_municipio] asc
));
