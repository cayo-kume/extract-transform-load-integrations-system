namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsRepresentativesCommandHandler
    {
        public string CreateGetRepresentativesLastChangeQuery(string doc_company);
        public string CreateGetRepresentativesByCodesQuery(string doc_company, List<int> codes);
        public string CreateGetReceptionSalesLastMovementDateQuery(string doc_company);
        public string CreateGetReceptionSalesHashesQuery(string doc_company, DateTime startMovementDate, DateTime endMovementDate);
    }
}
