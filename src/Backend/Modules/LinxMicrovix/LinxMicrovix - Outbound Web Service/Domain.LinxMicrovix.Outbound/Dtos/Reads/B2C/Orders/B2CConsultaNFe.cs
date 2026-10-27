namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaNFe (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaNFe
    {
        public string? id_nfe { get; init; }
        public string? id_pedido { get; init; }
        public string? documento { get; init; }
        public string? data_emissao { get; init; }
        public string? chave_nfe { get; init; }
        public string? situacao { get; init; }
        public string? xml { get; init; }
        public string? excluido { get; init; }
        public string? identificador_microvix { get; init; }
        public string? dt_insert { get; init; }
        public string? valor_nota { get; init; }
        public string? serie { get; init; }
        public string? frete { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
        public string? nProt { get; init; }
        public string? codigo_modelo_nf { get; init; }
        public string? justificativa { get; init; }
        public string? tpAmb { get; init; }
    }
}
