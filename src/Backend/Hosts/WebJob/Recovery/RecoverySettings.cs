namespace WebJob.Recovery
{
    /// <summary>
    /// seção Recovery do appsettings.json (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md §7): onde estão os WebJobs na Azure,
    /// a tolerância do heartbeat e as exceções por job. Sem segredo: a autenticação na Azure é o DefaultAzureCredential (a
    /// identidade gerenciada do App Service; no local, o az login)
    /// </summary>
    public class RecoverySettings
    {
        public const string SectionName = "Recovery";

        /// <summary>
        /// true: lê o heartbeat e registra quais WebJobs reiniciaria, sem chamar a Azure
        /// </summary>
        public bool DryRun { get; set; }

        public string? TenantId { get; set; }

        public string? SubscriptionId { get; set; }

        public string? ResourceGroup { get; set; }

        public string? AppServiceName { get; set; }

        /// <summary>
        /// o mesmo NameFormat do perfil de publicação: 'Spaces' ("&lt;Title&gt; - V&lt;versão&gt;") ou 'Safe' ("&lt;Title&gt;-V&lt;versão com _&gt;")
        /// </summary>
        public string NameFormat { get; set; } = "Spaces";

        /// <summary>
        /// folga somada ao limite: um job de intervalo fica off sem execução terminada há mais que o intervalo + isto; um diário,
        /// sem execução terminada desde o último horário + isto. 0 = a regra decidida, "1 × o intervalo" (§7.1); para jobs longos,
        /// o limite próprio em Thresholds
        /// </summary>
        public int GraceMinutes { get; set; }

        /// <summary>
        /// depois de reiniciar um WebJob, quanto tempo esperar antes de reiniciá-lo de novo (e o tempo que ele tem para
        /// terminar a primeira execução)
        /// </summary>
        public int RestartCooldownMinutes { get; set; } = 30;

        /// <summary>
        /// limite próprio por IdJob, em minutos, para jobs com execuções longas (ex.: { "13017": 240 }); no lugar do intervalo
        /// + GraceMinutes
        /// </summary>
        public Dictionary<string, int> Thresholds { get; set; } = [];

        /// <summary>
        /// jobs que a recuperação nunca reinicia. Sem valor padrão: o binder soma os itens do appsettings.json aos do array
        /// </summary>
        public int[] IgnoredJobs { get; set; } = [];
    }
}
