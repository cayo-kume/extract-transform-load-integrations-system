--Os serviços nunca são apagados por este fluxo (tickets antigos apontam para eles); as categorias removidas de um
--serviço que veio no lote são apagadas. A condição depende do serviço e das categorias do mesmo lote terem o mesmo
--integrated_or_updated_at, e do merge de MovideskServices rodar antes (docs/IMPLEMENTACAO-MOVIDESK-SERVICES.md 3.3).
declare @delete_by_service nvarchar(max) = 'exists (select 1 from [movidesk].MovideskServices p where p.doc_company = target.doc_company and p.id = target.serviceId and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskServices', @execute = 1;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskServicesCategories', @execute = 1, @delete_condition = @delete_by_service;
