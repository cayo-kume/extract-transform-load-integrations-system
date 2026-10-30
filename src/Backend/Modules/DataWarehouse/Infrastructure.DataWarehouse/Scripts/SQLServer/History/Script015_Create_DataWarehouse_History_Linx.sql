/****** Histórico do legado Linx Microvix (src/Backend/Modules/DataWarehouse/docs, 7.5 e 10.2) ******/
--snapshot único ('once') de lojas e produtos, chamado pela carga do legado. Cada um vira uma versão só nos membros Linx de
--DimStore e DimProduct; uma reexecução da carga que encontre mudança abre versão nova, como nas outras origens.
declare @table sysname;
declare sources cursor local fast_forward for
	select source_table from [dw_control].LandingSource where source_schema = 'linxmicrovix' and history_frequency = 'once' order by source_table;

open sources;
fetch next from sources into @table;

while @@fetch_status = 0
begin
	exec [dw_history].sp_Create_History @table_name = @table;
	fetch next from sources into @table;
end

close sources;
deallocate sources;
