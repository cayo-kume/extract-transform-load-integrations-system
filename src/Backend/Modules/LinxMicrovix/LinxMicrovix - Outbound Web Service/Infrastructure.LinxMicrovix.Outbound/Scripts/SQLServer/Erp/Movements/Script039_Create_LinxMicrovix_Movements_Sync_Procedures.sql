-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Movimento (LinxMicrovixMovementsService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxMovimento') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimento_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimento', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoPrincipal') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoPrincipal_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoPrincipal', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoExtensao') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoExtensao_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoExtensao', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoCamposAdicionais') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoCamposAdicionais_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoCamposAdicionais', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoObservacoes') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoObservacoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoObservacoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoObservacaoCF') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoObservacaoCF_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoObservacaoCF', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoIndicacoes') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoIndicacoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoIndicacoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoSerial') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoSerial_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoSerial', @execute = 1;
if object_id(N'[linxmicrovix].LinxSerialVenda') is not null and object_id(N'[linxmicrovix].[sp_LinxSerialVenda_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxSerialVenda', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoVendaConjugada') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoVendaConjugada_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoVendaConjugada', @execute = 1;
if object_id(N'[linxmicrovix].LinxPerguntaVenda') is not null and object_id(N'[linxmicrovix].[sp_LinxPerguntaVenda_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPerguntaVenda', @execute = 1;
if object_id(N'[linxmicrovix].LinxRespostaVenda') is not null and object_id(N'[linxmicrovix].[sp_LinxRespostaVenda_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxRespostaVenda', @execute = 1;
