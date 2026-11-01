using WebApp.Models;

namespace WebApp.Components.Display
{
    /// <summary>
    /// O que o HealthChip precisa para dizer a saúde e há quanto tempo: vem de um job (Jobs, Integrações), de uma linha do
    /// heartbeat (Visão geral) ou do cabeçalho de um job nos Logs de Erros.
    /// </summary>
    public sealed record HealthContext(
        JobHealth? Health,
        bool IsActive,
        DateTime? LastStart,
        DateTime? LastEnd,
        int IntervalTimeInMinutes,
        TimeOnly? RunningDailyAt,
        AcknowledgementModel? Acknowledgement,
        RunRequestModel? RunRequest,
        bool HasWarnings)
    {
        public static HealthContext From(JobModel job) => new(
            job.Health,
            job.IsActive,
            job.LastExecution?.StartDate,
            job.LastExecution?.EndDate,
            job.IntervalTimeInMinutes,
            job.RunningDailyAt,
            job.Acknowledgement,
            job.RunRequest,
            job.LastExecution?.Warnings > 0);

        public static HealthContext From(HeartbeatModel heartbeat) => new(
            heartbeat.Health,
            true,
            heartbeat.StartDate,
            heartbeat.EndDate,
            heartbeat.IntervalTimeInMinutes,
            heartbeat.RunningDailyAt,
            heartbeat.Acknowledgement,
            null,
            heartbeat.HasWarnings);

        public static HealthContext From(MessageJobSummaryModel job) => new(
            job.CurrentHealth,
            job.IsActive,
            job.LastStartDate,
            job.LastEndDate,
            job.IntervalTimeInMinutes,
            job.RunningDailyAt,
            job.Acknowledgement,
            null,
            false);

        /// <summary>
        /// Erro ou Atrasado reconhecido: fora de "Falhas Ativas", com chip neutro.
        /// </summary>
        public bool IsAcknowledged => Acknowledgement is not null && Health is JobHealth.Error or JobHealth.Late;

        /// <summary>
        /// Rodando além de 1 × o intervalo: o caso que o heartbeat do host recupera (jobs diários e sob demanda não têm limite).
        /// </summary>
        public bool IsRunningTooLong(DateTime now) =>
            Health == JobHealth.Running
            && LastStart is DateTime start
            && IntervalTimeInMinutes > 0
            && RunningDailyAt is null
            && now - start > TimeSpan.FromMinutes(IntervalTimeInMinutes);

        /// <summary>
        /// Quando o job deveria ter começado de novo (atrasado): o início da última execução mais 1 × o intervalo.
        /// </summary>
        public DateTime? ExpectedStart => LastStart is DateTime start && IntervalTimeInMinutes > 0
            ? start.AddMinutes(IntervalTimeInMinutes)
            : null;
    }
}
