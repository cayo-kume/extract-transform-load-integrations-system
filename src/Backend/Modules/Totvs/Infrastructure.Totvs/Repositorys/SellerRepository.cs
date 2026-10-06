using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Dtos.Seller;
using Domain.Totvs.Entities.Seller;

namespace Infrastructure.Totvs.Repositorys
{
    public class SellerRepository : ISellerRepository
    {
        private readonly ISellerCommandHandler _commandHandler;
        private readonly IIntegrationsSharedRepository _coreRepository;

        public SellerRepository(IIntegrationsSharedRepository coreRepository, ISellerCommandHandler commandHandler) =>
            (_coreRepository, _commandHandler) = (coreRepository, commandHandler);

        public async Task<bool> BulkInsertIntoTableSellersRaw(List<Seller> registros, Guid? parentExecutionGUID)
        {
            var sellersTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Seller.Seller(), tableName: "Sellers");
            var sellersCustomersTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Seller.Customer(), tableName: "SellersCustomer");
            var sellersBranchsInformationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Seller.BranchInformation(), tableName: "SellersBranchInformation");

            _coreRepository.FillSystemDataTable(sellersTable, registros.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(sellersCustomersTable, registros.Select(y => y.customers).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(sellersBranchsInformationsTable, registros.Select(y => y.branchInformations).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());

            _coreRepository.BulkInsertIntoTableRaw(sellersTable);
            _coreRepository.BulkInsertIntoTableRaw(sellersCustomersTable);
            _coreRepository.BulkInsertIntoTableRaw(sellersBranchsInformationsTable);

            await _coreRepository.CallDbProcMerge("totvs", sellersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", sellersCustomersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", sellersBranchsInformationsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Item?>> GetExistingSellers(List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<Domain.Totvs.Dtos.Seller.Item>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetExistingSellersQuery(batch);
                    var result = await _coreRepository.GetRecords<Domain.Totvs.Dtos.Seller.Item>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }
    }
}
