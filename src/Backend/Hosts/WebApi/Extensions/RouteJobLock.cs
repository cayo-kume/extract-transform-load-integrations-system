using Application.Shared.Abstractions.Handlers.Exceptions;
using Application.Shared.Abstractions.Services;
using Application.Shared.Exceptions;
using Hosts.Scheduling.RecurringJobs;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text.Json;

namespace WebApi.Extensions
{
    /// <summary>
    /// a trava dos agendadores nas rotas (docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md, 4.1; no teste A6, o disparo manual rodou junto
    /// com o agendado e leu a API em dobro). Quando a rota abre o log de um job da agenda (LoggerService.AddLog), pede a trava do
    /// job (a ConcurrencyKey do mapa, que o agendador segura junto com a da empresa) sem esperar. Ocupada: JobAlreadyRunningException,
    /// que vira 409 sem log em auditing. A trava fica com a requisição e é solta no fim do escopo. Jobs fora do mapa (consulta por
    /// chave, escritas) não pedem trava. Só vale nas requisições (RouteJobLockFilter liga): as execuções do Hangfire, no mesmo
    /// processo, já seguram a trava pelo SkipConcurrentExecutionFilter
    /// </summary>
    public sealed class RouteJobLock : IJobExecutionGuard, IDisposable
    {
        private readonly IJobLock _jobLock;
        private IDisposable? _handle;

        public RouteJobLock(IJobLock jobLock) =>
            _jobLock = jobLock;

        public bool Enabled { get; set; }

        public void BeforeLogStart(Enum job, string company)
        {
            if (!Enabled || _handle is not null)
                return;

            var idJob = Convert.ToInt32(job);

            if (!RecurringJobMap.Definitions.TryGetValue(idJob, out var definition))
                return;

            _handle = _jobLock.TryAcquire(definition.ConcurrencyKey)
                ?? throw new JobAlreadyRunningException(idJob, definition.ConcurrencyKey);
        }

        public void Dispose()
        {
            _handle?.Dispose();
            _handle = null;
        }
    }

    /// <summary>
    /// liga a RouteJobLock da requisição: só as actions dos controllers pedem a trava pelo LoggerService
    /// </summary>
    public class RouteJobLockFilter : IActionFilter
    {
        private readonly RouteJobLock _routeJobLock;

        public RouteJobLockFilter(RouteJobLock routeJobLock) =>
            _routeJobLock = routeJobLock;

        public void OnActionExecuting(ActionExecutingContext context) =>
            _routeJobLock.Enabled = true;

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }

    /// <summary>
    /// JobAlreadyRunningException: 409, com o motivo, e nada em auditing (a execução não começou)
    /// </summary>
    public class JobAlreadyRunningExceptionHandler : IExceptionHandler
    {
        public bool CanHandle(Exception exception) =>
            exception is JobAlreadyRunningException;

        public async Task HandleAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                Title = exception.Message,
                ErrorCode = "JOB_ALREADY_RUNNING"
            }));
        }
    }
}
