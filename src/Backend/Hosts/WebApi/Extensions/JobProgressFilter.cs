using Domain.Shared.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Extensions
{
    /// <summary>
    /// barra de progresso no console para o job chamado pela rota (JobProgress), registrada só em Development: acompanha a execução
    /// local pelo Swagger. Os jobs do Hangfire não passam por aqui e não desenham nada
    /// </summary>
    public class JobProgressFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var job = context.ActionDescriptor is ControllerActionDescriptor action
                ? $"{action.ControllerName}.{action.ActionName}"
                : context.ActionDescriptor.DisplayName ?? "job";

            using (JobProgress.Begin(job))
                await next();
        }
    }
}
