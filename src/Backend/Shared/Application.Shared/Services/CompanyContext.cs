using Application.Shared.Abstractions.Services;

namespace Application.Shared.Services
{
    public class CompanyContext : ICompanyContext
    {
        public string? Holder { get; private set; }

        public string? Company { get; private set; }

        public void SetCompany(string holder, string company)
        {
            if (string.IsNullOrWhiteSpace(holder) || !holder.All(char.IsAsciiDigit))
                throw new ArgumentException($"Holding inválida: '{holder}'. Use o CNPJ da holding, só dígitos.", nameof(holder));

            if (string.IsNullOrWhiteSpace(company) || !company.All(char.IsAsciiDigit))
                throw new ArgumentException($"Empresa inválida: '{company}'. Use o CNPJ da empresa, só dígitos.", nameof(company));

            // uma execução é de uma empresa só: o log aberto já usa a primeira
            if (Company is not null && (Company != company || Holder != holder))
                throw new InvalidOperationException($"A execução já é da empresa {Company} (holding {Holder}); não pode passar para {company} (holding {holder}).");

            Holder = holder;
            Company = company;
        }
    }
}
