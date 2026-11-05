-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Catálogos da API Person: procedures de sincronização (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.2) ******/
--sem filhas: nada é apagado (um item removido na Totvs fica no banco, como as filiais)
if object_id(N'[totvs].TotvsPersonClassifications') is not null and object_id(N'[totvs].[sp_TotvsPersonClassifications_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPersonClassifications', @execute = 1;
if object_id(N'[totvs].TotvsEmailTypes') is not null and object_id(N'[totvs].[sp_TotvsEmailTypes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsEmailTypes', @execute = 1;
if object_id(N'[totvs].TotvsPhoneTypes') is not null and object_id(N'[totvs].[sp_TotvsPhoneTypes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPhoneTypes', @execute = 1;
if object_id(N'[totvs].TotvsContactTypes') is not null and object_id(N'[totvs].[sp_TotvsContactTypes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsContactTypes', @execute = 1;
