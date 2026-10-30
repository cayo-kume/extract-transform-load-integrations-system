/****** datawarehouse.FactMonthlyStock: posição de estoque no fim de cada mês (docs/datawarehouse, 7.4 e 8.5) ******/
--uma linha por data, loja, produto e depósito (stockCode). O depósito vendável é o que resta da pergunta 33: quando for
--marcado em dw_reference.ValueCodes (value_type 'S'), os agregados filtram por ele; o fato guarda todos.
create table [datawarehouse].FactMonthlyStock (
	position_date            date              not null,   --último dia do mês
	store_sk                 char(32)          not null,
	product_sk               char(32)          not null,
	stock_code               int               not null,   --depósito da Totvs
	doc_company              nvarchar(14)      not null,
	branch_code              int               not null,
	product_code             int               not null,
	stock_description        nvarchar(60)      null,
	quantity                 decimal(18,6)     not null,   --stock
	reserved_by_order        decimal(18,6)     not null,   --salesOrder
	incoming_from_purchase   decimal(18,6)     not null,   --purchaseOrder
	unit_cost                decimal(18,6)     null,       --custo com papel margin_cost (2 ULTIMA COMPRA) vigente no fim do dia
	loaded_at                datetime2(3)      not null constraint df_FactMonthlyStock_loaded_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_FactMonthlyStock] primary key (position_date asc, store_sk asc, product_sk asc, stock_code asc)
);
GO

/****** datawarehouse.sp_FactMonthlyStock_Load ******/
--grava a posição de @position_date lida de dw_intermediate.fn_StockPosition (versão de dw_history.TotvsBalances vigente no fim
--do dia). Sem data, usa o último dia do mês anterior: é o que o ciclo diário chama no dia 1 (10.1). Reexecutar uma data
--substitui as linhas dela. A chave usa a loja e o produto da época; duas posições da origem que caiam no mesmo membro
--desconhecido somam.
create or alter procedure [datawarehouse].sp_FactMonthlyStock_Load
	@position_date date = null,
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	set @position_date = isnull(@position_date,
		eomonth(dateadd(month, -1, cast(sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time' as date))));

	begin transaction;

	delete from [datawarehouse].FactMonthlyStock where position_date = @position_date;

	insert into [datawarehouse].FactMonthlyStock (position_date, store_sk, product_sk, stock_code, doc_company, branch_code, product_code,
	                                              stock_description, quantity, reserved_by_order, incoming_from_purchase, unit_cost)
	select position_date, store_sk, product_sk, stock_code, min(doc_company), min(branch_code), min(product_code),
	       min(stock_description), sum(quantity), sum(reserved_by_order), sum(incoming_from_purchase), max(unit_cost)
	  from [dw_intermediate].fn_StockPosition(@position_date)
	 group by position_date, store_sk, product_sk, stock_code;

	set @rows = @@rowcount;

	commit transaction;
end;
GO
