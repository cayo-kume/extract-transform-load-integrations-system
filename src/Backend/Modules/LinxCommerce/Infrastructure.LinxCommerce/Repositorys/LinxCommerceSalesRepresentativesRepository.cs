using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Entities.Reads.SalesRepresentatives;
using Domain.Shared.Abstractions.Repositorys;
using System.Collections;
using System.Data;

namespace Infrastructure.LinxCommerce.Repositorys
{
    public class LinxCommerceSalesRepresentativesRepository : ILinxCommerceSalesRepresentativesRepository
    {
        private readonly ISharedRepository _coreRepository;

        public LinxCommerceSalesRepresentativesRepository(ISharedRepository coreRepository) =>
            _coreRepository = coreRepository;

        public async Task<bool> BulkInsertIntoLinxCommerceSalesRepresentatives(List<SalesRepresentative> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: vendedor antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new SalesRepresentative(), tableName: "LinxCommerceSalesRepresentatives"), data),
                (_coreRepository.CreateSystemDataTable(entity: new SalesRepresentativeAddress(), tableName: "LinxCommerceSalesRepresentativesAddresses"), data.SelectMany(x => x.addresses).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new SalesRepresentativeCustomer(), tableName: "LinxCommerceSalesRepresentativesCustomers"), data.SelectMany(x => x.customers).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do vendedor
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("linxcommerce", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
