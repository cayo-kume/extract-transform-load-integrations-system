namespace Application.AfterSale.Abstractions.Handlers.Commands
{
    public interface IAfterSaleReversesCommandHandler
    {
        string CreateGetReversesByIdsQuery(string doc_company, List<string> reversesIds);
        string CreateGetCourierAttributesByReverseIdQuery(string doc_company, List<Int64> reversesIds);

        string CreateGetSummarizedReversesByIdsQuery(string doc_company, List<string> reversesIds);

        string CreateGetSummarizedReversesIdsForGetReverseQuery();
        string CreateGetSummarizedReversesIdsForGetCourierAttributesQuery();

        string CreateGetStatusQuery(string doc_company);

        string CreateGetTransportationsQuery(string doc_company);
    }
}
