namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxLancContabil: informações dos Lançamentos Contábeis por conta e empresa (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxLancContabil
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_lanc { get; init; }
        public string? centro_custo { get; init; }
        public string? ind_conta { get; init; }
        public string? cod_conta { get; init; }
        public string? nome_conta { get; init; }
        public string? valor_conta { get; init; }
        public string? cred_deb { get; init; }
        public string? data_lanc { get; init; }
        public string? compl_conta { get; init; }
        public string? identificador { get; init; }
        public string? cod_historico { get; init; }
        public string? desc_historico { get; init; }
        public string? data_compensacao { get; init; }
        public string? fatura_origem { get; init; }
        public string? efetivado { get; init; }
        public string? timestamp { get; init; }
        public string? empresa { get; init; }
        public string? id_lanc { get; init; }
        public string? cancelado { get; init; }
    }
}
