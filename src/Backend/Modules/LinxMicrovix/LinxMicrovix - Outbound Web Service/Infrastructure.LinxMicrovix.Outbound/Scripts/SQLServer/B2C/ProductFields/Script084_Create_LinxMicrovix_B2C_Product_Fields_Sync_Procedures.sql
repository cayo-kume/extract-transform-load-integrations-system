-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Campos adicionais de produto do B2C (LinxMicrovixB2CProductFieldsService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].B2CConsultaProdutosCamposAdicionaisDetalhes') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosCamposAdicionaisDetalhes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosCamposAdicionaisDetalhes', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosCamposAdicionaisNomes') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosCamposAdicionaisNomes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosCamposAdicionaisNomes', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosCamposAdicionaisValores') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosCamposAdicionaisValores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosCamposAdicionaisValores', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaB2CProdutosCamposAdicionais') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaB2CProdutosCamposAdicionais_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaB2CProdutosCamposAdicionais', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaB2CProdutosCamposAdicionaisDados') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaB2CProdutosCamposAdicionaisDados_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaB2CProdutosCamposAdicionaisDados', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaB2CProdutosCamposAdicionaisMedia') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaB2CProdutosCamposAdicionaisMedia_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaB2CProdutosCamposAdicionaisMedia', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaB2CProdutosCamposAdicionaisMediaDados') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaB2CProdutosCamposAdicionaisMediaDados_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaB2CProdutosCamposAdicionaisMediaDados', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaB2CProdutosCamposAdicionaisOpcoes') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaB2CProdutosCamposAdicionaisOpcoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaB2CProdutosCamposAdicionaisOpcoes', @execute = 1;
