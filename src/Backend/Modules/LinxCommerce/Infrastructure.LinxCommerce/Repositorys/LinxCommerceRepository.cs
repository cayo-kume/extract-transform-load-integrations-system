using Application.LinxCommerce.Abstractions.Handlers.Commands;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.Requests;
using Application.Shared.Abstractions.Services;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.LinxCommerce.Repositorys
{
    public class LinxCommerceRepository : ILinxCommerceRepository
    {
        private readonly ILinxCommerceCommandHandler _linxCommerceCommandHandler;
        private readonly ISharedRepository _coreRepository;
        private readonly ICompanyContext _companyContext;

        public LinxCommerceRepository(ILinxCommerceCommandHandler linxCommerceCommandHandler, ISharedRepository coreRepository, ICompanyContext companyContext) =>
            (_linxCommerceCommandHandler, _coreRepository, _companyContext) = (linxCommerceCommandHandler, coreRepository, companyContext);

        public async Task<IEnumerable<Company?>> GetCompanys()
        {
            string sql = _linxCommerceCommandHandler.CreateGetCompanysQuery(
                _companyContext.Company ?? throw new InvalidOperationException("Parâmetros do módulo sem a empresa da execução: informe ICompanyContext.SetCompany."));

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
