/****** Clientes e fornecedores (LinxMicrovixCustomersService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxClientesFornec: os Clientes/Fornecedores cadastrados na loja e data pesquisada*/

create table [untreated].LinxClientesFornec (
	[integrated_or_updated_at]               datetime,
	[doc_company]                            nvarchar(14),
	[portal]                                 int,
	[cod_cliente]                            int,
	[razao_cliente]                          nvarchar(60),
	[nome_cliente]                           nvarchar(60),
	[doc_cliente]                            nvarchar(14),
	[tipo_cliente]                           nvarchar(1),
	[endereco_cliente]                       nvarchar(250),
	[numero_rua_cliente]                     nvarchar(20),
	[complement_end_cli]                     nvarchar(60),
	[bairro_cliente]                         nvarchar(60),
	[cep_cliente]                            nvarchar(9),
	[cidade_cliente]                         nvarchar(40),
	[uf_cliente]                             nvarchar(20),
	[pais]                                   nvarchar(80),
	[fone_cliente]                           nvarchar(20),
	[email_cliente]                          nvarchar(50),
	[sexo]                                   nvarchar(1),
	[data_cadastro]                          datetime,
	[data_nascimento]                        datetime,
	[cel_cliente]                            nvarchar(20),
	[ativo]                                  nvarchar(1),
	[dt_update]                              datetime,
	[inscricao_estadual]                     nvarchar(20),
	[incricao_municipal]                     nvarchar(20),
	[identidade_cliente]                     nvarchar(20),
	[cartao_fidelidade]                      nvarchar(20),
	[cod_ibge_municipio]                     int,
	[classe_cliente]                         nvarchar(83),
	[matricula_conveniado]                   nvarchar(20),
	[tipo_cadastro]                          nvarchar(1),
	[empresa_cadastro]                       int,
	[id_estado_civil]                        int,
	[fax_cliente]                            nvarchar(50),
	[site_cliente]                           nvarchar(50),
	[timestamp]                              bigint,
	[cliente_anonimo]                        bit,
	[limite_compras]                         decimal(28,8),
	[codigo_ws]                              nvarchar(100),
	[limite_credito_compra]                  decimal(28,8),
	[id_classe_fiscal]                       int,
	[obs]                                    nvarchar(max),
	[mae]                                    nvarchar(60),
	[cliente_contribuinte]                   bit,
	[id_tipo_registro_profissional]          int,
	[registro_profissional]                  nvarchar(50),
	[recebe_newsletter]                      bit,
	[diferencial_icms_fornec]                bit,
	[perc_diferencial_icms_fornec]           decimal(28,8),
	[id_classe_preferencial]                 int,
	[id_subclasse_preferencial]              int,
	[id_gestao_fiscal_perfil_tributario_crm] nvarchar(1000), --fora da documentação
	[fone_2]                                 nvarchar(20)
);

create table [linxmicrovix].LinxClientesFornec (
	[integrated_or_updated_at]               datetime,
	[doc_company]                            nvarchar(14) not null,
	[portal]                                 int not null,
	[cod_cliente]                            int not null,
	[razao_cliente]                          nvarchar(60),
	[nome_cliente]                           nvarchar(60),
	[doc_cliente]                            nvarchar(14),
	[tipo_cliente]                           nvarchar(1),
	[endereco_cliente]                       nvarchar(250),
	[numero_rua_cliente]                     nvarchar(20),
	[complement_end_cli]                     nvarchar(60),
	[bairro_cliente]                         nvarchar(60),
	[cep_cliente]                            nvarchar(9),
	[cidade_cliente]                         nvarchar(40),
	[uf_cliente]                             nvarchar(20),
	[pais]                                   nvarchar(80),
	[fone_cliente]                           nvarchar(20),
	[email_cliente]                          nvarchar(50),
	[sexo]                                   nvarchar(1),
	[data_cadastro]                          datetime,
	[data_nascimento]                        datetime,
	[cel_cliente]                            nvarchar(20),
	[ativo]                                  nvarchar(1),
	[dt_update]                              datetime,
	[inscricao_estadual]                     nvarchar(20),
	[incricao_municipal]                     nvarchar(20),
	[identidade_cliente]                     nvarchar(20),
	[cartao_fidelidade]                      nvarchar(20),
	[cod_ibge_municipio]                     int,
	[classe_cliente]                         nvarchar(83),
	[matricula_conveniado]                   nvarchar(20),
	[tipo_cadastro]                          nvarchar(1),
	[empresa_cadastro]                       int,
	[id_estado_civil]                        int,
	[fax_cliente]                            nvarchar(50),
	[site_cliente]                           nvarchar(50),
	[timestamp]                              bigint,
	[cliente_anonimo]                        bit,
	[limite_compras]                         decimal(28,8),
	[codigo_ws]                              nvarchar(100),
	[limite_credito_compra]                  decimal(28,8),
	[id_classe_fiscal]                       int,
	[obs]                                    nvarchar(max),
	[mae]                                    nvarchar(60),
	[cliente_contribuinte]                   bit,
	[id_tipo_registro_profissional]          int,
	[registro_profissional]                  nvarchar(50),
	[recebe_newsletter]                      bit,
	[diferencial_icms_fornec]                bit,
	[perc_diferencial_icms_fornec]           decimal(28,8),
	[id_classe_preferencial]                 int,
	[id_subclasse_preferencial]              int,
	[id_gestao_fiscal_perfil_tributario_crm] nvarchar(1000), --fora da documentação
	[fone_2]                                 nvarchar(20),
constraint [PK_LinxClientesFornec] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_cliente] asc
));

/*LinxClientesFornecCamposAdicionais: os campos adicionais preenchidos dos Clientes/Fornecedores cadastrados no portal Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxClientesFornecCamposAdicionais (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_cliente]              int,
	[campo]                    nvarchar(1000), --fora da documentação
	[valor]                    nvarchar(100),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxClientesFornecCamposAdicionais (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_cliente]              int not null,
	[campo]                    nvarchar(1000) not null, --fora da documentação
	[valor]                    nvarchar(100),
	[timestamp]                bigint,
constraint [PK_LinxClientesFornecCamposAdicionais] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_cliente] asc,
	[campo] asc
));

/*LinxClientesFornecClasses: todas as classes vinculadas a um cliente/fornecedor Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxClientesFornecClasses (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_cliente]              int,
	[cod_classe]               int,
	[nome_classe]              nvarchar(50),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxClientesFornecClasses (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_cliente]              int not null,
	[cod_classe]               int not null,
	[nome_classe]              nvarchar(50),
	[timestamp]                bigint,
constraint [PK_LinxClientesFornecClasses] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_cliente] asc,
	[cod_classe] asc
));

/*LinxClientesFornecContatos: os campos referentes ao contato do cadastro dos Clientes/Fornecedores cadastrados no portal*/

create table [untreated].LinxClientesFornecContatos (
	[integrated_or_updated_at]     datetime,
	[doc_company]                  nvarchar(14),
	[portal]                       int,
	[cod_cliente]                  int,
	[nome_contato]                 nvarchar(50),
	[sexo_contato]                 nvarchar(1),
	[contatos_clientes_parentesco] int,
	[fone1_contato]                nvarchar(20),
	[fone2_contato]                nvarchar(20),
	[celular_contato]              nvarchar(20),
	[email_contato]                nvarchar(50),
	[data_nasc_contato]            datetime,
	[tipo_contato]                 nvarchar(20),
	[timestamp]                    bigint
);

create table [linxmicrovix].LinxClientesFornecContatos (
	[integrated_or_updated_at]     datetime,
	[doc_company]                  nvarchar(14) not null,
	[portal]                       int not null,
	[cod_cliente]                  int not null,
	[nome_contato]                 nvarchar(50) not null,
	[sexo_contato]                 nvarchar(1),
	[contatos_clientes_parentesco] int,
	[fone1_contato]                nvarchar(20),
	[fone2_contato]                nvarchar(20),
	[celular_contato]              nvarchar(20),
	[email_contato]                nvarchar(50),
	[data_nasc_contato]            datetime,
	[tipo_contato]                 nvarchar(20) not null,
	[timestamp]                    bigint,
constraint [PK_LinxClientesFornecContatos] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_cliente] asc,
	[nome_contato] asc,
	[tipo_contato] asc
));

/*LinxClientesFornecContatosParentesco: os parentescos cadastrados Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxClientesFornecContatosParentesco (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_parentesco]            int,
	[descricao_parentesco]     nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxClientesFornecContatosParentesco (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_parentesco]            int not null,
	[descricao_parentesco]     nvarchar(50),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxClientesFornecContatosParentesco] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_parentesco] asc
));

/*LinxClientesFornecCreditoAvulso: os dados de crédito avulso dos clientes e fornecedores. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxClientesFornecCreditoAvulso (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[cod_cliente]              int,
	[controle]                 int,
	[data]                     datetime,
	[cd]                       nvarchar(1),
	[valor]                    decimal(19,4),
	[motivo]                   nvarchar(max),
	[timestamp]                bigint,
	[identificador]            uniqueidentifier
);

create table [linxmicrovix].LinxClientesFornecCreditoAvulso (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[cod_cliente]              int not null,
	[controle]                 int not null,
	[data]                     datetime,
	[cd]                       nvarchar(1),
	[valor]                    decimal(19,4),
	[motivo]                   nvarchar(max),
	[timestamp]                bigint,
	[identificador]            uniqueidentifier,
constraint [PK_LinxClientesFornecCreditoAvulso] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_cliente] asc,
	[controle] asc
));

/*LinxClientesFornecVendedores: o código do vendedor associado ao cadastro do cliente, conforme a tabela clientes_fornec_vendedores. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxClientesFornecVendedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_cliente]              int,
	[cod_vendedor]             int,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxClientesFornecVendedores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_cliente]              int not null,
	[cod_vendedor]             int not null,
	[timestamp]                bigint,
constraint [PK_LinxClientesFornecVendedores] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_cliente] asc,
	[cod_vendedor] asc
));

/*LinxClientesEnderecosEntrega: endereço de entrega do cliente.*/

create table [untreated].LinxClientesEnderecosEntrega (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_endereco_entrega]      int,
	[cod_cliente]              int,
	[endereco_cliente]         nvarchar(250),
	[numero_rua_cliente]       nvarchar(20),
	[complemento_end_cli]      nvarchar(60),
	[cep_cliente]              nvarchar(9),
	[bairro_cliente]           nvarchar(60),
	[cidade_cliente]           nvarchar(40),
	[uf_cliente]               nvarchar(2),
	[descricao]                nvarchar(250),
	[principal]                bit,
	[fone_cliente]             nvarchar(20),
	[fone_celular]             nvarchar(20),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].LinxClientesEnderecosEntrega (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_endereco_entrega]      int not null,
	[cod_cliente]              int,
	[endereco_cliente]         nvarchar(250),
	[numero_rua_cliente]       nvarchar(20),
	[complemento_end_cli]      nvarchar(60),
	[cep_cliente]              nvarchar(9),
	[bairro_cliente]           nvarchar(60),
	[cidade_cliente]           nvarchar(40),
	[uf_cliente]               nvarchar(2),
	[descricao]                nvarchar(250),
	[principal]                bit,
	[fone_cliente]             nvarchar(20),
	[fone_celular]             nvarchar(20),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_LinxClientesEnderecosEntrega] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_endereco_entrega] asc
));

/*LinxClientesRede: os registros da tabela clientes_rede no portal centralizador. Colunas da documentação: a chave da Newbloomers não tem acesso ao método. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxClientesRede (
	[integrated_or_updated_at]          datetime,
	[doc_company]                       nvarchar(14),
	[cnpj_emp]                          nvarchar(14), --loja consultada
	[id_clientes_rede]                  int,
	[doc_cliente]                       nvarchar(14),
	[nome_razao_social]                 nvarchar(60),
	[data_cadastro]                     datetime,
	[pf_pj]                             nvarchar(1),
	[tipo]                              nvarchar(1),
	[cidade]                            nvarchar(40),
	[uf]                                nvarchar(20),
	[pais]                              nvarchar(80),
	[id_estado_civil]                   int,
	[compras_a_prazo]                   bit,
	[aceita_boleto_bancario]            bit,
	[nome_fantasia]                     nvarchar(60),
	[numero_endereco]                   nvarchar(20),
	[complemento]                       nvarchar(60),
	[bairro]                            nvarchar(60),
	[cep]                               nvarchar(9),
	[telefone]                          nvarchar(20),
	[celular]                           nvarchar(20),
	[fax]                               nvarchar(50),
	[email]                             nvarchar(50),
	[site]                              nvarchar(50),
	[data_nascimento]                   datetime,
	[naturalidade]                      nvarchar(40),
	[sexo]                              nvarchar(1),
	[nome_pai]                          nvarchar(60),
	[nome_mae]                          nvarchar(60),
	[identidade_cliente]                nvarchar(20),
	[data_emissao_identidade_cliente]   datetime,
	[uf_emissao_identidade_cliente]     nvarchar(2),
	[orgao_emissao_identidade_cliente]  nvarchar(10),
	[nome_empresa_titular]              nvarchar(60),
	[telefone_empresa_titular]          nvarchar(20),
	[funcao_titular]                    nvarchar(40),
	[tempo_servico_titular]             nvarchar(20),
	[renda_titular]                     decimal(19,4),
	[inscricao_estadual]                nvarchar(20),
	[inscricao_municipal]               nvarchar(20),
	[cliente_optante_simples_municipal] bit,
	[cliente_optante_simples_federal]   bit,
	[limite_compras]                    decimal(19,4),
	[cartao_fidelidade]                 nvarchar(20),
	[desabilitado]                      bit,
	[motivo_bloqueio]                   nvarchar(50),
	[portal_origem]                     int,
	[empresa_origem]                    int,
	[cod_cliente_portal_origem]         int,
	[codigo_ws]                         nvarchar(14),
	[integrado_ws]                      bit,
	[timestamp]                         bigint
);

create table [linxmicrovix].LinxClientesRede (
	[integrated_or_updated_at]          datetime,
	[doc_company]                       nvarchar(14) not null,
	[cnpj_emp]                          nvarchar(14) not null, --loja consultada
	[id_clientes_rede]                  int not null,
	[doc_cliente]                       nvarchar(14),
	[nome_razao_social]                 nvarchar(60),
	[data_cadastro]                     datetime,
	[pf_pj]                             nvarchar(1),
	[tipo]                              nvarchar(1),
	[cidade]                            nvarchar(40),
	[uf]                                nvarchar(20),
	[pais]                              nvarchar(80),
	[id_estado_civil]                   int,
	[compras_a_prazo]                   bit,
	[aceita_boleto_bancario]            bit,
	[nome_fantasia]                     nvarchar(60),
	[numero_endereco]                   nvarchar(20),
	[complemento]                       nvarchar(60),
	[bairro]                            nvarchar(60),
	[cep]                               nvarchar(9),
	[telefone]                          nvarchar(20),
	[celular]                           nvarchar(20),
	[fax]                               nvarchar(50),
	[email]                             nvarchar(50),
	[site]                              nvarchar(50),
	[data_nascimento]                   datetime,
	[naturalidade]                      nvarchar(40),
	[sexo]                              nvarchar(1),
	[nome_pai]                          nvarchar(60),
	[nome_mae]                          nvarchar(60),
	[identidade_cliente]                nvarchar(20),
	[data_emissao_identidade_cliente]   datetime,
	[uf_emissao_identidade_cliente]     nvarchar(2),
	[orgao_emissao_identidade_cliente]  nvarchar(10),
	[nome_empresa_titular]              nvarchar(60),
	[telefone_empresa_titular]          nvarchar(20),
	[funcao_titular]                    nvarchar(40),
	[tempo_servico_titular]             nvarchar(20),
	[renda_titular]                     decimal(19,4),
	[inscricao_estadual]                nvarchar(20),
	[inscricao_municipal]               nvarchar(20),
	[cliente_optante_simples_municipal] bit,
	[cliente_optante_simples_federal]   bit,
	[limite_compras]                    decimal(19,4),
	[cartao_fidelidade]                 nvarchar(20),
	[desabilitado]                      bit,
	[motivo_bloqueio]                   nvarchar(50),
	[portal_origem]                     int,
	[empresa_origem]                    int,
	[cod_cliente_portal_origem]         int,
	[codigo_ws]                         nvarchar(14),
	[integrado_ws]                      bit,
	[timestamp]                         bigint,
constraint [PK_LinxClientesRede] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_clientes_rede] asc
));

/*LinxRamosAtividade: os ramos de atividades cadastrados. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxRamosAtividade (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_ramo_atividade]        int,
	[nome_ramo_atividade]      nvarchar(50),
	[codigo_ramo_atividade]    nvarchar(12),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxRamosAtividade (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_ramo_atividade]        int not null,
	[nome_ramo_atividade]      nvarchar(50),
	[codigo_ramo_atividade]    nvarchar(12),
	[timestamp]                bigint,
constraint [PK_LinxRamosAtividade] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_ramo_atividade] asc
));
