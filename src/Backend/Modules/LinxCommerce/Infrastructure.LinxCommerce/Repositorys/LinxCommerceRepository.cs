using Application.LinxCommerce.Abstractions.Handlers.Commands;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.Requests;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.LinxCommerce.Repositorys
{
    public class LinxCommerceRepository : ILinxCommerceRepository
    {
        private readonly ILinxCommerceCommandHandler _linxCommerceCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public LinxCommerceRepository(ILinxCommerceCommandHandler linxCommerceCommandHandler, ISharedRepository coreRepository) =>
            (_linxCommerceCommandHandler, _coreRepository) = (linxCommerceCommandHandler, coreRepository);

        public async Task<IEnumerable<Company?>> GetCompanys()
        {
            string sql = _linxCommerceCommandHandler.CreateGetCompanysQuery();

            return await _coreRepository.GetRecords<Company>(sql);
        }

        /// <summary>
        /// log das chamadas sob demanda: só inserção, então vai direto para o schema linxcommerce (sem untreated/merge)
        /// </summary>
        public bool InsertIntoLinxCommerceRequests(Request request)
        {
            var requestsTable = _coreRepository.CreateSystemDataTable(entity: new Request(), tableName: "LinxCommerceRequests");

            _coreRepository.FillSystemDataTable(requestsTable, [request]);

            return _coreRepository.BulkInsertIntoTableRaw(requestsTable, "linxcommerce");
        }
    }
}
