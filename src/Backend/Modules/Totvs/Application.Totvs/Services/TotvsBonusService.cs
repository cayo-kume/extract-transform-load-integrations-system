using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Totvs.Enums;
using Newtonsoft.Json;

namespace Application.Totvs.Services
{
    /// <summary>
    /// bônus da API Person (fase PE-4, docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.3, PE9): separado do TotvsPeopleService para isolar o
    /// efeito financeiro das escritas da PE-7. A leitura usa a base T11 do ITotvsService
    /// </summary>
    public class TotvsBonusService : ITotvsBonusService
    {
        private readonly ILoggerService _logger;
        private readonly ITotvsService _totvsService;

        public TotvsBonusService(ILoggerService logger, ITotvsService totvsService) =>
            (_logger, _totvsService) = (logger, totvsService);

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostListBalanceBonus(string user_name, string doc_company, Domain.Totvs.Dtos.Reads.People.Bonus.ListBalanceBonus body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostListBalanceBonus);

            var key = $"{body.personCpf}|{String.Join(",", (body.branchList ?? []).Select(x => x.branchCode?.ToString() ?? x.branchCnpj))}";

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // as lojas precisam ser da empresa (422 sem chamar a API): pelo código em TotvsBranches, ou pelo CNPJ
                var companyBranches = await _totvsService.GetBranchCodes(company);
                var outside = new List<string>();

                foreach (var branch in body.branchList ?? [])
                {
                    if (branch.branchCode is int code && !companyBranches.Contains(code))
                        outside.Add(code.ToString());
                    else if (!String.IsNullOrEmpty(branch.branchCnpj) && await _totvsService.GetBranchCodeByCnpj(company, branch.branchCnpj) is null)
                        outside.Add(branch.branchCnpj);
                }

                if (outside.Count > 0)
                    return _totvsService.BranchNotFound(doc_company, key, outside);

                // o token comum da empresa basta: com a loja na requisição do token o resultado é o mesmo (testado), então o token por loja da PE3
                // não foi preciso. Saldo do momento: não grava
                return await _totvsService.ReadOnDemand(company, EnumJob.TotvsPostListBalanceBonus, user_name, key, HttpMethod.Post, "person/v2/list-balance-bonus", JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "balanceBonus");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }
    }
}
