/****** dw_control.LandingSource: origem ligada ou desligada (docs/datawarehouse, 9.5 e 16, passo 4) ******/
--o job só chama as procedures das origens ligadas. Serve para criar a landing de uma origem antes de ela poder ser lida:
--a de saldos nasce desligada e só é ligada (update ... set is_enabled = 1) depois que a carga inicial dos saldos na Totvs
--terminar (1.2, item 15). As origens que já existem continuam ligadas.
alter table [dw_control].LandingSource add
	is_enabled bit not null constraint df_LandingSource_is_enabled default (1);
