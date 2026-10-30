INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (lojas: 8500 - 8505)*/
(8000, 8500, 'sp_LinxGrupoLojas_Sync', 'Procedure de sincronização entre as tabelas LinxGrupoLojas raw e trusted', 120, 1),
(8001, 8501, 'sp_LinxLojas_Sync', 'Procedure de sincronização entre as tabelas LinxLojas raw e trusted', 120, 1),
(8002, 8502, 'sp_LinxLojasParametros_Sync', 'Procedure de sincronização entre as tabelas LinxLojasParametros raw e trusted', 120, 1),
(8003, 8503, 'sp_LinxUsuarios_Sync', 'Procedure de sincronização entre as tabelas LinxUsuarios raw e trusted', 120, 1),
(8004, 8504, 'sp_LinxMetodos_Sync', 'Procedure de sincronização entre as tabelas LinxMetodos raw e trusted', 120, 1),
(8005, 8505, 'sp_LinxEntrantes_Sync', 'Procedure de sincronização entre as tabelas LinxEntrantes raw e trusted', 120, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8000, 'LinxGrupoLojas', 'Integração das lojas do grupo (LinxGrupoLojas), a cada 2 horas', 120, 2),
(80, 8001, 'LinxLojas', 'Integração do cadastro das lojas (LinxLojas), a cada 2 horas', 120, 2),
(80, 8002, 'LinxLojasParametros', 'Integração dos parâmetros do portal por loja (LinxLojasParametros), a cada 2 horas', 120, 2),
(80, 8003, 'LinxUsuarios', 'Integração dos usuários do portal (LinxUsuarios), a cada 2 horas', 120, 2),
(80, 8004, 'LinxMetodos', 'Integração do catálogo de métodos da API (LinxMetodos), a cada 2 horas', 120, 2),
(80, 8005, 'LinxEntrantes', 'Integração dos entrantes por loja (LinxEntrantes), a cada 2 horas', 120, 2);
