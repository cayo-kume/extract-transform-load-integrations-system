set ansi_nulls on
go

set quoted_identifier off
go

--declare @schema_name nvarchar(50) = 'general', @table_name nvarchar(50) = 'AfterSaleSummarizedReverses', @execute bit = 0

create procedure [dbo].[sp_Create_Merge_Procedure] (@schema_name nvarchar(50), @table_name nvarchar(50), @execute bit = 0) as
begin

	set nocount on

	set @schema_name = trim('[ ]' from @schema_name)
	set @table_name = trim('[ ]' from @table_name)

	declare
		@complete_table_name	 varchar(300) = concat(@schema_name, '.', @table_name),
		@complete_untreated_name varchar(300) = concat('untreated', '.', @table_name),

		@procedure_version		 varchar(10) = '0.7.3',
		@procedure_name			 varchar(300) = concat(@schema_name, ".sp_", @table_name, '_Sync'),

		@transaction_name		 varchar(32) = concat('t_', left(@table_name, 30))

	if (object_id (@complete_table_name) is null or object_id (@complete_untreated_name) is null)
		begin
			select 'The table: ' + @complete_table_name + ' or ' + @complete_untreated_name + ' does not exists, create the two tables before creating the synchronization procedure.' 
			return
		end

	--Is Microvix ERP Table (has timestamp column)
	declare 
		@has_timestamp_column bit = (
			select 
				count(*)
			from
				information_schema.columns
			where
				table_schema = @schema_name
				and	table_name = @table_name
				and	column_name collate Latin1_General_CI_AS = 'timestamp'
		)

	--------------------------------------------------------------------------------------------------------------------------------------
	--Variáveis auxiliares
	
	declare @pk_column_list				nvarchar(max),
			@pk_column_list_output		nvarchar(max),
			@column_list				nvarchar(max),
			@column_list_source			nvarchar(max),
			@output_agg_list			nvarchar(max),
			@sql_initial_treatment		nvarchar(max) = '',
			@sql_lock_start				nvarchar(max),
			@sql_delete_duplicate_rows	nvarchar(max) = '',
			@sql_merge					nvarchar(max) = '',
			@sql_merge_upd				nvarchar(max) = '',
			@sql_merge_ins				nvarchar(max) = '',
			@sql_log_final				nvarchar(max) = '',
			@sql_lock_end				nvarchar(max)

	declare @pk table (
		[column]   nvarchar(50), 
		datatype   varchar(40),
		[order]    int
	)

	--Faz o merge usando todos os campos
	begin	
		insert into @pk 
			select
				kcu.column_name as [column], 
				c.data_type, 
				kcu.ordinal_position as [order]
			from 
				information_schema.key_column_usage kcu
			join 
				information_schema.columns as c on kcu.table_name = c.table_name AND kcu.column_name = c.column_name AND kcu.table_schema = c.table_schema
			where	
				objectproperty(object_id(quotename(kcu.constraint_schema) + '.' + quotename(kcu.constraint_name)), 'IsPrimaryKey') = 1
				and	kcu.table_name = @table_name
				and	kcu.constraint_schema = @schema_name
	
		if @@rowcount = 0 
		begin
			select 'The table: ' + @complete_table_name + ' does not exists or havent any primary key.'
			return
		end
	end

	select 
		@output_agg_list = concat( 
			" concat(", 
				string_agg(	
					case 
						when datatype in ('datetime', 'datetime2') then concat('format(deleted.', quotename([column]), ',''yyyy.MM.dd HH:mm:ss'')')
						ELSE concat('deleted.', quotename([column]))
					end, ",'|',"
				) within group (order by [order]),
				iif(@has_timestamp_column = 1, ", '|', deleted.timestamp", ""),
				iif(count([column]) = 1 and @has_timestamp_column = 0, ",''", ""),
			")"
		)
	from 
		@pk

	select 
		@pk_column_list	= string_agg(quotename([column]), ', ') within group (order by [order]),
		@pk_column_list_output = string_agg(
									concat(
										"coalesce(",
											case 
												when datatype IN ('datetime', 'datetime2') then concat('format(inserted.', quotename([column]), ',''yyyy.MM.dd HH:mm:ss'')')
												else concat('inserted.', quotename([column]))
											end,
											",",
											char(9),
											"",
											case 
												when datatype IN ('datetime','datetime2') then concat('format(deleted.', quotename([column]),',''yyyy.MM.dd HH:mm:ss'')')
												else concat('deleted.', quotename([column]))
											end,
										")"
									),
									",'|'," + 
									char(10) + 
									replicate(char(9), 6)) within group (order by [order])
		from 
			@pk

	select 
		--lista de colunas do merge when not matched by target then insert
		@column_list = string_agg(convert(nvarchar(max), replicate(char(9), 2) + quotename(column_name)), ',' + char(10)) within group (order by ordinal_position),
		--lista de colunas do merge when not matched by target then insert values
		@column_list_source = string_agg(convert(nvarchar(max), replicate(char(9), 2) + 'source.' + quotename(column_name)), ',' + char(10)) within group (order by ordinal_position)
	from 
		information_schema.columns
	where
		table_name = @table_name
		and table_schema = @schema_name

	--lista de colunas do merge when matched then update set
	--a lista de campos, se feita utilizando o string_agg, estoura o tamanho. foi necessário criar o cursor abaixo
	declare @column_name nvarchar(128);
	declare cur cursor local forward_only for

	select quotename(column_name)
	from information_schema.columns
	where table_name = trim('[ ]' from @table_name)
	and	table_schema = @schema_name

	open cur;
	fetch next from cur into @column_name;

	if cursor_status('local', 'cur') < 1 or @column_name is null
		set @sql_merge_upd = ''
	else
	begin
		while @@fetch_status = 0
		begin
			set @sql_merge_upd += replicate(char(9), 2) + 'target.' + @column_name + ' = source.' + @column_name;

			fetch next from cur into @column_name;

			if @@fetch_status = 0
				set @sql_merge_upd += ',' + char(10);  -- ainda tem coluna: vírgula
			else
				set @sql_merge_upd += char(10); -- era a última: só quebra de linha
		end;
	end

	close cur;
	deallocate cur;

	set @sql_initial_treatment = concat(
		--------------------------------------------------------------------------------------------------------------------------------------
		--Início. logs, tratamento especial para tabelas, exclusão de duplicidades e de registros já integrados.
		"--declare @Guid uniqueidentifier = null, @Debug bit = 1", char(10),
		"create or alter proc ", @procedure_name, " (@Guid uniqueidentifier = null, @Debug bit = 0)", char(10),
		"as", char(10),
		"begin", char(10),
		"--procedure created automatically by the [general].[create_merge_procedure] process in ", format(sysutcdatetime() at time zone "UTC" at time zone "E. South America Standard Time", "yyyy.MM.dd HH:mm:ss"), char(10),
		"--version ", @procedure_version, char(10), char(10),
		"set nocount on;", char(10),
		"set transaction isolation level read uncommitted;", char(10),
		"set concat_null_yields_null off;", char(10), char(10),

		"declare @Execution	uniqueidentifier", char(10),
		"declare @Output dbo.MergeOutput", char(10),
		"declare @IdJob	smallint", char(10), char(10),

		"exec auditing.sp_Log_Start 'sp_", @table_name, "_Sync', @Guid, @IdJob, @Execution output", char(10), char(10),

		"declare @res bit", char(10), char(10),
		"begin try", char(10),
		"	begin transaction ", @transaction_name, ";", char(10), char(10)
	);

	set @sql_lock_start = concat(
		"	--parallelism control", char(10),
		"	exec dbo.sp_TryApplock", char(10),
		"	@Resource = ", "'", @procedure_name, "',", char(10),
		"	@LockAcquired = @res output", char(10), char(10),

		"	if @res < 0", char(10),
		"		raiserror ('The process is already running', 16, 1)", char(10), char(10),
	
		"	--------------------------------------------", char(10), char(10),

		"	if @Debug = 1", char(10),
		"		print convert(nvarchar(23), format(sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time', 'yyyy.MM.dd HH:mm:ss'), 121) + ' 1 - Begin - Registers Ignoreds'", char(10), char(10)
	);

	set @sql_delete_duplicate_rows = concat(
		-- TESTAR SE A UNIFICAÇÃO DESSES DOIS CASES VAI DAR CERTO
		--iif(
		--	@has_timestamp_column = 1,
		--	concat(
		--	"	--delete duplicate rows", char(10),
		--	"	;with repeating_rows as (", char(10),
		--	"		select ", @pk_column_list, ", [timestamp],", char(10),
		--	"			row_number() over (partiton by ", @pk_column_list, " order by [timestamp] desc) as rn", char(10),
		--	"		from untreated.", @table_name, " r)", char(10),
		--	"	delete", char(10),
		--	"	from repeating_rows", char(10),
		--	"	output 'duplicados',", @output_agg_list, "", char(10),
		--	"	into @output", char(10),
		--	"	where rn > 1;", char(10),
		--
		--	"	if @Debug = 1", char(10),
		--	"		print convert(nvarchar(23), format(sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time', 'yyyy.MM.dd HH:mm:ss'), 121) + ' 2 - Deleted Duplicated Registers ' + convert(nvarchar(10), @@rowcount)", char(10),
		--	"			+ convert(nvarchar(23), format(sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time', 'yyyy.MM.dd HH:mm:ss'), 121) + ' 3 - Begin Merge '", char(10)
		--	),
		--	(
		--		select 
		--			concat(
		--			"	--delete duplicate rows", char(10),
		--			"	;with repeating_rows as (", char(10),
		--			"		select ", string_agg([column], ',') within group (order by [order]), ", max(id) maxid", char(10),
		--			"		from ", @complete_untreated_name, char(10),
		--			"		group by ", string_agg([column], ',') within group (order by [order]), char(10),
		--			"		having count(1) > 1)", char(10),
		--			"	delete from ", @complete_untreated_name, char(10),
		--			"	output 'Ignoreds',", @output_agg_list, char(10),
		--			"	into @output", char(10),
		--			"	from ", @complete_untreated_name, " r", char(10),
		--			"	join repeating_rows", char(10),
		--			"	on ", string_agg(concat("r.", [column], " = repeating_rows.", [column]), "and" + char(10) + replicate(char(9), 3)) within group (order by [order]), char(10),
		--			"	and r.id != maxid;"
		--			) 
		--		from @pk
		--	)
		--), char(10), char(10),

		"	--delete duplicate rows", char(10),
		"	;with repeating_rows as (", char(10),
		"		select ", @pk_column_list, ", integrated_or_updated_at,", char(10),
		"			row_number() over (partition by ", @pk_column_list, " order by integrated_or_updated_at desc) as rn", char(10),
		"		from ", @complete_untreated_name, " r)", char(10),
		"	delete", char(10),
		"	from repeating_rows", char(10),
		"	output 'Duplicados',", @output_agg_list, char(10),
		"	into @output", char(10),
		"	where rn > 1;", char(10),

		"	if @Debug = 1", char(10),
		"		print convert(nvarchar(23), format(sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time', 'yyyy.MM.dd HH:mm:ss'), 121) + ' 2 - Deleted Duplicated Registers ' + convert(nvarchar(10), @@rowcount)", char(10),
		char(10), char(10)
	);

	set @sql_merge = concat(
		--------------------------------------------------------------------------------------------------------------------------------------
		--Merge
		concat(
		"	merge ", @complete_table_name, " as target", char(10),
		"	using ", @complete_untreated_name, " as source", char(10), 
		"	on (", char(10)
		),
		(select string_agg(concat("		target.", quotename([column])," = source.", quotename([column])), " and" + char(10) + replicate(char(9), 3) + ' ') from @pk), char(10),
		"	)", char(10)
	)

	set @sql_merge_upd = concat(
		--------------------------------------------------------------------------------------------------------------------------------------
		--Update
		"	when matched ",
		iif(@has_timestamp_column = 1, "and source.timestamp >= target.timestamp ", ""), "then update set", char(10),
		@sql_merge_upd
	);

	set @sql_merge_ins =  concat (
		--------------------------------------------------------------------------------------------------------------------------------------
		--Insert
		-- revisar: @column_list_source e montado com quotename, entao o alvo do replace
		-- precisa dos colchetes ('source.[lastupdateon]'); sem eles nunca casa.
		"	when not matched by target then", char(10),
		"	insert (", char(10),
		@column_list, char(10),
		"	)", char(10),
		"	values (", char(10),
		replace(@column_list_source collate Latin1_General_CI_AS, 'source.[lastupdateon]', 'getdate()'), char(10),
		"	)", char(10), char(10)
	);

	set @sql_log_final = concat(
		--------------------------------------------------------------------------------------------------------------------------------------
		--Insere todos os campos do output do merge, separados por ação.
		"	output iif($action = 'update', 'atualizados', 'inseridos'),", char(10),
		"	concat( ", @pk_column_list_output, iif(@has_timestamp_column = 1, ", '|', isnull(inserted.[timestamp], deleted.[timestamp])", ""), ", '')", char(10),
		"	into @output;", char(10), char(10),
		
		--------------------------------------------------------------------------------------------------------------------------------------
		--Finalização da procedure. loga, devolve o lock e fecha a transação.
		"	--truncate untreated table", char(10),
		"	delete r", char(10),
		"	from ", @complete_untreated_name, " r", char(10),
		"	join @output o", char(10),
		"	on o.chave = ", replace(@output_agg_list, 'deleted.', 'r.'), char(10), char(10),

		"	--Log registros que ficaram", char(10),
		"	insert into @output", char(10),
		"	select 'withou pk',", @output_agg_list, char(10),
		"	from ", @complete_untreated_name, " deleted", char(10), char(10)
	);

	set @sql_lock_end = concat(
		"	if @Debug = 1", char(10),
		"	begin", char(10),
		"		print convert(nvarchar(23), format(sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time', 'yyyy.MM.dd HH:mm:ss'), 121) + ' 9 - ' + convert(nvarchar(10), @@rowcount) + ' registers deleted'", char(10),
		"		select operacao, COUNT(*) c from @output group by operacao", char(10),
		"		select * from auditing.logs		where execution = @execution", char(10),
		"		select * from auditing.messages	where execution = @execution", char(10),
		"	end", char(10), char(10),
		"	commit transaction ", @transaction_name, ";", char(10), char(10),
		"	end try", char(10),
		"	begin catch", char(10),
		"		select error_message()", char(10),
		"		rollback transaction ", @transaction_name, char(10),
		"	end catch", char(10),
		"	--log end", char(10),
		"	declare @MergeOutputSTR nvarchar(max) =", char(10),
		"	(", char(10),
		"	select	operacao, chave", char(10),
		"	from @output", char(10),
		"	for xml	path('item'), root('root')", char(10),
		");", char(10),
		"	exec auditing.sp_Log_End @execution, @MergeOutputSTR;", char(10),
		"end"
	);

	----------------------------------------------------------------
	
	if @execute =0
	begin
		--Retorno da proc em texto
		select @sql_merge + char(10) + "go"
	end
	else
	begin
		--Cria automaticamente
		set @sql_initial_treatment+=concat(
			@sql_lock_start,
			@sql_delete_duplicate_rows,
			@sql_merge,
			@sql_merge_upd,
			@sql_merge_ins,
			@sql_log_final,
			@sql_lock_end
		)
		exec sp_executesql @sql_initial_treatment
	end
END

GO