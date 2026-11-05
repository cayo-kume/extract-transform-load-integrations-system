-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; docs/MIGRATOR-CSX.md)
/****** Fidelidade (LinxMicrovixLoyaltyService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxFidelidade: os dados da tabela fidelidade_parceiro_log cadastrados no portal. Chave a confirmar com uma resposta real com dados.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxFidelidade') = 1
create table [untreated].LinxFidelidade (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14),
	[cnpj_emp]                   nvarchar(14), --loja consultada
	[portal]                     int,
	[id_fidelidade_parceiro_log] int,
	[data_transacao]             datetime,
	[operacao]                   int,
	[aprovado_barramento]        nvarchar(4),
	[valor_monetario]            decimal(19,4),
	[numero_cartao]              nvarchar(100),
	[identificador_movimento]    uniqueidentifier,
	[timestamp]                  bigint
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxFidelidade') = 1
create table [linxmicrovix].LinxFidelidade (
	[integrated_or_updated_at]   datetime,
	[doc_company]                nvarchar(14) not null,
	[cnpj_emp]                   nvarchar(14) not null, --loja consultada
	[portal]                     int,
	[id_fidelidade_parceiro_log] int not null,
	[data_transacao]             datetime,
	[operacao]                   int,
	[aprovado_barramento]        nvarchar(4),
	[valor_monetario]            decimal(19,4),
	[numero_cartao]              nvarchar(100),
	[identificador_movimento]    uniqueidentifier,
	[timestamp]                  bigint,
constraint [PK_LinxFidelidade] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_fidelidade_parceiro_log] asc
));

/*LinxMovimentoReshop: movimento_campanha_reshop.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxMovimentoReshop') = 1
create table [untreated].LinxMovimentoReshop (
	[integrated_or_updated_at]     datetime,
	[doc_company]                  nvarchar(14),
	[id_movimento_campanha_reshop] int,
	[id_campanha]                  int,
	[identificador]                uniqueidentifier,
	[nome]                         nvarchar(100), --fora da documentação
	[descricao]                    nvarchar(200),
	[aplicar_desconto_venda]       bit,
	[valor_desconto_subtotal]      decimal(19,4),
	[valor_desconto_completo]      decimal(19,4),
	[timestamp]                    bigint,
	[portal]                       int
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxMovimentoReshop') = 1
create table [linxmicrovix].LinxMovimentoReshop (
	[integrated_or_updated_at]     datetime,
	[doc_company]                  nvarchar(14) not null,
	[id_movimento_campanha_reshop] int not null,
	[id_campanha]                  int,
	[identificador]                uniqueidentifier,
	[nome]                         nvarchar(100), --fora da documentação
	[descricao]                    nvarchar(200),
	[aplicar_desconto_venda]       bit,
	[valor_desconto_subtotal]      decimal(19,4),
	[valor_desconto_completo]      decimal(19,4),
	[timestamp]                    bigint,
	[portal]                       int not null,
constraint [PK_LinxMovimentoReshop] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_movimento_campanha_reshop] asc
));

/*LinxMovimentoReshopItens: movimento_campanha_reshop_item.*/

if dbo.fn_Migrator_Creates(N'[untreated].LinxMovimentoReshopItens') = 1
create table [untreated].LinxMovimentoReshopItens (
	[integrated_or_updated_at]          datetime,
	[doc_company]                       nvarchar(14),
	[id_movimento_campanha_reshop_item] int,
	[id_campanha]                       int,
	[identificador]                     uniqueidentifier,
	[valor_unitario]                    decimal(19,4),
	[valor_desconto_item]               decimal(19,4),
	[valor_total_desconto_item]         decimal(19,4),
	[quantidade]                        decimal(28,8),
	[valor_original]                    decimal(19,4),
	[timestamp]                         bigint,
	[ordem]                             int,
	[portal]                            int
);

if dbo.fn_Migrator_Creates(N'[linxmicrovix].LinxMovimentoReshopItens') = 1
create table [linxmicrovix].LinxMovimentoReshopItens (
	[integrated_or_updated_at]          datetime,
	[doc_company]                       nvarchar(14) not null,
	[id_movimento_campanha_reshop_item] int not null,
	[id_campanha]                       int,
	[identificador]                     uniqueidentifier,
	[valor_unitario]                    decimal(19,4),
	[valor_desconto_item]               decimal(19,4),
	[valor_total_desconto_item]         decimal(19,4),
	[quantidade]                        decimal(28,8),
	[valor_original]                    decimal(19,4),
	[timestamp]                         bigint,
	[ordem]                             int,
	[portal]                            int not null,
constraint [PK_LinxMovimentoReshopItens] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_movimento_campanha_reshop_item] asc
));
