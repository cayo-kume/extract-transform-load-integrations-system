/****** Campos adicionais de produto do B2C (LinxMicrovixB2CProductFieldsService): tabelas untreated e linxmicrovix (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da V64; respostas planas: uma tabela por método, sem filhas

/*B2CConsultaProdutosCamposAdicionaisDetalhes: */

create table [untreated].B2CConsultaProdutosCamposAdicionaisDetalhes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_campo_detalhe]         int,
	[ordem]                    int,
	[descricao]                nvarchar(30),
	[id_campo]                 int,
	[ativo]                    bit,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosCamposAdicionaisDetalhes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_campo_detalhe]         int not null,
	[ordem]                    int,
	[descricao]                nvarchar(30),
	[id_campo]                 int,
	[ativo]                    bit,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaProdutosCamposAdicionaisDetalhes] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_campo_detalhe] asc
));

/*B2CConsultaProdutosCamposAdicionaisNomes:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaProdutosCamposAdicionaisNomes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_campo]                 int,
	[ordem]                    int,
	[legenda]                  nvarchar(30),
	[tipo]                     nvarchar(1),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosCamposAdicionaisNomes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_campo]                 int not null,
	[ordem]                    int,
	[legenda]                  nvarchar(30),
	[tipo]                     nvarchar(1),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaProdutosCamposAdicionaisNomes] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_campo] asc
));

/*B2CConsultaProdutosCamposAdicionaisValores: */

create table [untreated].B2CConsultaProdutosCamposAdicionaisValores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_campo_valor]           int,
	[codigoproduto]            bigint,
	[id_campo_detalhe]         int,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosCamposAdicionaisValores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_campo_valor]           int,
	[codigoproduto]            bigint not null,
	[id_campo_detalhe]         int not null,
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaProdutosCamposAdicionaisValores] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_campo_detalhe] asc,
	[codigoproduto] asc
));

/*B2CConsultaB2CProdutosCamposAdicionais:  Colunas da documentação: a chave da Newbloomers não tem acesso ao método. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaB2CProdutosCamposAdicionais (
	[integrated_or_updated_at]          datetime,
	[doc_company]                       nvarchar(14),
	[cnpj_emp]                          nvarchar(14), --loja consultada
	[id_b2c_produtos_campos_adicionais] int,
	[nome_campo]                        nvarchar(max),
	[tipo]                              nvarchar(max),
	[decimais]                          int,
	[multipla_escolha]                  bit,
	[ordem]                             int,
	[ativo]                             bit,
	[nivel_id]                          int,
	[nivel_descricao]                   nvarchar(max),
	[timestamp]                         bigint,
	[portal]                            int
);

create table [linxmicrovix].B2CConsultaB2CProdutosCamposAdicionais (
	[integrated_or_updated_at]          datetime,
	[doc_company]                       nvarchar(14) not null,
	[cnpj_emp]                          nvarchar(14) not null, --loja consultada
	[id_b2c_produtos_campos_adicionais] int not null,
	[nome_campo]                        nvarchar(max),
	[tipo]                              nvarchar(max),
	[decimais]                          int,
	[multipla_escolha]                  bit,
	[ordem]                             int,
	[ativo]                             bit,
	[nivel_id]                          int,
	[nivel_descricao]                   nvarchar(max),
	[timestamp]                         bigint,
	[portal]                            int,
constraint [PK_B2CConsultaB2CProdutosCamposAdicionais] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_produtos_campos_adicionais] asc
));

/*B2CConsultaB2CProdutosCamposAdicionaisDados:  Colunas da documentação: a chave da Newbloomers não tem acesso ao método. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaB2CProdutosCamposAdicionaisDados (
	[integrated_or_updated_at]                 datetime,
	[doc_company]                              nvarchar(14),
	[cnpj_emp]                                 nvarchar(14), --loja consultada
	[id_b2c_produtos_campos_adicionais_dados]  int,
	[id_b2c_produtos_campos_adicionais_opcoes] int,
	[id_b2c_produtos_campos_adicionais]        int,
	[codigoproduto]                            bigint,
	[valor]                                    nvarchar(max),
	[timestamp]                                bigint,
	[portal]                                   int
);

create table [linxmicrovix].B2CConsultaB2CProdutosCamposAdicionaisDados (
	[integrated_or_updated_at]                 datetime,
	[doc_company]                              nvarchar(14) not null,
	[cnpj_emp]                                 nvarchar(14) not null, --loja consultada
	[id_b2c_produtos_campos_adicionais_dados]  int not null,
	[id_b2c_produtos_campos_adicionais_opcoes] int,
	[id_b2c_produtos_campos_adicionais]        int,
	[codigoproduto]                            bigint,
	[valor]                                    nvarchar(max),
	[timestamp]                                bigint,
	[portal]                                   int,
constraint [PK_B2CConsultaB2CProdutosCamposAdicionaisDados] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_produtos_campos_adicionais_dados] asc
));

/*B2CConsultaB2CProdutosCamposAdicionaisMedia:  Colunas da documentação: a chave da Newbloomers não tem acesso ao método. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaB2CProdutosCamposAdicionaisMedia (
	[integrated_or_updated_at]                datetime,
	[doc_company]                             nvarchar(14),
	[cnpj_emp]                                nvarchar(14), --loja consultada
	[id_b2c_produtos_campos_adicionais_media] int,
	[descricao]                               nvarchar(200),
	[url]                                     nvarchar(400),
	[tipo_media_descricao]                    nvarchar(50),
	[portal]                                  int,
	[media_principal]                         bit,
	[tipo_media_id]                           int,
	[timestamp]                               bigint
);

create table [linxmicrovix].B2CConsultaB2CProdutosCamposAdicionaisMedia (
	[integrated_or_updated_at]                datetime,
	[doc_company]                             nvarchar(14) not null,
	[cnpj_emp]                                nvarchar(14) not null, --loja consultada
	[id_b2c_produtos_campos_adicionais_media] int not null,
	[descricao]                               nvarchar(200),
	[url]                                     nvarchar(400),
	[tipo_media_descricao]                    nvarchar(50),
	[portal]                                  int,
	[media_principal]                         bit,
	[tipo_media_id]                           int,
	[timestamp]                               bigint,
constraint [PK_B2CConsultaB2CProdutosCamposAdicionaisMedia] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_produtos_campos_adicionais_media] asc
));

/*B2CConsultaB2CProdutosCamposAdicionaisMediaDados:  Colunas da documentação: a chave da Newbloomers não tem acesso ao método. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaB2CProdutosCamposAdicionaisMediaDados (
	[integrated_or_updated_at]                      datetime,
	[doc_company]                                   nvarchar(14),
	[cnpj_emp]                                      nvarchar(14), --loja consultada
	[id_b2c_produtos_campos_adicionais_media_dados] int,
	[id_b2c_produtos_campos_adicionais_media]       nvarchar(50),
	[codigoproduto]                                 nvarchar(300),
	[timestamp]                                     bigint,
	[portal]                                        int
);

create table [linxmicrovix].B2CConsultaB2CProdutosCamposAdicionaisMediaDados (
	[integrated_or_updated_at]                      datetime,
	[doc_company]                                   nvarchar(14) not null,
	[cnpj_emp]                                      nvarchar(14) not null, --loja consultada
	[id_b2c_produtos_campos_adicionais_media_dados] int not null,
	[id_b2c_produtos_campos_adicionais_media]       nvarchar(50),
	[codigoproduto]                                 nvarchar(300),
	[timestamp]                                     bigint,
	[portal]                                        int,
constraint [PK_B2CConsultaB2CProdutosCamposAdicionaisMediaDados] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_produtos_campos_adicionais_media_dados] asc
));

/*B2CConsultaB2CProdutosCamposAdicionaisOpcoes:  Colunas da documentação: a chave da Newbloomers não tem acesso ao método. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaB2CProdutosCamposAdicionaisOpcoes (
	[integrated_or_updated_at]                 datetime,
	[doc_company]                              nvarchar(14),
	[cnpj_emp]                                 nvarchar(14), --loja consultada
	[id_b2c_produtos_campos_adicionais_opcoes] int,
	[id_b2c_produtos_campos_adicionais]        int,
	[valor]                                    nvarchar(max),
	[ordem]                                    int,
	[ativo]                                    bit,
	[categoria_nivel]                          bigint,
	[timestamp]                                bigint,
	[portal]                                   int
);

create table [linxmicrovix].B2CConsultaB2CProdutosCamposAdicionaisOpcoes (
	[integrated_or_updated_at]                 datetime,
	[doc_company]                              nvarchar(14) not null,
	[cnpj_emp]                                 nvarchar(14) not null, --loja consultada
	[id_b2c_produtos_campos_adicionais_opcoes] int not null,
	[id_b2c_produtos_campos_adicionais]        int,
	[valor]                                    nvarchar(max),
	[ordem]                                    int,
	[ativo]                                    bit,
	[categoria_nivel]                          bigint,
	[timestamp]                                bigint,
	[portal]                                   int,
constraint [PK_B2CConsultaB2CProdutosCamposAdicionaisOpcoes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_b2c_produtos_campos_adicionais_opcoes] asc
));
