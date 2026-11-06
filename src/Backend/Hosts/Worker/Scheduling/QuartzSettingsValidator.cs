using Hosts.Scheduling;
using Microsoft.Extensions.Options;

namespace Worker.Scheduling
{
    /// <summary>
    /// valida a seção Quartz só quando Enabled = true (src/Backend/Hosts/Worker/.docs/PLANO-HOST-WORKER.md §3.1): com um parâmetro
    /// faltando, o host não sobe e a mensagem diz qual
    /// </summary>
    public class QuartzSettingsValidator : IValidateOptions<QuartzSettings>
    {
        private readonly IConfiguration _configuration;

        public QuartzSettingsValidator(IConfiguration configuration) =>
            _configuration = configuration;

        public ValidateOptionsResult Validate(string? name, QuartzSettings settings)
        {
            if (!settings.Enabled)
                return ValidateOptionsResult.Skip;

            var failures = new List<string>();

            if (string.IsNullOrWhiteSpace(settings.SchedulerName))
                failures.Add("Quartz:SchedulerName está vazio.");

            if (!TryFindTimeZone(settings.TimeZone, out _))
                failures.Add($"Quartz:TimeZone '{settings.TimeZone}' não é um fuso conhecido.");

            if (string.IsNullOrWhiteSpace(settings.Storage.ConnectionStringName))
                failures.Add("Quartz:Storage:ConnectionStringName está vazio.");
            else if (string.IsNullOrWhiteSpace(_configuration.GetConnectionString(settings.Storage.ConnectionStringName)))
                failures.Add($"ConnectionStrings:{settings.Storage.ConnectionStringName} (Quartz:Storage:ConnectionStringName) não existe ou está vazia.");

            if (string.IsNullOrWhiteSpace(settings.Storage.TablePrefix))
                failures.Add("Quartz:Storage:TablePrefix está vazio.");

            Positive(failures, "Quartz:Storage:ClusterCheckinIntervalInSeconds", settings.Storage.ClusterCheckinIntervalInSeconds);
            Positive(failures, "Quartz:Threads:MaxConcurrency", settings.Threads.MaxConcurrency);
            Positive(failures, "Quartz:Misfire:ThresholdInMinutes", settings.Misfire.ThresholdInMinutes);

            // o SchedulerRefreshJob também vira cron (§4.4)
            if (JobCron.FromInterval(settings.Schedule.RefreshIntervalInMinutes) is null)
                failures.Add($"Quartz:Schedule:RefreshIntervalInMinutes ({settings.Schedule.RefreshIntervalInMinutes}) tem que dividir a hora (1 a 30) ou ser horas que dividem o dia.");

            Positive(failures, "Quartz:Concurrency:MaxConsecutiveSkips", settings.Concurrency.MaxConsecutiveSkips);
            Positive(failures, "Quartz:Shutdown:TimeoutInMinutes", settings.Shutdown.TimeoutInMinutes);

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
