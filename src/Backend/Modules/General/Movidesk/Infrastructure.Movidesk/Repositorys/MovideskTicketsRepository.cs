using Application.Movidesk.Abstractions.Handlers.Commands;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Entities.Reads.Tickets;
using Domain.Shared.Abstractions.Repositorys;
using System.Collections;
using System.Data;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskTicketsRepository : IMovideskTicketsRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public MovideskTicketsRepository(IMovideskCommandHandler movideskCommandHandler, ISharedRepository coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);

        public async Task<DateTime?> GetTicketsLastUpdate(string doc_company)
        {
            string sql = _movideskCommandHandler.CreateGetTicketsLastUpdateQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Ticket?>> GetTicketsByIds(string doc_company, List<long> ids)
        {
            if (ids.Count == 0)
                return [];

            string sql = _movideskCommandHandler.CreateGetTicketsByIdsQuery(doc_company, ids);

            return await _coreRepository.GetRecords<Ticket>(sql);
        }

        public async Task<IEnumerable<Domain.Movidesk.Entities.Reads.Tickets.Action?>> GetActionsHtmlDescriptions(string doc_company, List<long> ticketIds)
        {
            var result = new List<Domain.Movidesk.Entities.Reads.Tickets.Action?>();

            // lotes de 1000 ids, para o IN não crescer sem limite num GetTickets com muitos tickets
            foreach (var chunk in ticketIds.Distinct().Chunk(1000))
            {
                string sql = _movideskCommandHandler.CreateGetActionsHtmlDescriptionsQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Domain.Movidesk.Entities.Reads.Tickets.Action>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoMovideskTickets(List<Ticket> data, Guid? parentExecutionGUID)
        {
            var actions = data.SelectMany(x => x.actions).ToList();
            var customFieldValues = data.SelectMany(x => x.customFieldValues).ToList();

            // ordem = ordem dos merges: pai, nível do meio, depois filhas e netas (FKs e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Ticket(), tableName: "MovideskTickets"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Entities.Reads.Tickets.Action(), tableName: "MovideskTicketsActions"), actions),
                (_coreRepository.CreateSystemDataTable(entity: new CustomFieldValue(), tableName: "MovideskTicketsCustomFieldValues"), customFieldValues),
                (_coreRepository.CreateSystemDataTable(entity: new TicketTag(), tableName: "MovideskTicketsTags"), data.SelectMany(x => x.tags).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Client(), tableName: "MovideskTicketsClients"), data.SelectMany(x => x.clients).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Relationship(), tableName: "MovideskTicketsRelationships"), data.SelectMany(x => x.relationships).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OwnerHistory(), tableName: "MovideskTicketsOwnerHistories"), data.SelectMany(x => x.ownerHistories).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new StatusHistory(), tableName: "MovideskTicketsStatusHistories"), data.SelectMany(x => x.statusHistories).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Asset(), tableName: "MovideskTicketsAssets"), data.SelectMany(x => x.assets).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new CustomFieldValueItem(), tableName: "MovideskTicketsCustomFieldValuesItems"), customFieldValues.SelectMany(x => x.items).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ActionTag(), tableName: "MovideskTicketsActionsTags"), actions.SelectMany(x => x.tags).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new TimeAppointment(), tableName: "MovideskTicketsActionsTimeAppointments"), actions.SelectMany(x => x.timeAppointments).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Expense(), tableName: "MovideskTicketsActionsExpenses"), actions.SelectMany(x => x.expenses).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Attachment(), tableName: "MovideskTicketsActionsAttachments"), actions.SelectMany(x => x.attachments).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge roda mesmo para tabelas sem linhas no lote: é ele que apaga as filhas removidas
            // (ex.: o ticket ficou sem tags -> nenhuma linha em untreated.MovideskTicketsTags, mas as tags antigas saem)
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("movidesk", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
