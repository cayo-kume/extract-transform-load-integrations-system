namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxSangriaSuprimentos: as informações das Sangrias ou Suprimentos feitos no caixa (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxSangriaSuprimentos
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? usuario { get; init; }
        public string? data { get; init; }
        public string? valor { get; init; }
        public string? obs { get; init; }
        public string? cancelado { get; init; }
        public string? conferido { get; init; }
        public string? cod_historico { get; init; }
        public string? desc_historico { get; init; }
        public string? id_sangria_suprimentos { get; init; }
        public string? timestamp { get; init; }
    }
}
