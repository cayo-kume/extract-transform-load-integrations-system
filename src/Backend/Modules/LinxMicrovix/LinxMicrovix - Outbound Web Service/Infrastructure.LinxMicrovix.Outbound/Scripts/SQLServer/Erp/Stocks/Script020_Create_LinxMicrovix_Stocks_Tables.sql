-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; docs/MIGRATOR-CSX.md)
/****** Estoque e balanços (LinxMicrovixStocksService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxProdutosDetalhes: informações dos produtos de acordo com a loja pesquisada*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxProdutosDetalhes') = 1
create table [untreated].LinxProdutosDetalhes (
	[integrated_or_updated_at]  datetime,
	[doc_company]               nvarchar(14),
	[cnpj_emp]                  nvarchar(14), --loja consultada
	[portal]                    int,
	[cod_produto]               bigint,
	[cod_barra]                 nvarchar(20),
	[quantidade]                decimal(19,4),
	[preco_custo]               decimal(19,4),
	[preco_venda]               decimal(19,4),
	[custo_medio]               decimal(19,4),
	[id_config_tributaria]      int,
	[desc_config_tributaria]    nvarchar(100),
	[despesas1]                 decimal(28,8),
	[qtde_minima]               decimal(28,8),
	[qtde_maxima]               decimal(28,8),
	[ipi]                       decimal(28,8),
	[timestamp]                 bigint,
	[custototal]                decimal(28,8),
	[empresa]                   int,
	[id_produtos_status_compra] int,
	[localizacao]               nvarchar(50)
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxProdutosDetalhes') = 1
create table [linxmicrovix].LinxProdutosDetalhes (
	[integrated_or_updated_at]  datetime,
	[doc_company]               nvarchar(14) not null,
	[cnpj_emp]                  nvarchar(14) not null, --loja consultada
	[portal]                    int,
	[cod_produto]               bigint not null,
	[cod_barra]                 nvarchar(20),
	[quantidade]                decimal(19,4),
	[preco_custo]               decimal(19,4),
	[preco_venda]               decimal(19,4),
	[custo_medio]               decimal(19,4),
	[id_config_tributaria]      int,
	[desc_config_tributaria]    nvarchar(100),
	[despesas1]                 decimal(28,8),
	[qtde_minima]               decimal(28,8),
	[qtde_maxima]               decimal(28,8),
	[ipi]                       decimal(28,8),
	[timestamp]                 bigint,
	[custototal]                decimal(28,8),
	[empresa]                   int,
	[id_produtos_status_compra] int,
	[localizacao]               nvarchar(50),
constraint [PK_LinxProdutosDetalhes] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_produto] asc
));

/*LinxProdutosDetalhesSimplificado: os dados adicionais de produtos de forma simplificada*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxProdutosDetalhesSimplificado') = 1
create table [untreated].LinxProdutosDetalhesSimplificado (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[cod_produto]              bigint,
	[quantidade]               decimal(19,4),
	[id_config_tributaria]     int,
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxProdutosDetalhesSimplificado') = 1
create table [linxmicrovix].LinxProdutosDetalhesSimplificado (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[cod_produto]              bigint not null,
	[quantidade]               decimal(19,4),
	[id_config_tributaria]     int,
	[timestamp]                bigint,
constraint [PK_LinxProdutosDetalhesSimplificado] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_produto] asc
));

/*LinxProdutosDetalhesDepositos: os saldos dos produtos por depósito.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxProdutosDetalhesDepositos') = 1
create table [untreated].LinxProdutosDetalhesDepositos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_produto]              bigint,
	[empresa]                  int,
	[cod_deposito]             int,
	[saldo]                    decimal(28,8),
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxProdutosDetalhesDepositos') = 1
create table [linxmicrovix].LinxProdutosDetalhesDepositos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_produto]              bigint not null,
	[empresa]                  int,
	[cod_deposito]             int not null,
	[saldo]                    decimal(28,8),
	[timestamp]                bigint,
constraint [PK_LinxProdutosDetalhesDepositos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_deposito] asc,
	[cod_produto] asc
));

/*LinxProdutosDepositos: os depósitos cadastrados no ERP*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxProdutosDepositos') = 1
create table [untreated].LinxProdutosDepositos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[cod_deposito]             int,
	[nome_deposito]            nvarchar(50),
	[disponivel]               bit,
	[disponivel_transferencia] bit,
	[timestamp]                bigint,
	[outlet]                   bit --fora da documentação
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxProdutosDepositos') = 1
create table [linxmicrovix].LinxProdutosDepositos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[cod_deposito]             int not null,
	[nome_deposito]            nvarchar(50),
	[disponivel]               bit,
	[disponivel_transferencia] bit,
	[timestamp]                bigint,
	[outlet]                   bit, --fora da documentação
constraint [PK_LinxProdutosDepositos] primary key (
	[doc_company] asc,
	[portal] asc,
	[cod_deposito] asc
));

/*LinxProdutosInventario: informações do saldo do produto na data pesquisada*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxProdutosInventario') = 1
create table [untreated].LinxProdutosInventario (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[cod_produto]              bigint,
	[cod_barra]                nvarchar(20),
	[quantidade]               decimal(28,8),
	[empresa]                  int,
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxProdutosInventario') = 1
create table [linxmicrovix].LinxProdutosInventario (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[cod_produto]              bigint not null,
	[cod_barra]                nvarchar(20),
	[quantidade]               decimal(28,8),
	[empresa]                  int,
	[timestamp]                bigint,
constraint [PK_LinxProdutosInventario] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[cod_produto] asc
));

/*LinxListagemBalanco: a listagem de balanços.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxListagemBalanco') = 1
create table [untreated].LinxListagemBalanco (
	[integrated_or_updated_at]  datetime,
	[doc_company]               nvarchar(14),
	[cnpj_emp]                  nvarchar(14), --loja consultada
	[id_balanco]                int,
	[data]                      datetime,
	[nome_arquivo]              nvarchar(50),
	[processado]                nvarchar(1),
	[data_ultimo_processamento] datetime,
	[qtde_produtos]             nvarchar(1000), --int na documentação; a resposta real não cabe nesse tipo
	[qtde_itens]                int,
	[timestamp]                 bigint,
	[portal]                    int
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxListagemBalanco') = 1
create table [linxmicrovix].LinxListagemBalanco (
	[integrated_or_updated_at]  datetime,
	[doc_company]               nvarchar(14) not null,
	[cnpj_emp]                  nvarchar(14) not null, --loja consultada
	[id_balanco]                int not null,
	[data]                      datetime,
	[nome_arquivo]              nvarchar(50),
	[processado]                nvarchar(1),
	[data_ultimo_processamento] datetime,
	[qtde_produtos]             nvarchar(1000), --int na documentação; a resposta real não cabe nesse tipo
	[qtde_itens]                int,
	[timestamp]                 bigint,
	[portal]                    int,
constraint [PK_LinxListagemBalanco] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_balanco] asc
));

/*LinxBalancos: os balanços. Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxBalancos') = 1
create table [untreated].LinxBalancos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_balancos]              int,
	[descricao]                nvarchar(250),
	[data_lancamento]          datetime,
	[status]                   nvarchar(100),
	[id_deposito]              int,
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxBalancos') = 1
create table [linxmicrovix].LinxBalancos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_balancos]              int not null,
	[descricao]                nvarchar(250),
	[data_lancamento]          datetime,
	[status]                   nvarchar(100),
	[id_deposito]              int,
	[timestamp]                bigint,
constraint [PK_LinxBalancos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_balancos] asc
));

/*LinxBalancosArquivos: os dados dos arquivos de balanços. Colunas da documentação: a chave da Newbloomers não tem acesso ao método. Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxBalancosArquivos') = 1
create table [untreated].LinxBalancosArquivos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[id_balancos_arquivos]     int,
	[codigoproduto]            bigint,
	[qtdeproduto]              decimal(19,4),
	[lote]                     nvarchar(60),
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxBalancosArquivos') = 1
create table [linxmicrovix].LinxBalancosArquivos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[id_balancos_arquivos]     int not null,
	[codigoproduto]            bigint,
	[qtdeproduto]              decimal(19,4),
	[lote]                     nvarchar(60),
	[timestamp]                bigint,
constraint [PK_LinxBalancosArquivos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_balancos_arquivos] asc
));

/*LinxBalancosSecoes: as seções dos balanços. Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxBalancosSecoes') = 1
create table [untreated].LinxBalancosSecoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_balancos_secoes]       int,
	[id_balancos]              int,
	[id_balancos_arquivos]     int,
	[status]                   nvarchar(100),
	[timestamp]                bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxBalancosSecoes') = 1
create table [linxmicrovix].LinxBalancosSecoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_balancos_secoes]       int not null,
	[id_balancos]              int,
	[id_balancos_arquivos]     int,
	[status]                   nvarchar(100),
	[timestamp]                bigint,
constraint [PK_LinxBalancosSecoes] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_balancos_secoes] asc
));
