/****** Catálogo das origens do DW (docs/datawarehouse, 5.1 e 5.2) ******/
--uma linha por tabela confiável que o DW lê. Os geradores dw_landing.sp_Create_Landing e dw_history.sp_Create_History
--criam a landing, o histórico e as procedures a partir daqui; cada fase insere as suas linhas no próprio script.
create table [dw_control].LandingSource (
	source_schema          sysname        not null,   --schema do módulo (totvs, linxmicrovix...); é o source do watermark
	source_table           sysname        not null,   --tabela de origem; a landing e o histórico têm o mesmo nome
	included_columns       nvarchar(max)  null,       --lista separada por vírgula; nulo = todas. A PK e integrated_or_updated_at entram sempre
	excluded_columns       nvarchar(max)  null,       --lista separada por vírgula: dado pessoal identificador (5.2, regra 6)
	source_updated_column  sysname        null,       --data da alteração na origem (maxChangeFilterDate, lastChangeDate); nulo = integrated_or_updated_at
	detect_missing         bit            not null constraint df_LandingSource_detect_missing default (0),   --filha que o merge da origem apaga (5.2, regra 3)
	history_frequency      varchar(10)    null,       --hourly, daily; nulo = sem histórico
	phase                  tinyint        not null,   --fase do plano em que a origem entrou (seção 2)
	is_enabled             bit            not null constraint df_LandingSource_is_enabled default (1),   --origem ligada; a de saldos nasce desligada até a carga inicial na Totvs terminar (9.5)
constraint [pk_LandingSource] primary key (
	source_schema asc,
	source_table asc
),
--history_frequency once: snapshot único, chamado pela carga do legado Linx (RunLinxLegacy; 7.5 e 10.2)
constraint [ck_LandingSource_history_frequency] check (history_frequency in ('hourly', 'daily', 'once'))
);
