-- roda em toda migração: cada procedure só é criada com a tabela já criada e se ainda não existe (docs/MIGRATOR-CSX.md)
--Filhas e netas apagam as linhas que não vieram no lote do ticket (src/Backend/Modules/General/Movidesk/docs/MOVIDESK-MODELAGEM-TICKETS.md 4.8).
--A condição depende do ticket e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge de MovideskTickets rodar antes.
declare @delete_by_ticket nvarchar(max) = 'exists (select 1 from [movidesk].MovideskTickets p where p.doc_company = target.doc_company and p.id = target.ticketId and p.integrated_or_updated_at > target.integrated_or_updated_at)';

if object_id(N'[movidesk].MovideskTickets') is not null and object_id(N'[movidesk].[sp_MovideskTickets_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTickets', @execute = 1;

if object_id(N'[movidesk].MovideskTicketsTags') is not null and object_id(N'[movidesk].[sp_MovideskTicketsTags_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsTags', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsClients') is not null and object_id(N'[movidesk].[sp_MovideskTicketsClients_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsClients', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsRelationships') is not null and object_id(N'[movidesk].[sp_MovideskTicketsRelationships_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsRelationships', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsOwnerHistories') is not null and object_id(N'[movidesk].[sp_MovideskTicketsOwnerHistories_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsOwnerHistories', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsStatusHistories') is not null and object_id(N'[movidesk].[sp_MovideskTicketsStatusHistories_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsStatusHistories', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsCustomFieldValues') is not null and object_id(N'[movidesk].[sp_MovideskTicketsCustomFieldValues_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsCustomFieldValues', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsCustomFieldValuesItems') is not null and object_id(N'[movidesk].[sp_MovideskTicketsCustomFieldValuesItems_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsCustomFieldValuesItems', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsAssets') is not null and object_id(N'[movidesk].[sp_MovideskTicketsAssets_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsAssets', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsActions') is not null and object_id(N'[movidesk].[sp_MovideskTicketsActions_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsActions', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsActionsTags') is not null and object_id(N'[movidesk].[sp_MovideskTicketsActionsTags_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsActionsTags', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsActionsTimeAppointments') is not null and object_id(N'[movidesk].[sp_MovideskTicketsActionsTimeAppointments_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsActionsTimeAppointments', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsActionsExpenses') is not null and object_id(N'[movidesk].[sp_MovideskTicketsActionsExpenses_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsActionsExpenses', @execute = 1, @delete_condition = @delete_by_ticket;
if object_id(N'[movidesk].MovideskTicketsActionsAttachments') is not null and object_id(N'[movidesk].[sp_MovideskTicketsActionsAttachments_Sync]') is null
exec dbo.sp_Create_Merge_Procedure @schema_name = 'movidesk', @table_name = 'MovideskTicketsActionsAttachments', @execute = 1, @delete_condition = @delete_by_ticket;
