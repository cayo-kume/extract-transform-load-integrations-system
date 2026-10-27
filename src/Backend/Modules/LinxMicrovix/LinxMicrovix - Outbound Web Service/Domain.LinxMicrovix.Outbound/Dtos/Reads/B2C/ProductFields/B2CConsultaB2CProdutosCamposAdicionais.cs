namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaB2CProdutosCamposAdicionais (colunas da V64: a chave da Newbloomers não tem acesso ao método, a conferir com uma resposta real)
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionais
    {
        public string? id_b2c_produtos_campos_adicionais { get; init; }
        public string? nome_campo { get; init; }
        public string? tipo { get; init; }
        public string? decimais { get; init; }
        public string? multipla_escolha { get; init; }
        public string? ordem { get; init; }
        public string? ativo { get; init; }
        public string? nivel_id { get; init; }
        public string? nivel_descricao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
