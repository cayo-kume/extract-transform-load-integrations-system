using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Parameters;

namespace Infrastructure.Totvs.Repositorys
{
    public class TotvsRepository : ITotvsRepository
    {
        private readonly ITotvsCommandHandler _totvsCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsRepository(ITotvsCommandHandler totvsCommandHandler, ISharedRepository coreRepository) =>
            (_totvsCommandHandler, _coreRepository) = (totvsCommandHandler, coreRepository);

        public async Task<IEnumerable<Company?>> GetCompanys()
        {
            string sql = _totvsCommandHandler.CreateGetCompanysQuery();

            return await _coreRepository.GetRecords<Company>(sql);
        }

        public async Task<IEnumerable<int>> GetBranchCodes(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetBranchCodesQuery(doc_company);

            return (await _coreRepository.GetRecords<int>(sql)).ToList();
        }

        public async Task<IEnumerable<int>> GetBrandBranchCodes(string doc_company, int baseBranchCode)
        {
            string sql = _totvsCommandHandler.CreateGetBrandBranchCodesQuery(doc_company, baseBranchCode);

            return (await _coreRepository.GetRecords<int>(sql)).ToList();
        }

        public async Task<int?> GetBranchCodeByCnpj(string doc_company, string cnpj)
        {
            string sql = _totvsCommandHandler.CreateGetBranchCodeByCnpjQuery(doc_company, cnpj);

            return await _coreRepository.GetRecord<int?>(sql);
        }

        /// <summary>
        /// log das chamadas sob demanda: só inserção, então vai direto para o schema totvs (sem untreated/merge)
        /// </summary>
        public bool InsertIntoTotvsRequests(Domain.Totvs.Entities.Writes.Requests.Request request)
        {
            var requestsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Writes.Requests.Request(), tableName: "TotvsRequests");

            _coreRepository.FillSystemDataTable(requestsTable, [request]);

            return _coreRepository.BulkInsertIntoTableRaw(requestsTable, "totvs");
        }

        public async Task<IEnumerable<Domain.Totvs.Entities.Writes.Requests.RequestCheck?>> GetRecentRequests(string doc_company, string route, string field_key_value, DateTime since)
        {
            string sql = _totvsCommandHandler.CreateGetRecentRequestsQuery(doc_company, route, field_key_value, since);

            return await _coreRepository.GetRecords<Domain.Totvs.Entities.Writes.Requests.RequestCheck>(sql);
        }
    }
}
