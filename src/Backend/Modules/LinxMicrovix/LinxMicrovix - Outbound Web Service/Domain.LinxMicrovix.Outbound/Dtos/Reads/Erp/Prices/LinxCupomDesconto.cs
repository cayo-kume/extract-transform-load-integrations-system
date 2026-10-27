namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCupomDesconto: os registros do cadastro de cupons de desconto. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCupomDesconto
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? id_cupom_desconto { get; init; }
        public string? cupom { get; init; }
        public string? descricao { get; init; }
        public string? percentual_desconto { get; init; }
        public string? data_inicial_vigencia { get; init; }
        public string? data_final_vigencia { get; init; }
        public string? qtde_original { get; init; }
        public string? qtde_disponivel { get; init; }
        public string? cod_vendedor { get; init; }
        public string? disponivel { get; init; }
        public string? timestamp { get; init; }
    }
}
