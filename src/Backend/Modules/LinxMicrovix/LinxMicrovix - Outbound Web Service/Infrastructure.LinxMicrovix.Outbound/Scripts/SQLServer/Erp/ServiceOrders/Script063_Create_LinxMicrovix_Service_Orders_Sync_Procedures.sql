-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
/****** Ordens de serviço (LinxMicrovixServiceOrdersService): procedures de sincronização (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

if object_id(N'[linxmicrovix].LinxOrdensServico') is not null and object_id(N'[linxmicrovix].[sp_LinxOrdensServico_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOrdensServico', @execute = 1;
if object_id(N'[linxmicrovix].LinxOrdensServicoProdutos') is not null and object_id(N'[linxmicrovix].[sp_LinxOrdensServicoProdutos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOrdensServicoProdutos', @execute = 1;
if object_id(N'[linxmicrovix].LinxServicos') is not null and object_id(N'[linxmicrovix].[sp_LinxServicos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxServicos', @execute = 1;
if object_id(N'[linxmicrovix].LinxServicosDetalhes') is not null and object_id(N'[linxmicrovix].[sp_LinxServicosDetalhes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxServicosDetalhes', @execute = 1;
if object_id(N'[linxmicrovix].LinxServicoClientes') is not null and object_id(N'[linxmicrovix].[sp_LinxServicoClientes_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxServicoClientes', @execute = 1;
if object_id(N'[linxmicrovix].LinxServicoAgendaComandas') is not null and object_id(N'[linxmicrovix].[sp_LinxServicoAgendaComandas_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxServicoAgendaComandas', @execute = 1;
if object_id(N'[linxmicrovix].LinxServicoAgendaEventos') is not null and object_id(N'[linxmicrovix].[sp_LinxServicoAgendaEventos_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxServicoAgendaEventos', @execute = 1;
if object_id(N'[linxmicrovix].LinxOrdemServicoExternaDav') is not null and object_id(N'[linxmicrovix].[sp_LinxOrdemServicoExternaDav_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOrdemServicoExternaDav', @execute = 1;
if object_id(N'[linxmicrovix].LinxMovimentoOrdemServicoExterna') is not null and object_id(N'[linxmicrovix].[sp_LinxMovimentoOrdemServicoExterna_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoOrdemServicoExterna', @execute = 1;
if object_id(N'[linxmicrovix].LinxValeOrdemServicoExterna') is not null and object_id(N'[linxmicrovix].[sp_LinxValeOrdemServicoExterna_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxValeOrdemServicoExterna', @execute = 1;
if object_id(N'[linxmicrovix].LinxPagamentoParcialDav') is not null and object_id(N'[linxmicrovix].[sp_LinxPagamentoParcialDav_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPagamentoParcialDav', @execute = 1;
