using MudBlazor;
using WebApp.Models;

namespace WebApp.Components.Display
{
    /// <summary>
    /// Textos, cores e ícones dos estados do painel, para todas as telas mostrarem saúde, nível e agenda do mesmo jeito.
    /// </summary>
    public static class MonitorDisplay
    {
        public static string HealthLabel(JobHealth? health) => health switch
        {
            JobHealth.Ok => "Ok",
            JobHealth.Late => "Atrasado",
            JobHealth.Error => "Erro",
            JobHealth.Running => "Rodando",
            _ => "Sem execução"
        };

        public static Color HealthColor(JobHealth? health) => health switch
        {
            JobHealth.Ok => Color.Success,
            JobHealth.Late => Color.Warning,
            JobHealth.Error => Color.Error,
            JobHealth.Running => Color.Info,
            _ => Color.Default
        };

        public static string HealthIcon(JobHealth? health) => health switch
        {
            JobHealth.Ok => Icons.Material.Filled.CheckCircle,
            JobHealth.Late => Icons.Material.Filled.Schedule,
            JobHealth.Error => Icons.Material.Filled.Error,
            JobHealth.Running => Icons.Material.Filled.Sync,
            _ => Icons.Material.Filled.RemoveCircleOutline
        };

        public static string LevelLabel(MessageLevel level) => level switch
        {
            MessageLevel.Information => "Informação",
            MessageLevel.Warning => "Aviso",
            MessageLevel.Error => "Erro",
            _ => level.ToString()
        };

        public static Color LevelColor(MessageLevel level) => level switch
        {
            MessageLevel.Information => Color.Info,
            MessageLevel.Warning => Color.Warning,
            MessageLevel.Error => Color.Error,
            _ => Color.Default
        };

        public static string LevelIcon(MessageLevel level) => level switch
        {
            MessageLevel.Information => Icons.Material.Filled.Info,
            MessageLevel.Warning => Icons.Material.Filled.Warning,
            MessageLevel.Error => Icons.Material.Filled.Error,
            _ => Icons.Material.Filled.Circle
        };

        public static string Schedule(JobModel job) => job switch
        {
            { RunningDailyAt: TimeOnly at } => $"Diário às {at:HH\\:mm}",
            { IntervalTimeInMinutes: 0 } => "Sob demanda",
            { IntervalTimeInMinutes: var minutes } when minutes % 60 == 0 => $"A cada {minutes / 60} h",
            { IntervalTimeInMinutes: var minutes } => $"A cada {minutes} min"
        };

        public static string Duration(TimeSpan? duration) => duration switch
        {
            null => "—",
            { TotalDays: >= 1 } d => d.ToString(@"d\d\ hh\:mm\:ss"),
            var d => d.Value.ToString(@"hh\:mm\:ss")
        };

        public static string Date(DateTime? date) => date?.ToString("dd/MM/yyyy HH:mm:ss") ?? "—";

        /// <summary>
        /// Hora curta: "05:12" se for hoje, "31/10 23:05" se não.
        /// </summary>
        public static string Time(DateTime date, DateTime now) =>
            date.Date == now.Date ? date.ToString("HH:mm") : date.ToString("dd/MM HH:mm");

        /// <summary>
        /// Tempo decorrido em poucos caracteres: "45 min", "2h13", "3 d 4 h".
        /// </summary>
        public static string Elapsed(TimeSpan elapsed) => elapsed switch
        {
            { TotalMinutes: < 1 } => "< 1 min",
            { TotalHours: < 1 } => $"{(int)elapsed.TotalMinutes} min",
            { TotalDays: < 1 } => $"{(int)elapsed.TotalHours}h{elapsed.Minutes:00}",
            _ => $"{(int)elapsed.TotalDays} d {elapsed.Hours} h"
        };

        /// <summary>
        /// "há 4 min", "há 2h13", "agora".
        /// </summary>
        public static string Ago(DateTime date, DateTime now) =>
            now - date < TimeSpan.FromMinutes(1) ? "agora" : $"há {Elapsed(now - date)}";

        /// <summary>
        /// A mesma frase em todas as telas, para o que mudou ficar registrado na tela (e, na fase 5, na auditoria).
        /// </summary>
        public static string JobToggled(int idJob, string? title, bool isActive) =>
            $"Job {idJob} {title} {(isActive ? "ativado" : "desativado")}";

        /// <summary>
        /// Valor do período na URL: 12h, 24h ou 7d.
        /// </summary>
        public static string PeriodQuery(Period period) => period switch
        {
            Period.Last12Hours => "12h",
            Period.Last7Days => "7d",
            _ => "24h"
        };

        public static Period? ParsePeriod(string? value) => value switch
        {
            "12h" => Period.Last12Hours,
            "24h" => Period.Last24Hours,
            "7d" => Period.Last7Days,
            _ => null
        };

        /// <summary>
        /// Link para a tela Integrações já aberta num job, no período da tela de origem.
        /// </summary>
        public static string JobLink(int idApp, int idJob, int idType, Period period) =>
            $"/integrations?app={idApp}&job={idJob}&type={idType}&period={PeriodQuery(period)}";

        /// <summary>
        /// Link para os Logs de Erros já abertos na aplicação (e no job, se informado), no período da tela de origem.
        /// </summary>
        public static string ErrorsLink(int idApp, int? idJob, int? idType, Period period) =>
            idJob is null
                ? $"/errors?app={idApp}&period={PeriodQuery(period)}"
                : $"/errors?app={idApp}&job={idJob}&type={idType}&period={PeriodQuery(period)}";
    }
}
