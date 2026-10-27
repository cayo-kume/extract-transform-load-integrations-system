using Application.LinxMicrovix.Outbound.Abstractions.Handlers.Commands;
using Domain.LinxMicrovix.Outbound.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.Enums;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.LinxMicrovix.Outbound.Repositorys
{
    public class LinxMicrovixRepository : ILinxMicrovixRepository
    {
        private readonly ILinxMicrovixCommandHandler _linxMicrovixCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public LinxMicrovixRepository(ILinxMicrovixCommandHandler linxMicrovixCommandHandler, ISharedRepository coreRepository) =>
            (_linxMicrovixCommandHandler, _coreRepository) = (linxMicrovixCommandHandler, coreRepository);

        public async Task<IEnumerable<Company?>> GetCompanys()
        {
            string sql = _linxMicrovixCommandHandler.CreateGetCompanysQuery();

            return await _coreRepository.GetRecords<Company>(sql);
        }

        public async Task<IEnumerable<string>> GetStores(string doc_company, EnumApi api)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetStoresQuery(doc_company, api);

            return (await _coreRepository.GetRecords<string>(sql))
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .ToList();
        }

        public async Task<long> GetLastTimestamp(string doc_company, string tableName, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetLastTimestampQuery(doc_company, tableName, cnpj_emp);

            return await _coreRepository.GetRecord<long>(sql);
        }

        public async Task<long> GetLastTimestamp(string doc_company, string tableName, string? cnpj_emp, string filterColumn, string filterValue)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetLastTimestampQuery(doc_company, tableName, cnpj_emp, filterColumn, filterValue);

            return await _coreRepository.GetRecord<long>(sql);
        }

        public async Task<IEnumerable<string>> GetSourceValues(string doc_company, string sourceTable, string sourceColumn, string? cnpj_emp, string? changedSinceTable)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetSourceValuesQuery(doc_company, sourceTable, sourceColumn, cnpj_emp, changedSinceTable);

            return (await _coreRepository.GetRecords<string>(sql))
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .ToList();
        }

        /// <summary>
        /// log das chamadas sob demanda: só inserção, então vai direto para o schema linxmicrovix (sem untreated/merge)
        /// </summary>
        public bool InsertIntoLinxMicrovixRequests(Domain.LinxMicrovix.Outbound.Entities.Writes.Requests.Request request)
        {
            var requestsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.LinxMicrovix.Outbound.Entities.Writes.Requests.Request(), tableName: "LinxMicrovixRequests");

            _coreRepository.FillSystemDataTable(requestsTable, [request]);

            return _coreRepository.BulkInsertIntoTableRaw(requestsTable, "linxmicrovix");
        }
    }
}
