namespace Application.AfterSale.Abstractions.Handlers.Commands
{
    public interface IAfterSaleRefundsCommandHandler
    {
        string CreateGetSummarizedRefundsByIdsQuery(string doc_company, List<string> refundsIds);
        string CreateGetSummarizedRefundsLastUpdateQuery(string doc_company);
        string CreateGetSummarizedRefundsIdsForGetRefundQuery();
        string CreateGetRefundsByIdsQuery(string doc_company, List<string> refundsIds);
        string CreateGetStatusRefundsQuery(string doc_company);
        string CreateGetActionsRefundsQuery(string doc_company);
        string CreateGetBanksRefundsQuery(string doc_company);
        string CreateGetTypesRefundsQuery(string doc_company);
    }
}
