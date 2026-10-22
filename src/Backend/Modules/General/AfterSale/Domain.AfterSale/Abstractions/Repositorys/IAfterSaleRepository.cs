using Domain.AfterSale.Entities.Parameters;

namespace Domain.AfterSale.Abstractions.Repositorys;

public interface IAfterSaleRepository
{
    /*Select in Database*/
    public Task<IEnumerable<Company?>> GetCompanys();
    public Task<Company?> GetCompany(string doc_company);

    /*Insert in Database*/
    public bool InsertIntoAfterSaleRequests(Entities.Writes.Requests.Request request);

    /*Call Proc Merge*/
    //public Task<bool> CallProcMerge();
}
