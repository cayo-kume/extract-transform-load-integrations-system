/****** Vendedores e funcionários (LinxMicrovixSellersService): tabelas untreated e linxmicrovix (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxVendedores: os Vendedores cadastrados na loja*/

create table [untreated].LinxVendedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_vendedor]             int,
	[nome_vendedor]            nvarchar(50),
	[tipo_vendedor]            nvarchar(1),
	[end_vend_rua]             nvarchar(250),
	[end_vend_numero]          int,
	[end_vend_complemento]     nvarchar(60),
	[end_vend_bairro]          nvarchar(60),
	[end_vend_cep]             nvarchar(9),
	[end_vend_cidade]          nvarchar(60),
	[end_vend_uf]              nvarchar(2),
	[fone_vendedor]            nvarchar(30),
	[mail_vendedor]            nvarchar(50),
	[dt_upd]                   datetime,
	[cpf_vendedor]             nvarchar(11),
	[ativo]                    nvarchar(1),
	[data_admissao]            datetime,
	[data_saida]               datetime,
	[timestamp]                bigint,
	[matricula]                nvarchar(30),
	[id_tipo_venda]            int,
	[descricao_tipo_venda]     nvarchar(100),
	[cargo]                    nvarchar(20),
	[funcao]                   nvarchar(100),
	[nome_exibicao]            nvarchar(15),
	[empresa]                  int,
	[codigo_integracao_ws]     nvarchar(50),
	[meta_peso]                decimal(19,4)
);

create table [linxmicrovix].LinxVendedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_vendedor]             int not null,
	[nome_vendedor]            nvarchar(50),
	[tipo_vendedor]            nvarchar(1),
	[end_vend_rua]             nvarchar(250),
	[end_vend_numero]          int,
	[end_vend_complemento]     nvarchar(60),
	[end_vend_bairro]          nvarchar(60),
	[end_vend_cep]             nvarchar(9),
	[end_vend_cidade]          nvarchar(60),
	[end_vend_uf]              nvarchar(2),
	[fone_vendedor]            nvarchar(30),
	[mail_vendedor]            nvarchar(50),
	[dt_upd]                   datetime,
	[cpf_vendedor]             nvarchar(11),
	[ativo]                    nvarchar(1),
	[data_admissao]            datetime,
	[data_saida]               datetime,
	[timestamp]                bigint,
	[matricula]                nvarchar(30),
	[id_tipo_venda]            int,
	[descricao_tipo_venda]     nvarchar(100),
	[cargo]                    nvarchar(20),
	[funcao]                   nvarchar(100),
	[nome_exibicao]            nvarchar(15),
	[empresa]                  int,
	[codigo_integracao_ws]     nvarchar(50),
	[meta_peso]                decimal(19,4),
constraint [PK_LinxVendedores] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_vendedor] asc
));

/*LinxVendedoresFerias: as férias cadatradas para os vendedores. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxVendedoresFerias (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_vendedor]             int,
	[data_inicial]             datetime,
	[data_final]               datetime,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxVendedoresFerias (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_vendedor]             int not null,
	[data_inicial]             datetime not null,
	[data_final]               datetime,
	[timestamp]                bigint,
constraint [PK_LinxVendedoresFerias] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_vendedor] asc,
	[data_inicial] asc
));

/*LinxMetasVendedores: as Metas de Vendedores das lojas Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxMetasVendedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_meta]                  int,
	[descricao_meta]           nvarchar(50),
	[data_inicial_meta]        datetime,
	[data_final_meta]          datetime,
	[valor_meta_loja]          decimal(19,4),
	[valor_meta_vendedor]      decimal(19,4),
	[cod_vendedor]             int,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxMetasVendedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_meta]                  int not null,
	[descricao_meta]           nvarchar(50),
	[data_inicial_meta]        datetime,
	[data_final_meta]          datetime,
	[valor_meta_loja]          decimal(19,4),
	[valor_meta_vendedor]      decimal(19,4),
	[cod_vendedor]             int not null,
	[timestamp]                bigint,
constraint [PK_LinxMetasVendedores] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_meta] asc,
	[cod_vendedor] asc
));

/*LinxMetasVendedoresDia: as Metas de Vendedores das lojas por dia Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxMetasVendedoresDia (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_meta]                  int,
	[descricao_meta]           nvarchar(50),
	[data_inicial_meta]        datetime,
	[data_final_meta]          datetime,
	[dia]                      datetime,
	[valor_meta_diaria]        decimal(19,4),
	[cod_vendedor]             int,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxMetasVendedoresDia (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_meta]                  int not null,
	[descricao_meta]           nvarchar(50),
	[data_inicial_meta]        datetime,
	[data_final_meta]          datetime,
	[dia]                      datetime not null,
	[valor_meta_diaria]        decimal(19,4),
	[cod_vendedor]             int not null,
	[timestamp]                bigint,
constraint [PK_LinxMetasVendedoresDia] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_meta] asc,
	[cod_vendedor] asc,
	[dia] asc
));

/*LinxComissoesVendedores: as comissões do vendedores*/

create table [untreated].LinxComissoesVendedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[vendedor]                 int,
	[empresa]                  int,
	[data_origem]              datetime,
	[valor_base]               decimal(28,8),
	[valor_comissao]           decimal(28,8),
	[cancelado]                nvarchar(1),
	[disponivel]               nvarchar(1),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxComissoesVendedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[vendedor]                 int not null,
	[empresa]                  int not null,
	[data_origem]              datetime not null,
	[valor_base]               decimal(28,8) not null,
	[valor_comissao]           decimal(28,8) not null,
	[cancelado]                nvarchar(1) not null,
	[disponivel]               nvarchar(1),
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_LinxComissoesVendedores] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[vendedor] asc,
	[empresa] asc,
	[data_origem] asc,
	[valor_base] asc,
	[valor_comissao] asc,
	[cancelado] asc
));

/*LinxListaDaVez: os registros gerados na rotina de Lista da Vez*/

create table [untreated].LinxListaDaVez (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_vendedor]             int,
	[data]                     datetime,
	[motivo_nao_venda]         nvarchar(103),
	[qtde_ocorrencias]         int,
	[data_hora_ini_atend]      datetime,
	[data_hora_fim_atend]      datetime,
	[obs]                      nvarchar(max),
	[desc_produto_neg]         nvarchar(50),
	[valor_produto_neg]        decimal(19,4),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxListaDaVez (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_vendedor]             int not null,
	[data]                     datetime not null,
	[motivo_nao_venda]         nvarchar(103),
	[qtde_ocorrencias]         int,
	[data_hora_ini_atend]      datetime,
	[data_hora_fim_atend]      datetime,
	[obs]                      nvarchar(max),
	[desc_produto_neg]         nvarchar(50),
	[valor_produto_neg]        decimal(19,4),
	[timestamp]                bigint not null,
constraint [PK_LinxListaDaVez] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_vendedor] asc,
	[data] asc,
	[timestamp] asc
));

/*LinxFuncionariosFuncoes: o cadastro da funções de funcionários. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxFuncionariosFuncoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_funcionarios_funcao]   int,
	[descricao]                nvarchar(100),
	[ativo]                    bit,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxFuncionariosFuncoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_funcionarios_funcao]   int not null,
	[descricao]                nvarchar(100),
	[ativo]                    bit,
	[timestamp]                bigint,
constraint [PK_LinxFuncionariosFuncoes] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_funcionarios_funcao] asc
));

/*LinxRegistroProfissional: os registros da tabela tipo_registro_profissional. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxRegistroProfissional (
	[integrated_or_updated_at]             datetime,
	[doc_company]                          nvarchar(14),
	[cnpj_emp]                             nvarchar(14), --loja consultada
	[portal]                               int,
	[id_tipo_registro_profissional]        int,
	[descricao_tipo_registro_profissional] nvarchar(100),
	[ativo]                                bit,
	[excluido]                             bit,
	[timestamp]                            bigint
);

create table [linxmicrovix].LinxRegistroProfissional (
	[integrated_or_updated_at]             datetime,
	[doc_company]                          nvarchar(14) not null,
	[cnpj_emp]                             nvarchar(14) not null, --loja consultada
	[portal]                               int,
	[id_tipo_registro_profissional]        int not null,
	[descricao_tipo_registro_profissional] nvarchar(100),
	[ativo]                                bit,
	[excluido]                             bit,
	[timestamp]                            bigint,
constraint [PK_LinxRegistroProfissional] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_tipo_registro_profissional] asc
));
