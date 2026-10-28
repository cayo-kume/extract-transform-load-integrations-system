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

        // grupo das escritas de bônus na configuração (Totvs:Writes:Bonus:Enabled, PE5): separado das outras escritas da Person por ter efeito
        // financeiro, e desligado até a autenticação dos endpoints de inbound (T2)
        private const string WritesGroup = "Bonus";

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostBonusDueDate(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Bonus.BonusDueDate body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostBonusDueDate);

            var key = $"{body.branchCode}|{body.personCode}|{body.movementDate:yyyy-MM-dd}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (!(await _totvsService.GetBranchCodes(company)).Contains(body.branchCode!.Value))
                    return _totvsService.BranchNotFound(doc_company, key, [body.branchCode.Value.ToString()]);

                // gera crédito e não tem estorno: repetir duplicaria o crédito, daí a trava (T5)
                return await _totvsService.Write(company, EnumJob.TotvsPostBonusDueDate, user_name, key, HttpMethod.Post, "person/v2/bonus-due-date", body,
                    "Confira o saldo de bônus do cliente (POST ListBalanceBonus) antes de repetir.", repeatLock: true);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostBonusConsume(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Bonus.BonusConsumption body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostBonusConsume);

            var key = $"{body.branchCode}|{(object?)body.personCode ?? body.personCpfCnpj}|{body.usedValue}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (!(await _totvsService.GetBranchCodes(company)).Contains(body.branchCode!.Value))
                    return _totvsService.BranchNotFound(doc_company, key, [body.branchCode.Value.ToString()]);

                // PE4: com o documento da pessoa, o saldo da loja é conferido antes (o list-balance-bonus pede o documento, não o código); a Totvs
                // continua sendo a palavra final. A leitura fica em TotvsRequests como as outras
                if (!String.IsNullOrEmpty(body.personCpfCnpj))
                {
                    var balance = await _totvsService.ReadOnDemand(company, EnumJob.TotvsPostListBalanceBonus, user_name, key, HttpMethod.Post, "person/v2/list-balance-bonus",
                        JsonConvert.SerializeObject(new { personCpf = body.personCpfCnpj, branchList = new[] { new { branchCode = body.branchCode } } }), "balanceBonus");
                    decimal? available = null;

                    if (balance.data is { ValueKind: System.Text.Json.JsonValueKind.Object } data && data.TryGetProperty("balanceBonus", out var list) && list.ValueKind == System.Text.Json.JsonValueKind.Array)
                        foreach (var item in list.EnumerateArray())
                            if (item.TryGetProperty("branchCode", out var branch) && branch.ValueKind == System.Text.Json.JsonValueKind.Number && branch.GetInt32() == body.branchCode
                                && item.TryGetProperty("balanceValue", out var value) && value.ValueKind == System.Text.Json.JsonValueKind.Number)
                                available = value.GetDecimal();

                    if (balance.statusCode == 404 || (balance.success && (available ?? 0) < body.usedValue))
                    {
                        var refused = $"Saldo de bônus da pessoa na loja {body.branchCode} ({available ?? 0:0.00}) menor que o consumo pedido ({body.usedValue:0.00}).";
                        _logger.AddMessage(refused);

                        // a escrita não foi chamada: 422, não 502
                        return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = refused };
                    }
                }

                // tira saldo e não tem estorno: repetir debitaria de novo, daí a trava (T5)
                return await _totvsService.Write(company, EnumJob.TotvsPostBonusConsume, user_name, key, HttpMethod.Post, "person/v2/bonus-consume", body,
                    "Confira o saldo de bônus do cliente (POST ListBalanceBonus) antes de repetir.", repeatLock: true);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }
    }
}
