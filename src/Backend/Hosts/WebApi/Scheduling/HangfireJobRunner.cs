using Hangfire;
using Hosts.Scheduling;

namespace WebApi.Scheduling
{
    /// <summary>
    /// o que o Hangfire executa em cada job da agenda: só repassa para o ScheduledJobRunner (Hosts.Scheduling), que chama o
    /// método do mapa e grava o log do erro quando a execução cai no meio, na empresa do recurring job. O nome do recurring job
    /// (&lt;IdJob&gt;-&lt;Title&gt; - V&lt;versão&gt; - &lt;empresa&gt;) vai como argumento só para aparecer no dashboard
    /// </summary>
    public class HangfireJobRunner
    {
        private readonly ScheduledJobRunner _runner;

        public HangfireJobRunner(ScheduledJobRunner runner) =>
            _runner = runner;

        [JobDisplayName("{2}")]
        public Task Run(int idJob, string company, string recurringJobId) =>
            _runner.Run(idJob, company);
    }
}
