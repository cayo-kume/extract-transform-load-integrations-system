using Application.Shared.Abstractions.Services;
using Domain.Shared.Abstractions.Repositorys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Extensions
{
    /// <summary>
    /// empresa da execução numa rota (.docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md), informada ao ICompanyContext da
    /// requisição antes da action: o {docCompany} da rota ou, sem ele, o cabeçalho X-Company (CNPJ da empresa, auditing.Companies).
    /// Com os dois, precisam ser a mesma empresa. A holding sai da empresa em auditing.Companies. Sem nenhum, 400 (o job não
    /// teria empresa para o log), salvo as rotas com [AllowWithoutCompany]
    /// </summary>
    public class CompanyContextFilter : IAsyncActionFilter
    {
        public const string CompanyHeader = "X-Company";

        private const string DocCompanyRouteValue = "docCompany";

        private readonly ICompanyContext _companyContext;
        private readonly ICompanyRepository _companyRepository;

        public CompanyContextFilter(ICompanyContext companyContext, ICompanyRepository companyRepository) =>
            (_companyContext, _companyRepository) = (companyContext, companyRepository);

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var header = context.HttpContext.Request.Headers[CompanyHeader].ToString();
            var company = string.IsNullOrWhiteSpace(header) ? null : header.Trim();

            if (company is not null && !company.All(char.IsAsciiDigit))
            {
                context.Result = new BadRequestObjectResult($"{CompanyHeader} '{company}' inválido: use o CNPJ da empresa, só dígitos.");
                return;
            }

            if (context.RouteData.Values.TryGetValue(DocCompanyRouteValue, out var value) && value?.ToString() is { Length: > 0 } docCompany)
            {
                if (company is not null && company != docCompany)
                {
                    context.Result = new BadRequestObjectResult($"{CompanyHeader} {company} diferente do CNPJ {docCompany} da rota.");
                    return;
                }

                company = docCompany;
            }

            if (company is not null)
            {
                var holder = await _companyRepository.GetHolder(company);

                if (holder is null)
                {
                    context.Result = new BadRequestObjectResult($"CNPJ {company} não cadastrado em auditing.Companies.");
                    return;
                }

                _companyContext.SetCompany(holder, company);
            }
            // sem empresa, o job recusaria abrir o log (500 no meio da execução): 400 antes, salvo as rotas que não rodam job
            else if (!context.ActionDescriptor.EndpointMetadata.OfType<AllowWithoutCompanyAttribute>().Any())
            {
                context.Result = new BadRequestObjectResult($"Informe o cabeçalho {CompanyHeader} (CNPJ da empresa, só dígitos) ou use a rota com {{docCompany}}.");
                return;
            }

            await next();
        }
    }
}
