using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Worker.Scheduling
{
    /// <summary>
    /// um host de agenda por vez (src/Backend/Hosts/Worker/docs/PLANO-HOST-WORKER.md §6.2): na subida, confere se o Hangfire do WebApi
    /// está agendando no mesmo banco (servidor com heartbeat recente em HangFire.Server) e, se estiver, avisa no log. Não impede a
    /// subida: a trava já impede que um job rode junto nos dois, e um servidor do Hangfire que caiu sem se desregistrar deixaria o
    /// Worker parado à toa. Os WebJobs não são conferidos (precisaria da API da Azure)
    /// </summary>
    public class SchedulingHostGuard : IHostedService
    {
        // o schema padrão do Hangfire (Hangfire:Storage:SchemaName no WebApi); com outro schema, a conferência não acha a tabela
        private const string HangfireServerTable = "[HangFire].[Server]";
        private const int HeartbeatMinutes = 5;

        private readonly IConfiguration _configuration;
        private readonly IOptions<QuartzSettings> _settings;
        private readonly ILogger<SchedulingHostGuard> _logger;

        public SchedulingHostGuard(IConfiguration configuration, IOptions<QuartzSettings> settings, ILogger<SchedulingHostGuard> logger) =>
            (_configuration, _settings, _logger) = (configuration, settings, logger);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new SqlConnection(_configuration.GetConnectionString(_settings.Value.Storage.ConnectionStringName));
                await connection.OpenAsync(cancellationToken);

                await using var command = connection.CreateCommand();
                command.CommandText =
                    $"IF OBJECT_ID(N'{HangfireServerTable}', N'U') IS NULL SELECT 0 " +
                    $"ELSE SELECT COUNT(*) FROM {HangfireServerTable} WHERE LastHeartbeat > DATEADD(minute, -{HeartbeatMinutes}, SYSUTCDATETIME())";

                var servers = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));

                if (servers > 0)
                    _logger.LogWarning(
                        "{Servers} servidor(es) do Hangfire com heartbeat nos últimos {Minutes} minutos neste banco: o WebApi está agendando junto com o Worker. " +
                        "Use um host de agenda por vez (Hangfire:Enabled = false no WebApi); a trava impede que um job rode junto nos dois.",
                        servers, HeartbeatMinutes);
            }
            catch (Exception ex)
            {
                // a conferência é só um aviso: não derruba a subida
                _logger.LogWarning(ex, "Não foi possível conferir se o Hangfire está agendando no mesmo banco.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
