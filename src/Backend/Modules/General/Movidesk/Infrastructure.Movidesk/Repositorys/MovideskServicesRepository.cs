using Application.Movidesk.Abstractions.Handlers.Commands;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Entities.Reads.Services;
using Domain.Shared.Abstractions.Repositorys;
using System.Collections;
using System.Data;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskServicesRepository : IMovideskServicesRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public MovideskServicesRepository(IMovideskCommandHandler movideskCommandHandler, ISharedRepository coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);

        public async Task<bool> BulkInsertIntoMovideskServices(List<Service> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: serviço antes das categorias (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Service(), tableName: "MovideskServices"), data),
                (_coreRepository.CreateSystemDataTable(entity: new ServiceCategory(), tableName: "MovideskServicesCategories"), data.SelectMany(x => x.categories).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das categorias roda mesmo sem linhas no lote: é ele que apaga as categorias removidas
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("general", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
