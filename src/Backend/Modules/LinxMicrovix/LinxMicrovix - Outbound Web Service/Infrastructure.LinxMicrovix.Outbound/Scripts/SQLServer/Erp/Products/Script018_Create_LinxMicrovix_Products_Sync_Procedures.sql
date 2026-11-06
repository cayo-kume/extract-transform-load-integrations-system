-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Produtos (LinxMicrovixProductsService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxProdutos') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutos', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosAssociados') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosAssociados_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosAssociados', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosCamposAdicionais') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosCamposAdicionais_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosCamposAdicionais', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosCodBar') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosCodBar_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosCodBar', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosFornec') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosFornec_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosFornec', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosInformacoes') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosInformacoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosInformacoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosImagensURL') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosImagensURL_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosImagensURL', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosLotes') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosLotes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosLotes', @execute = 1;
if object_id(N'[linxmicrovix].LinxLotesProdutos') is not null and object_id(N'[linxmicrovix].[sp_LinxLotesProdutos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxLotesProdutos', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosSerial') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosSerial_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosSerial', @execute = 1;
