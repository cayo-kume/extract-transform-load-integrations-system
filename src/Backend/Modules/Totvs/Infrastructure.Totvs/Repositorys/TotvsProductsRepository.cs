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
        private readonly ITotvsCommandHandler _totvsCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsProductsRepository(ITotvsCommandHandler totvsCommandHandler, ISharedRepository coreRepository) =>
            (_totvsCommandHandler, _coreRepository) = (totvsCommandHandler, coreRepository);

        public async Task<DateTime?> GetSkusLastChange(string doc_company, int branchInfoCode)
        {
            string sql = _totvsCommandHandler.CreateGetSkusLastChangeQuery(doc_company, branchInfoCode);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Sku?>> GetSkusByIds(string doc_company, int branchInfoCode, List<int> referenceIds)
        {
            var result = new List<Sku?>();

            // lotes de 1000 ids, como no GetPeopleByIds do Movidesk
            foreach (var chunk in referenceIds.Distinct().Chunk(1000))
            {
                string sql = _totvsCommandHandler.CreateGetSkusByIdsQuery(doc_company, branchInfoCode, chunk.ToList());

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
            // (FKs e condições de exclusão do merge, Script011_Create_Totvs_Products_Sync_Procedures)
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
    }
}
