using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.LinxMicrovix.Outbound.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Extensions
{
    /// <summary>
    /// rota do LinxMicrovix ERP que lê as lojas de linxmicrovix.LinxGrupoLojas (ILinxMicrovixService.GetStores): com a empresa
    /// da execução (CompanyContextFilter) com chave ERP e sem nenhuma loja gravada, 409 antes do job, em vez de uma execução
    /// que termina com 0 registros e só o aviso no log (como o RequiresB2CStores no B2C). Empresa sem chave ERP segue como
    /// antes (o job a pula)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresErpStoresAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var services = context.HttpContext.RequestServices;
            var company = services.GetRequiredService<ICompanyContext>().Company;
            var linxMicrovixService = services.GetRequiredService<ILinxMicrovixService>();

            var parameters = (await linxMicrovixService.GetCompanys()).FirstOrDefault(x => x.doc_company == company);

            if (parameters?.erp_chave is not null && (await linxMicrovixService.GetStores(parameters, EnumApi.Erp)).Count == 0)
            {
                // o LinxGrupoLojas responde sem lojas, sem erro, quando o grupo cadastrado não é o do cliente (testado com o
                // número do portal no grupo)
                context.Result = new ConflictObjectResult(
                    $"As lojas do ERP de {company} ainda não estão cadastradas em linxmicrovix.LinxGrupoLojas: carregue o " +
                    "LinxGrupoLojas antes deste método. Se ele terminar sem lojas, confira o grupo em LinxMicrovixParameters " +
                    "(vazio traz todas as lojas da chave).");
                return;
            }

            await next();
        }
    }
}
