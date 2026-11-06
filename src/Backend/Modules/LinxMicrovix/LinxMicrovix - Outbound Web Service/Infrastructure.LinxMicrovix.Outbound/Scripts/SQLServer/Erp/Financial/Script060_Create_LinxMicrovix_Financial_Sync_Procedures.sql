-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Financeiro (LinxMicrovixFinancialService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxFaturas') is not null and object_id(N'[linxmicrovix].[sp_LinxFaturas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxFaturas', @execute = 1;
if object_id(N'[linxmicrovix].LinxFaturasOrigem') is not null and object_id(N'[linxmicrovix].[sp_LinxFaturasOrigem_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxFaturasOrigem', @execute = 1;
if object_id(N'[linxmicrovix].LinxPortadores') is not null and object_id(N'[linxmicrovix].[sp_LinxPortadores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPortadores', @execute = 1;
if object_id(N'[linxmicrovix].LinxAntecipacoesFinanceiras') is not null and object_id(N'[linxmicrovix].[sp_LinxAntecipacoesFinanceiras_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxAntecipacoesFinanceiras', @execute = 1;
if object_id(N'[linxmicrovix].LinxAntecipacoesFinanceirasPlanos') is not null and object_id(N'[linxmicrovix].[sp_LinxAntecipacoesFinanceirasPlanos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxAntecipacoesFinanceirasPlanos', @execute = 1;
if object_id(N'[linxmicrovix].LinxDesfazimentosAntecipacoes') is not null and object_id(N'[linxmicrovix].[sp_LinxDesfazimentosAntecipacoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxDesfazimentosAntecipacoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxDesfazimentosAntecipacoesDocumento') is not null and object_id(N'[linxmicrovix].[sp_LinxDesfazimentosAntecipacoesDocumento_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxDesfazimentosAntecipacoesDocumento', @execute = 1;
if object_id(N'[linxmicrovix].LinxDesfazimentosAntecipacoesItens') is not null and object_id(N'[linxmicrovix].[sp_LinxDesfazimentosAntecipacoesItens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxDesfazimentosAntecipacoesItens', @execute = 1;
if object_id(N'[linxmicrovix].LinxCategoriasFinanceiras') is not null and object_id(N'[linxmicrovix].[sp_LinxCategoriasFinanceiras_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCategoriasFinanceiras', @execute = 1;
if object_id(N'[linxmicrovix].LinxCentroCusto') is not null and object_id(N'[linxmicrovix].[sp_LinxCentroCusto_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCentroCusto', @execute = 1;
if object_id(N'[linxmicrovix].LinxPlanoContas') is not null and object_id(N'[linxmicrovix].[sp_LinxPlanoContas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPlanoContas', @execute = 1;
if object_id(N'[linxmicrovix].LinxLancContabil') is not null and object_id(N'[linxmicrovix].[sp_LinxLancContabil_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxLancContabil', @execute = 1;
if object_id(N'[linxmicrovix].LinxHistoricosContabeis') is not null and object_id(N'[linxmicrovix].[sp_LinxHistoricosContabeis_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxHistoricosContabeis', @execute = 1;
if object_id(N'[linxmicrovix].LinxExtratoBancarioLancamentos') is not null and object_id(N'[linxmicrovix].[sp_LinxExtratoBancarioLancamentos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxExtratoBancarioLancamentos', @execute = 1;
if object_id(N'[linxmicrovix].LinxExtratoBancarioConciliacoes') is not null and object_id(N'[linxmicrovix].[sp_LinxExtratoBancarioConciliacoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxExtratoBancarioConciliacoes', @execute = 1;
