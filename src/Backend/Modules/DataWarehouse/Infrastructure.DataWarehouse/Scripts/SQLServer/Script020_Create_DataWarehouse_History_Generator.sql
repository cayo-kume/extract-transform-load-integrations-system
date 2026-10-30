/****** dw_history.sp_Create_History: gera o histórico SCD2 de uma landing (docs/datawarehouse, 3.3 e seção 6) ******/
--para dw_landing.@table_name, cria (se ainda não existir):
--  dw_history.<tabela>: PK da origem + valid_from, valid_to, is_current, row_hash e as colunas de negócio da landing
--      (as mesmas que entram no hash). Um índice único filtrado garante uma só versão vigente por chave;
--e cria ou altera:
--  dw_history.sp_<tabela>_Snapshot: snapshot incremental. Lê só as linhas que a carga gravou na landing desde a execução
--      anterior (loaded_at acima do watermark source = dw_history, entity = tabela), fecha a versão vigente das que mudaram
--      de hash e abre a versão nova, tudo numa transação com o avanço do watermark.
--valid_from é a data da alteração na origem (source_updated_at). Se ela não for maior que o valid_from da versão vigente
--(maxChangeFilterDate repetida ou que voltou), a versão nova começa 1 ms depois da vigente: a PK não colide (risco R2).
create or alter procedure [dw_history].sp_Create_History
	@table_name  sysname
as
begin
	set nocount on;

	declare
		@landing_object  nvarchar(300) = '[dw_landing].' + quotename(@table_name),
		@history_object  nvarchar(300) = '[dw_history].' + quotename(@table_name),
		@message         nvarchar(2048);

	if object_id(@landing_object) is null
	begin
		set @message = concat('sp_Create_History: a landing ', @landing_object, ' não existe; rode dw_landing.sp_Create_Landing antes.');
		throw 50020, @message, 1;
	end

	declare @columns table (
		column_id  int,
		name       sysname,
		type_sql   nvarchar(100),
		pk_order   int
	);

	--as colunas da landing, menos as de controle: as mesmas do hash (sp_Create_Landing) e a PK
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
		pk.key_ordinal
	from sys.columns c
	join sys.types t
	  on t.user_type_id = c.user_type_id
	left join (
		select ic.column_id, ic.key_ordinal
		  from sys.indexes i
		  join sys.index_columns ic
		    on ic.object_id = i.object_id
		   and ic.index_id = i.index_id
		 where i.object_id = object_id(@landing_object)
		   and i.is_primary_key = 1
	) pk
	  on pk.column_id = c.column_id
	where c.object_id = object_id(@landing_object)
	  and c.name not in ('integrated_or_updated_at', 'maxChangeFilterDate', 'lastChangeDate', 'lastchange', 'json_hash',
	                     'source_updated_at', 'row_hash', 'loaded_at', 'missing_since');

	declare
		@now_expr        nvarchar(200) = N'sysutcdatetime() at time zone ''UTC'' at time zone ''E. South America Standard Time''',
		@pk_ddl          nvarchar(max),
		@business_ddl    nvarchar(max),
		@pk_list         nvarchar(max),
		@business_list   nvarchar(max),
		@pk_join_sh      nvarchar(max),
		@pk_join_hc      nvarchar(max),
		@sql             nvarchar(max);

	select
		@pk_ddl     = string_agg(cast(concat(char(9), quotename(name), ' ', type_sql, ' not null') as nvarchar(max)), ',' + char(10)) within group (order by pk_order),
		@pk_list    = string_agg(cast(quotename(name) as nvarchar(max)), ', ') within group (order by pk_order),
		@pk_join_sh = string_agg(cast(concat('h.', quotename(name), ' = s.', quotename(name)) as nvarchar(max)), ' and ') within group (order by pk_order),
		@pk_join_hc = string_agg(cast(concat('h.', quotename(name), ' = c.', quotename(name)) as nvarchar(max)), ' and ') within group (order by pk_order)
	from @columns
	where pk_order is not null;

	select
		@business_ddl  = string_agg(cast(concat(char(9), quotename(name), ' ', type_sql, ' null') as nvarchar(max)), ',' + char(10)) within group (order by column_id),
		@business_list = string_agg(cast(quotename(name) as nvarchar(max)), ', ') within group (order by column_id)
	from @columns
	where pk_order is null;

	--------------------------------------------------------------------------------------------------------------------------------------
	--Tabela
	if object_id(@history_object) is null
	begin
		set @sql = concat(
			'create table ', @history_object, ' (', char(10),
			@pk_ddl, ',', char(10),
			char(9), '[valid_from] datetime2(3) not null,', char(10),
			char(9), '[valid_to] datetime2(3) null,', char(10),
			char(9), '[is_current] bit not null,', char(10),
			char(9), '[row_hash] binary(32) not null', iif(@business_ddl is null, '', ','), char(10),
			isnull(@business_ddl + char(10), ''),
			'constraint ', quotename(concat('pk_history_', @table_name)), ' primary key (', @pk_list, ', [valid_from])', char(10),
			');', char(10),
			'create unique index ', quotename(concat('ux_history_', @table_name, '_current')), ' on ', @history_object, ' (', @pk_list, ') where [is_current] = 1;');

		exec sp_executesql @sql;
	end

	--------------------------------------------------------------------------------------------------------------------------------------
	--Snapshot
	set @sql = concat(
		'create or alter procedure [dw_history].', quotename(concat('sp_', @table_name, '_Snapshot')), char(10),
		'	@rows int = null output', char(10),
		'as', char(10),
		'begin', char(10),
		'	--gerada por dw_history.sp_Create_History: não edite, gere de novo', char(10),
		'	set nocount on;', char(10),
		'	set xact_abort on;', char(10), char(10),
		'	declare @since datetime2(3), @until datetime2(3);', char(10), char(10),
		'	exec [dw_control].sp_Watermark_Get N''dw_history'', N', quotename(@table_name, ''''), ', @since output;', char(10),
		'	set @since = isnull(@since, ''19000101'');', char(10),
		'	select @until = max(loaded_at) from ', @landing_object, ';', char(10),
		'	set @rows = 0;', char(10), char(10),
		'	if @until is null or @until <= @since', char(10),
		'		return;   --a carga não gravou nada desde o último snapshot', char(10), char(10),
		'	begin transaction;', char(10), char(10),
		'	--candidatas: gravadas desde o último snapshot e sem versão vigente com o mesmo hash', char(10),
		'	select s.', replace(@pk_list, ', ', ', s.'), iif(@business_list is null, '', concat(', s.', replace(@business_list, ', ', ', s.'))), ', s.row_hash,', char(10),
		'	       case when h.valid_from is null or cast(s.source_updated_at as datetime2(3)) > h.valid_from', char(10),
		'	            then cast(s.source_updated_at as datetime2(3))', char(10),
		'	            else dateadd(millisecond, 1, h.valid_from) end as new_valid_from', char(10),
		'	  into #candidates', char(10),
		'	  from ', @landing_object, ' s', char(10),
		'	  left join ', @history_object, ' h', char(10),
		'	    on ', @pk_join_sh, ' and h.is_current = 1', char(10),
		'	 where s.loaded_at > @since', char(10),
		'	   and s.loaded_at <= @until', char(10),
		'	   and (h.row_hash is null or h.row_hash <> s.row_hash);', char(10), char(10),
		'	--fecha a versão vigente das que mudaram', char(10),
		'	update h', char(10),
		'	   set h.valid_to = c.new_valid_from,', char(10),
		'	       h.is_current = 0', char(10),
		'	  from ', @history_object, ' h', char(10),
		'	  join #candidates c', char(10),
		'	    on ', @pk_join_hc, char(10),
		'	 where h.is_current = 1;', char(10), char(10),
		'	--abre a versão nova', char(10),
		'	insert into ', @history_object, ' (', @pk_list, ', valid_from, valid_to, is_current, row_hash', iif(@business_list is null, '', ', ' + @business_list), ')', char(10),
		'	select ', @pk_list, ', new_valid_from, null, 1, row_hash', iif(@business_list is null, '', ', ' + @business_list), char(10),
		'	  from #candidates;', char(10), char(10),
		'	set @rows = @@rowcount;', char(10), char(10),
		'	--avança o watermark na mesma transação: se o snapshot falhar, a próxima execução relê as mesmas linhas', char(10),
		'	exec [dw_control].sp_Watermark_Set N''dw_history'', N', quotename(@table_name, ''''), ', @until;', char(10), char(10),
		'	commit transaction;', char(10),
		'end');

	exec sp_executesql @sql;
end;
