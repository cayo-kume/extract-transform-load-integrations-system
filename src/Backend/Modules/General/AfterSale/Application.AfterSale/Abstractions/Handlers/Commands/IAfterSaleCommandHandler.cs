namespace Application.AfterSale.Abstractions.Handlers.Commands
{
    public interface IAfterSaleCommandHandler
    {
        string CreateGetCompanyQuery(string cnpj_emp);
        string CreateGetCompanysQuery();

        string CreateGetReversesByIdsQuery(string doc_company, List<string> reversesIds);
        string CreateGetCourierAttributesByReverseIdQuery(string doc_company, List<Int64> reversesIds);

        string CreateGetSummarizedReversesByIdsQuery(string doc_company, List<string> reversesIds);
        string CreateGetSummarizedRefundsByIdsQuery(string doc_company, List<string> refundsIds);
        string CreateGetStatusRefundsQuery(string doc_company);
        string CreateGetActionsRefundsQuery(string doc_company);
        string CreateGetBanksRefundsQuery(string doc_company);
        string CreateGetTypesRefundsQuery(string doc_company);

        string CreateGetSummarizedReversesIdsForGetReverseQuery();
        string CreateGetSummarizedReversesIdsForGetCourierAttributesQuery();

        string CreateGetStatusQuery(string doc_company);

        string CreateGetTransportationsQuery(string doc_company);
    }
}
