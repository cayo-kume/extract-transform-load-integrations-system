namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsPersonCatalogsCommandHandler
    {
        public string CreateGetPersonClassificationsLastChangeQuery(string doc_company);
        public string CreateGetPersonClassificationsQuery(string doc_company);
        public string CreateGetEmailTypesLastChangeQuery(string doc_company);
        public string CreateGetEmailTypesQuery(string doc_company);
        public string CreateGetPhoneTypesLastChangeQuery(string doc_company);
        public string CreateGetPhoneTypesQuery(string doc_company);
        public string CreateGetContactTypesLastChangeQuery(string doc_company);
        public string CreateGetContactTypesQuery(string doc_company);
    }
}
