/****** Resultado das checagens (src/Backend/Modules/DataWarehouse/docs, 9.2 e 3.9) ******/
--uma linha por checagem por execução: o job conta as linhas de cada view datawarehouse.vw_Check_* e grava aqui.
--fica em dw_control, fora da limpeza de 15 dias do auditing (dbo.sp_Delete_Auditing_Logs).
create table [dw_control].CheckResult (
	check_name      varchar(100)  not null,
	executed_at     datetime2(3)  not null,   --horário de Brasília
	error_rows      int           not null,
	passed          bit           not null,
constraint [pk_CheckResult] primary key (
	check_name asc,
	executed_at asc
));
