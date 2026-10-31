using Application.DataWarehouse.Abstractions.Services;

namespace Hosts.Scheduling.RecurringJobs
{
    /// <summary>
    /// jobs recorrentes do data warehouse: IdJob de auditing.Jobs → método do service (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md
    /// §4.2). Se roda, quando e em que fila vem da tabela.
    /// Os jobs do DW dividem a chave group:datawarehouse: escrevem nas mesmas landings, e a trava do DataWarehouseService
    /// (SemaphoreSlim estático) não vale entre processos. O 9003 e o 9004 são manuais e ficam fora.
    /// </summary>
    public static class DataWarehouseRecurringJobs
    {
        public static IEnumerable<RecurringJobDefinition> All =>
        [
            RecurringJobDefinition.For<IDataWarehouseService>(9000, s => s.RunHourly(), "datawarehouse"),
            RecurringJobDefinition.For<IDataWarehouseService>(9001, s => s.RunDaily(), "datawarehouse"),
            RecurringJobDefinition.For<IDataWarehouseService>(9002, s => s.RunShortCycle(), "datawarehouse")
        ];
    }
}
