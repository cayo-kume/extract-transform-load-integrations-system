using Application.Shared.Abstractions.Services;

namespace Application.Shared.Services
{
    public class CompanyContext : ICompanyContext
    {
        public string? Company { get; private set; }

        public void SetCompany(string company)
        {
            if (string.IsNullOrWhiteSpace(company) || !company.All(char.IsAsciiDigit))
                throw new ArgumentException($"Empresa inválida: '{company}'. Use o CNPJ da holding, só dígitos.", nameof(company));

            // uma execução é de uma empresa só: o log aberto já usa a primeira
            if (Company is not null && Company != company)
                throw new InvalidOperationException($"A execução já é da empresa {Company}; não pode passar para {company}.");

            Company = company;
        }
    }
}
