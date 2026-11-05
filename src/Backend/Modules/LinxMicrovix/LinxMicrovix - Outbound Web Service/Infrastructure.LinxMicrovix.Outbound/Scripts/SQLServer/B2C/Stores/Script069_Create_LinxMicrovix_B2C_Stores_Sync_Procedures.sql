-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Empresas, vendedores, transportadores e planos do B2C (LinxMicrovixB2CStoresService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].B2CConsultaEmpresas') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaEmpresas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaEmpresas', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaCNPJsChave') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaCNPJsChave_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaCNPJsChave', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaVendedores') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaVendedores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaVendedores', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaTransportadores') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaTransportadores_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaTransportadores', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaFormasPagamento') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaFormasPagamento_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaFormasPagamento', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaPlanos') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaPlanos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPlanos', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaPlanosParcelas') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaPlanosParcelas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPlanosParcelas', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaTiposCobrancaFrete') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaTiposCobrancaFrete_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaTiposCobrancaFrete', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaTipoEncomenda') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaTipoEncomenda_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaTipoEncomenda', @execute = 1;
