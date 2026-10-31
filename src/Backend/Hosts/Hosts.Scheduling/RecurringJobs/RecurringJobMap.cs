using System.Text.RegularExpressions;

namespace Hosts.Scheduling.RecurringJobs
{
    /// <summary>
    /// todos os mapas IdJob → método dos módulos registrados no host (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.2). Um
    /// módulo novo entra aqui junto com o registro no ServicesExtensions (passo 6)
    /// </summary>
    public static partial class RecurringJobMap
    {
        public static IReadOnlyDictionary<int, RecurringJobDefinition> Definitions { get; } = Build();

        // os recurring jobs da agenda se chamam <IdJob>-<Title> - V<versão>; os de manutenção do host (SchedulerRefresh,
        // FailedJobsCleanup) não
        [GeneratedRegex(@"^(\d+)-")]
        private static partial Regex RecurringJobIdRegex();

        /// <summary>
        /// nome do recurring job, com a versão do método (§4.10): ex.: "8006-LinxClientesFornec - V1.0.0". Uma versão nova muda o
        /// nome, e o SchedulerRefresh troca o job antigo pelo novo no ciclo seguinte
        /// </summary>
        public static string RecurringJobId(int idJob, string? title, string version) =>
            $"{idJob}-{title} - V{version}";

        /// <summary>
        /// IdJob de um recurring job da agenda; null nos jobs de manutenção do host
        /// </summary>
        public static int? IdJobOf(string? recurringJobId)
        {
            var match = recurringJobId is null ? null : RecurringJobIdRegex().Match(recurringJobId);

            return match is { Success: true } && int.TryParse(match.Groups[1].Value, out var idJob) ? idJob : null;
        }

        private static Dictionary<int, RecurringJobDefinition> Build() =>
            Validate(AfterSaleRecurringJobs.All
                .Concat(DataWarehouseRecurringJobs.All)
                .Concat(LinxCommerceRecurringJobs.All)
                .Concat(LinxMicrovixRecurringJobs.All)
                .Concat(MovideskRecurringJobs.All)
                .Concat(ProceduresRecurringJobs.All)
                .Concat(TotvsRecurringJobs.All));

        /// <summary>
        /// monta o dicionário IdJob → definição e recusa o mapa com IdJob repetido, método sem versão ou job sem título. Público
        /// para um host que tem jobs só dele (o WebJob, com a recuperação) validar o mapa inteiro do mesmo jeito
        /// </summary>
        public static Dictionary<int, RecurringJobDefinition> Validate(IEnumerable<RecurringJobDefinition> all)
        {
            var definitions = new Dictionary<int, RecurringJobDefinition>();

            foreach (var definition in all)
            {
                // IdJob repetido é erro de mapa: o host não sobe
                if (!definitions.TryAdd(definition.IdJob, definition))
                    throw new InvalidOperationException($"IdJob {definition.IdJob} aparece duas vezes nos mapas de jobs recorrentes.");
            }

            // todo método chamado por um job recorrente tem a versão (§4.10); faltando, o mapa é recusado com a lista inteira
            var withoutVersion = definitions.Values.Where(d => d.Version is null).Select(d => $"{d.IdJob} ({d.Target})").ToList();

            if (withoutVersion.Count > 0)
                throw new InvalidOperationException(
                    $"Métodos de jobs recorrentes sem [JobVersion] na classe do service: {string.Join(", ", withoutVersion)}.");

            // o título (nome do WebJob) vem do EnumJob do módulo; IdJob fora do EnumJob é erro de mapa
            var withoutTitle = definitions.Values.Where(d => string.IsNullOrWhiteSpace(d.Title)).Select(d => $"{d.IdJob} ({d.Target})").ToList();

            if (withoutTitle.Count > 0)
                throw new InvalidOperationException(
                    $"Jobs recorrentes sem título no EnumJob do módulo: {string.Join(", ", withoutTitle)}.");

            return definitions;
        }
    }
}
