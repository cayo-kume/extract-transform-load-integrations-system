-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
/****** Preços, promoções e cupons (LinxMicrovixPricesService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxProdutosTabelas') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosTabelas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosTabelas', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosTabelasPrecos') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosTabelasPrecos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosTabelasPrecos', @execute = 1;
if object_id(N'[linxmicrovix].LinxProdutosPromocoes') is not null and object_id(N'[linxmicrovix].[sp_LinxProdutosPromocoes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxProdutosPromocoes', @execute = 1;
if object_id(N'[linxmicrovix].LinxAcoesPromocionais') is not null and object_id(N'[linxmicrovix].[sp_LinxAcoesPromocionais_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxAcoesPromocionais', @execute = 1;
if object_id(N'[linxmicrovix].LinxAcoesPromocionaisCombinacaoProdutosItens') is not null and object_id(N'[linxmicrovix].[sp_LinxAcoesPromocionaisCombinacaoProdutosItens_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxAcoesPromocionaisCombinacaoProdutosItens', @execute = 1;
if object_id(N'[linxmicrovix].LinxAcoesPromocionaisProdutosCortesia') is not null and object_id(N'[linxmicrovix].[sp_LinxAcoesPromocionaisProdutosCortesia_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxAcoesPromocionaisProdutosCortesia', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoAcoesPromocionais') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoAcoesPromocionais_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoAcoesPromocionais', @execute = 1;
if object_id(N'[linxmicrovix].LinxCupomDesconto') is not null and object_id(N'[linxmicrovix].[sp_LinxCupomDesconto_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCupomDesconto', @execute = 1;
if object_id(N'[linxmicrovix].LinxCupomDescontoVendas') is not null and object_id(N'[linxmicrovix].[sp_LinxCupomDescontoVendas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxCupomDescontoVendas', @execute = 1;
if object_id(N'[linxmicrovix].LinxMotivoDesconto') is not null and object_id(N'[linxmicrovix].[sp_LinxMotivoDesconto_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMotivoDesconto', @execute = 1;
