using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.Products.Values;
using System.Collections;
using System.Data;

namespace Infrastructure.Totvs.Repositorys
{
    /// <summary>
    /// preços e custos dos produtos, por loja (fase PR-2, docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9)
    /// </summary>
    public class TotvsProductPricesRepository : ITotvsProductPricesRepository
    {
        private readonly ITotvsCommandHandler _totvsCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsProductPricesRepository(ITotvsCommandHandler totvsCommandHandler, ISharedRepository coreRepository) =>
            (_totvsCommandHandler, _coreRepository) = (totvsCommandHandler, coreRepository);

        public async Task<ValueState?> GetPricesState(string doc_company, int branchCode)
        {
            string sql = _totvsCommandHandler.CreateGetPricesStateQuery(doc_company, branchCode);

            return await _coreRepository.GetRecord<ValueState>(sql);
        }

        public async Task<IEnumerable<ValueChange?>> GetPricesChanges(string doc_company, int branchCode, List<int> productCodes)
        {
            var result = new List<ValueChange?>();

            // lotes de 1000 códigos, como no GetPeopleByIds do Movidesk
            foreach (var chunk in productCodes.Distinct().Chunk(1000))
            {
                string sql = _totvsCommandHandler.CreateGetPricesChangesQuery(doc_company, branchCode, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<ValueChange>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsPrices(List<Price> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o valor antes da filha (condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Price(), tableName: "TotvsPrices"), data),
                (_coreRepository.CreateSystemDataTable(entity: new PriceOtherPromotion(), tableName: "TotvsPricesOtherPromotions"), data.SelectMany(x => x.otherPromotions).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge da filha roda mesmo sem linhas no lote: é ele que apaga as que saíram do produto
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<ValueState?> GetCostsState(string doc_company, int branchCode)
        {
            string sql = _totvsCommandHandler.CreateGetCostsStateQuery(doc_company, branchCode);

            return await _coreRepository.GetRecord<ValueState>(sql);
        }

        public async Task<IEnumerable<ValueChange?>> GetCostsChanges(string doc_company, int branchCode, List<int> productCodes)
        {
            var result = new List<ValueChange?>();

            // lotes de 1000 códigos, como no GetPeopleByIds do Movidesk
            foreach (var chunk in productCodes.Distinct().Chunk(1000))
            {
                string sql = _totvsCommandHandler.CreateGetCostsChangesQuery(doc_company, branchCode, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<ValueChange>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsCosts(List<Cost> data, Guid? parentExecutionGUID)
        {
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Cost(), tableName: "TotvsCosts"), data),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
