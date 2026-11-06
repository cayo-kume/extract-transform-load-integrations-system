-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Estoque e balanços (LinxMicrovixStocksService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxProdutosDetalhes') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosDetalhes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosDetalhes', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosDetalhesSimplificado') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosDetalhesSimplificado_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosDetalhesSimplificado', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosDetalhesDepositos') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosDetalhesDepositos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosDetalhesDepositos', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosDepositos') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosDepositos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosDepositos', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosInventario') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosInventario_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosInventario', @execute = 1;
if object_id(N'[linxmicrovix].LinxListagemBalanco') is not null and object_id(N'[linxmicrovix].[sp_LinxListagemBalanco_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxListagemBalanco', @execute = 1;
if object_id(N'[linxmicrovix].LinxBalancos') is not null and object_id(N'[linxmicrovix].[sp_LinxBalancos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxBalancos', @execute = 1;
if object_id(N'[linxmicrovix].LinxBalancosArquivos') is not null and object_id(N'[linxmicrovix].[sp_LinxBalancosArquivos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxBalancosArquivos', @execute = 1;
if object_id(N'[linxmicrovix].LinxBalancosSecoes') is not null and object_id(N'[linxmicrovix].[sp_LinxBalancosSecoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxBalancosSecoes', @execute = 1;
