namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxVendasPosAgrupamentoTmp: os registros da tabela vendas_pos_agrupamento_tmp (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxVendasPosAgrupamentoTmp
    {
        public string? portal { get; init; }
        public string? id_vendas_pos_agrupamento_tmp { get; init; }
        public string? id_vendas_pos { get; init; }
        public string? id_vendas_pos_origem { get; init; }
        public string? timestamp { get; init; }
    }
}
