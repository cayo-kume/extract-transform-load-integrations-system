using Application.Movidesk.Abstractions.Handlers.Commands;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Entities.Parameters;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskRepository : IMovideskRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public MovideskRepository(IMovideskCommandHandler movideskCommandHandler, ISharedRepository coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);

        public async Task<IEnumerable<Company?>> GetCompanys()
        {
            string sql = _movideskCommandHandler.CreateGetCompanysQuery();

            return await _coreRepository.GetRecords<Company>(sql);
        }

        public async Task<Company?> GetCompany(string doc_company)
        {
            string sql = _movideskCommandHandler.CreateGetCompanyQuery(doc_company);

            return await _coreRepository.GetRecord<Company>(sql);
        }

        /// <summary>
        /// log table of the on-demand calls: insert-only, so it goes straight to the general schema (no untreated/merge)
        /// </summary>
        public bool InsertIntoMovideskRequests(Domain.Movidesk.Entities.Writes.Requests.Request request)
        {
            var requestsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Entities.Writes.Requests.Request(), tableName: "MovideskRequests");

            _coreRepository.FillSystemDataTable(requestsTable, [request]);

            return _coreRepository.BulkInsertIntoTableRaw(requestsTable, "general");
        }
    }
}
