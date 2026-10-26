using Domain.Totvs.Entities.Parameters;

namespace Application.Totvs.Abstractions.Services
{
    /// <summary>
    /// métodos em comum usados pelos services do módulo; as buscas na API ficam no próprio método de cada service
    /// </summary>
    public interface ITotvsService
    {
        /// <summary>
        /// horário do lote: o registro e todas as filhas gravam o mesmo integrated_or_updated_at (condição de exclusão do merge)
        /// </summary>
        public DateTime GetBatchTime();

        /// <summary>
        /// empresas de totvs.TotvsParameters (hoje só a holding Newbloomers), em cache
        /// </summary>
        public Task<IEnumerable<Company>> GetCompanys();

        /// <summary>
        /// uma empresa de totvs.TotvsParameters pelo CNPJ (rotas sob demanda); null quando não está cadastrada
        /// </summary>
        public Task<Company?> GetCompany(string doc_company);

        /// <summary>
        /// códigos das lojas da empresa em totvs.TotvsBranches (gravadas pelo GetBranchesList), para o branchCodeList das buscas.
        /// Sem cache: uma loja nova vale já na próxima execução
        /// </summary>
        public Task<List<int>> GetBranchCodes(Company company);

        /// <summary>
        /// lojas de uma marca em totvs.TotvsBranches: as de mesma raiz de CNPJ da matriz (empresa base dos produtos). Sem cache
        /// </summary>
        public Task<List<int>> GetBrandBranchCodes(Company company, int baseBranchCode);

        /// <summary>
        /// access_token do authorization/v2/token da empresa, em cache até perto do expires_in.
        /// null quando o login falha (a mensagem vai para o log da execução)
        /// </summary>
        public Task<string?> GetToken(Company company);

        /// <summary>
        /// tira o token da empresa do cache (ex.: depois de um 401), para o próximo GetToken fazer um novo login
        /// </summary>
        public void RemoveToken(Company company);

        /// <summary>
        /// registra uma chamada sob demanda à API da Totvs em totvs.TotvsRequests (a rota nunca leva o token; as buscas são POST,
        /// então o filtro enviado vai em request_body)
        /// </summary>
        public void SaveRequest(Domain.Totvs.Enums.EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess, string? requestBody = null);
    }
}
