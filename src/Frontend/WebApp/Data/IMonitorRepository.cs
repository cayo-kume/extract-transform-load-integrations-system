using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Leitura das tabelas auditing.* para o painel. Até a fase 5 do plano (docs/PLANO-WEBAPP.md), a única implementação é o
    /// MockMonitorRepository; depois, o MonitorRepository lê o banco principal com Dapper.
    /// </summary>
    public interface IMonitorRepository
    {
        /// <summary>
        /// Uma linha por aplicação: jobs, execuções, registros, avisos e erros no período.
        /// </summary>
        Task<IReadOnlyList<AppSummaryModel>> GetAppSummariesAsync(Period period, CancellationToken cancellationToken = default);

        /// <summary>
        /// Jobs com a última execução e a saúde, filtrados pela aplicação (já resolvida, ver JobModel.IdApp) e pelo tipo.
        /// </summary>
        Task<IReadOnlyList<JobModel>> GetJobsAsync(int? idApp, int? idType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Execuções de um job no período, da mais recente para a mais antiga. page começa em 0.
        /// </summary>
        Task<PagedResult<ExecutionModel>> GetExecutionsAsync(int idJob, int idType, Period period, int page, int pageSize, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MessageModel>> GetMessagesAsync(Guid execution, CancellationToken cancellationToken = default);

        Task<PagedResult<RecordModel>> GetRecordsAsync(Guid execution, int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Última execução de cada job no período, da mais recente para a mais antiga, até top linhas.
        /// </summary>
        Task<IReadOnlyList<HeartbeatModel>> GetHeartbeatAsync(Period period, int top, CancellationToken cancellationToken = default);

        /// <summary>
        /// Mensagens a partir de um nível no período (Logs de Erros), com a aplicação, o job e o início da execução.
        /// </summary>
        Task<PagedResult<MessageModel>> GetMessagesByLevelAsync(Period period, MessageLevel minLevel, int? idApp, int page, int pageSize, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TypeModel>> GetTypesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Credenciais e parâmetros globais do módulo, para o diálogo de configurações da Visão geral. Somente leitura
        /// na fase 1; credencial vem mascarada.
        /// </summary>
        Task<IReadOnlyList<ModuleSettingModel>> GetModuleSettingsAsync(int idApp, CancellationToken cancellationToken = default);

        /// <summary>
        /// Liga ou desliga um job. Na implementação real, é um UPDATE em auditing.Jobs.IsActive na linha da empresa.
        /// </summary>
        Task SetJobActiveAsync(int idJob, int idType, bool isActive, CancellationToken cancellationToken = default);

        /// <summary>
        /// Liga ou desliga todos os jobs de um módulo (o chip "Ativa" do card).
        /// </summary>
        Task SetModuleActiveAsync(int idApp, bool isActive, CancellationToken cancellationToken = default);

        /// <summary>
        /// Imagem própria da aplicação para a empresa logada. A imagem padrão do módulo é a que não tem registro aqui.
        /// </summary>
        Task SetAppImageAsync(int idApp, string contentType, byte[] data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove a imagem própria; o card volta para a imagem padrão do módulo.
        /// </summary>
        Task RemoveAppImageAsync(int idApp, CancellationToken cancellationToken = default);
    }
}
