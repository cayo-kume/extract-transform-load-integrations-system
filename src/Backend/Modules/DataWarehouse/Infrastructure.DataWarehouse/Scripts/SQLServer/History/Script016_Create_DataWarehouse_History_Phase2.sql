/****** Histórico das origens da fase 2 (src/Backend/Modules/DataWarehouse/docs, seção 6 e 7.4) ******/
--só os saldos têm histórico: SCD2 com snapshot diário no fim do dia (pergunta 10). Tickets e reembolsos entram nos fatos
--pelo estado atual; o próprio Movidesk e o AfterSale guardam o histórico de status nas tabelas *StatusHistories.
declare @table sysname;
declare sources cursor local fast_forward for
	select source_table from [dw_control].LandingSource where phase = 2 and history_frequency is not null order by source_table;

open sources;
fetch next from sources into @table;

while @@fetch_status = 0
begin
	exec [dw_history].sp_Create_History @table_name = @table;
	fetch next from sources into @table;
end

close sources;
deallocate sources;
