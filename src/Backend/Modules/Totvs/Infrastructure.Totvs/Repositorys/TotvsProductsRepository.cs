using Domain.Totvs.Entities.Reads.Products.Values;
using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.Products;
using System.Collections;
using System.Data;

namespace Infrastructure.Totvs.Repositorys
{
    public class TotvsProductsRepository : ITotvsProductsRepository
    {
        private readonly ITotvsProductsCommandHandler _totvsProductsCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsProductsRepository(ITotvsProductsCommandHandler totvsProductsCommandHandler, ISharedRepository coreRepository) =>
            (_totvsProductsCommandHandler, _coreRepository) = (totvsProductsCommandHandler, coreRepository);

        public async Task<DateTime?> GetSkusLastChange(string doc_company, int branchInfoCode)
        {
            string sql = _totvsProductsCommandHandler.CreateGetSkusLastChangeQuery(doc_company, branchInfoCode);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Sku?>> GetSkusByIds(string doc_company, int branchInfoCode, List<int> referenceIds)
        {
            var result = new List<Sku?>();

            // lotes de 1000 ids, como no GetPeopleByIds do Movidesk
            foreach (var chunk in referenceIds.Distinct().Chunk(1000))
            {
                string sql = _totvsProductsCommandHandler.CreateGetSkusByIdsQuery(doc_company, branchInfoCode, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Sku>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsSkus(List<Sku> data, Guid? parentExecutionGUID)
        {
            var colors = data.SelectMany(x => x.colors).ToList();
            var measurements = colors.SelectMany(x => x.measurementTableOmni).ToList();
            var products = colors.SelectMany(x => x.products).ToList();
            var instructions = products.SelectMany(x => x.conservationInstructions).ToList();

            // ordem = ordem dos merges: referência, filhas da referência, cores, filhas da cor, produtos e filhas do produto
            // (FKs e condições de exclusão do merge, Script012_Create_Totvs_Products_Sync_Procedures)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Sku(), tableName: "TotvsSkus"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Grid(), tableName: "TotvsSkusGrids"), data.SelectMany(x => x.grids).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Observation(), tableName: "TotvsSkusObservations"), data.SelectMany(x => x.observations).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Detail(), tableName: "TotvsSkusDetails"), data.SelectMany(x => x.details).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Composition(), tableName: "TotvsSkusCompositions"), data.SelectMany(x => x.compositions).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ReferenceCategory(), tableName: "TotvsSkusReferenceCategories"), data.SelectMany(x => x.referenceCategories).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new IntegrationGroupCode(), tableName: "TotvsSkusIntegrationGroupCodes"), data.SelectMany(x => x.integrationGroupCodes).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Color(), tableName: "TotvsSkusColors"), colors),
                (_coreRepository.CreateSystemDataTable(entity: new MeasurementTableOmni(), tableName: "TotvsSkusColorsMeasurementsTableOmni"), measurements),
                (_coreRepository.CreateSystemDataTable(entity: new Medida(), tableName: "TotvsSkusColorsMeasurementsTableOmniMedidas"), measurements.SelectMany(x => x.medidas).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Product(), tableName: "TotvsSkusColorsProducts"), products),
                (_coreRepository.CreateSystemDataTable(entity: new Barcode(), tableName: "TotvsProductsBarCodes"), products.SelectMany(x => x.barCodes).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Classification(), tableName: "TotvsProductsClassifications"), products.SelectMany(x => x.classifications).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new AdditionalField(), tableName: "TotvsProductsAdditionalFields"), products.SelectMany(x => x.additionalFields).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Supplier(), tableName: "TotvsProductsSuppliers"), products.SelectMany(x => x.suppliers).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Manufacturer(), tableName: "TotvsProductsManufacturers"), products.SelectMany(x => x.manufacturers).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ReferenceCodeSequence(), tableName: "TotvsProductsReferenceCodeSequences"), products.SelectMany(x => x.referenceCodeSequences).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new WebData(), tableName: "TotvsProductsWebData"), products.SelectMany(x => x.webData).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new WebDetail(), tableName: "TotvsProductsWebDetails"), products.SelectMany(x => x.webDetails).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new BranchProductBlocked(), tableName: "TotvsProductsBranchesBlocked"), products.SelectMany(x => x.branchesProductBlocked).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ConservationInstruction(), tableName: "TotvsProductsConservationInstructions"), instructions),
                (_coreRepository.CreateSystemDataTable(entity: new ConservationInstructionItem(), tableName: "TotvsProductsConservationInstructionsItems"), instructions.SelectMany(x => x.items).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram da referência ou do produto
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<ValueState?> GetBalancesState(string doc_company, int branchCode)
        {
            string sql = _totvsProductsCommandHandler.CreateGetBalancesStateQuery(doc_company, branchCode);

            return await _coreRepository.GetRecord<ValueState>(sql);
        }

        public async Task<IEnumerable<ValueChange?>> GetBalancesChanges(string doc_company, int branchCode, List<int> productCodes)
        {
            var result = new List<ValueChange?>();

            // lotes de 1000 códigos, como no GetPeopleByIds do Movidesk
            foreach (var chunk in productCodes.Distinct().Chunk(1000))
            {
                string sql = _totvsProductsCommandHandler.CreateGetBalancesChangesQuery(doc_company, branchCode, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<ValueChange>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsBalances(List<Balance> data, List<BalanceLocation> locations, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o saldo antes dos locais do produto (condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Balance(), tableName: "TotvsBalances"), data),
                (_coreRepository.CreateSystemDataTable(entity: new BalanceLocation(), tableName: "TotvsBalancesLocations"), locations),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge dos locais roda mesmo sem linhas no lote: é ele que apaga os que saíram do produto
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<IEnumerable<int>> GetSkusProductCodes(string doc_company, int branchInfoCode)
        {
            string sql = _totvsProductsCommandHandler.CreateGetSkusProductCodesQuery(doc_company, branchInfoCode);

            return (await _coreRepository.GetRecords<int>(sql)).Select(x => x);
        }

        public async Task<IEnumerable<Domain.Totvs.Entities.Reads.Products.Similars.ProductSimilar?>> GetSimilarHashes(string doc_company)
        {
            string sql = _totvsProductsCommandHandler.CreateGetSimilarHashesQuery(doc_company);

            return await _coreRepository.GetRecords<Domain.Totvs.Entities.Reads.Products.Similars.ProductSimilar>(sql);
        }

        public async Task<DateTime?> GetSimilarLastWindowEnd(string doc_company)
        {
            string sql = _totvsProductsCommandHandler.CreateGetSimilarLastWindowEndQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsProductsSimilar(List<Domain.Totvs.Entities.Reads.Products.Similars.ProductSimilar> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o produto antes dos similares (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.Products.Similars.ProductSimilar(), tableName: "TotvsProductsSimilar"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.Products.Similars.ProductSimilarItem(), tableName: "TotvsProductsSimilarItems"), data.SelectMany(x => x.items).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do registro
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<IEnumerable<Domain.Totvs.Entities.Reads.Products.Batches.Batch?>> GetBatchesHashes(string doc_company, int branchCode)
        {
            string sql = _totvsProductsCommandHandler.CreateGetBatchesHashesQuery(doc_company, branchCode);

            return await _coreRepository.GetRecords<Domain.Totvs.Entities.Reads.Products.Batches.Batch>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsBatches(List<Domain.Totvs.Entities.Reads.Products.Batches.Batch> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: lote, itens e campos adicionais (FKs e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.Products.Batches.Batch(), tableName: "TotvsBatches"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.Products.Batches.BatchItem(), tableName: "TotvsBatchesItems"), data.SelectMany(x => x.items).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.Products.Batches.BatchItemAdditionalField(), tableName: "TotvsBatchesItemsAdditionalFields"), data.SelectMany(x => x.additionalFields).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do registro
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
