-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (.docs/MIGRATOR-CSX.md)
--Os serviços nunca são apagados por este fluxo (tickets antigos apontam para eles); as categorias removidas de um
--serviço que veio no lote são apagadas. A condição depende do serviço e das categorias do mesmo lote terem o mesmo
--integrated_or_updated_at, e do merge de MovideskServices rodar antes (src/Backend/Modules/General/Movidesk/.docs/IMPLEMENTACAO-MOVIDESK-SERVICES.md 3.3).
declare @delete_by_service nvarchar(max) = 'exists (select 1 from [movidesk].MovideskServices p where p.doc_company = target.doc_company and p.id = target.serviceId and p.integrated_or_updated_at > target.integrated_or_updated_at)';

if object_id(N'[movidesk].MovideskServices') is not null and object_id(N'[movidesk].[sp_MovideskServices_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskServices', @execute = 1;
if object_id(N'[movidesk].MovideskServicesCategories') is not null and object_id(N'[movidesk].[sp_MovideskServicesCategories_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskServicesCategories', @execute = 1, @delete_condition = @delete_by_service;
