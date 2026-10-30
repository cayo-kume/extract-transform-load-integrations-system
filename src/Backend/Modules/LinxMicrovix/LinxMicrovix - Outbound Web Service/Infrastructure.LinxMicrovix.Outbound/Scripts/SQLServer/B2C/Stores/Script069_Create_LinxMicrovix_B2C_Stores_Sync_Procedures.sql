/****** Empresas, vendedores, transportadores e planos do B2C (LinxMicrovixB2CStoresService): procedures de sincronização (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.3) ******/
--respostas planas, sem filhas: nenhuma condição de exclusão. Nas tabelas com coluna timestamp a merge só atualiza quando o
--timestamp que chega é maior ou igual ao gravado (dbo.sp_Create_Merge_Procedure)

exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaEmpresas', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaCNPJsChave', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaVendedores', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaTransportadores', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaFormasPagamento', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPlanos', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaPlanosParcelas', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaTiposCobrancaFrete', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'linxmicrovix', @table_name = 'B2CConsultaTipoEncomenda', @execute = 1;
