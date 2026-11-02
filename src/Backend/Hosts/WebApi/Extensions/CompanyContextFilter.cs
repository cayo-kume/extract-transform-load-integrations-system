using Application.Shared.Abstractions.Services;
using Domain.Shared.Abstractions.Repositorys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Extensions
{
    /// <summary>
    /// empresa da execução numa rota (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.4), informada ao ICompanyContext da requisição antes da
    /// action: o cabeçalho X-Company (CNPJ da holding, monitor.Company) ou, sem ele, a holding do {docCompany} da rota em
    /// monitor.HoldingCompanies. Com os dois, o docCompany precisa ser da holding informada. Sem nenhum, a rota segue sem empresa e
    /// o LoggerService recusa abrir o log
    /// </summary>
    public class CompanyContextFilter : IAsyncActionFilter
    {
        public const string CompanyHeader = "X-Company";

        private const string DocCompanyRouteValue = "docCompany";

        private readonly ICompanyContext _companyContext;
        private readonly IHoldingCompanyRepository _holdingCompanyRepository;

        public CompanyContextFilter(ICompanyContext companyContext, IHoldingCompanyRepository holdingCompanyRepository) =>
            (_companyContext, _holdingCompanyRepository) = (companyContext, holdingCompanyRepository);

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var header = context.HttpContext.Request.Headers[CompanyHeader].ToString();
            var company = string.IsNullOrWhiteSpace(header) ? null : header.Trim();

            if (company is not null && !company.All(char.IsAsciiDigit))
            {
                context.Result = new BadRequestObjectResult($"{CompanyHeader} '{company}' inválido: use o CNPJ da holding, só dígitos.");
                return;
            }

            if (context.RouteData.Values.TryGetValue(DocCompanyRouteValue, out var value) && value?.ToString() is { Length: > 0 } docCompany)
            {
                var holding = await _holdingCompanyRepository.GetHolding(docCompany);

                if (holding is null)
                {
                    context.Result = new BadRequestObjectResult($"CNPJ {docCompany} sem holding em monitor.HoldingCompanies.");
                    return;
                }

                if (company is not null && company != holding)
                {
                    context.Result = new BadRequestObjectResult($"CNPJ {docCompany} é da holding {holding}, não de {company}.");
                    return;
                }

                company = holding;
            }

            if (company is not null)
                _companyContext.SetCompany(company);

            await next();
        }
    }
}
