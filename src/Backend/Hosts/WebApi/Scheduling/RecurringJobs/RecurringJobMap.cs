using System.Text.RegularExpressions;

namespace WebApi.Scheduling.RecurringJobs
{
    /// <summary>
    /// todos os mapas IdJob → método dos módulos registrados no host (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.2). Um
    /// módulo novo entra aqui junto com o registro no ServicesExtensions (passo 6)
    /// </summary>
    public static partial class RecurringJobMap
    {
        public static IReadOnlyDictionary<int, RecurringJobDefinition> Definitions { get; } = Build();

        // os recurring jobs da agenda se chamam <IdJob>-<Title>; os de manutenção do host (SchedulerRefresh, FailedJobsCleanup) não
        [GeneratedRegex(@"^(\d+)-")]
        private static partial Regex RecurringJobIdRegex();

        public static string RecurringJobId(int idJob, string? title) =>
            $"{idJob}-{title}";

        /// <summary>
        /// IdJob de um recurring job da agenda; null nos jobs de manutenção do host
        /// </summary>
        public static int? IdJobOf(string? recurringJobId)
        {
            var match = recurringJobId is null ? null : RecurringJobIdRegex().Match(recurringJobId);

            return match is { Success: true } && int.TryParse(match.Groups[1].Value, out var idJob) ? idJob : null;
        }

        private static Dictionary<int, RecurringJobDefinition> Build()
        {
            var definitions = new Dictionary<int, RecurringJobDefinition>();

            foreach (var definition in AfterSaleRecurringJobs.All
                .Concat(DataWarehouseRecurringJobs.All)
                .Concat(LinxCommerceRecurringJobs.All)
                .Concat(LinxMicrovixRecurringJobs.All)
                .Concat(MovideskRecurringJobs.All)
                .Concat(TotvsRecurringJobs.All))
            {
                // IdJob repetido é erro de mapa: o host não sobe
                if (!definitions.TryAdd(definition.IdJob, definition))
                    throw new InvalidOperationException($"IdJob {definition.IdJob} aparece duas vezes nos mapas de jobs recorrentes.");
            }

            return definitions;
        }
    }
}
