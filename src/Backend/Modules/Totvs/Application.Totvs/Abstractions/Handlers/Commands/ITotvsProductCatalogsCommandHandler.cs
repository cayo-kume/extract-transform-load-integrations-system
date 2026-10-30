namespace Application.Totvs.Abstractions.Handlers.Commands
{
    public interface ITotvsProductCatalogsCommandHandler
    {
        public string CreateGetClassificationTypesLastChangeQuery(string doc_company);
        public string CreateGetClassificationTypesQuery(string doc_company);
        public string CreateGetProductClassificationsLastChangeQuery(string doc_company);
        public string CreateGetProductClassificationsQuery(string doc_company);
        public string CreateGetCategoriesLastChangeQuery(string doc_company);
        public string CreateGetCategoriesQuery(string doc_company);
        public string CreateGetGridsLastChangeQuery(string doc_company);
        public string CreateGetGridsQuery(string doc_company);
        public string CreateGetMeasurementUnitsLastChangeQuery(string doc_company);
        public string CreateGetMeasurementUnitsQuery(string doc_company);
        public string CreateGetColorsHashesQuery(string doc_company);
        public string CreateGetAdditionalFieldsTypesLastChangeQuery(string doc_company);
        public string CreateGetAdditionalFieldsTypesQuery(string doc_company);
        public string CreateGetProductGrouperConfigurationsLastChangeQuery(string doc_company);
        public string CreateGetProductGrouperConfigurationsQuery(string doc_company);
        public string CreateGetInstructionItemsLastChangeQuery(string doc_company);
        public string CreateGetInstructionItemsQuery(string doc_company);
        public string CreateGetCompositionsLastChangeQuery(string doc_company);
        public string CreateGetCompositionsQuery(string doc_company);
        public string CreateGetPriceTablesLastChangeQuery(string doc_company);
        public string CreateGetPriceTablesQuery(string doc_company);
        public string CreateGetPriceTableScalesLastChangeQuery(string doc_company);
        public string CreateGetPriceTableScalesQuery(string doc_company);
    }
}
