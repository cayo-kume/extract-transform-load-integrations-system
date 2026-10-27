namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosPromocoes: informações das promoções dos produtos de acordo com a loja pesquisada (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosPromocoes
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_produto { get; init; }
        public string? preco_promocao { get; init; }
        public string? data_inicio_promocao { get; init; }
        public string? data_termino_promocao { get; init; }
        public string? data_cadastro_promocao { get; init; }
        public string? promocao_ativa { get; init; }
        public string? id_campanha { get; init; }
        public string? nome_campanha { get; init; }
        public string? promocao_opcional { get; init; }
        public string? custo_total_campanha { get; init; }
        public string? timestamp { get; init; }
    }
}
