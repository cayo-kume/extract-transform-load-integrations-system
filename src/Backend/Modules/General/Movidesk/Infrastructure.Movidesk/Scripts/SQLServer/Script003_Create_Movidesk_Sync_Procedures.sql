--Filhas e netas apagam as linhas que não vieram no lote do ticket (docs/MOVIDESK-MODELAGEM-TICKETS.md 4.8).
--A condição depende do ticket e das filhas do mesmo lote terem o mesmo integrated_or_updated_at, e do merge de MovideskTickets rodar antes.
declare @delete_by_ticket nvarchar(max) = 'exists (select 1 from [general].MovideskTickets p where p.doc_company = target.doc_company and p.id = target.ticketId and p.integrated_or_updated_at > target.integrated_or_updated_at)';

exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTickets', @execute = 1;

exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsTags', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsClients', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsRelationships', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsOwnerHistories', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsStatusHistories', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsCustomFieldValues', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsCustomFieldValuesItems', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsAssets', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsActions', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsActionsTags', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsActionsTimeAppointments', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsActionsExpenses', @execute = 1, @delete_condition = @delete_by_ticket;
exec dbo.sp_Create_Merge_Procedure @schema_name = 'general', @table_name = 'MovideskTicketsActionsAttachments', @execute = 1, @delete_condition = @delete_by_ticket;
