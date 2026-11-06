-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Pagamentos e caixa (LinxMicrovixPaymentsService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxMovimentoCartoes') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoCartoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoCartoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoPlanos') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoPlanos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoPlanos', @execute = 1;
if object_id(N'[linxmicrovix].LinxPlanos') is not null and object_id(N'[linxmicrovix].[sp_LinxPlanos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPlanos', @execute = 1;
if object_id(N'[linxmicrovix].LinxPlanosParcelas') is not null and object_id(N'[linxmicrovix].[sp_LinxPlanosParcelas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPlanosParcelas', @execute = 1;
if object_id(N'[linxmicrovix].LinxPlanosBandeiras') is not null and object_id(N'[linxmicrovix].[sp_LinxPlanosBandeiras_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPlanosBandeiras', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoGiftCard') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoGiftCard_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoGiftCard', @execute = 1;
if object_id(N'[linxmicrovix].LinxValesComprasEnviadosAPI') is not null and object_id(N'[linxmicrovix].[sp_LinxValesComprasEnviadosAPI_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxValesComprasEnviadosAPI', @execute = 1;
if object_id(N'[linxmicrovix].LinxSangriaSuprimentos') is not null and object_id(N'[linxmicrovix].[sp_LinxSangriaSuprimentos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxSangriaSuprimentos', @execute = 1;
if object_id(N'[linxmicrovix].LinxFechamentoCaixa') is not null and object_id(N'[linxmicrovix].[sp_LinxFechamentoCaixa_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxFechamentoCaixa', @execute = 1;
