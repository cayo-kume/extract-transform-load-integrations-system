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
        /// código da loja da empresa pelo CNPJ, em totvs.TotvsBranches; null quando o CNPJ não é de uma loja dela. Sem cache
        /// </summary>
        public Task<int?> GetBranchCodeByCnpj(Company company, string cnpj);

        /// <summary>
        /// escritas ligadas por configuração (T4 do docs/PLANO-TOTVS-METODOS-FALTANTES.md): Totvs:Writes:&lt;grupo&gt;:Enabled,
        /// desligada quando a chave não existe
        /// </summary>
        public bool IsWriteEnabled(string group);

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
        /// <summary>
        /// leitura sob demanda sem persistência (T11, docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1): chama a Totvs com uma nova tentativa
        /// (401, 429, 502/503/504), registra em totvs.TotvsRequests o corpo enviado e o recebido e devolve o JSON em Result.data. O 404 e o
        /// 400 com KeyNotFound, NotFound ou PathParamNotFound viram 404; com emptyListProperty, essa lista vazia também
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> ReadOnDemand(Company company, Domain.Totvs.Enums.EnumJob job, string user_name, string key, HttpMethod method, string route, string? requestBody = null, string? emptyListProperty = null);

        /// <summary>
        /// Result da empresa não cadastrada em totvs.TotvsParameters (422 sem chamar a API)
        /// </summary>
        public Domain.Totvs.Dtos.Writes.Result CompanyNotFound(string doc_company, string key);

        /// <summary>
        /// Result da loja fora de totvs.TotvsBranches da empresa (422 sem chamar a API)
        /// </summary>
        public Domain.Totvs.Dtos.Writes.Result BranchNotFound(string doc_company, string key, IEnumerable<string> branches);

        /// <summary>
        /// escrita na Totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §1.2): serializa o corpo sem os nulos, chama a rota com uma nova tentativa
        /// só em 401 e 429 (nunca em 5xx: a escrita pode ter sido feita), registra em totvs.TotvsRequests o corpo enviado, o recebido e o status
        /// e devolve o Result. Com repeatLock, recusa com 409 (sem chamar a API) a mesma rota + chave + corpo dos últimos 10 minutos que a Totvs
        /// aceitou ou que ficou sem resposta certa (T5). checkHint completa a mensagem quando não dá para saber se a Totvs gravou
        /// </summary>
        public Task<Domain.Totvs.Dtos.Writes.Result> Write(Company company, Domain.Totvs.Enums.EnumJob job, string user_name, string key, HttpMethod method, string route, object? body, string checkHint, bool repeatLock = false);

        /// <summary>
        /// Result da escrita desligada por configuração (T4: 422 sem chamar a API)
        /// </summary>
        public Domain.Totvs.Dtos.Writes.Result WriteDisabled(string group, string key);

        public void SaveRequest(Domain.Totvs.Enums.EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess, string? requestBody = null, int? statusCode = null);
    }
}
