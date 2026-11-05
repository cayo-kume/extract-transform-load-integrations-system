-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Produtos do B2C (LinxMicrovixB2CProductsService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].B2CConsultaProdutos') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutos', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosAssociados') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosAssociados_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosAssociados', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosCampanhas') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosCampanhas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosCampanhas', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosCodebar') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosCodebar_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosCodebar', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosDimensoes') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosDimensoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosDimensoes', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosInformacoes') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosInformacoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosInformacoes', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosStatus') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosStatus_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosStatus', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosFlags') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosFlags_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosFlags', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosTags') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosTags_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosTags', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosPalavrasChavePesquisa') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosPalavrasChavePesquisa_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosPalavrasChavePesquisa', @execute = 1;
