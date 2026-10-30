/****** dw_landing.sp_Create_Landing: gera a landing de uma origem do catálogo (src/Backend/Modules/DataWarehouse/docs, 4.1, 5.2 e seção 6) ******/
--para a origem @source_schema.@source_table de dw_control.LandingSource, cria (se ainda não existir):
--  dw_landing.<tabela>: as colunas da origem que o catálogo deixa (mesmos nomes e tipos, PK da origem), mais
--      source_updated_at (data da alteração na origem), row_hash (SHA-256 das colunas de negócio),
--      loaded_at (quando a carga gravou a linha; o snapshot lê por ele) e missing_since (filha apagada na origem);
--e cria ou altera:
--  dw_landing.sp_<tabela>_Load: MERGE incremental pelo watermark (source = schema, entity = tabela), relendo 3 dias
--      (5.2, regra 1). Só atualiza a linha quando o hash muda, e só então grava loaded_at;
--  dw_landing.sp_<tabela>_DetectMissing (detect_missing = 1): marca missing_since nas chaves que sumiram da origem.
--o hash ignora a PK e as colunas de controle (integrated_or_updated_at, maxChangeFilterDate, lastChangeDate,
--lastchange, json_hash): só mudança de negócio gera versão nova no histórico.
create or alter procedure [dw_landing].sp_Create_Landing
	@source_schema  sysname,
	@source_table   sysname
as
begin
	set nocount on;

	declare
		@source_object   nvarchar(300) = quotename(@source_schema) + '.' + quotename(@source_table),
		@landing_object  nvarchar(300) = '[dw_landing].' + quotename(@source_table),
		@included        nvarchar(max),
		@excluded        nvarchar(max),
		@updated_column  sysname,
		@detect_missing  bit,
		@message         nvarchar(2048);

	select @included = included_columns, @excluded = excluded_columns, @updated_column = source_updated_column, @detect_missing = detect_missing
	  from [dw_control].LandingSource
	 where source_schema = @source_schema
	   and source_table = @source_table;

	if @@rowcount = 0
	begin
		set @message = concat('sp_Create_Landing: ', @source_object, ' não está em dw_control.LandingSource.');
		throw 50010, @message, 1;
	end

	if object_id(@source_object) is null
	begin
		set @message = concat('sp_Create_Landing: a tabela de origem ', @source_object, ' não existe.');
		throw 50011, @message, 1;
	end

	declare @columns table (
		column_id  int,
		name       sysname,
		type_sql   nvarchar(100),
		pk_order   int,
		in_hash    bit,
		is_date    bit,
		is_binary  bit
	);

	insert into @columns
	select
		c.column_id,
		c.name,
		case
			when t.name in ('nvarchar', 'nchar') then concat(t.name, '(', iif(c.max_length = -1, 'max', cast(c.max_length / 2 as varchar(10))), ')')
			when t.name in ('varchar', 'char', 'varbinary', 'binary') then concat(t.name, '(', iif(c.max_length = -1, 'max', cast(c.max_length as varchar(10))), ')')
			when t.name in ('decimal', 'numeric') then concat(t.name, '(', c.precision, ',', c.scale, ')')
			when t.name in ('datetime2', 'time', 'datetimeoffset') then concat(t.name, '(', c.scale, ')')
			else t.name
		end,
		pk.key_ordinal,
		iif(pk.key_ordinal is null and c.name not in ('integrated_or_updated_at', 'maxChangeFilterDate', 'lastChangeDate', 'lastchange', 'json_hash'), 1, 0),
		iif(t.name in ('date', 'datetime', 'datetime2', 'smalldatetime', 'datetimeoffset', 'time'), 1, 0),
		iif(t.name in ('binary', 'varbinary', 'timestamp'), 1, 0)
	from sys.columns c
	join sys.types t
	  on t.user_type_id = c.user_type_id
	left join (
		select ic.column_id, ic.key_ordinal
		  from sys.indexes i
		  join sys.index_columns ic
		    on ic.object_id = i.object_id
		   and ic.index_id = i.index_id
		 where i.object_id = object_id(@source_object)
		   and i.is_primary_key = 1
	) pk
	  on pk.column_id = c.column_id
	where c.object_id = object_id(@source_object)
	  and (
			@included is null
			or pk.key_ordinal is not null
			or c.name in ('integrated_or_updated_at', @updated_column)
			or c.name collate database_default in (select trim(value) from string_split(@included, ','))
		  )
	  and (
			@excluded is null
			or c.name collate database_default not in (select trim(value) from string_split(@excluded, ','))
		  );

	if not exists (select 1 from @columns where pk_order is not null)
	begin
		set @message = concat('sp_Create_Landing: ', @source_object, ' não tem chave primária.');
		throw 50012, @message, 1;
	end

	if not exists (select 1 from @columns where name = 'integrated_or_updated_at')
	begin
		set @message = concat('sp_Create_Landing: ', @source_object, ' não tem integrated_or_updated_at.');
		throw 50013, @message, 1;
	end

	if (select count(*) from @columns where in_hash = 1) > 250
	begin
		set @message = concat('sp_Create_Landing: ', @source_object, ' tem mais de 250 colunas de negócio; o concat_ws do hash não comporta.');
		throw 50014, @message, 1;
	end

	declare
		@now_expr        nvarchar(200) = N'sysutcdatetime() at time zone ''UTC'' at time zone ''E. South America Standard Time''',
		@column_ddl      nvarchar(max),
		@pk_list         nvarchar(max),
		@column_list     nvarchar(max),
		@source_list     nvarchar(max),
		@update_list     nvarchar(max),
		@on_list         nvarchar(max),
		@exists_list     nvarchar(max),
		@hash_expr       nvarchar(max),
		@updated_expr    nvarchar(max),
		@sql             nvarchar(max);

	select
		@column_ddl  = string_agg(cast(concat(char(9), quotename(name), ' ', type_sql, iif(pk_order is null, ' null', ' not null')) as nvarchar(max)), ',' + char(10)) within group (order by column_id),
		@column_list = string_agg(cast(quotename(name) as nvarchar(max)), ', ') within group (order by column_id),
		@source_list = string_agg(cast('source.' + quotename(name) as nvarchar(max)), ', ') within group (order by column_id)
	from @columns;

	select
		@pk_list     = string_agg(cast(quotename(name) as nvarchar(max)), ', ') within group (order by pk_order),
		@on_list     = string_agg(cast(concat('target.', quotename(name), ' = source.', quotename(name)) as nvarchar(max)), ' and ') within group (order by pk_order),
		@exists_list = string_agg(cast(concat('s.', quotename(name), ' = l.', quotename(name)) as nvarchar(max)), ' and ') within group (order by pk_order)
	from @columns
	where pk_order is not null;

	select @update_list = string_agg(cast(concat('target.', quotename(name), ' = source.', quotename(name)) as nvarchar(max)), ', ') within group (order by column_id)
	  from @columns
	 where pk_order is null;

	--o primeiro valor fixo deixa o concat_ws válido com uma coluna só; nulo vira ~ para não colidir com vazio
	select @hash_expr = concat(
		'hashbytes(''SHA2_256'', concat_ws(N''|'', N''v1'', ',
		string_agg(cast(concat(
			'isnull(',
			case
				when is_date = 1 then concat('convert(nvarchar(40), ', quotename(name), ', 126)')
				when is_binary = 1 then concat('convert(nvarchar(max), ', quotename(name), ', 1)')
				else concat('convert(nvarchar(max), ', quotename(name), ')')
			end,
			', N''~'')') as nvarchar(max)), ', ') within group (order by column_id),
		'))')
	  from @columns
	 where in_hash = 1;

	set @hash_expr = isnull(@hash_expr, 'hashbytes(''SHA2_256'', N''v1'')');

	set @updated_expr = iif(
		@updated_column is null,
		'isnull(integrated_or_updated_at, ''19000101'')',
		concat('isnull(cast(', quotename(@updated_column), ' as datetime), isnull(integrated_or_updated_at, ''19000101''))'));

	--------------------------------------------------------------------------------------------------------------------------------------
	--Tabela
	if object_id(@landing_object) is null
	begin
		set @sql = concat(
			'create table ', @landing_object, ' (', char(10),
			@column_ddl, ',', char(10),
			char(9), '[source_updated_at] datetime not null,', char(10),
			char(9), '[row_hash] binary(32) not null,', char(10),
			char(9), '[loaded_at] datetime2(3) not null constraint ', quotename(concat('df_landing_', @source_table, '_loaded_at')), ' default (', @now_expr, '),', char(10),
			char(9), '[missing_since] datetime2(3) null,', char(10),
			'constraint ', quotename(concat('pk_landing_', @source_table)), ' primary key (', @pk_list, ')', char(10),
			');', char(10),
			'create index ', quotename(concat('ix_landing_', @source_table, '_loaded_at')), ' on ', @landing_object, ' ([loaded_at]);');

		exec sp_executesql @sql;
	end

	--------------------------------------------------------------------------------------------------------------------------------------
	--Carga
	set @sql = concat(
		'create or alter procedure [dw_landing].', quotename(concat('sp_', @source_table, '_Load')), char(10),
		'	@rows int = null output', char(10),
		'as', char(10),
		'begin', char(10),
		'	--gerada por dw_landing.sp_Create_Landing: não edite, gere de novo', char(10),
		'	set nocount on;', char(10),
		'	set xact_abort on;', char(10), char(10),
		'	declare @since datetime2(3), @from datetime, @until datetime,', char(10),
		'	        @now datetime2(3) = ', @now_expr, ';', char(10), char(10),
		'	exec [dw_control].sp_Watermark_Get N', quotename(@source_schema, ''''), ', N', quotename(@source_table, ''''), ', @since output;', char(10),
		'	set @from = isnull(dateadd(day, -3, @since), ''19000101'');   --3 dias de sobreposição (5.2, regra 1)', char(10), char(10),
		'	select @until = max(integrated_or_updated_at) from ', @source_object, ' where isnull(integrated_or_updated_at, ''19000101'') >= @from;', char(10),
		'	set @rows = 0;', char(10), char(10),
		'	begin transaction;', char(10), char(10),
		'	merge ', @landing_object, ' as target', char(10),
		'	using (', char(10),
		'		select ', @column_list, ',', char(10),
		'		       ', @updated_expr, ' as source_updated_at,', char(10),
		'		       ', @hash_expr, ' as row_hash', char(10),
		'		  from ', @source_object, char(10),
		'		 where isnull(integrated_or_updated_at, ''19000101'') >= @from', char(10),
		'	) as source', char(10),
		'	on (', @on_list, ')', char(10),
		'	when matched and (target.row_hash <> source.row_hash or target.missing_since is not null) then', char(10),
		'		update set ', isnull(@update_list + ', ', ''), 'target.source_updated_at = source.source_updated_at, target.row_hash = source.row_hash, target.loaded_at = @now, target.missing_since = null', char(10),
		'	when not matched by target then', char(10),
		'		insert (', @column_list, ', source_updated_at, row_hash, loaded_at)', char(10),
		'		values (', @source_list, ', source.source_updated_at, source.row_hash, @now);', char(10), char(10),
		'	set @rows = @@rowcount;', char(10), char(10),
		'	if @until is not null', char(10),
		'		exec [dw_control].sp_Watermark_Set N', quotename(@source_schema, ''''), ', N', quotename(@source_table, ''''), ', @until;', char(10), char(10),
		'	commit transaction;', char(10),
		'end');

	exec sp_executesql @sql;

	--------------------------------------------------------------------------------------------------------------------------------------
	--Exclusões de filhas (5.2, regra 3)
	if @detect_missing = 1
	begin
		set @sql = concat(
			'create or alter procedure [dw_landing].', quotename(concat('sp_', @source_table, '_DetectMissing')), char(10),
			'	@rows int = null output', char(10),
			'as', char(10),
			'begin', char(10),
			'	--gerada por dw_landing.sp_Create_Landing: marca as chaves que o merge da origem apagou; a carga limpa a marca se a chave voltar', char(10),
			'	set nocount on;', char(10), char(10),
			'	declare @now datetime2(3) = ', @now_expr, ';', char(10), char(10),
			'	update l', char(10),
			'	   set l.missing_since = @now', char(10),
			'	  from ', @landing_object, ' l', char(10),
			'	 where l.missing_since is null', char(10),
			'	   and not exists (select 1 from ', @source_object, ' s where ', @exists_list, ');', char(10), char(10),
			'	set @rows = @@rowcount;', char(10),
			'end');

		exec sp_executesql @sql;
	end
end;
