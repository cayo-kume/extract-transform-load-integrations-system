/****** Fidelidade (LinxMicrovixLoyaltyService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxFidelidade: os dados da tabela fidelidade_parceiro_log cadastrados no portal. Chave a confirmar com uma resposta real com dados.*/

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
