namespace WebJob.Scheduling
{
    /// <summary>
    /// seção WebJob do appsettings.json (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md §4.1). O IdJob vem da linha de comando
    /// (WebJob.exe --job 8006), que o run.cmd de cada WebJob passa na Azure
    /// </summary>
    public class WebJobSettings
    {
        public const string SectionName = "WebJob";

        /// <summary>
        /// o job que este processo roda (--job)
        /// </summary>
        public int? IdJob { get; set; }

        /// <summary>
        /// fuso da agenda (intervalos e RunningDailyAt), o mesmo do Hangfire do WebApi
        /// </summary>
        public string TimeZone { get; set; } = "America/Sao_Paulo";

        /// <summary>
        /// de quanto em quanto tempo o processo relê auditing.Jobs enquanto espera: um job inativo, sem agenda ou com o próximo
        /// horário distante. Uma mudança na tabela vale, no máximo, depois desse tempo
        /// </summary>
        public int RecheckIntervalInMinutes { get; set; } = 5;
    }
}
