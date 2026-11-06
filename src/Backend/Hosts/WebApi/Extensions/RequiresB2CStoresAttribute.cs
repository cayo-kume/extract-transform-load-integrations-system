using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.LinxMicrovix.Outbound.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Extensions
{
    /// <summary>
    /// rota do LinxMicrovix B2C que lê as lojas de linxmicrovix.B2CConsultaEmpresas (ILinxMicrovixService.GetStores): com a
    /// empresa da execução (CompanyContextFilter) com chave B2C e sem nenhuma loja gravada, 409 antes do job, em vez de uma
    /// execução que termina com 0 registros e só o aviso no log. Empresa sem chave B2C segue como antes (o job a pula)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresB2CStoresAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var services = context.HttpContext.RequestServices;
            var company = services.GetRequiredService<ICompanyContext>().Company;
            var linxMicrovixService = services.GetRequiredService<ILinxMicrovixService>();

            var parameters = (await linxMicrovixService.GetCompanys()).FirstOrDefault(x => x.doc_company == company);

            if (parameters?.b2c_chave is not null && (await linxMicrovixService.GetStores(parameters, EnumApi.B2C)).Count == 0)
            {
                context.Result = new ConflictObjectResult(
                    $"As empresas (lojas) B2C de {company} ainda não estão cadastradas em linxmicrovix.B2CConsultaEmpresas: " +
                    "carregue o B2CConsultaEmpresas antes deste método.");
                return;
            }

            await next();
        }
    }
}
