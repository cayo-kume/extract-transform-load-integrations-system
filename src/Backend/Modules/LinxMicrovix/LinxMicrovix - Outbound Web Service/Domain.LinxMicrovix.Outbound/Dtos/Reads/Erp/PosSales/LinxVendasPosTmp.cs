namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxVendasPosTmp: os registros da tabela vendas_pos_tmp (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxVendasPosTmp
    {
        public string? portal { get; init; }
        public string? id_vendas_pos { get; init; }
        public string? numero_ficha { get; init; }
        public string? numero_sequencial { get; init; }
        public string? cod_cliente { get; init; }
        public string? data_lcto { get; init; }
        public string? empresa { get; init; }
        public string? cliente_indicador { get; init; }
        public string? observacao_dav { get; init; }
        public string? observacao { get; init; }
        public string? codigo_sistema_terceiro { get; init; }
        public string? cancelado { get; init; }
        public string? timestamp { get; init; }
    }
}
