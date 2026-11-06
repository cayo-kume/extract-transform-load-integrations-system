using Hosts.Scheduling;
using Hosts.Scheduling.RecurringJobs;
using Infrastructure.AfterSale.DependencyInjection;
using Infrastructure.DataWarehouse.DependencyInjection;
using Infrastructure.LinxCommerce.DependencyInjection;
using Infrastructure.LinxMicrovix.Outbound.DependencyInjection;
using Infrastructure.Movidesk.DependencyInjection;
using Infrastructure.Shared;
using Infrastructure.Totvs.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl.Matchers;
using Worker.Scheduling;

namespace Worker.Extensions
{
    public static class ServicesExtensions
    {
        /// <summary>
        /// os mesmos módulos dos hosts WebApi e WebJob (src/Backend/Hosts/Worker/.docs/PLANO-HOST-WORKER.md, passo 3) e o executor de
        /// jobs. Um módulo novo entra aqui e nos outros dois hosts, junto com o mapa em Hosts.Scheduling
        /// </summary>
        public static HostApplicationBuilder AddServices(this HostApplicationBuilder builder)
        {
            builder.Services.AddScopedSharedServices();

            builder.Services.AddScopedLinxCommerceServices(builder.Configuration);
            builder.Services.AddScopedLinxMicrovixServices(builder.Configuration);
            builder.Services.AddScopedAfterSaleServices(builder.Configuration);
            builder.Services.AddScopedMovideskServices(builder.Configuration);
            builder.Services.AddScopedTotvsServices(builder.Configuration);
            builder.Services.AddScopedDataWarehouseServices(builder.Configuration);

            builder.Services.AddScheduledJobRunner();

            return builder;
        }

        /// <summary>
        /// o Quartz (passo 3): AdoJobStore em cluster no banco das integrações, pool de threads, limiar de misfire e o
        /// SchedulerRefreshJob, que monta a agenda a partir de auditing.Jobs na subida e a cada RefreshIntervalInMinutes. Com
        /// Quartz:Enabled = false, nada do Quartz é registrado e o host sobe sem agendar nada
        /// </summary>
        public static HostApplicationBuilder AddQuartzService(this HostApplicationBuilder builder)
        {
            var section = builder.Configuration.GetSection(QuartzSettings.SectionName);
            var settings = section.Get<QuartzSettings>() ?? new QuartzSettings();

            builder.Services.AddOptions<QuartzSettings>().Bind(section).ValidateOnStart();
            builder.Services.AddSingleton<IValidateOptions<QuartzSettings>, QuartzSettingsValidator>();

            if (!settings.Enabled)
                return builder;

            // monta os mapas já na subida: IdJob repetido ou método sem [JobVersion] (§4.5) impedem o host de subir
            _ = RecurringJobMap.Definitions;

            QuartzSettingsValidator.TryFindTimeZone(settings.TimeZone, out var timeZone);
            var connectionString = builder.Configuration.GetConnectionString(settings.Storage.ConnectionStringName) ?? "";

            builder.Services.AddSingleton<JobSkipCounter>();

            builder.Services.AddQuartz(q =>
            {
                q.SchedulerName = settings.SchedulerName;
                q.SchedulerId = InstanceIdOf(settings, builder.Configuration);
                q.MisfireThreshold = TimeSpan.FromMinutes(settings.Misfire.ThresholdInMinutes);
                q.UseDefaultThreadPool(pool => pool.MaxConcurrency = settings.Threads.MaxConcurrency);

                q.UsePersistentStore(store =>
                {
                    // o JobDataMap só com texto (§3.3)
                    store.UseProperties = true;
                    store.PerformSchemaValidation = true;
                    store.UseSqlServer(sql =>
                    {
                        sql.ConnectionString = connectionString;
                        sql.TablePrefix = settings.Storage.TablePrefix;
                    });
                    store.UseSystemTextJsonSerializer();

                    if (settings.Storage.Clustered)
                        store.UseClustering(cluster => cluster.CheckinInterval = TimeSpan.FromSeconds(settings.Storage.ClusterCheckinIntervalInSeconds));
                });

                // descartes pelo misfire, no mesmo contador dos pulos (§5.3, item 2)
                q.AddTriggerListener<MisfireListener>(GroupMatcher<TriggerKey>.GroupEquals(QuartzJobRunner.Group));

                // agenda a partir de auditing.Jobs (§4.4): a cada RefreshIntervalInMinutes e, já na subida, uma vez
                q.AddJob<SchedulerRefreshJob>(SchedulerRefreshJob.Key, job => job.StoreDurably().WithDescription("Agenda: releitura de auditing.Jobs"));

                q.AddTrigger(trigger => trigger
                    .ForJob(SchedulerRefreshJob.Key)
                    .WithIdentity("SchedulerRefresh", SchedulerRefreshJob.Key.Group)
                    .WithCronSchedule(JobCron.ToQuartz(JobCron.FromInterval(settings.Schedule.RefreshIntervalInMinutes)!), c => c.InTimeZone(timeZone).WithMisfireHandlingInstructionDoNothing()));

                q.AddTrigger(trigger => trigger
                    .ForJob(SchedulerRefreshJob.Key)
                    .WithIdentity("SchedulerRefresh-Startup", SchedulerRefreshJob.Key.Group)
                    .StartNow());
            });

            // um tipo de host de agenda por banco (.docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md, 4.2): o heartbeat em
            // auditing.SchedulingHosts sobe antes do Quartz e, sem banco, diz isso no log antes da mensagem de schema do Quartz
            builder.Services.AddSchedulingHostRegistration("Quartz");

            // na parada, espera as execuções em andamento terminarem, até Shutdown:TimeoutInMinutes
            builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
            builder.Services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(settings.Shutdown.TimeoutInMinutes));

            return builder;
        }

        /// <summary>
        /// Quartz:InstanceId ou, vazio, &lt;máquina&gt;-&lt;Service:Name&gt; (o nome do serviço do Windows, EtlWorker por padrão)
        /// </summary>
        public static string InstanceIdOf(QuartzSettings settings, IConfiguration configuration) =>
            settings.InstanceId is { Length: > 0 } instanceId
                ? instanceId
                : $"{Environment.MachineName}-{(configuration["Service:Name"] is { Length: > 0 } name ? name : "EtlWorker")}";

        /// <summary>
        /// serviço do Windows (passo 6.1): o nome vem de Service:Name, o mesmo do perfil de publicação, e vira também a origem no Log
        /// de Eventos (só Warning para cima, pela seção Logging:EventLog). Fora de um serviço (dotnet run, console), não faz nada
        /// </summary>
        public static HostApplicationBuilder AddWindowsServiceHosting(this HostApplicationBuilder builder)
        {
            var serviceName = builder.Configuration["Service:Name"] is { Length: > 0 } name ? name : "EtlWorker";

            builder.Services.AddWindowsService(options => options.ServiceName = serviceName);

            return builder;
        }
    }
}
