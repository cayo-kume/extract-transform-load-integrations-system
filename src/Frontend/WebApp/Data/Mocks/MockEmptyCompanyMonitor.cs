using WebApp.Models;

namespace WebApp.Data.Mocks
{
    /// <summary>
    /// O painel do mock para uma empresa que não é a da configuração: o MockTimeline só gera execuções para ela, então as
    /// outras empresas da holding aparecem sem jobs, execuções nem mensagens. O que não depende da empresa (tipos, parâmetros,
    /// hosts, filas e imagens) vem do MockMonitorRepository.
    /// </summary>
    internal class MockEmptyCompanyMonitor(MockMonitorRepository inner, string company) : IMonitorRepository
    {
        public string Company { get; } = company;

        public IMonitorRepository For(string other) => inner.For(other);

        public Task<IReadOnlyList<AppSummaryModel>> GetAppSummariesAsync(Period period, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<AppSummaryModel>>([]);

        public Task<IReadOnlyList<AppBucketsModel>> GetExecutionBucketsAsync(Period period, int buckets, bool previousWindow = false, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<AppBucketsModel>>([]);

        public Task<IReadOnlyList<JobModel>> GetJobsAsync(int? idApp, int? idType, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JobModel>>([]);

        public Task<PagedResult<ExecutionModel>> GetExecutionsAsync(int idJob, int idType, Period period, int page, int pageSize, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PagedResult<ExecutionModel>());

        public Task<ExecutionModel?> GetExecutionAsync(Guid execution, CancellationToken cancellationToken = default) => Task.FromResult<ExecutionModel?>(null);

        public Task<IReadOnlyList<MessageModel>> GetMessagesAsync(Guid execution, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<MessageModel>>([]);

        public Task<PagedResult<RecordModel>> GetRecordsAsync(Guid execution, int page, int pageSize, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PagedResult<RecordModel>());

        public Task<IReadOnlyList<HeartbeatModel>> GetHeartbeatAsync(Period period, int top, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<HeartbeatModel>>([]);

        public Task<PagedResult<MessageModel>> GetMessagesByLevelAsync(MessageQuery query, int page, int pageSize, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PagedResult<MessageModel>());

        public Task<IReadOnlyList<MessageJobSummaryModel>> GetMessageJobsAsync(MessageQuery query, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<MessageJobSummaryModel>>([]);

        public Task<IReadOnlyList<MessageGroupModel>> GetMessageGroupsAsync(MessageQuery query, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<MessageGroupModel>>([]);

        public Task<IReadOnlyList<TypeModel>> GetTypesAsync(CancellationToken cancellationToken = default) => inner.GetTypesAsync(cancellationToken);

        public Task<IReadOnlyList<ModuleSettingModel>> GetModuleSettingsAsync(int idApp, CancellationToken cancellationToken = default) =>
            inner.GetModuleSettingsAsync(idApp, cancellationToken);

        // sem jobs desta empresa no mock, as escritas de job não têm onde gravar
        public Task SetJobActiveAsync(int idJob, int idType, bool isActive, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task SetJobScheduleAsync(int idJob, int idType, int intervalTimeInMinutes, TimeOnly? runningDailyAt, string queue, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<IReadOnlyList<string>> GetQueuesAsync(CancellationToken cancellationToken = default) => inner.GetQueuesAsync(cancellationToken);

        public Task<IReadOnlyList<SchedulingHostModel>> GetSchedulingHostsAsync(CancellationToken cancellationToken = default) =>
            inner.GetSchedulingHostsAsync(cancellationToken);

        public Task SetModuleActiveAsync(int idApp, bool isActive, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task AcknowledgeAsync(int idJob, int idType, string? note, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task RemoveAcknowledgementAsync(int idJob, int idType, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task RequestRunAsync(int idJob, int idType, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task SetAppImageAsync(int idApp, string contentType, byte[] data, CancellationToken cancellationToken = default) =>
            inner.SetAppImageAsync(idApp, contentType, data, cancellationToken);

        public Task RemoveAppImageAsync(int idApp, CancellationToken cancellationToken = default) => inner.RemoveAppImageAsync(idApp, cancellationToken);
    }
}
