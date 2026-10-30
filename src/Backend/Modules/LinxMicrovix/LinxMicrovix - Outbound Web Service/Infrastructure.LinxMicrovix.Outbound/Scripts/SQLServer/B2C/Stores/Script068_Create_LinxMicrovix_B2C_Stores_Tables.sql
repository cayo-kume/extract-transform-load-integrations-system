/****** Empresas, vendedores, transportadores e planos do B2C (LinxMicrovixB2CStoresService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da V64; respostas planas: uma tabela por método, sem filhas

/*B2CConsultaEmpresas:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaEmpresas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[empresa]                  int,
	[nome_emp]                 nvarchar(50),
	[cnpjEmp]                  nvarchar(14),
	[end_unidade]              nvarchar(250),
	[complemento_end_unidade]  nvarchar(60),
	[nr_rua_unidade]           nvarchar(20),
	[bairro_unidade]           nvarchar(60),
	[cep_unidade]              nvarchar(9),
	[cidade_unidade]           nvarchar(50),
	[uf_unidade]               nvarchar(2),
	[email_unidade]            nvarchar(50),
	[timestamp]                bigint,
	[data_criacao]             datetime,
	[centro_distribuicao]      bit,
	[portal]                   int,
	[envia_produtos_b2c]       bit,
	[envia_estoque_b2c]        bit,
	[integracao_oms]           bit
);

create table [linxmicrovix].B2CConsultaEmpresas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[empresa]                  int not null,
	[nome_emp]                 nvarchar(50),
	[cnpjEmp]                  nvarchar(14),
	[end_unidade]              nvarchar(250),
	[complemento_end_unidade]  nvarchar(60),
	[nr_rua_unidade]           nvarchar(20),
	[bairro_unidade]           nvarchar(60),
	[cep_unidade]              nvarchar(9),
	[cidade_unidade]           nvarchar(50),
	[uf_unidade]               nvarchar(2),
	[email_unidade]            nvarchar(50),
	[timestamp]                bigint,
	[data_criacao]             datetime,
	[centro_distribuicao]      bit,
	[portal]                   int,
	[envia_produtos_b2c]       bit,
	[envia_estoque_b2c]        bit,
	[integracao_oms]           bit,
constraint [PK_B2CConsultaEmpresas] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[empresa] asc
));

/*B2CConsultaCNPJsChave: */

create table [untreated].B2CConsultaCNPJsChave (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[CNPJ]                     nvarchar(14),
	[nome_empresa]             nvarchar(300),
	[id_empresas_rede]         int,
	[rede]                     nvarchar(100),
	[portal]                   int,
	[nome_portal]              nvarchar(50),
	[empresa]                  int,
	[classificacao_portal]     nvarchar(50),
	[b2c]                      bit,
	[oms]                      bit,
	[b2c_marketplace]          bit
);

create table [linxmicrovix].B2CConsultaCNPJsChave (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[CNPJ]                     nvarchar(14) not null,
	[nome_empresa]             nvarchar(300),
	[id_empresas_rede]         int,
	[rede]                     nvarchar(100),
	[portal]                   int not null,
	[nome_portal]              nvarchar(50),
	[empresa]                  int,
	[classificacao_portal]     nvarchar(50),
	[b2c]                      bit,
	[oms]                      bit,
	[b2c_marketplace]          bit,
constraint [PK_B2CConsultaCNPJsChave] primary key (
	[doc_company] asc,
	[portal] asc,
	[CNPJ] asc
));

/*B2CConsultaVendedores: */

create table [untreated].B2CConsultaVendedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cod_vendedor]             int,
	[nome_vendedor]            nvarchar(50),
	[comissao_produtos]        decimal(19,4),
	[comissao_servicos]        decimal(19,4),
	[tipo]                     nvarchar(1),
	[ativo]                    bit,
	[comissionado]             int, --fora da documentação
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaVendedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cod_vendedor]             int not null,
	[nome_vendedor]            nvarchar(50),
	[comissao_produtos]        decimal(19,4),
	[comissao_servicos]        decimal(19,4),
	[tipo]                     nvarchar(1),
	[ativo]                    bit,
	[comissionado]             int, --fora da documentação
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaVendedores] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_vendedor] asc
));

/*B2CConsultaTransportadores: */

create table [untreated].B2CConsultaTransportadores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cod_transportador]        int,
	[nome]                     nvarchar(250), --fora da documentação
	[nome_fantasia]            nvarchar(60),
	[tipo_pessoa]              nvarchar(1),
	[tipo_transportador]       nvarchar(1),
	[endereco]                 nvarchar(250),
	[numero_rua]               nvarchar(20),
	[bairro]                   nvarchar(60),
	[cep]                      nvarchar(9),
	[cidade]                   nvarchar(40),
	[uf]                       nvarchar(20),
	[documento]                nvarchar(14),
	[fone]                     nvarchar(20),
	[email]                    nvarchar(50),
	[pais]                     nvarchar(80),
	[obs]                      nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaTransportadores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cod_transportador]        int not null,
	[nome]                     nvarchar(250), --fora da documentação
	[nome_fantasia]            nvarchar(60),
	[tipo_pessoa]              nvarchar(1),
	[tipo_transportador]       nvarchar(1),
	[endereco]                 nvarchar(250),
	[numero_rua]               nvarchar(20),
	[bairro]                   nvarchar(60),
	[cep]                      nvarchar(9),
	[cidade]                   nvarchar(40),
	[uf]                       nvarchar(20),
	[documento]                nvarchar(14),
	[fone]                     nvarchar(20),
	[email]                    nvarchar(50),
	[pais]                     nvarchar(80),
	[obs]                      nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaTransportadores] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_transportador] asc
));

/*B2CConsultaFormasPagamento:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaFormasPagamento (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cod_forma_pgto]           int,
	[forma_pgto]               nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaFormasPagamento (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cod_forma_pgto]           int not null,
	[forma_pgto]               nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaFormasPagamento] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_forma_pgto] asc
));

/*B2CConsultaPlanos: */

create table [untreated].B2CConsultaPlanos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[plano]                    int,
	[nome_plano]               nvarchar(100),
	[forma_pagamento]          int,
	[qtde_parcelas]            int,
	[valor_minimo_parcela]     decimal(19,4),
	[indice]                   decimal(19,4),
	[timestamp]                bigint,
	[desativado]               nvarchar(1),
	[tipo_plano]               nvarchar(1),
	[portal]                   int,
	[usa_tef]                  nvarchar(1)
);

create table [linxmicrovix].B2CConsultaPlanos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[plano]                    int not null,
	[nome_plano]               nvarchar(100),
	[forma_pagamento]          int,
	[qtde_parcelas]            int,
	[valor_minimo_parcela]     decimal(19,4),
	[indice]                   decimal(19,4),
	[timestamp]                bigint,
	[desativado]               nvarchar(1),
	[tipo_plano]               nvarchar(1),
	[portal]                   int not null,
	[usa_tef]                  nvarchar(1),
constraint [PK_B2CConsultaPlanos] primary key (
	[doc_company] asc,
	[portal] asc,
	[plano] asc
));

/*B2CConsultaPlanosParcelas: */

create table [untreated].B2CConsultaPlanosParcelas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[plano]                    int,
	[ordem_parcela]            int,
	[prazo_parc]               int,
	[id_planos_parcelas]       int,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaPlanosParcelas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[plano]                    int,
	[ordem_parcela]            int,
	[prazo_parc]               int,
	[id_planos_parcelas]       int not null,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaPlanosParcelas] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_planos_parcelas] asc
));

/*B2CConsultaTiposCobrancaFrete:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaTiposCobrancaFrete (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[codigo_tipo_cobranca_frete] int,
	[nome_tipo_cobranca_frete]   nvarchar(100), --fora da documentação
	[timestamp]                  bigint,
	[portal]                     int
);

create table [linxmicrovix].B2CConsultaTiposCobrancaFrete (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[codigo_tipo_cobranca_frete] int not null,
	[nome_tipo_cobranca_frete]   nvarchar(100), --fora da documentação
	[timestamp]                  bigint,
	[portal]                     int not null,
constraint [PK_B2CConsultaTiposCobrancaFrete] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigo_tipo_cobranca_frete] asc
));

/*B2CConsultaTipoEncomenda:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaTipoEncomenda (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_tipo_encomenda]        int,
	[nm_tipo_encomenda]        nvarchar(100),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaTipoEncomenda (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_tipo_encomenda]        int not null,
	[nm_tipo_encomenda]        nvarchar(100),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaTipoEncomenda] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_tipo_encomenda] asc
));
