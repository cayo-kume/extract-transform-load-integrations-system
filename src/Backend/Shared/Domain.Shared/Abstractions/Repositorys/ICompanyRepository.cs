namespace Domain.Shared.Abstractions.Repositorys
{
    /// <summary>
    /// empresas das holdings em auditing.Companies: uma empresa é de uma holding só, e a holding da execução sai da empresa
    /// (docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md)
    /// </summary>
    public interface ICompanyRepository
    {
        /// <summary>
        /// holding da empresa; null quando ela não está em auditing.Companies
        /// </summary>
        public Task<string?> GetHolder(string company);
    }
}
