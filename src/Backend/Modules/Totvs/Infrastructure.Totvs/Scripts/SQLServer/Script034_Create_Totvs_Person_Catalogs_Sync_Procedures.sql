/****** Catálogos da API Person: procedures de sincronização (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.2) ******/
--sem filhas: nada é apagado (um item removido na Totvs fica no banco, como as filiais)
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPersonClassifications', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsEmailTypes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsPhoneTypes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'totvs', @table_name = 'TotvsContactTypes', @execute = 1;
