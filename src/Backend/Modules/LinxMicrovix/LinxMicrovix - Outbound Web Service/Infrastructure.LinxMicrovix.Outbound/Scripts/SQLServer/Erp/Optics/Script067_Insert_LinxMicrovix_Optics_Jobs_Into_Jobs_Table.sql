INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (ótica: 8666 - 8675)*/
(8166, 8666, 'sp_LinxProdutosOpticosTipo_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosOpticosTipo raw e trusted', 120, NULL, 'default', 1),
(8167, 8667, 'sp_LinxProdutosOpticosTipoAro_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosOpticosTipoAro raw e trusted', 120, NULL, 'default', 1),
(8168, 8668, 'sp_LinxProdutosOpticosFormatoAro_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosOpticosFormatoAro raw e trusted', 120, NULL, 'default', 1),
(8169, 8669, 'sp_LinxDadosOpticosDav_Sync', 'Procedure de sincronização entre as tabelas LinxDadosOpticosDav raw e trusted', 120, NULL, 'default', 1),
(8170, 8670, 'sp_LinxOticoPrismaDescricao_Sync', 'Procedure de sincronização entre as tabelas LinxOticoPrismaDescricao raw e trusted', 120, NULL, 'default', 1),
(8171, 8671, 'sp_LinxOticoReceitas_Sync', 'Procedure de sincronização entre as tabelas LinxOticoReceitas raw e trusted', 120, NULL, 'default', 1),
(8172, 8672, 'sp_LinxPosicaoOsRamoOptico_Sync', 'Procedure de sincronização entre as tabelas LinxPosicaoOsRamoOptico raw e trusted', 120, NULL, 'default', 1),
(8173, 8673, 'sp_LinxOrdensServicoPosicaoOsRamoOpticoHistorico_Sync', 'Procedure de sincronização entre as tabelas LinxOrdensServicoPosicaoOsRamoOpticoHistorico raw e trusted', 120, NULL, 'default', 1),
(8174, 8674, 'sp_LinxOrcamentoComponenteFormula_Sync', 'Procedure de sincronização entre as tabelas LinxOrcamentoComponenteFormula raw e trusted', 120, NULL, 'default', 1),
(8175, 8675, 'sp_LinxComponenteFormulaDav_Sync', 'Procedure de sincronização entre as tabelas LinxComponenteFormulaDav raw e trusted', 120, NULL, 'default', 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8166, 'LinxProdutosOpticosTipo', 'Integração de dados da tabela LinxProdutosOpticosTipo, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8167, 'LinxProdutosOpticosTipoAro', 'Integração de dados da tabela LinxProdutosOpticosTipoAro, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8168, 'LinxProdutosOpticosFormatoAro', 'Integração de dados da tabela LinxProdutosOpticosFormatoAro, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8169, 'LinxDadosOpticosDav', 'Integração de dados da tabela LinxDadosOpticosDav, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8170, 'LinxOticoPrismaDescricao', 'Integração de dados da tabela LinxOticoPrismaDescricao, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8171, 'LinxOticoReceitas', 'Integração de dados da tabela LinxOticoReceitas, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8172, 'LinxPosicaoOsRamoOptico', 'Integração de dados da tabela LinxPosicaoOsRamoOptico, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8173, 'LinxOrdensServicoPosicaoOsRamoOpticoHistorico', 'Integração de dados da tabela LinxOrdensServicoPosicaoOsRamoOpticoHistorico, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8174, 'LinxOrcamentoComponenteFormula', 'Integração de dados da tabela LinxOrcamentoComponenteFormula, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8175, 'LinxComponenteFormulaDav', 'Integração de dados da tabela LinxComponenteFormulaDav, a cada 2 horas', 120, NULL, 'default', 2);
