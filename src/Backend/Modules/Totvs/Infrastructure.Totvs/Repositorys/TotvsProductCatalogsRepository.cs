using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.Products.Catalogs;
using System.Collections;
using System.Data;

namespace Infrastructure.Totvs.Repositorys
{
    /// <summary>
    /// catálogos da API Product (fase PR-1, docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.1)
    /// </summary>
    public class TotvsProductCatalogsRepository : ITotvsProductCatalogsRepository
    {
        private readonly ITotvsProductCatalogsCommandHandler _totvsProductCatalogsCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsProductCatalogsRepository(ITotvsProductCatalogsCommandHandler totvsProductCatalogsCommandHandler, ISharedRepository coreRepository) =>
            (_totvsProductCatalogsCommandHandler, _coreRepository) = (totvsProductCatalogsCommandHandler, coreRepository);

        public async Task<DateTime?> GetClassificationTypesLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetClassificationTypesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<ClassificationType?>> GetClassificationTypes(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetClassificationTypesQuery(doc_company);

            return await _coreRepository.GetRecords<ClassificationType>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsClassificationTypes(List<ClassificationType> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new ClassificationType(), tableName: "TotvsClassificationTypes");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetProductClassificationsLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetProductClassificationsLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<ProductClassification?>> GetProductClassifications(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetProductClassificationsQuery(doc_company);

            return await _coreRepository.GetRecords<ProductClassification>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsProductClassifications(List<ProductClassification> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new ProductClassification(), tableName: "TotvsProductClassifications");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetCategoriesLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetCategoriesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Category?>> GetCategories(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetCategoriesQuery(doc_company);

            return await _coreRepository.GetRecords<Category>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsCategories(List<Category> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new Category(), tableName: "TotvsCategories");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetGridsLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetGridsLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Grid?>> GetGrids(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetGridsQuery(doc_company);

            return await _coreRepository.GetRecords<Grid>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsGrids(List<Grid> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o catálogo antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Grid(), tableName: "TotvsGrids"), data),
                (_coreRepository.CreateSystemDataTable(entity: new GridSize(), tableName: "TotvsGridsSizes"), data.SelectMany(x => x.sizes).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do catálogo
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetMeasurementUnitsLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetMeasurementUnitsLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<MeasurementUnit?>> GetMeasurementUnits(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetMeasurementUnitsQuery(doc_company);

            return await _coreRepository.GetRecords<MeasurementUnit>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsMeasurementUnits(List<MeasurementUnit> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o catálogo antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new MeasurementUnit(), tableName: "TotvsMeasurementUnits"), data),
                (_coreRepository.CreateSystemDataTable(entity: new MeasurementUnitVariation(), tableName: "TotvsMeasurementUnitsVariations"), data.SelectMany(x => x.variations).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do catálogo
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<IEnumerable<Color?>> GetColorsHashes(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetColorsHashesQuery(doc_company);

            return await _coreRepository.GetRecords<Color>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsColors(List<Color> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o catálogo antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Color(), tableName: "TotvsColors"), data),
                (_coreRepository.CreateSystemDataTable(entity: new ColorAdditionalInformation(), tableName: "TotvsColorsAdditionalInformation"), data.SelectMany(x => x.additionalInformation).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do catálogo
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetAdditionalFieldsTypesLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetAdditionalFieldsTypesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<AdditionalFieldType?>> GetAdditionalFieldsTypes(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetAdditionalFieldsTypesQuery(doc_company);

            return await _coreRepository.GetRecords<AdditionalFieldType>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsAdditionalFieldsTypes(List<AdditionalFieldType> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new AdditionalFieldType(), tableName: "TotvsAdditionalFieldsTypes");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetProductGrouperConfigurationsLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetProductGrouperConfigurationsLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<ProductGrouperConfiguration?>> GetProductGrouperConfigurations(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetProductGrouperConfigurationsQuery(doc_company);

            return await _coreRepository.GetRecords<ProductGrouperConfiguration>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsProductGrouperConfigurations(List<ProductGrouperConfiguration> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o catálogo antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new ProductGrouperConfiguration(), tableName: "TotvsProductGrouperConfigurations"), data),
                (_coreRepository.CreateSystemDataTable(entity: new ProductGrouperConfigurationItem(), tableName: "TotvsProductGrouperConfigurationsItems"), data.SelectMany(x => x.items).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do catálogo
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetInstructionItemsLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetInstructionItemsLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<InstructionItem?>> GetInstructionItems(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetInstructionItemsQuery(doc_company);

            return await _coreRepository.GetRecords<InstructionItem>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsInstructionItems(List<InstructionItem> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new InstructionItem(), tableName: "TotvsInstructionItems");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetCompositionsLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetCompositionsLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Composition?>> GetCompositions(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetCompositionsQuery(doc_company);

            return await _coreRepository.GetRecords<Composition>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsCompositions(List<Composition> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o catálogo antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Composition(), tableName: "TotvsCompositions"), data),
                (_coreRepository.CreateSystemDataTable(entity: new CompositionFiber(), tableName: "TotvsCompositionsFibers"), data.SelectMany(x => x.fibers).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do catálogo
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetPriceTablesLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetPriceTablesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<PriceTable?>> GetPriceTables(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetPriceTablesQuery(doc_company);

            return await _coreRepository.GetRecords<PriceTable>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsPriceTables(List<PriceTable> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o catálogo antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new PriceTable(), tableName: "TotvsPriceTables"), data),
                (_coreRepository.CreateSystemDataTable(entity: new PriceTableSalesOrderClassification(), tableName: "TotvsPriceTablesSalesOrderClassifications"), data.SelectMany(x => x.salesOrderClassifications).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new PriceTablePersonClassification(), tableName: "TotvsPriceTablesPersonClassifications"), data.SelectMany(x => x.personClassifications).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new PriceTablePaymentCondition(), tableName: "TotvsPriceTablesPaymentConditions"), data.SelectMany(x => x.paymentConditions).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new PriceTableAveragePeriod(), tableName: "TotvsPriceTablesAveragePeriods"), data.SelectMany(x => x.averagePeriods).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new PriceTableAveragePeriodQuantity(), tableName: "TotvsPriceTablesAveragePeriodQuantities"), data.SelectMany(x => x.averagePeriodQuantities).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do catálogo
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetPriceTableScalesLastChange(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetPriceTableScalesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<PriceTableScale?>> GetPriceTableScales(string doc_company)
        {
            string sql = _totvsProductCatalogsCommandHandler.CreateGetPriceTableScalesQuery(doc_company);

            return await _coreRepository.GetRecords<PriceTableScale>(sql);
        }


        public async Task<bool> BulkInsertIntoTotvsPriceTableScales(List<PriceTableScale> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o catálogo antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new PriceTableScale(), tableName: "TotvsPriceTableScales"), data),
                (_coreRepository.CreateSystemDataTable(entity: new PriceTableScaleItem(), tableName: "TotvsPriceTableScalesItems"), data.SelectMany(x => x.items).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do catálogo
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
