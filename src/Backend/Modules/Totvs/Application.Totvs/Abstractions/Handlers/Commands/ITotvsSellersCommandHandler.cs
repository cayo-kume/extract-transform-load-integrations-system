namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsSellersCommandHandler
    {
        public string CreateGetSellersLastChangeQuery(string doc_company);
        public string CreateGetSellersByCodesQuery(string doc_company, List<int> codes);
        public string CreateGetOperationalAreasHashesQuery(string doc_company);
    }
}
