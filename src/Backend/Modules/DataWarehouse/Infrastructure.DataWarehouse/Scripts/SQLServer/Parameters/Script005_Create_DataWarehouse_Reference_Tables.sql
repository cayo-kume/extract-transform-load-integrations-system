/****** Tabelas de referência (docs/datawarehouse, 4.1, 7.1, 7.5 e 8.7) ******/
--mantidas por script: os valores entram no Script070 e as mudanças em scripts novos. As tabelas ficam na camada base
--porque as views da fase 1 (vw_OrderParts) as referenciam, e o SQL Server valida a view na criação.

--feriados nacionais, para DimDate (is_holiday, is_business_day)
create table [dw_reference].Holidays (
	holiday_date  date           not null,
	name          nvarchar(100)  not null,
constraint [pk_Holidays] primary key (holiday_date asc)
);

--marca pela raiz do CNPJ da filial: série do número do pedido (docs/regras-prefixo-pedidos.md, §1.1) e DimBrand
create table [dw_reference].BrandSeries (
	cnpj_root     char(8)        not null,
	brand         nvarchar(60)   not null,
	order_series  varchar(5)     not null,   --MI-, OA-, MX-
constraint [pk_BrandSeries] primary key (cnpj_root asc)
);

--canal e tipo do pedido (docs/regras-prefixo-pedidos.md; pergunta 15). Vale a primeira regra que casa, pela prioridade:
--match_column é a coluna do pedido ou da nota comparada com like match_pattern (integrationCode: não nulo casa com '%').
--is_sale = 0 para os tipos que não são venda (LJ reposição de lojas, RC recebimento): saem de captado e faturado (7.2)
create table [dw_reference].ChannelRules (
	rule_id        int            not null,
	priority       int            not null,
	source_erp     varchar(20)    not null,
	match_column   varchar(50)    not null,   --integrationCode, operationName, representativeName, inclusionComponentCode
	match_pattern  nvarchar(100)  not null,
	channel        varchar(30)    not null,   --ecommerce, store, b2b, transfer
	order_type     varchar(5)     null,       --VD, LJ, VF, RC, RV, ST, PDV
	is_sale        bit            not null,
	description    nvarchar(200)  null,
constraint [pk_ChannelRules] primary key (rule_id asc),
constraint [ck_ChannelRules_match_column] check (match_column in ('integrationCode', 'operationName', 'representativeName', 'inclusionComponentCode'))
);

--exceções do mapa loja × ERP × período (7.5): só os CNPJ cuja troca de ERP não sai da última venda Linx e da primeira nota Totvs
create table [dw_reference].StoreErpMap (
	cnpj          nvarchar(14)   not null,
	source_erp    varchar(20)    not null,
	valid_from    date           not null,
	valid_to      date           null,
constraint [pk_StoreErpMap] primary key (cnpj asc, source_erp asc, valid_from asc)
);

--correções do de/para de produto Linx × Totvs que o texto não resolve (pergunta 34, fase 3)
create table [dw_reference].ProductErpMap (
	doc_company          nvarchar(14)   not null,
	linx_product_code    nvarchar(50)   not null,
	totvs_product_code   int            not null,
constraint [pk_ProductErpMap] primary key (doc_company asc, linx_product_code asc)
);

--tipos de valor da Totvs (PRDFL003) e o papel de cada um no DW (8.7, pergunta 33): as procedures leem o papel
create table [dw_reference].ValueCodes (
	value_type     char(1)        not null,   --C custo, P preço, S saldo
	code           int            not null,
	name           nvarchar(60)   not null,
	currency_code  int            null,       --1 REAL, 2 DOLAR, 3 EURO
	[role]         varchar(30)    null,
constraint [pk_ValueCodes] primary key (value_type asc, code asc)
);
create unique index [ux_ValueCodes_role] on [dw_reference].ValueCodes ([role]) where [role] is not null;
