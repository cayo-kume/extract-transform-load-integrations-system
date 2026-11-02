using Domain.Movidesk.Enums;

namespace Application.Movidesk.Abstractions.Services
{
    /// <summary>
    /// métodos em comum usados pelos services do módulo (tickets, pessoas, serviços)
    /// </summary>
    public interface IMovideskService
    {
        /// <summary>
        /// horário do lote: o registro e todas as filhas gravam o mesmo integrated_or_updated_at (condição de exclusão do merge)
        /// </summary>
        public DateTime GetBatchTime();

        /// <summary>
        /// início da carga inicial (banco vazio), limitado por InitialLoad:MaxMonths (IInitialLoad). <paramref name="since"/> em
        /// UTC, como as datas da API. Sem a chave, devolve <paramref name="since"/> como veio
        /// </summary>
        /// <param name="since">início que a leitura usaria sem o teto, em UTC</param>
        /// <returns></returns>
        public DateTime GetInitialLoad(DateTime since);

        /// <summary>
        /// registra uma chamada sob demanda à API da Movidesk em movidesk.MovideskRequests (a rota nunca leva o token)
        /// </summary>
        public void SaveRequest(EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess, string? requestBody = null);
    }
}
