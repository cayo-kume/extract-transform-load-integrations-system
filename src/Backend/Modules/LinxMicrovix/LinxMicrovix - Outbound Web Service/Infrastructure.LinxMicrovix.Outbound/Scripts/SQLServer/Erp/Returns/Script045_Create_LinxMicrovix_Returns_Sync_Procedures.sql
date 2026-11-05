-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Trocas e devoluções (LinxMicrovixReturnsService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxMovimentoTrocas') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoTrocas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoTrocas', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoTrocafone') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoTrocafone_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoTrocafone', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoDevolucoesItens') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoDevolucoesItens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoDevolucoesItens', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoOrigemDevolucoes') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoOrigemDevolucoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoOrigemDevolucoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxMotivoDevolucao') is not null and object_id(N'[linxmicrovix].[sp_LinxMotivoDevolucao_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMotivoDevolucao', @execute = 1;
if object_id(N'[linxmicrovix].LinxTrocaUnificadaResumoVendas') is not null and object_id(N'[linxmicrovix].[sp_LinxTrocaUnificadaResumoVendas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxTrocaUnificadaResumoVendas', @execute = 1;
if object_id(N'[linxmicrovix].LinxTrocaUnificadaResumoVendasItens') is not null and object_id(N'[linxmicrovix].[sp_LinxTrocaUnificadaResumoVendasItens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxTrocaUnificadaResumoVendasItens', @execute = 1;
if object_id(N'[linxmicrovix].LinxTrocaUnificadaResumoBaixa') is not null and object_id(N'[linxmicrovix].[sp_LinxTrocaUnificadaResumoBaixa_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxTrocaUnificadaResumoBaixa', @execute = 1;
if object_id(N'[linxmicrovix].LinxTradeinParceiro') is not null and object_id(N'[linxmicrovix].[sp_LinxTradeinParceiro_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxTradeinParceiro', @execute = 1;
if object_id(N'[linxmicrovix].LinxDevolucaoRemanejoFabrica') is not null and object_id(N'[linxmicrovix].[sp_LinxDevolucaoRemanejoFabrica_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxDevolucaoRemanejoFabrica', @execute = 1;
if object_id(N'[linxmicrovix].LinxDevolucaoRemanejoFabricaItem') is not null and object_id(N'[linxmicrovix].[sp_LinxDevolucaoRemanejoFabricaItem_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxDevolucaoRemanejoFabricaItem', @execute = 1;
if object_id(N'[linxmicrovix].LinxDevolucaoRemanejoFabricaStatus') is not null and object_id(N'[linxmicrovix].[sp_LinxDevolucaoRemanejoFabricaStatus_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxDevolucaoRemanejoFabricaStatus', @execute = 1;
if object_id(N'[linxmicrovix].LinxDevolucaoRemanejoFabricaTipo') is not null and object_id(N'[linxmicrovix].[sp_LinxDevolucaoRemanejoFabricaTipo_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxDevolucaoRemanejoFabricaTipo', @execute = 1;
