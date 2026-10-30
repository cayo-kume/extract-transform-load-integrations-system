using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace WebApi.Scheduling
{
    /// <summary>
    /// valida a seção Hangfire só quando Enabled = true (§3.1): com o Hangfire ligado e um parâmetro faltando, o host não sobe e a
    /// mensagem diz qual. Com Enabled = false, nada é validado
    /// </summary>
    public partial class HangfireSettingsValidator : IValidateOptions<HangfireSettings>
    {
        private readonly IConfiguration _configuration;

        public HangfireSettingsValidator(IConfiguration configuration) =>
            _configuration = configuration;

        // o Hangfire só aceita filas com minúsculas, dígitos, _ e -
        [GeneratedRegex("^[a-z0-9_-]+$")]
        private static partial Regex QueueNameRegex();

        public ValidateOptionsResult Validate(string? name, HangfireSettings settings)
        {
            if (!settings.Enabled)
                return ValidateOptionsResult.Skip;

            var failures = new List<string>();

            if (!TryFindTimeZone(settings.TimeZone, out _))
                failures.Add($"Hangfire:TimeZone '{settings.TimeZone}' não é um fuso conhecido.");

            if (string.IsNullOrWhiteSpace(settings.Storage.ConnectionStringName))
                failures.Add("Hangfire:Storage:ConnectionStringName está vazio.");
            else if (string.IsNullOrWhiteSpace(_configuration.GetConnectionString(settings.Storage.ConnectionStringName)))
                failures.Add($"ConnectionStrings:{settings.Storage.ConnectionStringName} (Hangfire:Storage:ConnectionStringName) não existe ou está vazia.");

            if (string.IsNullOrWhiteSpace(settings.Storage.SchemaName))
                failures.Add("Hangfire:Storage:SchemaName está vazio.");

            Positive(failures, "Hangfire:Storage:CommandBatchMaxTimeoutInMinutes", settings.Storage.CommandBatchMaxTimeoutInMinutes);
            Positive(failures, "Hangfire:Storage:SlidingInvisibilityTimeoutInMinutes", settings.Storage.SlidingInvisibilityTimeoutInMinutes);
            Positive(failures, "Hangfire:Storage:JobExpirationTimeoutInMinutes", settings.Storage.JobExpirationTimeoutInMinutes);
            Positive(failures, "Hangfire:Storage:QueuePollIntervalInSeconds", settings.Storage.QueuePollIntervalInSeconds);

            if (settings.Server.Queues is null || settings.Server.Queues.Length == 0)
                failures.Add("Hangfire:Server:Queues está vazio.");
            else
                foreach (var queue in settings.Server.Queues.Where(q => q is null || !QueueNameRegex().IsMatch(q)))
                    failures.Add($"Hangfire:Server:Queues tem a fila '{queue}': use só minúsculas, dígitos, _ e -.");

            Positive(failures, "Hangfire:Server:WorkerCount", settings.Server.WorkerCount);
            // o SchedulerRefresh também vira cron (§4.4)
            if (JobCron.FromInterval(settings.Schedule.RefreshIntervalInMinutes) is null)
                failures.Add($"Hangfire:Schedule:RefreshIntervalInMinutes ({settings.Schedule.RefreshIntervalInMinutes}) tem que dividir a hora (1 a 30) ou ser horas que dividem o dia.");

            Positive(failures, "Hangfire:Concurrency:MaxConsecutiveSkips", settings.Concurrency.MaxConsecutiveSkips);
            Positive(failures, "Hangfire:Maintenance:FailedJobsRetentionInDays", settings.Maintenance.FailedJobsRetentionInDays);
            Positive(failures, "Hangfire:Dashboard:StatsPollingIntervalInSeconds", settings.Dashboard.StatsPollingIntervalInSeconds);

            if (string.IsNullOrWhiteSpace(settings.Dashboard.Path) || !settings.Dashboard.Path.StartsWith('/'))
                failures.Add("Hangfire:Dashboard:Path tem que começar com '/'.");

            // sem credencial o host não sobe: não existe dashboard aberto por esquecimento (§3.2)
            if (string.IsNullOrWhiteSpace(settings.Dashboard.UserName) || string.IsNullOrWhiteSpace(settings.Dashboard.Password))
                failures.Add("Hangfire:Dashboard:UserName e Hangfire:Dashboard:Password são obrigatórios (variável de ambiente ou user-secrets, nunca o appsettings.json).");

            return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
        }

        public static bool TryFindTimeZone(string? id, out TimeZoneInfo timeZone)
        {
            timeZone = TimeZoneInfo.Utc;

            if (string.IsNullOrWhiteSpace(id))
                return false;

            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(id);
                return true;
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                return false;
            }
        }

        private static void Positive(List<string> failures, string key, int value)
        {
            if (value <= 0)
                failures.Add($"{key} tem que ser maior que zero.");
        }
    }
}
