using Domain.AfterSale.Entities.Parameters;

namespace Domain.AfterSale.Abstractions.Repositorys;

public interface IAfterSaleRepository
{
    /*Select in Database*/
    public Task<IEnumerable<Company?>> GetCompanys();
    public Task<Company?> GetCompany(string cnpj_emp);

    /*Call Proc Merge*/
    //public Task<bool> CallProcMerge();
}
