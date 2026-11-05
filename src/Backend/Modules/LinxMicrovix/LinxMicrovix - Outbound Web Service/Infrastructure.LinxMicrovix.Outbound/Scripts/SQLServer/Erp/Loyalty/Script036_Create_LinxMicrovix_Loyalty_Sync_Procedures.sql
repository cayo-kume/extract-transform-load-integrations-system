-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Fidelidade (LinxMicrovixLoyaltyService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxFidelidade') is not null and object_id(N'[linxmicrovix].[sp_LinxFidelidade_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxFidelidade', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoReshop') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoReshop_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoReshop', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoReshopItens') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoReshopItens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoReshopItens', @execute = 1;
