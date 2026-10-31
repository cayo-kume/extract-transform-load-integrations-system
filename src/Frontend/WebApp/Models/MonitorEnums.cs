namespace WebApp.Models
{
    /// <summary>
    /// Nível de uma mensagem de auditing.Messages (src/Backend/Shared/docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md).
    /// </summary>
    public enum MessageLevel
    {
        Information = 1,
        Warning = 2,
        Error = 3
    }

    /// <summary>
    /// Saúde de um job pela última execução: 1 × IntervalTimeInMinutes de prazo, igual para todos os jobs.
    /// </summary>
    public enum JobHealth
    {
        Ok,
        Late,
        Error,
        Running
    }

    /// <summary>
    /// Período dos filtros do painel, contado para trás a partir de agora.
    /// </summary>
    public enum Period
    {
        Last12Hours,
        Last24Hours,
        Last7Days
    }

    public static class PeriodExtensions
    {
        public static TimeSpan ToTimeSpan(this Period period) => period switch
        {
            Period.Last12Hours => TimeSpan.FromHours(12),
            Period.Last24Hours => TimeSpan.FromHours(24),
            Period.Last7Days => TimeSpan.FromDays(7),
            _ => throw new ArgumentOutOfRangeException(nameof(period), period, null)
        };
    }
}
