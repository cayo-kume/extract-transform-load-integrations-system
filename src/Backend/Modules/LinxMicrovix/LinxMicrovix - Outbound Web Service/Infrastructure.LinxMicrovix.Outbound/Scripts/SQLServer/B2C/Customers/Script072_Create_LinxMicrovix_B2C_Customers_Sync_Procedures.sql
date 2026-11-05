-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Clientes do B2C (LinxMicrovixB2CCustomersService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].B2CConsultaClientes') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaClientes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaClientes', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaClientesContatos') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaClientesContatos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaClientesContatos', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaClientesContatosParentesco') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaClientesContatosParentesco_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaClientesContatosParentesco', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaClientesEnderecosEntrega') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaClientesEnderecosEntrega_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaClientesEnderecosEntrega', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaClientesEstadoCivil') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaClientesEstadoCivil_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaClientesEstadoCivil', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaClientesSaldo') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaClientesSaldo_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaClientesSaldo', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaClientesSaldoLinx') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaClientesSaldoLinx_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaClientesSaldoLinx', @execute = 1;
