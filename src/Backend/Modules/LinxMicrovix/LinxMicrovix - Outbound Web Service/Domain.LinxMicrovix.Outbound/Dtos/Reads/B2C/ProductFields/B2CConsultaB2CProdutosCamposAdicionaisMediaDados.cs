namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaB2CProdutosCamposAdicionaisMediaDados (colunas da V64: a chave da Newbloomers não tem acesso ao método, a conferir com uma resposta real)
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionaisMediaDados
    {
        public string? id_b2c_produtos_campos_adicionais_media_dados { get; init; }
        public string? id_b2c_produtos_campos_adicionais_media { get; init; }
        public string? codigoproduto { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
