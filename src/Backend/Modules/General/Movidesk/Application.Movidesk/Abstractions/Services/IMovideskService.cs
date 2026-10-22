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
        /// registra uma chamada sob demanda à API da Movidesk em general.MovideskRequests (a rota nunca leva o token)
        /// </summary>
        public void SaveRequest(EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess, string? requestBody = null);
    }
}
