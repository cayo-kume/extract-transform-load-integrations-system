/****** Watermarks (src/Backend/Modules/DataWarehouse/docs, 8.1 e 4.2) ******/
--uma linha por fonte e entidade: as origens lidas pela landing (source = módulo, entity = tabela de origem),
--os snapshots (source = dw_history, entity = tabela) e a varredura de cada módulo de saída (source = módulo, entity = view de dw_outbound).
--horário de Brasília sem fuso, como o integrated_or_updated_at das integrações.
create table [dw_control].Watermark (
	[source]        varchar(50)   not null,
	[entity]        varchar(100)  not null,
	last_value      datetime2(3)  not null,
	updated_at      datetime2(3)  not null constraint df_Watermark_updated_at default (sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'),
constraint [pk_Watermark] primary key (
	[source] asc,
	[entity] asc
));
GO

/****** dw_control.sp_Watermark_Get: lê o watermark de uma fonte e entidade ******/
--os módulos de saída não têm select em dw_control (12.2): leem e gravam o próprio watermark só pelas procedures.
--sem linha, devolve null: quem chama decide a janela inicial.
create or alter procedure [dw_control].sp_Watermark_Get
	@source      varchar(50),
	@entity      varchar(100),
	@last_value  datetime2(3) output
as
begin
	set nocount on;

	select @last_value = last_value
	  from [dw_control].Watermark
	 where [source] = @source
	   and [entity] = @entity;
end;
GO

/****** dw_control.sp_Watermark_Set: grava o watermark de uma fonte e entidade ******/
--merge: cria a linha na primeira carga e atualiza nas seguintes. Grava o valor informado, mesmo que menor que o atual,
--para permitir refazer uma janela; quem avança o watermark só depois do sucesso da carga é quem chama.
create or alter procedure [dw_control].sp_Watermark_Set
	@source      varchar(50),
	@entity      varchar(100),
	@last_value  datetime2(3)
as
begin
	set nocount on;

	if @source is null or @entity is null or @last_value is null
		throw 50001, 'sp_Watermark_Set: source, entity e last_value são obrigatórios.', 1;

	merge [dw_control].Watermark as w
	using (select @source as [source], @entity as [entity]) as o
	   on w.[source] = o.[source]
	  and w.[entity] = o.[entity]
	when matched then
		update set last_value = @last_value,
		           updated_at = sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time'
	when not matched then
		insert ([source], [entity], last_value)
		values (o.[source], o.[entity], @last_value);
end;
