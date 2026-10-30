/****** Histórico das origens Totvs da fase 1 (docs/datawarehouse, seção 6 e 16, passo 2) ******/
--um histórico SCD2 e um snapshot por origem com history_frequency no catálogo: notas e pedidos a cada hora; lojas, pessoas,
--empresas, referências, produtos, custos e preços uma vez por dia (pergunta 9, 7.4). A frequência é de quem chama o
--snapshot (o job, 10.1); a procedure é a mesma.
declare @table sysname;
declare sources cursor local fast_forward for
	select source_table from [dw_control].LandingSource where source_schema = 'totvs' and phase = 1 and history_frequency is not null order by source_table;

open sources;
fetch next from sources into @table;

while @@fetch_status = 0
begin
	exec [dw_history].sp_Create_History @table_name = @table;
	fetch next from sources into @table;
end

close sources;
deallocate sources;
