namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxXMLDocumentos: o XML dos documentos. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxXMLDocumentos
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? documento { get; init; }
        public string? serie { get; init; }
        public string? data_emissao { get; init; }
        public string? chave_nfe { get; init; }
        public string? situacao { get; init; }
        public string? xml { get; init; }
        public string? excluido { get; init; }
        public string? identificador_microvix { get; init; }
        public string? dt_insert { get; init; }
        public string? timestamp { get; init; }
        public string? nProtCanc { get; init; }
        public string? nProtInut { get; init; }
        public string? xmlDistribuicao { get; init; }
        public string? nProtDeneg { get; init; }
        public string? cStat { get; init; }
        public string? id_nfe { get; init; }
        public string? justificativa { get; init; }
        public string? cod_cliente { get; init; }
    }
}
