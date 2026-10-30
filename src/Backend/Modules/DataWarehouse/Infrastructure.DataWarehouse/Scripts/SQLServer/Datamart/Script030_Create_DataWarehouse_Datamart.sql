/****** datamart.DailySales (src/Backend/Modules/DataWarehouse/docs, 4.1 e 7.3) ******/
--venda por dia, loja e canal: faturado pela data da nota (FactSalesItem) e captado pela data do pedido (FactOrderItem).
--só os tipos que são venda (is_sale): reposição de lojas e recebimento ficam fora (7.2). As ferramentas de BI só somam
--estas colunas; nenhuma recalcula a regra (7.3).
create table [datamart].DailySales (
	sale_date             date            not null,
	store_sk              char(32)        not null,
	channel_sk            char(32)        not null,
	invoiced_quantity     decimal(18,6)   not null,
	gross_amount          decimal(18,6)   not null,
	discount_amount       decimal(18,6)   not null,
	invoiced_amount       decimal(18,6)   not null,
	cost_amount           decimal(18,6)   null,       --unit_cost × quantidade, com o sinal da nota
	margin_amount         decimal(18,6)   null,       --invoiced_amount - cost_amount
	captured_quantity     decimal(18,6)   not null,
	captured_amount       decimal(18,6)   not null,
	refreshed_at          datetime2(3)    not null,
constraint [pk_DailySales] primary key (sale_date asc, store_sk asc, channel_sk asc)
);
GO

/****** datamart.sp_DailySales_Refresh ******/
--recalcula tudo a partir dos fatos (recarga completa, pergunta 8)
create or alter procedure [datamart].sp_DailySales_Refresh
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @now datetime2(3) = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time';

	begin transaction;

	delete from [datamart].DailySales;

	insert into [datamart].DailySales (sale_date, store_sk, channel_sk, invoiced_quantity, gross_amount, discount_amount, invoiced_amount,
	                                   cost_amount, margin_amount, captured_quantity, captured_amount, refreshed_at)
	select sale_date, store_sk, channel_sk,
	       sum(invoiced_quantity), sum(gross_amount), sum(discount_amount), sum(invoiced_amount),
	       sum(cost_amount), sum(invoiced_amount) - sum(cost_amount),
	       sum(captured_quantity), sum(captured_amount), @now
	  from (
		select f.invoice_date as sale_date, f.store_sk, f.channel_sk,
		       iif(f.is_invoiced = 1, sign(f.invoiced_amount) * f.quantity, 0) as invoiced_quantity,
		       iif(f.is_invoiced = 1, f.gross_amount, 0) as gross_amount,
		       iif(f.is_invoiced = 1, f.discount_amount, 0) as discount_amount,
		       f.invoiced_amount,
		       iif(f.is_invoiced = 1, sign(f.invoiced_amount) * f.quantity * f.unit_cost, 0) as cost_amount,
		       0 as captured_quantity, 0 as captured_amount
		  from [datawarehouse].FactSalesItem f
		 where f.is_sale = 1
		union all
		select o.order_date, o.store_sk, o.channel_sk, 0, 0, 0, 0, 0,
		       iif(o.is_captured = 1, o.quantity, 0), iif(o.is_captured = 1, o.amount, 0)
		  from [datawarehouse].FactOrderItem o
		 where o.is_sale = 1
	  ) x
	 group by sale_date, store_sk, channel_sk;

	set @rows = @@rowcount;

	commit transaction;
end;
