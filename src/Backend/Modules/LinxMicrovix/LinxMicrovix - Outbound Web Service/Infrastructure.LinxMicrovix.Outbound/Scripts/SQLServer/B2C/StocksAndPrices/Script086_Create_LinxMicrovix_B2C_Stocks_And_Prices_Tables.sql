/****** Estoque, custos e preços do B2C (LinxMicrovixB2CStocksAndPricesService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da V64; respostas planas: uma tabela por método, sem filhas

/*B2CConsultaProdutosCustos: */

create table [untreated].B2CConsultaProdutosCustos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_produtos_custos]       int,
	[codigoproduto]            bigint,
	[empresa]                  int,
	[custoicms1]               decimal(28,8),
	[ipi1]                     decimal(28,8),
	[markup]                   decimal(28,8),
	[customedio]               decimal(28,8),
	[frete1]                   decimal(28,8),
	[precisao]                 int,
	[precominimo]              decimal(28,8),
	[dt_update]                datetime, --fora da documentação
	[custoliquido]             decimal(28,8),
	[precovenda]               decimal(28,8),
	[custototal]               decimal(28,8),
	[precocompra]              decimal(28,8),
	[timestamp]                bigint,
	[desconto_especial]        decimal(28,8),
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosCustos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_produtos_custos]       int,
	[codigoproduto]            bigint not null,
	[empresa]                  int,
	[custoicms1]               decimal(28,8),
	[ipi1]                     decimal(28,8),
	[markup]                   decimal(28,8),
	[customedio]               decimal(28,8),
	[frete1]                   decimal(28,8),
	[precisao]                 int,
	[precominimo]              decimal(28,8),
	[dt_update]                datetime, --fora da documentação
	[custoliquido]             decimal(28,8),
	[precovenda]               decimal(28,8),
	[custototal]               decimal(28,8),
	[precocompra]              decimal(28,8),
	[timestamp]                bigint,
	[desconto_especial]        decimal(28,8),
	[portal]                   int not null,
constraint [PK_B2CConsultaProdutosCustos] primary key (
	[doc_company] asc,
	[portal] asc,
	[codigoproduto] asc
));

/*B2CConsultaProdutosDepositos: */

create table [untreated].B2CConsultaProdutosDepositos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_deposito]              int,
	[nome_deposito]            nvarchar(50),
	[disponivel]               nvarchar(1),
	[disponivel_transferencia] bit,
	[disponivel_franquias]     bit,
	[timestamp]                bigint,
	[portal]                   int,
	[outlet]                   bit
);

create table [linxmicrovix].B2CConsultaProdutosDepositos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_deposito]              int not null,
	[nome_deposito]            nvarchar(50),
	[disponivel]               nvarchar(1),
	[disponivel_transferencia] bit,
	[disponivel_franquias]     bit,
	[timestamp]                bigint,
	[portal]                   int not null,
	[outlet]                   bit,
constraint [PK_B2CConsultaProdutosDepositos] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_deposito] asc
));

/*B2CConsultaProdutosDetalhes: */

create table [untreated].B2CConsultaProdutosDetalhes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id_prod_det]              bigint,
	[codigoproduto]            bigint,
	[empresa]                  int,
	[saldo]                    decimal(28,8),
	[controla_lote]            bit,
	[nomeproduto_alternativo]  nvarchar(250),
	[timestamp]                bigint,
	[referencia]               nvarchar(50), --fora da documentação
	[localizacao]              nvarchar(50),
	[portal]                   int,
	[tempo_preparacao_estoque] int
);

create table [linxmicrovix].B2CConsultaProdutosDetalhes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id_prod_det]              bigint not null,
	[codigoproduto]            bigint,
	[empresa]                  int,
	[saldo]                    decimal(28,8),
	[controla_lote]            bit,
	[nomeproduto_alternativo]  nvarchar(250),
	[timestamp]                bigint,
	[referencia]               nvarchar(50), --fora da documentação
	[localizacao]              nvarchar(50),
	[portal]                   int,
	[tempo_preparacao_estoque] int,
constraint [PK_B2CConsultaProdutosDetalhes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_prod_det] asc
));

/*B2CConsultaProdutosDetalhesDepositos: */

create table [untreated].B2CConsultaProdutosDetalhesDepositos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[codigoproduto]            bigint,
	[empresa]                  int,
	[id_deposito]              int,
	[saldo]                    decimal(28,8),
	[timestamp]                bigint,
	[portal]                   int,
	[tempo_preparacao_estoque] int
);

create table [linxmicrovix].B2CConsultaProdutosDetalhesDepositos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[codigoproduto]            bigint not null,
	[empresa]                  int,
	[id_deposito]              int,
	[saldo]                    decimal(28,8),
	[timestamp]                bigint,
	[portal]                   int,
	[tempo_preparacao_estoque] int,
constraint [PK_B2CConsultaProdutosDetalhesDepositos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[codigoproduto] asc
));

/*B2CConsultaProdutosPromocao:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaProdutosPromocao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[codigo_promocao]          bigint,
	[empresa]                  int,
	[codigoproduto]            bigint,
	[preco]                    decimal(19,4),
	[data_inicio]              datetime,
	[data_termino]             datetime,
	[data_cadastro]            datetime,
	[ativa]                    nvarchar(1),
	[codigo_campanha]          int,
	[promocao_opcional]        bit,
	[timestamp]                bigint,
	[referencia]               nvarchar(20),
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosPromocao (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[codigo_promocao]          bigint not null,
	[empresa]                  int,
	[codigoproduto]            bigint not null,
	[preco]                    decimal(19,4),
	[data_inicio]              datetime,
	[data_termino]             datetime,
	[data_cadastro]            datetime,
	[ativa]                    nvarchar(1),
	[codigo_campanha]          int,
	[promocao_opcional]        bit,
	[timestamp]                bigint,
	[referencia]               nvarchar(20),
	[portal]                   int,
constraint [PK_B2CConsultaProdutosPromocao] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[codigo_promocao] asc,
	[codigoproduto] asc
));

/*B2CConsultaProdutosTabelas:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaProdutosTabelas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_tabela]                int,
	[nome_tabela]              nvarchar(50),
	[ativa]                    nvarchar(1),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosTabelas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_tabela]                int not null,
	[nome_tabela]              nvarchar(50),
	[ativa]                    nvarchar(1),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaProdutosTabelas] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_tabela] asc
));

/*B2CConsultaProdutosTabelasPrecos: */

create table [untreated].B2CConsultaProdutosTabelasPrecos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_prod_tab_preco]        int,
	[id_tabela]                int,
	[codigoproduto]            bigint,
	[precovenda]               decimal(19,4),
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosTabelasPrecos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_prod_tab_preco]        int,
	[id_tabela]                int not null,
	[codigoproduto]            bigint not null,
	[precovenda]               decimal(19,4),
	[timestamp]                bigint,
	[portal]                   int not null,
constraint [PK_B2CConsultaProdutosTabelasPrecos] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_tabela] asc,
	[codigoproduto] asc
));
