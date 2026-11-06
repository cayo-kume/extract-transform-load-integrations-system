using Application.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Extensions
{
    /// <summary>
    /// rotas do LinxMicrovix: quando a Linx recusa o método em todas as chamadas da execução ("o usuário não possui acesso ao
    /// comando": a chave não tem o método liberado) e nada foi gravado, 403 com o motivo, em vez de 200 com 0 registros e o
    /// erro só no log. Com alguma loja gravada, a resposta continua 200 e as recusas ficam no log
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LinxMicrovixAccessFilterAttribute : Attribute, IAsyncActionFilter
    {
        // trecho da resposta da Linx para método que a chave não tem liberado (testado no B2CConsultaClientesContatos)
        private const string AccessDenied = "não possui acesso ao comando";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executed = await next();

            if (executed.Exception is not null || executed.Result is not OkObjectResult)
                return;

            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerService>();
            var refused = logger.ErrorMessages.Where(x => x.Contains(AccessDenied, StringComparison.OrdinalIgnoreCase)).ToList();

            if (refused.Count == 0 || logger.RecordsCount > 0)
                return;

            var method = (context.ActionDescriptor as ControllerActionDescriptor)?.ActionName ?? "o método";
            var company = context.HttpContext.RequestServices.GetRequiredService<ICompanyContext>().Company;

            executed.Result = new ObjectResult(
                $"A Linx recusou o {method} em {refused.Count} chamada(s) da empresa {company}: a chave não tem esse método liberado " +
                "(\"Não foi possível encontrar o comando especificado ou o usuário não possui acesso ao comando\"). Peça à Linx a " +
                "liberação para o usuário do web service; nada foi gravado.")
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
