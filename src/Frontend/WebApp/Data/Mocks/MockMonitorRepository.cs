using WebApp.Models;

namespace WebApp.Data.Mocks
{
    /// <summary>
    /// Implementação falsa do IMonitorRepository: os jobs reais do MockCatalog e execuções geradas pelo MockTimeline.
    /// Registrada como singleton, porque não guarda estado por usuário e mantém um cache curto dos resumos.
    /// </summary>
    public class MockMonitorRepository : IMonitorRepository
    {
        private static readonly IReadOnlyList<TypeModel> Types =
        [
            new() { IdType = 1, JobType = "Procedure" },
            new() { IdType = 2, JobType = "Integração" }
        ];

        private readonly MockTimeline _timeline = new();
        private readonly object _cacheLock = new();
        private (Period Period, DateTime Minute, IReadOnlyList<AppSummaryModel> Result)? _summariesCache;

        public async Task<IReadOnlyList<AppSummaryModel>> GetAppSummariesAsync(Period period, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.Now;
            var minute = now.AddTicks(-(now.Ticks % TimeSpan.TicksPerMinute));

            lock (_cacheLock)
            {
                if (_summariesCache is { } cache && cache.Period == period && cache.Minute == minute)
                    return cache.Result;
            }

            var from = now - period.ToTimeSpan();
            var result = _timeline.Apps.Values
                .OrderBy(x => x.Title)
                .Select(app =>
                {
                    var jobs = _timeline.Jobs.Where(x => x.IdApp == app.IdParent).ToList();
                    var executions = jobs.SelectMany(x => _timeline.Between(x, from, now, now)).ToList();

                    return new AppSummaryModel
                    {
                        IdParent = app.IdParent,
                        Title = app.Title,
                        Description = app.Description,
                        Integrations = jobs.Count(x => x.Catalog.IdType == 2),
                        Procedures = jobs.Count(x => x.Catalog.IdType == 1),
                        ActiveJobs = jobs.Count(x => x.IsActive),
                        Executions = executions.Count,
                        FailedExecutions = executions.Count(x => x.Errors > 0),
                        Records = executions.Sum(x => x.Records),
                        Warnings = executions.Sum(x => x.Warnings),
                        Errors = executions.Sum(x => x.Errors)
                    };
                })
                .ToList();

            lock (_cacheLock)
                _summariesCache = (period, minute, result);

            return result;
        }

        public async Task<IReadOnlyList<JobModel>> GetJobsAsync(int? idApp, int? idType, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.Now;

            return _timeline.Jobs
                .Where(x => idApp is null || x.IdApp == idApp)
                .Where(x => idType is null || x.Catalog.IdType == idType)
                .Select(x =>
                {
                    var last = _timeline.Last(x, now);

                    return new JobModel
                    {
                        IdParent = x.Catalog.IdParent,
                        IdApp = x.IdApp,
                        IdJob = x.Catalog.IdJob,
                        IdType = x.Catalog.IdType,
                        JobType = JobType(x.Catalog.IdType),
                        Title = x.Catalog.Title,
                        Description = x.Catalog.Description,
                        IntervalTimeInMinutes = x.Catalog.IntervalTimeInMinutes,
                        IsActive = x.IsActive,
                        RunningDailyAt = x.Catalog.RunningDailyAt,
                        Queue = x.Catalog.Queue,
                        LastExecution = last is null ? null : ToModel(last),
                        Health = _timeline.Health(x, last, now)
                    };
                })
                .ToList();
        }

        public async Task<PagedResult<ExecutionModel>> GetExecutionsAsync(int idJob, int idType, Period period, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var job = _timeline.Find(idJob, idType);
            if (job is null)
                return new PagedResult<ExecutionModel>();

            var now = DateTime.Now;
            var executions = _timeline.Between(job, now - period.ToTimeSpan(), now, now).ToList();
            executions.Reverse();

            return new PagedResult<ExecutionModel>
            {
                Items = executions.Skip(page * pageSize).Take(pageSize).Select(ToModel).ToList(),
                TotalCount = executions.Count
            };
        }

        public async Task<IReadOnlyList<MessageModel>> GetMessagesAsync(Guid execution, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var found = _timeline.Decode(execution, DateTime.Now);
            return found is null ? [] : MockTexts.Messages(found, AppTitle(found.Job));
        }

        public async Task<PagedResult<RecordModel>> GetRecordsAsync(Guid execution, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var found = _timeline.Decode(execution, DateTime.Now);
            if (found is null)
                return new PagedResult<RecordModel>();

            return new PagedResult<RecordModel>
            {
                Items = Enumerable.Range(page * pageSize, Math.Max(0, Math.Min(pageSize, found.Records - page * pageSize)))
                    .Select(i => MockTexts.Record(found, i))
                    .ToList(),
                TotalCount = found.Records
            };
        }

        public async Task<IReadOnlyList<HeartbeatModel>> GetHeartbeatAsync(Period period, int top, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.Now;
            var from = now - period.ToTimeSpan();

            return _timeline.Jobs
                .Select(x => (Job: x, Last: _timeline.Last(x, now)))
                .Where(x => x.Last is not null && x.Last.StartDate >= from)
                .OrderByDescending(x => x.Last!.StartDate)
                .Take(top)
                .Select(x => new HeartbeatModel
                {
                    IdApp = x.Job.IdApp,
                    AppTitle = AppTitle(x.Job),
                    IdJob = x.Job.Catalog.IdJob,
                    IdType = x.Job.Catalog.IdType,
                    JobType = JobType(x.Job.Catalog.IdType),
                    JobTitle = x.Job.Catalog.Title,
                    StartDate = x.Last!.StartDate,
                    EndDate = x.Last.EndDate,
                    Records = x.Last.Records,
                    Health = _timeline.Health(x.Job, x.Last, now) ?? JobHealth.Ok,
                    HasWarnings = x.Last.Warnings > 0
                })
                .ToList();
        }

        public async Task<PagedResult<MessageModel>> GetMessagesByLevelAsync(Period period, MessageLevel minLevel, int? idApp, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);

            var now = DateTime.Now;
            var from = now - period.ToTimeSpan();

            var executions = _timeline.Jobs
                .Where(x => idApp is null || x.IdApp == idApp)
                .SelectMany(x => _timeline.Between(x, from, now, now))
                .Where(x => x.Errors > 0 || (minLevel <= MessageLevel.Warning && x.Warnings > 0))
                .OrderByDescending(x => x.StartDate)
                .ToList();

            // cada execução tem um erro ou de 1 a 3 avisos: conta as mensagens sem gerar o texto de todas
            int Count(MockExecution x) => (minLevel <= MessageLevel.Warning ? x.Warnings : 0) + x.Errors;

            var skip = page * pageSize;
            var items = new List<MessageModel>();

            foreach (var execution in executions)
            {
                var count = Count(execution);
                if (skip >= count)
                {
                    skip -= count;
                    continue;
                }

                var appTitle = AppTitle(execution.Job);
                foreach (var message in MockTexts.Messages(execution, appTitle).Where(x => x.Level >= minLevel).Skip(skip))
                {
                    message.IdApp = execution.Job.IdApp;
                    message.AppTitle = appTitle;
                    message.JobTitle = execution.Job.Catalog.Title;
                    message.ExecutionStartDate = execution.StartDate;
                    items.Add(message);

                    if (items.Count == pageSize)
                        break;
                }

                skip = 0;
                if (items.Count == pageSize)
                    break;
            }

            return new PagedResult<MessageModel>
            {
                Items = items,
                TotalCount = executions.Sum(Count)
            };
        }

        public async Task<IReadOnlyList<TypeModel>> GetTypesAsync(CancellationToken cancellationToken = default)
        {
            await Delay(cancellationToken);
            return Types;
        }

        private string AppTitle(MockJob job) => _timeline.Apps[job.IdApp].Title;

        private static string? JobType(int idType) => Types.FirstOrDefault(x => x.IdType == idType)?.JobType;

        private static ExecutionModel ToModel(MockExecution execution) => new()
        {
            IdExecution = (long)(MockTimeline.Hash(execution.Job, execution.K, 0) % 900_000_000_000) + 1,
            IdJob = execution.Job.Catalog.IdJob,
            IdType = execution.Job.Catalog.IdType,
            Execution = execution.Execution,
            StartDate = execution.StartDate,
            EndDate = execution.EndDate,
            Records = execution.Records,
            Warnings = execution.Warnings,
            Errors = execution.Errors
        };

        // o mesmo atraso que o banco terá, para o SpaceLoading aparecer
        private static Task Delay(CancellationToken cancellationToken) =>
            Task.Delay(Random.Shared.Next(150, 400), cancellationToken);
    }
}
