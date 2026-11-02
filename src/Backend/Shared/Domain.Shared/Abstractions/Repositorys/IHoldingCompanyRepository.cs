namespace Domain.Shared.Abstractions.Repositorys
{
    /// <summary>
    /// de/para monitor.HoldingCompanies: CNPJ dos parâmetros (doc_company) → empresa do painel (docs/PLANO-TELA-CADASTRO-EMPRESA.md,
    /// 4.2.1)
    /// </summary>
    public interface IHoldingCompanyRepository
    {
        /// <summary>
        /// holding do CNPJ; null quando ele não está em monitor.HoldingCompanies
        /// </summary>
        public Task<string?> GetHolding(string docCompany);
    }
}
