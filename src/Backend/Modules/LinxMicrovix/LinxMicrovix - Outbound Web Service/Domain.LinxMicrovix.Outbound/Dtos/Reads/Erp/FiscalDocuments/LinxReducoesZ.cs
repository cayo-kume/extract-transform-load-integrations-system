namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxReducoesZ: as ReduçõesZ de acordo com a loja pesquisada, somente para clientes que ainda utilizam a impressora fiscal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxReducoesZ
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? cnpj { get; init; }
        public string? data_fechamento { get; init; }
        public string? numero_ecf { get; init; }
        public string? numeroserie { get; init; }
        public string? numero_reducoes { get; init; }
        public string? cupom_inicial { get; init; }
        public string? cupom_final { get; init; }
        public string? gt_inicial { get; init; }
        public string? gt_final { get; init; }
        public string? qtde_cupons { get; init; }
        public string? venda_bruta { get; init; }
        public string? descontos { get; init; }
        public string? acrescimos { get; init; }
        public string? cancelamentos { get; init; }
        public string? cro { get; init; }
        public string? usuario { get; init; }
        public string? aliquota { get; init; }
        public string? valor { get; init; }
        public string? data_mov { get; init; }
        public string? timestamp { get; init; }
    }
}
