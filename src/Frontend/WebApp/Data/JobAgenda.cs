using System.Globalization;
using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Agenda sugerida de cada job do catálogo, fixa no WebApp: o .docs/AGENDA-SUGERIDA-JOBS.csv embutido no assembly (o mesmo
    /// que o tools/seeds/Import-TestSeeds.ps1 usa). A tela de contratar módulo parte dela, e quem cadastra pode mudar antes de
    /// gravar (.docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md). Um job novo entra no .csv no mesmo commit do script do
    /// catálogo; sem linha, o sob demanda fica com intervalo 0 e o recorrente, de hora em hora.
    /// </summary>
    public static class JobAgenda
    {
        private const string ResourceName = "WebApp.AgendaSugeridaJobs.csv";

        private static readonly Lazy<IReadOnlyDictionary<(int IdJob, int IdType), CatalogJobModel>> Suggestions = new(Load);

        /// <summary>
        /// Preenche a agenda do job com a sugestão (o job vem do catálogo, com IsOnDemand).
        /// </summary>
        public static CatalogJobModel Suggest(CatalogJobModel job)
        {
            if (Suggestions.Value.TryGetValue((job.IdJob, job.IdType), out var suggestion))
            {
                job.IntervalTimeInMinutes = suggestion.IntervalTimeInMinutes;
                job.RunningDailyAt = suggestion.RunningDailyAt;
                job.Queue = suggestion.Queue;
            }
            else
            {
                job.IntervalTimeInMinutes = job.IsOnDemand ? 0 : 60;
                job.RunningDailyAt = null;
                job.Queue = "default";
            }

            // o sob demanda não entra na agenda, qualquer que seja a linha do .csv
            if (job.IsOnDemand)
            {
                job.IntervalTimeInMinutes = 0;
                job.RunningDailyAt = null;
            }

            return job;
        }

        // IdJob;IdType;IdParent;Title;IntervalTimeInMinutes;RunningDailyAt;Queue;IsOnDemand
        private static IReadOnlyDictionary<(int, int), CatalogJobModel> Load()
        {
            using var stream = typeof(JobAgenda).Assembly.GetManifestResourceStream(ResourceName)
                ?? throw new InvalidOperationException($"{ResourceName} não está embutido no WebApp (WebApp.csproj).");
            using var reader = new StreamReader(stream);

            var suggestions = new Dictionary<(int, int), CatalogJobModel>();
            reader.ReadLine();

            while (reader.ReadLine() is { } line)
            {
                var columns = line.Split(';');
                if (columns.Length < 8)
                    continue;

                var job = new CatalogJobModel
                {
                    IdJob = int.Parse(columns[0], CultureInfo.InvariantCulture),
                    IdType = int.Parse(columns[1], CultureInfo.InvariantCulture),
                    Title = columns[3],
                    IntervalTimeInMinutes = int.Parse(columns[4], CultureInfo.InvariantCulture),
                    RunningDailyAt = columns[5].Length > 0 ? TimeOnly.Parse(columns[5], CultureInfo.InvariantCulture) : null,
                    Queue = columns[6].Length > 0 ? columns[6] : "default",
                    IsOnDemand = columns[7] == "1"
                };

                suggestions[(job.IdJob, job.IdType)] = job;
            }

            return suggestions;
        }
    }
}
