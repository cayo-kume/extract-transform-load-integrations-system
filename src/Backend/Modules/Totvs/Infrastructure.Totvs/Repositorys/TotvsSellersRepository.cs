using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.Sellers;
using System.Collections;
using System.Data;

namespace Infrastructure.Totvs.Repositorys
{
    public class TotvsSellersRepository : ITotvsSellersRepository
    {
        private readonly ITotvsSellersCommandHandler _totvsSellersCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsSellersRepository(ITotvsSellersCommandHandler totvsSellersCommandHandler, ISharedRepository coreRepository) =>
            (_totvsSellersCommandHandler, _coreRepository) = (totvsSellersCommandHandler, coreRepository);

        public async Task<DateTime?> GetSellersLastChange(string doc_company)
        {
            string sql = _totvsSellersCommandHandler.CreateGetSellersLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Seller?>> GetSellersByCodes(string doc_company, List<int> codes)
        {
            var result = new List<Seller?>();

            // lotes de 1000 códigos, como no GetPeopleByIds do Movidesk
            foreach (var chunk in codes.Distinct().Chunk(1000))
            {
                string sql = _totvsSellersCommandHandler.CreateGetSellersByCodesQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Seller>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsSellers(List<Seller> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: vendedor antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Seller(), tableName: "TotvsSellers"), data),
                (_coreRepository.CreateSystemDataTable(entity: new BranchInformation(), tableName: "TotvsSellersBranchInformations"), data.SelectMany(x => x.branchInformations).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Customer(), tableName: "TotvsSellersCustomers"), data.SelectMany(x => x.customers).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do vendedor
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<IEnumerable<OperationalArea?>> GetOperationalAreasHashes(string doc_company)
        {
            string sql = _totvsSellersCommandHandler.CreateGetOperationalAreasHashesQuery(doc_company);

            return await _coreRepository.GetRecords<OperationalArea>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsOperationalAreas(List<OperationalArea> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: região antes das filhas (FKs e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new OperationalArea(), tableName: "TotvsOperationalAreas"), data),
                (_coreRepository.CreateSystemDataTable(entity: new OperationalAreaState(), tableName: "TotvsOperationalAreasStates"), data.SelectMany(x => x.states).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OperationalAreaCep(), tableName: "TotvsOperationalAreasCeps"), data.SelectMany(x => x.ceps).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OperationalAreaCity(), tableName: "TotvsOperationalAreasCities"), data.SelectMany(x => x.cities).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram da região
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
