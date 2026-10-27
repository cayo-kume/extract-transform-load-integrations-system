namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoTrocafone: os valores e números dos vouchers Trocafone realizadas nas vendas (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoTrocafone
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? identificador { get; init; }
        public string? num_vale { get; init; }
        public string? valor_vale { get; init; }
        public string? voucher { get; init; }
        public string? nome_produto { get; init; }
        public string? condicao { get; init; }
        public string? id_tradein_parceiro { get; init; }
        public string? timestamp { get; init; }
    }
}
