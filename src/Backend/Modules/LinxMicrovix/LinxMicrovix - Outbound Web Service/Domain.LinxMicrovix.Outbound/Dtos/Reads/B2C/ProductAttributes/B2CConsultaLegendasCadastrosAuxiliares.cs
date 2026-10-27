namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaLegendasCadastrosAuxiliares (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaLegendasCadastrosAuxiliares
    {
        public string? empresa { get; init; }
        public string? legenda_setor { get; init; }
        public string? legenda_linha { get; init; }
        public string? legenda_marca { get; init; }
        public string? legenda_colecao { get; init; }
        public string? legenda_grade1 { get; init; }
        public string? legenda_grade2 { get; init; }
        public string? legenda_espessura { get; init; }
        public string? legenda_classificacao { get; init; }
        public string? timestamp { get; init; }
    }
}
