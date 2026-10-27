/****** Clientes do B2C (LinxMicrovixB2CCustomersService): tabelas untreated e linxmicrovix (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da V64; respostas planas: uma tabela por método, sem filhas

/*B2CConsultaClientes: */

create table [untreated].B2CConsultaClientes (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[cod_cliente_b2c]            int,
	[cod_cliente_erp]            int,
	[doc_cliente]                nvarchar(14),
	[nm_cliente]                 nvarchar(250), --50 na documentação; a resposta real tem até 60
	[nm_mae]                     nvarchar(50),
	[nm_pai]                     nvarchar(50),
	[nm_conjuge]                 nvarchar(50),
	[dt_cadastro]                datetime,
	[dt_nasc_cliente]            datetime,
	[end_cliente]                nvarchar(250),
	[complemento_end_cliente]    nvarchar(250), --50 na documentação; a resposta real tem até 60
	[nr_rua_cliente]             nvarchar(20),
	[bairro_cliente]             nvarchar(60),
	[cep_cliente]                nvarchar(9),
	[cidade_cliente]             nvarchar(40),
	[uf_cliente]                 nvarchar(2),
	[fone_cliente]               nvarchar(20),
	[fone_comercial]             nvarchar(20),
	[cel_cliente]                nvarchar(20),
	[email_cliente]              nvarchar(50),
	[rg_cliente]                 nvarchar(20),
	[rg_orgao_emissor]           nvarchar(7),
	[estado_civil_cliente]       int,
	[empresa_cliente]            nvarchar(30),
	[cargo_cliente]              nvarchar(30),
	[sexo_cliente]               nvarchar(1),
	[dt_update]                  datetime,
	[ativo]                      bit,
	[receber_email]              bit,
	[dt_expedicao_rg]            datetime,
	[naturalidade]               nvarchar(40),
	[tempo_residencia]           int,
	[renda]                      decimal(19,4),
	[numero_compl_rua_cliente]   nvarchar(10),
	[timestamp]                  bigint,
	[tipo_pessoa]                nvarchar(1),
	[portal]                     int,
	[aceita_programa_fidelidade] bit,
	[inscricao_cliente]          nvarchar(20)
);

create table [linxmicrovix].B2CConsultaClientes (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[cod_cliente_b2c]            int,
	[cod_cliente_erp]            int not null,
	[doc_cliente]                nvarchar(14),
	[nm_cliente]                 nvarchar(250), --50 na documentação; a resposta real tem até 60
	[nm_mae]                     nvarchar(50),
	[nm_pai]                     nvarchar(50),
	[nm_conjuge]                 nvarchar(50),
	[dt_cadastro]                datetime,
	[dt_nasc_cliente]            datetime,
	[end_cliente]                nvarchar(250),
	[complemento_end_cliente]    nvarchar(250), --50 na documentação; a resposta real tem até 60
	[nr_rua_cliente]             nvarchar(20),
	[bairro_cliente]             nvarchar(60),
	[cep_cliente]                nvarchar(9),
	[cidade_cliente]             nvarchar(40),
	[uf_cliente]                 nvarchar(2),
	[fone_cliente]               nvarchar(20),
	[fone_comercial]             nvarchar(20),
	[cel_cliente]                nvarchar(20),
	[email_cliente]              nvarchar(50),
	[rg_cliente]                 nvarchar(20),
	[rg_orgao_emissor]           nvarchar(7),
	[estado_civil_cliente]       int,
	[empresa_cliente]            nvarchar(30),
	[cargo_cliente]              nvarchar(30),
	[sexo_cliente]               nvarchar(1),
	[dt_update]                  datetime,
	[ativo]                      bit,
	[receber_email]              bit,
	[dt_expedicao_rg]            datetime,
	[naturalidade]               nvarchar(40),
	[tempo_residencia]           int,
	[renda]                      decimal(19,4),
	[numero_compl_rua_cliente]   nvarchar(10),
	[timestamp]                  bigint,
	[tipo_pessoa]                nvarchar(1),
	[portal]                     int not null,
	[aceita_programa_fidelidade] bit,
	[inscricao_cliente]          nvarchar(20),
constraint [PK_B2CConsultaClientes] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_cliente_erp] asc
));

/*B2CConsultaClientesContatos:  Colunas da documentação: a chave da Newbloomers não tem acesso ao método. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaClientesContatos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id_clientes_contatos]     int,
	[id_contato_b2c]           int,
	[nome_contato]             nvarchar(50),
	[data_nasc_contato]        datetime,
	[sexo_contato]             nvarchar(1),
	[id_parentesco]            int,
	[fone_contato]             nvarchar(20),
	[celular_contato]          nvarchar(20),
	[email_contato]            nvarchar(50),
	[cod_cliente_erp]          int,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaClientesContatos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id_clientes_contatos]     int not null,
	[id_contato_b2c]           int,
	[nome_contato]             nvarchar(50),
	[data_nasc_contato]        datetime,
	[sexo_contato]             nvarchar(1),
	[id_parentesco]            int,
	[fone_contato]             nvarchar(20),
	[celular_contato]          nvarchar(20),
	[email_contato]            nvarchar(50),
	[cod_cliente_erp]          int,
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_B2CConsultaClientesContatos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_clientes_contatos] asc
));

/*B2CConsultaClientesContatosParentesco:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaClientesContatosParentesco (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_parentesco]            int,
	[descricao_parentesco]     nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaClientesContatosParentesco (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_parentesco]            int not null,
	[descricao_parentesco]     nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaClientesContatosParentesco] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_parentesco] asc
));

/*B2CConsultaClientesEnderecosEntrega:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaClientesEnderecosEntrega (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_endereco_entrega]      int,
	[cod_cliente_erp]          int,
	[cod_cliente_b2c]          int,
	[endereco_cliente]         nvarchar(250),
	[numero_rua_cliente]       nvarchar(20),
	[complemento_end_cli]      nvarchar(60),
	[cep_cliente]              nvarchar(9),
	[bairro_cliente]           nvarchar(60),
	[cidade_cliente]           nvarchar(40),
	[uf_cliente]               nvarchar(2),
	[descricao]                nvarchar(250),
	[principal]                bit,
	[id_cidade]                int,
	[timestamp]                bigint,
	[id_endereco_entrega_b2c]  bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaClientesEnderecosEntrega (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_endereco_entrega]      int not null,
	[cod_cliente_erp]          int,
	[cod_cliente_b2c]          int,
	[endereco_cliente]         nvarchar(250),
	[numero_rua_cliente]       nvarchar(20),
	[complemento_end_cli]      nvarchar(60),
	[cep_cliente]              nvarchar(9),
	[bairro_cliente]           nvarchar(60),
	[cidade_cliente]           nvarchar(40),
	[uf_cliente]               nvarchar(2),
	[descricao]                nvarchar(250),
	[principal]                bit,
	[id_cidade]                int,
	[timestamp]                bigint,
	[id_endereco_entrega_b2c]  bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaClientesEnderecosEntrega] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_endereco_entrega] asc
));

/*B2CConsultaClientesEstadoCivil:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaClientesEstadoCivil (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_estado_civil]          int,
	[estado_civil]             nvarchar(20),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaClientesEstadoCivil (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_estado_civil]          int not null,
	[estado_civil]             nvarchar(20),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaClientesEstadoCivil] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_estado_civil] asc
));

/*B2CConsultaClientesSaldo:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaClientesSaldo (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[saldo]                    decimal(19,4),
	[cod_cliente_erp]          int,
	[empresa]                  int,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaClientesSaldo (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[saldo]                    decimal(19,4),
	[cod_cliente_erp]          int not null,
	[empresa]                  int not null,
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_B2CConsultaClientesSaldo] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_cliente_erp] asc,
	[empresa] asc
));

/*B2CConsultaClientesSaldoLinx:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaClientesSaldoLinx (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[cod_cliente_erp]          int,
	[cod_cliente_b2c]          int,
	[empresa]                  int,
	[valor]                    decimal(19,4),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaClientesSaldoLinx (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[cod_cliente_erp]          int not null,
	[cod_cliente_b2c]          int,
	[empresa]                  int not null,
	[valor]                    decimal(19,4),
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_B2CConsultaClientesSaldoLinx] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_cliente_erp] asc,
	[empresa] asc
));
