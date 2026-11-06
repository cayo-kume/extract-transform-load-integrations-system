-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Remessas (LinxMicrovixConsignmentsService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxMovimentoRemessas') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoRemessas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoRemessas', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoRemessasItens') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoRemessasItens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoRemessasItens', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoRemessasAcertos') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoRemessasAcertos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoRemessasAcertos', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoRemessasAcertosItens') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoRemessasAcertosItens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoRemessasAcertosItens', @execute = 1;
if object_id(N'[linxmicrovix].LinxRemessasIdentificadores') is not null and object_id(N'[linxmicrovix].[sp_LinxRemessasIdentificadores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxRemessasIdentificadores', @execute = 1;
if object_id(N'[linxmicrovix].LinxRemessasOperacoes') is not null and object_id(N'[linxmicrovix].[sp_LinxRemessasOperacoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxRemessasOperacoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxRemessasRetornoTipos') is not null and object_id(N'[linxmicrovix].[sp_LinxRemessasRetornoTipos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxRemessasRetornoTipos', @execute = 1;
