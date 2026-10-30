/****** Campos adicionais de produto do B2C (LinxMicrovixB2CProductFieldsService): procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosCamposAdicionaisDetalhes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosCamposAdicionaisNomes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosCamposAdicionaisValores', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaB2CProdutosCamposAdicionais', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaB2CProdutosCamposAdicionaisDados', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaB2CProdutosCamposAdicionaisMedia', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaB2CProdutosCamposAdicionaisMediaDados', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaB2CProdutosCamposAdicionaisOpcoes', @execute = 1;
