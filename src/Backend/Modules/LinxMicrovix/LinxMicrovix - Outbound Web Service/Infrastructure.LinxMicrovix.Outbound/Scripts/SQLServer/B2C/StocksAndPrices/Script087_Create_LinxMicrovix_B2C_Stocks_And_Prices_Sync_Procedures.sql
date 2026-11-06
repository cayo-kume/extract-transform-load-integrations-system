-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Estoque, custos e preços do B2C (LinxMicrovixB2CStocksAndPricesService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].B2CConsultaProdutosCustos') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosCustos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosCustos', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosDepositos') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosDepositos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosDepositos', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosDetalhes') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosDetalhes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosDetalhes', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosDetalhesDepositos') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosDetalhesDepositos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosDetalhesDepositos', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosPromocao') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosPromocao_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosPromocao', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosTabelas') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosTabelas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosTabelas', @execute = 1;
if object_id(N'[linxmicrovix].B2CConsultaProdutosTabelasPrecos') is not null and object_id(N'[linxmicrovix].[sp_B2CConsultaProdutosTabelasPrecos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaProdutosTabelasPrecos', @execute = 1;
