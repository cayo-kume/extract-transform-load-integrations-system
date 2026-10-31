using WebApp.Models;

namespace WebApp.Data.Mocks
{
    /// <summary>
    /// Gera as execuções dos jobs do MockCatalog sem guardar nada: cada execução é a posição k na agenda do job, e tudo o que
    /// ela tem (duração, registros, avisos e erros) sai de um hash de (IdJob, IdType, k). Assim os dados são os mesmos a cada
    /// carga da página, e as execuções novas aparecem conforme o relógio anda.
    /// </summary>
    internal sealed class MockTimeline
    {
        private static readonly DateTime Origin = new(2026, 1, 1);
        private const int OnDemandSlotsPerDay = 4;

        public IReadOnlyList<MockJob> Jobs { get; }
        public IReadOnlyDictionary<int, CatalogApp> Apps { get; }

        private readonly Dictionary<(int IdJob, int IdType), MockJob> _byKey;

        public MockTimeline()
        {
            Apps = MockCatalog.Apps.ToDictionary(x => x.IdParent);

            var appByJob = MockCatalog.Jobs
                .Where(x => Apps.ContainsKey(x.IdParent))
                .GroupBy(x => x.IdJob)
                .ToDictionary(x => x.Key, x => x.First().IdParent);

            Jobs = MockCatalog.Jobs
                .Select(x => new MockJob(x, Apps.ContainsKey(x.IdParent) ? x.IdParent : appByJob[x.IdParent]))
                .ToList();

            _byKey = Jobs.ToDictionary(x => (x.Catalog.IdJob, x.Catalog.IdType));
        }

        public MockJob? Find(int idJob, int idType) =>
            _byKey.TryGetValue((idJob, idType), out var job) ? job : null;

        /// <summary>
        /// Execuções do job que começaram entre from e to (no máximo agora), em ordem de início.
        /// </summary>
        public IEnumerable<MockExecution> Between(MockJob job, DateTime from, DateTime to, DateTime now)
        {
            if (!job.IsActive)
                yield break;

            var cutoff = Cutoff(job, now);
            if (to > cutoff)
                to = cutoff;

            foreach (var k in Slots(job, from, to))
                yield return Build(job, k, now);
        }

        /// <summary>
        /// Última execução do job até agora, olhando até 30 dias para trás.
        /// </summary>
        public MockExecution? Last(MockJob job, DateTime now) =>
            LastSlot(job, now) is long k ? Build(job, k, now) : null;

        public MockExecution? Decode(Guid execution, DateTime now)
        {
            var bytes = execution.ToByteArray();
            var job = Find(BitConverter.ToInt32(bytes, 0), BitConverter.ToInt32(bytes, 4));
            if (job is null)
                return null;

            var result = Build(job, BitConverter.ToInt64(bytes, 8), now);
            return result.StartDate <= Cutoff(job, now) ? result : null;
        }

        private static long? LastSlot(MockJob job, DateTime now)
        {
            if (!job.IsActive)
                return null;

            var cutoff = Cutoff(job, now);
            var window = job.Catalog.IntervalTimeInMinutes > 0 && job.Catalog.RunningDailyAt is null
                ? TimeSpan.FromMinutes(job.Catalog.IntervalTimeInMinutes * 2)
                : TimeSpan.FromDays(2);

            while (window <= TimeSpan.FromDays(30))
            {
                var last = Slots(job, cutoff - window, cutoff).Select(k => (long?)k).LastOrDefault();
                if (last is not null)
                    return last;

                window *= 4;
            }

            return null;
        }

        public JobHealth? Health(MockJob job, MockExecution? last, DateTime now)
        {
            if (!job.IsActive)
                return null;

            if (last is null)
                return job.Catalog.IntervalTimeInMinutes > 0 ? JobHealth.Late : null;

            if (last.EndDate is null)
                return JobHealth.Running;

            if (last.Errors > 0)
                return JobHealth.Error;

            if (job.Catalog.IntervalTimeInMinutes > 0 && now - last.StartDate > TimeSpan.FromMinutes(job.Catalog.IntervalTimeInMinutes))
                return JobHealth.Late;

            return JobHealth.Ok;
        }

        private static DateTime Cutoff(MockJob job, DateTime now) =>
            job.Profile == MockProfile.Late ? now - job.LateBy : now;

        private static IEnumerable<long> Slots(MockJob job, DateTime from, DateTime to)
        {
            if (to < from)
                yield break;

            var catalog = job.Catalog;

            if (catalog.RunningDailyAt is TimeOnly at)
            {
                var first = (long)Math.Ceiling((from - Origin - at.ToTimeSpan()).TotalDays);
                var last = (long)Math.Floor((to - Origin - at.ToTimeSpan()).TotalDays);
                for (var k = Math.Max(first, 0); k <= last; k++)
                    yield return k;
            }
            else if (catalog.IntervalTimeInMinutes > 0)
            {
                var interval = catalog.IntervalTimeInMinutes;
                var first = (long)Math.Ceiling(((from - Origin).TotalMinutes - job.OffsetMinutes) / interval);
                var last = (long)Math.Floor(((to - Origin).TotalMinutes - job.OffsetMinutes) / interval);
                for (var k = Math.Max(first, 0); k <= last; k++)
                    yield return k;
            }
            else
            {
                for (var day = (long)(from.Date - Origin).TotalDays; day <= (long)(to.Date - Origin).TotalDays; day++)
                {
                    foreach (var k in OnDemandSlots(job, day))
                    {
                        var start = OnDemandStart(job, k);
                        if (start >= from && start <= to)
                            yield return k;
                    }
                }
            }
        }

        private static IEnumerable<long> OnDemandSlots(MockJob job, long day)
        {
            var count = (int)(Hash(job, day, 11) % 3);
            return Enumerable.Range(0, count)
                .Select(i => day * OnDemandSlotsPerDay + i)
                .OrderBy(k => OnDemandStart(job, k));
        }

        private static DateTime OnDemandStart(MockJob job, long k) =>
            Origin.AddDays(k / OnDemandSlotsPerDay).AddMinutes(8 * 60 + (double)(Hash(job, k, 12) % (11 * 60)));

        private static DateTime StartOf(MockJob job, long k)
        {
            var catalog = job.Catalog;

            if (catalog.RunningDailyAt is TimeOnly at)
                return Origin.AddDays(k) + at.ToTimeSpan();

            if (catalog.IntervalTimeInMinutes > 0)
                return Origin.AddMinutes(k * catalog.IntervalTimeInMinutes + job.OffsetMinutes);

            return OnDemandStart(job, k);
        }

        private MockExecution Build(MockJob job, long k, DateTime now)
        {
            var catalog = job.Catalog;
            var start = StartOf(job, k);
            var end = start.AddSeconds(DurationSeconds(job, k));
            var running = end > now;

            var isProcedure = catalog.IdType == 1;
            var failingLast = job.Profile == MockProfile.FailingLast && LastSlot(job, now) == k;
            var errors = running ? 0 : failingLast || Hash(job, k, 2) % 1000 < (ulong)(isProcedure ? 8 : 25) ? 1 : 0;
            var warningRate = isProcedure ? 0UL : job.Profile == MockProfile.Noisy ? 45UL : 6UL;
            var warnings = running || errors > 0 ? 0 : Hash(job, k, 3) % 100 < warningRate ? 1 + (int)(Hash(job, k, 4) % 3) : 0;
            var records = running || (errors > 0 && Hash(job, k, 5) % 2 == 0) || Hash(job, k, 6) % 100 < 15
                ? 0
                : (int)(Hash(job, k, 7) % (ulong)job.MaxRecords);

            return new MockExecution(job, k, Encode(catalog, k), start, running ? null : end, records, warnings, errors);
        }

        // não passa de (1 × intervalo) para o job normal não aparecer atrasado; os de perfil LongRunning ficam quase sempre rodando
        private static double DurationSeconds(MockJob job, long k)
        {
            var catalog = job.Catalog;
            var h = Hash(job, k, 1);

            if (catalog.IdType == 1)
                return 2 + h % 90;

            if (catalog.RunningDailyAt is not null)
                return 5 * 60 + h % (35 * 60);

            if (catalog.IntervalTimeInMinutes == 0)
                return 2 + h % 60;

            var max = Math.Min(catalog.IntervalTimeInMinutes * 60 * (job.Profile == MockProfile.LongRunning ? 0.95 : 0.4), 50 * 60);
            return 5 + h % (ulong)Math.Max(max, 10);
        }

        private static Guid Encode(CatalogJob catalog, long k)
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(catalog.IdJob).CopyTo(bytes, 0);
            BitConverter.GetBytes(catalog.IdType).CopyTo(bytes, 4);
            BitConverter.GetBytes(k).CopyTo(bytes, 8);
            return new Guid(bytes);
        }

        public static ulong Hash(MockJob job, long k, int salt) =>
            Hash(((ulong)job.Catalog.IdJob << 32) ^ ((ulong)job.Catalog.IdType << 28) ^ ((ulong)k * 0x9E3779B97F4A7C15UL) ^ (ulong)salt);

        // SplitMix64
        public static ulong Hash(ulong x)
        {
            x += 0x9E3779B97F4A7C15UL;
            x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
            x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
            return x ^ (x >> 31);
        }
    }

    internal enum MockProfile
    {
        Normal,
        Inactive,
        Late,
        FailingLast,
        Noisy,
        LongRunning
    }

    internal sealed class MockJob
    {
        public CatalogJob Catalog { get; }
        public int IdApp { get; }
        public MockProfile Profile { get; }
        public bool IsActive => Profile != MockProfile.Inactive;
        public int OffsetMinutes { get; }
        public int MaxRecords { get; }
        public TimeSpan LateBy { get; }

        public MockJob(CatalogJob catalog, int idApp)
        {
            Catalog = catalog;
            IdApp = idApp;

            var h = MockTimeline.Hash(((ulong)catalog.IdJob << 8) ^ (ulong)catalog.IdType);
            var roll = (int)(h % 100);
            var scheduled = catalog.IntervalTimeInMinutes > 0;

            Profile = roll switch
            {
                < 4 => MockProfile.Inactive,
                < 8 when scheduled => MockProfile.Late,
                < 12 => MockProfile.FailingLast,
                < 17 when catalog.IdType == 2 => MockProfile.Noisy,
                < 20 when scheduled && catalog.IdType == 2 && catalog.RunningDailyAt is null => MockProfile.LongRunning,
                _ => MockProfile.Normal
            };

            OffsetMinutes = scheduled ? (int)((h >> 8) % (ulong)Math.Min(catalog.IntervalTimeInMinutes, 60)) : 0;
            MaxRecords = 20 + (int)((h >> 16) % 3000);
            LateBy = TimeSpan.FromMinutes(Math.Max(catalog.IntervalTimeInMinutes, 1) * (2 + (int)((h >> 24) % 5)));
        }
    }

    internal sealed record MockExecution(
        MockJob Job,
        long K,
        Guid Execution,
        DateTime StartDate,
        DateTime? EndDate,
        int Records,
        int Warnings,
        int Errors
    );
}
