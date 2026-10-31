using Application.Movidesk.Abstractions.Services;

namespace Hosts.Scheduling.RecurringJobs
{
    /// <summary>
    /// jobs recorrentes do Movidesk: IdJob de auditing.Jobs → método do service (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md
    /// §4.2). Se roda, quando e em que fila vem da tabela.
    /// </summary>
    public static class MovideskRecurringJobs
    {
        public static IEnumerable<RecurringJobDefinition> All =>
        [
            RecurringJobDefinition.For<IMovideskPeopleService>(3001, s => s.GetPeople()),

            RecurringJobDefinition.For<IMovideskServicesService>(3004, s => s.GetServices()),

            RecurringJobDefinition.For<IMovideskTicketsService>(3000, s => s.GetTickets())
        ];
    }
}
