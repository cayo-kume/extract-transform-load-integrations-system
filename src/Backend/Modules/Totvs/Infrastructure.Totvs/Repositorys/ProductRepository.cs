using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Product;

namespace Infrastructure.Totvs.Repositorys
{
    public class ProductRepository : IProductRepository
    {
        private readonly IProductCommandHandler _commandHandler;
        private readonly IIntegrationsSharedRepository _coreRepository;

        public ProductRepository(IIntegrationsSharedRepository coreRepository, IProductCommandHandler commandHandler) =>
            (_coreRepository, _commandHandler) = (coreRepository, commandHandler);

        public async Task<bool> BulkInsertIntoTableSkusRaw(List<Sku> registros, Guid? parentExecutionGUID)
        {
            var colors = registros.SelectMany(s => s.colors).ToList();
            var measurements = colors.SelectMany(c => c.measurementTableOmni).ToList();
            var products = colors.SelectMany(c => c.products).ToList();
            var conservationInstructions = products.SelectMany(p => p.conservationInstructions).ToList();
            var skusTable = _coreRepository.CreateSystemDataTable(entity: new Sku(), tableName: "Skus");
            var skusObservationsTable = _coreRepository.CreateSystemDataTable(entity: new Observation(), tableName: "SkusObservations");
            var skusDetailsTable = _coreRepository.CreateSystemDataTable(entity: new Detail(), tableName: "SkusDetails");
            var skusCompositionsTable = _coreRepository.CreateSystemDataTable(entity: new Composition(), tableName: "SkusCompositions");
            var skusReferenceCategoriesTable = _coreRepository.CreateSystemDataTable(entity: new ReferenceCategory(), tableName: "SkusReferenceCategories");
            var skusColorsTable = _coreRepository.CreateSystemDataTable(entity: new Color(), tableName: "SkusColors");
            var skusColorsMeasurementsTableOmniTable = _coreRepository.CreateSystemDataTable(entity: new MeasurementTableOmni(), tableName: "SkusColorsMeasurementsTableOmni");
            var skusColorsMeasurementsTableOmniMedidasTable = _coreRepository.CreateSystemDataTable(entity: new Medida(), tableName: "SkusColorsMeasurementsTableOmniMedidas");
            var skusColorsProductsTable = _coreRepository.CreateSystemDataTable(entity: new Product(), tableName: "SkusColorsProducts");
            var productsBarCodesTable = _coreRepository.CreateSystemDataTable(entity: new Barcode(), tableName: "ProductsBarCodes");
            var productsClassificationsTable = _coreRepository.CreateSystemDataTable(entity: new Classification(), tableName: "ProductsClassifications");
            var productsAdditionalFieldsTable = _coreRepository.CreateSystemDataTable(entity: new AdditionalField(), tableName: "ProductsAdditionalFields");
            var productsSuppliersTable = _coreRepository.CreateSystemDataTable(entity: new Supplier(), tableName: "ProductsSuppliers");
            var productsManufacturersTable = _coreRepository.CreateSystemDataTable(entity: new Manufacturer(), tableName: "ProductsManufacturers");
            var productsReferenceCodeSequencesTable = _coreRepository.CreateSystemDataTable(entity: new ReferenceCodeSequence(), tableName: "ProductsReferenceCodeSequences");
            var productsWebDataTable = _coreRepository.CreateSystemDataTable(entity: new WebData(), tableName: "ProductsWebData");
            var productsWebDetailsTable = _coreRepository.CreateSystemDataTable(entity: new WebDetail(), tableName: "ProductsWebDetails");
            var productsBranchesBlockedTable = _coreRepository.CreateSystemDataTable(entity: new BranchesProductBlocked(), tableName: "ProductsBranchesBlocked");
            var productsConservationInstructionsTable = _coreRepository.CreateSystemDataTable(entity: new ConservationInstruction(), tableName: "ProductsConservationInstructions");
            var productsConservationInstructionsItemsTable = _coreRepository.CreateSystemDataTable(entity: new ConservationInstructionsItem(), tableName: "ProductsConservationInstructionsItems");

            _coreRepository.FillSystemDataTable(skusTable, registros);
            _coreRepository.FillSystemDataTable(skusObservationsTable, registros.SelectMany(s => s.observations).ToList());
            _coreRepository.FillSystemDataTable(skusDetailsTable, registros.SelectMany(s => s.details).ToList());
            _coreRepository.FillSystemDataTable(skusCompositionsTable, registros.SelectMany(s => s.compositions).ToList());
            _coreRepository.FillSystemDataTable(skusReferenceCategoriesTable, registros.SelectMany(s => s.referenceCategories).ToList());
            _coreRepository.FillSystemDataTable(skusColorsTable, colors);
            _coreRepository.FillSystemDataTable(skusColorsMeasurementsTableOmniTable, measurements);
            _coreRepository.FillSystemDataTable(skusColorsMeasurementsTableOmniMedidasTable, measurements.SelectMany(m => m.medidas).ToList());
            _coreRepository.FillSystemDataTable(skusColorsProductsTable, products);
            _coreRepository.FillSystemDataTable(productsBarCodesTable, products.SelectMany(p => p.barCodes).ToList());
            _coreRepository.FillSystemDataTable(productsClassificationsTable, products.SelectMany(p => p.classifications).ToList());
            _coreRepository.FillSystemDataTable(productsAdditionalFieldsTable, products.SelectMany(p => p.additionalFields).ToList());
            _coreRepository.FillSystemDataTable(productsSuppliersTable, products.SelectMany(p => p.suppliers).ToList());
            _coreRepository.FillSystemDataTable(productsManufacturersTable, products.SelectMany(p => p.manufacturers).ToList());
            _coreRepository.FillSystemDataTable(productsReferenceCodeSequencesTable, products.SelectMany(p => p.referenceCodeSequences).ToList());
            _coreRepository.FillSystemDataTable(productsWebDataTable, products.SelectMany(p => p.webDatas).ToList());
            _coreRepository.FillSystemDataTable(productsWebDetailsTable, products.SelectMany(p => p.webDetails).ToList());
            _coreRepository.FillSystemDataTable(productsBranchesBlockedTable, products.SelectMany(p => p.branchesProductBlocked).ToList());
            _coreRepository.FillSystemDataTable(productsConservationInstructionsTable, conservationInstructions);
            _coreRepository.FillSystemDataTable(productsConservationInstructionsItemsTable, conservationInstructions.SelectMany(ci => ci.items).ToList());

            _coreRepository.BulkInsertIntoTableRaw(skusTable);
            _coreRepository.BulkInsertIntoTableRaw(skusObservationsTable);
            _coreRepository.BulkInsertIntoTableRaw(skusDetailsTable);
            _coreRepository.BulkInsertIntoTableRaw(skusCompositionsTable);
            _coreRepository.BulkInsertIntoTableRaw(skusReferenceCategoriesTable);
            _coreRepository.BulkInsertIntoTableRaw(skusColorsTable);
            _coreRepository.BulkInsertIntoTableRaw(skusColorsMeasurementsTableOmniTable);
            _coreRepository.BulkInsertIntoTableRaw(skusColorsMeasurementsTableOmniMedidasTable);
            _coreRepository.BulkInsertIntoTableRaw(skusColorsProductsTable);
            _coreRepository.BulkInsertIntoTableRaw(productsBarCodesTable);
            _coreRepository.BulkInsertIntoTableRaw(productsClassificationsTable);
            _coreRepository.BulkInsertIntoTableRaw(productsAdditionalFieldsTable);
            _coreRepository.BulkInsertIntoTableRaw(productsSuppliersTable);
            _coreRepository.BulkInsertIntoTableRaw(productsManufacturersTable);
            _coreRepository.BulkInsertIntoTableRaw(productsReferenceCodeSequencesTable);
            _coreRepository.BulkInsertIntoTableRaw(productsWebDataTable);
            _coreRepository.BulkInsertIntoTableRaw(productsWebDetailsTable);
            _coreRepository.BulkInsertIntoTableRaw(productsBranchesBlockedTable);
            _coreRepository.BulkInsertIntoTableRaw(productsConservationInstructionsTable);
            _coreRepository.BulkInsertIntoTableRaw(productsConservationInstructionsItemsTable);

            await _coreRepository.CallDbProcMerge("totvs", skusTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", skusObservationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", skusDetailsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", skusCompositionsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", skusReferenceCategoriesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", skusColorsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", skusColorsMeasurementsTableOmniTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", skusColorsMeasurementsTableOmniMedidasTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", skusColorsProductsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsBarCodesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsClassificationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsAdditionalFieldsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsSuppliersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsManufacturersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsReferenceCodeSequencesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsWebDataTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsWebDetailsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsBranchesBlockedTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsConservationInstructionsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsConservationInstructionsItemsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.Totvs.Dtos.Product.Sku?>> GetExistingSkus(List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<Domain.Totvs.Dtos.Product.Sku>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetExistingSkusQuery(batch);
                    var result = await _coreRepository.GetRecords<Domain.Totvs.Dtos.Product.Sku>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }
    }
}
