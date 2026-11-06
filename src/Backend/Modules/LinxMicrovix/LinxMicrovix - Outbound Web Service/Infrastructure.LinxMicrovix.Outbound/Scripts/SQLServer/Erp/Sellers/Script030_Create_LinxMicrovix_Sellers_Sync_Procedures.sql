-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Vendedores e funcionários (LinxMicrovixSellersService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxVendedores') is not null and object_id(N'[linxmicrovix].[sp_LinxVendedores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxVendedores', @execute = 1;
if object_id(N'[linxmicrovix].LinxVendedoresFerias') is not null and object_id(N'[linxmicrovix].[sp_LinxVendedoresFerias_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxVendedoresFerias', @execute = 1;
if object_id(N'[linxmicrovix].LinxMetasVendedores') is not null and object_id(N'[linxmicrovix].[sp_LinxMetasVendedores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMetasVendedores', @execute = 1;
if object_id(N'[linxmicrovix].LinxMetasVendedoresDia') is not null and object_id(N'[linxmicrovix].[sp_LinxMetasVendedoresDia_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMetasVendedoresDia', @execute = 1;
if object_id(N'[linxmicrovix].LinxComissoesVendedores') is not null and object_id(N'[linxmicrovix].[sp_LinxComissoesVendedores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxComissoesVendedores', @execute = 1;
if object_id(N'[linxmicrovix].LinxListaDaVez') is not null and object_id(N'[linxmicrovix].[sp_LinxListaDaVez_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxListaDaVez', @execute = 1;
if object_id(N'[linxmicrovix].LinxFuncionariosFuncoes') is not null and object_id(N'[linxmicrovix].[sp_LinxFuncionariosFuncoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxFuncionariosFuncoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxRegistroProfissional') is not null and object_id(N'[linxmicrovix].[sp_LinxRegistroProfissional_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxRegistroProfissional', @execute = 1;
