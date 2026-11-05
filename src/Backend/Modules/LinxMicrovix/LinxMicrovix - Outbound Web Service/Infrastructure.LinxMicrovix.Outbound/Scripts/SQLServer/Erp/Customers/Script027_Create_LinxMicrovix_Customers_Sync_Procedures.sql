-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Clientes e fornecedores (LinxMicrovixCustomersService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxClientesFornec') is not null and object_id(N'[linxmicrovix].[sp_LinxClientesFornec_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClientesFornec', @execute = 1;
if object_id(N'[linxmicrovix].LinxClientesFornecCamposAdicionais') is not null and object_id(N'[linxmicrovix].[sp_LinxClientesFornecCamposAdicionais_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClientesFornecCamposAdicionais', @execute = 1;
if object_id(N'[linxmicrovix].LinxClientesFornecClasses') is not null and object_id(N'[linxmicrovix].[sp_LinxClientesFornecClasses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClientesFornecClasses', @execute = 1;
if object_id(N'[linxmicrovix].LinxClientesFornecContatos') is not null and object_id(N'[linxmicrovix].[sp_LinxClientesFornecContatos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClientesFornecContatos', @execute = 1;
if object_id(N'[linxmicrovix].LinxClientesFornecContatosParentesco') is not null and object_id(N'[linxmicrovix].[sp_LinxClientesFornecContatosParentesco_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClientesFornecContatosParentesco', @execute = 1;
if object_id(N'[linxmicrovix].LinxClientesFornecCreditoAvulso') is not null and object_id(N'[linxmicrovix].[sp_LinxClientesFornecCreditoAvulso_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClientesFornecCreditoAvulso', @execute = 1;
if object_id(N'[linxmicrovix].LinxClientesFornecVendedores') is not null and object_id(N'[linxmicrovix].[sp_LinxClientesFornecVendedores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClientesFornecVendedores', @execute = 1;
if object_id(N'[linxmicrovix].LinxClientesEnderecosEntrega') is not null and object_id(N'[linxmicrovix].[sp_LinxClientesEnderecosEntrega_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClientesEnderecosEntrega', @execute = 1;
if object_id(N'[linxmicrovix].LinxClientesRede') is not null and object_id(N'[linxmicrovix].[sp_LinxClientesRede_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxClientesRede', @execute = 1;
if object_id(N'[linxmicrovix].LinxRamosAtividade') is not null and object_id(N'[linxmicrovix].[sp_LinxRamosAtividade_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxRamosAtividade', @execute = 1;
