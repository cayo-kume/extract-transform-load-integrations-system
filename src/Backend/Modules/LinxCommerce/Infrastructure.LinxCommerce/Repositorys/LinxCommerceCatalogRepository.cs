using Application.LinxCommerce.Abstractions.Handlers.Commands;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Entities.Reads.Catalog;
using Domain.Shared.Abstractions.Repositorys;
using System.Collections;
using System.Data;

namespace Infrastructure.LinxCommerce.Repositorys
{
    public class LinxCommerceCatalogRepository : ILinxCommerceCatalogRepository
    {
        private readonly ILinxCommerceCommandHandler _linxCommerceCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public LinxCommerceCatalogRepository(ILinxCommerceCommandHandler linxCommerceCommandHandler, ISharedRepository coreRepository) =>
            (_linxCommerceCommandHandler, _coreRepository) = (linxCommerceCommandHandler, coreRepository);

        public async Task<DateTimeOffset?> GetProductsLastModifiedDate(string doc_company)
        {
            string sql = _linxCommerceCommandHandler.CreateGetProductsLastModifiedDateQuery(doc_company);

            return await _coreRepository.GetRecord<DateTimeOffset?>(sql);
        }

        public async Task<IEnumerable<Product?>> GetProductsByIds(string doc_company, List<int> ids)
        {
            var result = new List<Product?>();

            // lotes de 1000 ids, como no GetPeopleByIds do Movidesk
            foreach (var chunk in ids.Distinct().Chunk(1000))
            {
                string sql = _linxCommerceCommandHandler.CreateGetProductsByIdsQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Product>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoLinxCommerceProducts(List<Product> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: produto antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Product(), tableName: "LinxCommerceProducts"), data),
                (_coreRepository.CreateSystemDataTable(entity: new ProductMetadataValue(), tableName: "LinxCommerceProductsMetadataValues"), data.SelectMany(x => x.metadataValues).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ProductMedia(), tableName: "LinxCommerceProductsMedias"), data.SelectMany(x => x.medias).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ProductMediaAssociation(), tableName: "LinxCommerceProductsMediaAssociations"), data.SelectMany(x => x.mediaAssociations).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do produto
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("linxcommerce", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTimeOffset?> GetSkusLastModifiedDate(string doc_company)
        {
            string sql = _linxCommerceCommandHandler.CreateGetSkusLastModifiedDateQuery(doc_company);

            return await _coreRepository.GetRecord<DateTimeOffset?>(sql);
        }

        public async Task<IEnumerable<Sku?>> GetSkusByIds(string doc_company, List<int> ids)
        {
            var result = new List<Sku?>();

            foreach (var chunk in ids.Distinct().Chunk(1000))
            {
                string sql = _linxCommerceCommandHandler.CreateGetSkusByIdsQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Sku>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoLinxCommerceSkus(List<Sku> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: SKU antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Sku(), tableName: "LinxCommerceSkus"), data),
                (_coreRepository.CreateSystemDataTable(entity: new SkuMetadataValue(), tableName: "LinxCommerceSkusMetadataValues"), data.SelectMany(x => x.metadataValues).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new SkuParentRelation(), tableName: "LinxCommerceSkusParentRelations"), data.SelectMany(x => x.parentRelations).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do SKU
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("linxcommerce", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
