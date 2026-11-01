using MudBlazor;
using WebApp.Models;

namespace WebApp.Components.Display
{
    /// <summary>
    /// Retorno de ligar e desligar um job, igual nas telas Jobs e na aba Agenda: o texto do que mudou e "Desfazer".
    /// </summary>
    public static class JobFeedback
    {
        /// <summary>
        /// undo roda fora do ciclo de render da tela: quem chama passa um InvokeAsync que reverte e recarrega.
        /// </summary>
        public static void JobToggled(this ISnackbar snackbar, JobModel job, bool isActive, Func<Task> undo)
        {
            snackbar.Add(MonitorDisplay.JobToggled(job.IdJob, job.Title, isActive), isActive ? Severity.Success : Severity.Warning, config =>
            {
                config.Action = "Desfazer";
                config.ActionColor = Color.Inherit;
                config.ShowCloseIcon = true;
                config.VisibleStateDuration = 10_000;
                config.OnClick = _ => undo();
            });
        }
    }
}
