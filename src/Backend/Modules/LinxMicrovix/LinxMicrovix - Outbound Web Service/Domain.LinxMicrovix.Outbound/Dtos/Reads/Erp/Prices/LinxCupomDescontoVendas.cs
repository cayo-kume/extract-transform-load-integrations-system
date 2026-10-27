namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCupomDescontoVendas: o id do cupom de desconto, o valor do cupom e identificador das vendas que utilizaram cupons de desconto. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCupomDescontoVendas
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? id_cupom_desconto_vendas { get; init; }
        public string? id_cupom_desconto { get; init; }
        public string? identificador { get; init; }
        public string? valor { get; init; }
        public string? timestamp { get; init; }
        public string? id_vendas_pos { get; init; }
    }
}
