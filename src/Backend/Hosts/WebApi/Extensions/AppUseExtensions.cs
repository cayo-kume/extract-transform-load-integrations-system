using Application.Shared.Middlewares;
using Hangfire;
using Microsoft.Extensions.Options;
using WebApi.Scheduling;

namespace WebApi.Extensions
{
    public static class AppUseExtensions
    {
        public static IApplicationBuilder UseApplication(this IApplicationBuilder app)
        {
            app.UseMiddleware<APIExceptionHandlingMiddleware>();
            app.UseSwagger(c => { c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0; });
            //#if RELEASE
            app.UseSwaggerUI();
            //#else
            //app.UseSwaggerUI(c =>
            //{
            //    c.OAuthClientId("");
            //    c.OAuthClientSecret("");
            //});
            //#endif
            app.UseHttpsRedirection();
            app.UseAuthorization();

            var hangfireSettings = app.ApplicationServices.GetRequiredService<IOptions<HangfireSettings>>().Value;

            if (hangfireSettings.Enabled)
                app.UseHangfire(hangfireSettings);

            return app;
        }

        /// <summary>
        /// dashboard com Basic Authentication (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §3.2) e os jobs de manutenção do próprio
        /// host; só roda com Hangfire:Enabled = true
        /// </summary>
        private static IApplicationBuilder UseHangfire(this IApplicationBuilder app, HangfireSettings settings)
        {
            app.UseHangfireDashboard(settings.Dashboard.Path, new DashboardOptions
            {
                DashboardTitle = "WebApi - Hangfire",
                Authorization = [new DashboardBasicAuthorizationFilter(settings.Dashboard.UserName!, settings.Dashboard.Password!)],
                IsReadOnlyFunc = _ => settings.Dashboard.ReadOnly,
                StatsPollingInterval = settings.Dashboard.StatsPollingIntervalInSeconds * 1000
            });

            HangfireSettingsValidator.TryFindTimeZone(settings.TimeZone, out var timeZone);

            var recurringJobManager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();

            // jobs de manutenção do host, fora de auditing.Jobs: só somem com Hangfire:Enabled = false
            // limpeza da tela de Failed (§3.4)
            recurringJobManager.AddOrUpdate<FailedJobsCleanupJob>(
                FailedJobsCleanupJob.RecurringJobId,
                job => job.Run(),
                FailedJobsCleanupJob.Cron,
                new RecurringJobOptions { TimeZone = timeZone });

            // agenda a partir de auditing.Jobs (§4.4): a cada RefreshIntervalInMinutes e, já na subida, uma vez
            recurringJobManager.AddOrUpdate<SchedulerRefreshJob>(
                SchedulerRefreshJob.RecurringJobId,
                job => job.Run(),
                JobCron.FromInterval(settings.Schedule.RefreshIntervalInMinutes)!,
                new RecurringJobOptions { TimeZone = timeZone });

            recurringJobManager.Trigger(SchedulerRefreshJob.RecurringJobId);

            return app;
        }
    }
}
