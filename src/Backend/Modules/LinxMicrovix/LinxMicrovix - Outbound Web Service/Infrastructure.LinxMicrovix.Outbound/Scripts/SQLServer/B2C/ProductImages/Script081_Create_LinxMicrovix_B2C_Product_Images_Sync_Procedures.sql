-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Imagens de produto do B2C (LinxMicrovixB2CProductImagesService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].B2CConsultaProdutosImagens') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosImagens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosImagens', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosImagensURL') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosImagensURL_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosImagensURL', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaImagens') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaImagens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaImagens', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaImagensHD') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaImagensHD_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaImagensHD', @execute = 1;
