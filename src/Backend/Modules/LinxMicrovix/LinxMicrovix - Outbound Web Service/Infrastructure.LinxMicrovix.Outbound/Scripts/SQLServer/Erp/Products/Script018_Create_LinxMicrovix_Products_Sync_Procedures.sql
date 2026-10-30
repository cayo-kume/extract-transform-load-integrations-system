/****** Produtos (LinxMicrovixProductsService): procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutos', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosAssociados', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosCamposAdicionais', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosCodBar', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosFornec', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosInformacoes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosImagensURL', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosLotes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxLotesProdutos', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosSerial', @execute = 1;
