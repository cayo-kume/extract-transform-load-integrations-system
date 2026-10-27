namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaB2CProdutosCamposAdicionaisMedia (colunas da V64: a chave da Newbloomers não tem acesso ao método, a conferir com uma resposta real)
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionaisMedia
    {
        public string? id_b2c_produtos_campos_adicionais_media { get; init; }
        public string? descricao { get; init; }
        public string? url { get; init; }
        public string? tipo_media_descricao { get; init; }
        public string? portal { get; init; }
        public string? media_principal { get; init; }
        public string? tipo_media_id { get; init; }
        public string? timestamp { get; init; }
    }
}
