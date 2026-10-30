/****** Remessas (LinxMicrovixConsignmentsService): tabelas untreated e linxmicrovix (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da v274; respostas planas: uma tabela por método, sem filhas

/*LinxMovimentoRemessas: os dados da tabela remessas no portal.*/

create table [untreated].LinxMovimentoRemessas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_remessas]              int,
	[id_tipo]                  int,
	[identificador_remessa]    uniqueidentifier,
	[status]                   int,
	[status_descricao]         nvarchar(35),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxMovimentoRemessas (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_remessas]              int not null,
	[id_tipo]                  int,
	[identificador_remessa]    uniqueidentifier,
	[status]                   int,
	[status_descricao]         nvarchar(35),
	[timestamp]                bigint,
constraint [PK_LinxMovimentoRemessas] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_remessas] asc
));

/*LinxMovimentoRemessasItens: os dados da tabela remessas_itens no portal.*/

create table [untreated].LinxMovimentoRemessasItens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_remessas_itens]        int,
	[id_remessas]              int,
	[transacao]                int,
	[qtde_total]               decimal(28,8),
	[qtde_venda]               decimal(28,8),
	[qtde_devolvido]           decimal(28,8),
	[qtde_pendente]            decimal(28,8),
	[qtde_pendente_pagamento]  decimal(28,8),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxMovimentoRemessasItens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_remessas_itens]        int not null,
	[id_remessas]              int,
	[transacao]                int,
	[qtde_total]               decimal(28,8),
	[qtde_venda]               decimal(28,8),
	[qtde_devolvido]           decimal(28,8),
	[qtde_pendente]            decimal(28,8),
	[qtde_pendente_pagamento]  decimal(28,8),
	[timestamp]                bigint,
constraint [PK_LinxMovimentoRemessasItens] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_remessas_itens] asc
));

/*LinxMovimentoRemessasAcertos: os dados da tabela remessas_acertos no portal.*/

create table [untreated].LinxMovimentoRemessasAcertos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_remessas_acertos]      int,
	[id_remessas]              int,
	[identificador_venda]      uniqueidentifier,
	[identificador_retorno]    uniqueidentifier,
	[identificador_devolucao]  uniqueidentifier,
	[data]                     datetime,
	[id_vendas_pos]            bigint,
	[excluido]                 bit,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxMovimentoRemessasAcertos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_remessas_acertos]      int not null,
	[id_remessas]              int,
	[identificador_venda]      uniqueidentifier,
	[identificador_retorno]    uniqueidentifier,
	[identificador_devolucao]  uniqueidentifier,
	[data]                     datetime,
	[id_vendas_pos]            bigint,
	[excluido]                 bit,
	[timestamp]                bigint,
constraint [PK_LinxMovimentoRemessasAcertos] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_remessas_acertos] asc
));

/*LinxMovimentoRemessasAcertosItens: os dados da tabela remessas_acertos_itens no portal.*/

create table [untreated].LinxMovimentoRemessasAcertosItens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_remessas_acertos]      int,
	[transacao]                int,
	[qtde_total]               decimal(28,8),
	[id_remessa_operacoes]     int,
	[id_remessas_itens]        int,
	[timestamp]                bigint
);

create table [linxmicrovix].LinxMovimentoRemessasAcertosItens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[empresa]                  int,
	[id_remessas_acertos]      int,
	[transacao]                int not null,
	[qtde_total]               decimal(28,8),
	[id_remessa_operacoes]     int,
	[id_remessas_itens]        int,
	[timestamp]                bigint,
constraint [PK_LinxMovimentoRemessasAcertosItens] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[transacao] asc
));

/*LinxRemessasIdentificadores: recebe o identificador da venda e retorna a relação com os identificadores das notas dos itens da remessa. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxRemessasIdentificadores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[identificador_venda]      uniqueidentifier,
	[identificador_remessa]    uniqueidentifier,
	[id_remessas]              int,
	[id_remessas_acertos]      int,
	[transacao_acerto]         int,
	[qtde_total_acerto]        decimal(28,8),
	[transacao_remessa]        int,
	[identificador_devolucao]  uniqueidentifier,
	[id_remessa_operacoes]     int,
	[portal]                   int
);

create table [linxmicrovix].LinxRemessasIdentificadores (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[identificador_venda]      uniqueidentifier not null,
	[identificador_remessa]    uniqueidentifier not null,
	[id_remessas]              int,
	[id_remessas_acertos]      int,
	[transacao_acerto]         int not null,
	[qtde_total_acerto]        decimal(28,8),
	[transacao_remessa]        int,
	[identificador_devolucao]  uniqueidentifier,
	[id_remessa_operacoes]     int,
	[portal]                   int,
constraint [PK_LinxRemessasIdentificadores] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[identificador_venda] asc,
	[identificador_remessa] asc,
	[transacao_acerto] asc
));

/*LinxRemessasOperacoes: os dados da tabela remessa_operacoes no portal. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxRemessasOperacoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_remessa_operacoes]     int,
	[descricao]                nvarchar(30),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxRemessasOperacoes (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_remessa_operacoes]     int not null,
	[descricao]                nvarchar(30),
	[timestamp]                bigint,
constraint [PK_LinxRemessasOperacoes] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_remessa_operacoes] asc
));

/*LinxRemessasRetornoTipos: os dados da tabela remessa_retorno_tipos no portal. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].LinxRemessasRetornoTipos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[portal]                   int,
	[id_remessa_retorno_tipos] int,
	[descricao]                nvarchar(50),
	[timestamp]                bigint
);

create table [linxmicrovix].LinxRemessasRetornoTipos (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[portal]                   int not null,
	[id_remessa_retorno_tipos] int not null,
	[descricao]                nvarchar(50),
	[timestamp]                bigint,
constraint [PK_LinxRemessasRetornoTipos] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_remessa_retorno_tipos] asc
));
