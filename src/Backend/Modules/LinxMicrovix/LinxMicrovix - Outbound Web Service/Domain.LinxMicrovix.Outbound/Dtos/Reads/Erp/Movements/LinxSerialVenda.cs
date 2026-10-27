namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxSerialVenda: os registros da tabela serial_venda (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxSerialVenda
    {
        public string? portal { get; init; }
        public string? id_serial_venda { get; init; }
        public string? transacao { get; init; }
        public string? codigoproduto { get; init; }
        public string? serial { get; init; }
        public string? timestamp { get; init; }
    }
}
