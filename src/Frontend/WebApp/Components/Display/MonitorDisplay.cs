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
    }
}
