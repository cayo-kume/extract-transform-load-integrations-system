-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Atributos de produto do B2C (LinxMicrovixB2CProductAttributesService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].B2CConsultaClassificacao') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaClassificacao_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaClassificacao', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaColecoes') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaColecoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaColecoes', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaEspessuras') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaEspessuras_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaEspessuras', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaGrade1') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaGrade1_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaGrade1', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaGrade2') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaGrade2_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaGrade2', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaLinhas') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaLinhas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaLinhas', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaMarcas') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaMarcas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaMarcas', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaSetores') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaSetores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaSetores', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaUnidade') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaUnidade_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaUnidade', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaFornecedores') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaFornecedores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaFornecedores', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaFlags') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaFlags_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaFlags', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaTags') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaTags_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaTags', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaPalavrasChavePesquisa') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaPalavrasChavePesquisa_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPalavrasChavePesquisa', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaLegendasCadastrosAuxiliares') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaLegendasCadastrosAuxiliares_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaLegendasCadastrosAuxiliares', @execute = 1;
