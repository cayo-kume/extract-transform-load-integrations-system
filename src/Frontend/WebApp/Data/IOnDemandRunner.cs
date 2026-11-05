using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Integrações sob demanda do host WebApi, chamadas pela tela Sob Demanda. Até a fase 5, a única implementação é o
    /// MockOnDemandRunner (respostas simuladas); depois, o catálogo sai do swagger.json do host e a chamada é HTTP, do servidor
    /// do WebApp para o WebApi (docs/PLANO-WEBAPP.md, passo 11.5).
    /// </summary>
    public interface IOnDemandRunner
    {
        Task<IReadOnlyList<OnDemandOperation>> GetOperationsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Chama a operação para a empresa (CNPJ: a credencial e os jobs são dela) com os valores dos parâmetros (pelo nome)
        /// e o corpo JSON, quando ela espera um.
        /// </summary>
        Task<OnDemandResult> ExecuteAsync(OnDemandOperation operation, string company, IReadOnlyDictionary<string, string?> values, string? body, CancellationToken cancellationToken = default);

        /// <summary>
        /// Últimas chamadas da operação feitas pelo painel, da mais recente para a mais antiga.
        /// </summary>
        Task<IReadOnlyList<OnDemandCall>> GetCallsAsync(int idJob, int top, CancellationToken cancellationToken = default);
    }
}
