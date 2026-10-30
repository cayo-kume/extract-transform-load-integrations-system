/****** Imagens de produto do B2C (LinxMicrovixB2CProductImagesService): procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosImagens', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosImagensURL', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaImagens', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaImagensHD', @execute = 1;
