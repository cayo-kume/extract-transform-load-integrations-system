using Domain.Shared.Abstractions.Repositorys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Hosts.Scheduling
{
    /// <summary>
    /// heartbeat da instância em auditing.SchedulingHosts a cada 30 s (SchedulingHostRegistry). O primeiro é feito na subida, antes
    /// do agendador (registrado antes dele): o Hangfire roda os atrasados logo na subida, e o Quartz valida as tabelas, que sem
    /// banco dá uma mensagem que leva a procurar tabela faltando. Na parada, a linha sai na hora, para outro tipo assumir
    /// </summary>
    public class SchedulingHostHeartbeat : BackgroundService
    {
        private readonly SchedulingHostRegistry _registry;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SchedulingHostHeartbeat> _logger;

        // a falha do banco é registrada uma vez, não a cada 30 s
        private bool _failing;

        public SchedulingHostHeartbeat(SchedulingHostRegistry registry, IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<SchedulingHostHeartbeat> logger) =>
            (_registry, _scopeFactory, _configuration, _logger) = (registry, scopeFactory, configuration, logger);

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await Beat(startup: true);
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(SchedulingHostRegistry.HeartbeatInterval);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                    await Beat(startup: false);
            }
            catch (OperationCanceledException)
            {
                // parada do host
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<IJobScheduleRepository>().RemoveSchedulingHost(_registry.InstanceId);
            }
            catch (Exception ex)
            {
                // a linha expira sozinha em 2 min
                _logger.LogWarning(ex, "Não foi possível tirar a instância {Instance} de auditing.SchedulingHosts; ela expira em {Expire}.", _registry.InstanceName, SchedulingHostRegistry.ExpireAfter);
            }
        }

        private async Task Beat(bool startup)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var types = await scope.ServiceProvider.GetRequiredService<IJobScheduleRepository>()
                    .HeartbeatSchedulingHost(_registry.InstanceId, _registry.HostType, _registry.InstanceName, SchedulingHostRegistry.ExpireAfter);

                if (_failing)
                    _logger.LogInformation("Conexão com o banco de volta: heartbeat de {HostType} em auditing.SchedulingHosts.", _registry.HostType);

                _failing = false;

                var changed = _registry.Update(types, out var owner);

                if (_registry.AllowOtherHostTypes)
                {
                    if (startup && types.Count > 1)
                        _logger.LogWarning("Scheduling:AllowOtherHostTypes = true: {HostType} agenda junto com {Others} neste banco. A trava impede que um job rode junto, mas os jobs rodam duas vezes por ciclo.",
                            _registry.HostType, string.Join(", ", types.Where(t => t.HostType != _registry.HostType).Select(t => t.HostType)));

                    return;
                }

                if (!_registry.IsActive && (startup || changed))
                    _logger.LogWarning("{Owner} agenda neste banco desde {Since:yyyy-MM-dd HH:mm} UTC ({Instances} instância(s)): este host ({HostType}) não executa os jobs da agenda até ele sair " +
                        "(parada, ou {Expire} sem heartbeat em auditing.SchedulingHosts). Para os dois agendarem de propósito: Scheduling:AllowOtherHostTypes = true.",
                        owner!.HostType, owner.StartedAt, owner.Instances, _registry.HostType, SchedulingHostRegistry.ExpireAfter);
                else if (_registry.IsActive && changed)
                    _logger.LogWarning("Nenhum outro tipo de host agenda neste banco: {HostType} passa a executar os jobs da agenda.", _registry.HostType);
            }
            catch (Exception ex)
            {
                if (_failing)
                    return;

                _failing = true;

                if (Find<SqlException>(ex) is { Number: 208 })
                {
                    _logger.LogWarning("A tabela auditing.SchedulingHosts não existe neste banco: rode o migrator do Shared. Até lá, {HostType} agenda sem conferir os outros hosts.", _registry.HostType);
                    return;
                }

                // sem banco, nenhum job roda: o estado de agenda fica como estava. A mensagem diz o que é, porque a do Quartz
                // ("Database schema validation failed ... created the database tables") leva a procurar tabela faltando
                var (server, database) = Describe(_configuration.GetConnectionString("AuditingConnection") ?? _configuration.GetConnectionString("Connection"));

                _logger.LogError(ex, "Sem conexão com o banco (servidor {Server}, banco {Database}) para o heartbeat de {HostType} em auditing.SchedulingHosts: {Message}. " +
                    "Sem banco, o Quartz cai na subida com 'Database schema validation failed', que neste caso não é tabela faltando, e o Hangfire repete a conexão sozinho.",
                    server, database, _registry.HostType, ex.InnerException?.Message ?? ex.Message);
            }
        }

        private static T? Find<T>(Exception? ex) where T : Exception =>
            ex is null ? null : ex as T ?? Find<T>(ex.InnerException);

        private static (string? Server, string? Database) Describe(string? connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                return (builder.DataSource, builder.InitialCatalog);
            }
            catch (ArgumentException)
            {
                return (null, null);
            }
        }
    }
}
