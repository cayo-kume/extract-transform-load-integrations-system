namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaPalavrasChavePesquisa (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaPalavrasChavePesquisa
    {
        public string? portal { get; init; }
        public string? id_b2c_palavras_chave_pesquisa { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
    }
}
