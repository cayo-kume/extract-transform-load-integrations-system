/****** Ordens de serviço (LinxMicrovixServiceOrdersService): procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOrdensServico', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOrdensServicoProdutos', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxServicos', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxServicosDetalhes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxServicoClientes', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxServicoAgendaComandas', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxServicoAgendaEventos', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxOrdemServicoExternaDav', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxMovimentoOrdemServicoExterna', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxValeOrdemServicoExterna', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'LinxPagamentoParcialDav', @execute = 1;
