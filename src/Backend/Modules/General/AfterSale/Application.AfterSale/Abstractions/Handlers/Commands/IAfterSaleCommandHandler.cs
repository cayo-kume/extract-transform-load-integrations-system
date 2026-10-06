namespace Application.AfterSale.Abstractions.Handlers.Commands
{
    public interface IAfterSaleCommandHandler
    {
        string CreateGetCompanyQuery(string cnpj_emp);
        string CreateGetCompanysQuery();

        string CreateGetReversesByIdsQuery(List<string> reversesIds);
        string CreateGetCourierAttributesByReverseIdQuery(List<Int64> reversesIds);

        string CreateGetSummarizedReversesByIdsQuery(List<string> reversesIds);

        string CreateGetSummarizedReversesIdsForGetReverseQuery();
        string CreateGetSummarizedReversesIdsForGetCourierAttributesQuery();
        
        string CreateGetStatusQuery(/*string doc_company*/);

        string CreateGetTransportationsQuery();
    }
}
