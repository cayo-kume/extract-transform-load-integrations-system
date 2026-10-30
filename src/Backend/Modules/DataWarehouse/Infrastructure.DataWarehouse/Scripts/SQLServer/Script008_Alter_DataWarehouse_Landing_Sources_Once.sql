/****** dw_control.LandingSource: histórico de snapshot único (docs/datawarehouse, 7.5 e 10.2) ******/
--o legado Linx tem um snapshot só, na carga do legado, e não entra nos ciclos horário e diário: history_frequency 'once'.
--O job agendado chama os snapshots 'hourly' e 'daily'; o 'once' é chamado pela carga do legado (RunLinxLegacy).
alter table [dw_control].LandingSource drop constraint [ck_LandingSource_history_frequency];

alter table [dw_control].LandingSource add
	constraint [ck_LandingSource_history_frequency] check (history_frequency in ('hourly', 'daily', 'once'));
