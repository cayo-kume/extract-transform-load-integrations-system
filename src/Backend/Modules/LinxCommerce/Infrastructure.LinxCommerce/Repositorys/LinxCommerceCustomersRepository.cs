using Application.LinxCommerce.Abstractions.Handlers.Commands;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Entities.Reads.Customers;
using Domain.Shared.Abstractions.Repositorys;
using System.Collections;
using System.Data;

namespace Infrastructure.LinxCommerce.Repositorys
{
    public class LinxCommerceCustomersRepository : ILinxCommerceCustomersRepository
    {
        private readonly ILinxCommerceCustomersCommandHandler _linxCommerceCustomersCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public LinxCommerceCustomersRepository(ILinxCommerceCustomersCommandHandler linxCommerceCustomersCommandHandler, ISharedRepository coreRepository) =>
            (_linxCommerceCustomersCommandHandler, _coreRepository) = (linxCommerceCustomersCommandHandler, coreRepository);

        public async Task<DateTime?> GetCustomersLastBatchTime(string doc_company)
        {
            string sql = _linxCommerceCustomersCommandHandler.CreateGetCustomersLastBatchTimeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<bool> BulkInsertIntoLinxCommerceCustomers(List<Customer> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: cliente antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Customer(), tableName: "LinxCommerceCustomers"), data),
                (_coreRepository.CreateSystemDataTable(entity: new CustomerAddress(), tableName: "LinxCommerceCustomersAddresses"), data.SelectMany(x => x.addresses).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new CustomerGroup(), tableName: "LinxCommerceCustomersGroups"), data.SelectMany(x => x.groups).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do cliente
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("linxcommerce", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
