namespace Application.AfterSale.Abstractions.Handlers.Commands
{
    public interface IAfterSaleCommandHandler
    {
        string CreateGetCompanyQuery(string doc_company);
        string CreateGetCompanysQuery();
    }
}
