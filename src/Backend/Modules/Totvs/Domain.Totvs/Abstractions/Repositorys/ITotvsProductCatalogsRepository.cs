using Domain.Totvs.Entities.Reads.Products.Catalogs;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsProductCatalogsRepository
    {
        public Task<DateTime?> GetClassificationTypesLastChange(string doc_company);
        public Task<IEnumerable<ClassificationType?>> GetClassificationTypes(string doc_company);

        public Task<bool> BulkInsertIntoTotvsClassificationTypes(List<ClassificationType> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetProductClassificationsLastChange(string doc_company);
        public Task<IEnumerable<ProductClassification?>> GetProductClassifications(string doc_company);

        public Task<bool> BulkInsertIntoTotvsProductClassifications(List<ProductClassification> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetCategoriesLastChange(string doc_company);
        public Task<IEnumerable<Category?>> GetCategories(string doc_company);

        public Task<bool> BulkInsertIntoTotvsCategories(List<Category> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetGridsLastChange(string doc_company);
        public Task<IEnumerable<Grid?>> GetGrids(string doc_company);

        public Task<bool> BulkInsertIntoTotvsGrids(List<Grid> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetMeasurementUnitsLastChange(string doc_company);
        public Task<IEnumerable<MeasurementUnit?>> GetMeasurementUnits(string doc_company);

        public Task<bool> BulkInsertIntoTotvsMeasurementUnits(List<MeasurementUnit> data, Guid? parentExecutionGUID);

        public Task<IEnumerable<Color?>> GetColorsHashes(string doc_company);

        public Task<bool> BulkInsertIntoTotvsColors(List<Color> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetAdditionalFieldsTypesLastChange(string doc_company);
        public Task<IEnumerable<AdditionalFieldType?>> GetAdditionalFieldsTypes(string doc_company);

        public Task<bool> BulkInsertIntoTotvsAdditionalFieldsTypes(List<AdditionalFieldType> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetProductGrouperConfigurationsLastChange(string doc_company);
        public Task<IEnumerable<ProductGrouperConfiguration?>> GetProductGrouperConfigurations(string doc_company);

        public Task<bool> BulkInsertIntoTotvsProductGrouperConfigurations(List<ProductGrouperConfiguration> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetInstructionItemsLastChange(string doc_company);
        public Task<IEnumerable<InstructionItem?>> GetInstructionItems(string doc_company);

        public Task<bool> BulkInsertIntoTotvsInstructionItems(List<InstructionItem> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetCompositionsLastChange(string doc_company);
        public Task<IEnumerable<Composition?>> GetCompositions(string doc_company);

        public Task<bool> BulkInsertIntoTotvsCompositions(List<Composition> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetPriceTablesLastChange(string doc_company);
        public Task<IEnumerable<PriceTable?>> GetPriceTables(string doc_company);

        public Task<bool> BulkInsertIntoTotvsPriceTables(List<PriceTable> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetPriceTableScalesLastChange(string doc_company);
        public Task<IEnumerable<PriceTableScale?>> GetPriceTableScales(string doc_company);

        public Task<bool> BulkInsertIntoTotvsPriceTableScales(List<PriceTableScale> data, Guid? parentExecutionGUID);
    }
}
