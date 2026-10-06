using Application.AfterSale.Abstractions.Handlers.Commands;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.AfterSale.Entities.Parameters;
using Domain.Shared.Abstractions.Repositorys;
using System.Data;

namespace Infrastructure.AfterSale.Repositorys;

public class AfterSaleRepository : IAfterSaleRepository
{
    private readonly IAfterSaleCommandHandler _afterSaleCommandHandler;
    private readonly ISharedRepository _coreRepository;

    public AfterSaleRepository(IAfterSaleCommandHandler afterSaleCommandHandler, ISharedRepository coreRepository) =>
            (_afterSaleCommandHandler, _coreRepository) = (afterSaleCommandHandler, coreRepository);

    public async Task<IEnumerable<Company?>> GetCompanys()
    {
        string sql = _afterSaleCommandHandler.CreateGetCompanysQuery();

        return await _coreRepository.GetRecords<Company>(sql);
    }

    public async Task<Company?> GetCompany(string cnpj_emp)
    {
        string sql = _afterSaleCommandHandler.CreateGetCompanyQuery(cnpj_emp);

        return await _coreRepository.GetRecord<Company>(sql);
    }

}