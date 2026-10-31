using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Execução de SQL livre da tela Executar Querys, separada do IMonitorRepository porque terá conexão e usuário de
    /// banco próprios (etl_query). Até a fase 5 do plano (docs/PLANO-WEBAPP.md), a única implementação é o MockQueryRunner.
    /// </summary>
    public interface IQueryRunner
    {
        /// <summary>
        /// Linhas do SELECT que vão para a tela; o resto só entra na contagem.
        /// </summary>
        const int MaxRows = 1000;

        /// <summary>
        /// Prazo para "Confirmar" ou "Desfazer" uma alteração; sem resposta, a transação é desfeita.
        /// </summary>
        static readonly TimeSpan ConfirmationTimeout = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Bancos do servidor em que o usuário entra, sem os de sistema.
        /// </summary>
        Task<IReadOnlyList<string>> GetDatabasesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Executa o SQL no banco. Comando que altera algo fica numa transação aberta (QueryResult.TransactionId), que
        /// precisa de CommitAsync ou RollbackAsync.
        /// </summary>
        Task<QueryResult> ExecuteAsync(string database, string sql, CancellationToken cancellationToken = default);

        Task CommitAsync(Guid transactionId, CancellationToken cancellationToken = default);

        Task RollbackAsync(Guid transactionId, CancellationToken cancellationToken = default);
    }
}
