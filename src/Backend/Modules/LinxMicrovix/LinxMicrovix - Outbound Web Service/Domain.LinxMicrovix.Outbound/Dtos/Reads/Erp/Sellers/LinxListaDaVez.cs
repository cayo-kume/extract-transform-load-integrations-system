namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxListaDaVez: os registros gerados na rotina de Lista da Vez (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxListaDaVez
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_vendedor { get; init; }
        public string? data { get; init; }
        public string? motivo_nao_venda { get; init; }
        public string? qtde_ocorrencias { get; init; }
        public string? data_hora_ini_atend { get; init; }
        public string? data_hora_fim_atend { get; init; }
        public string? obs { get; init; }
        public string? desc_produto_neg { get; init; }
        public string? valor_produto_neg { get; init; }
        public string? timestamp { get; init; }
    }
}
