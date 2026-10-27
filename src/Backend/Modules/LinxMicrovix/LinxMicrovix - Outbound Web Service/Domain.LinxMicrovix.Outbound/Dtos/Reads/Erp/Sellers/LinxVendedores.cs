namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxVendedores: os Vendedores cadastrados na loja (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxVendedores
    {
        public string? portal { get; init; }
        public string? cod_vendedor { get; init; }
        public string? nome_vendedor { get; init; }
        public string? tipo_vendedor { get; init; }
        public string? end_vend_rua { get; init; }
        public string? end_vend_numero { get; init; }
        public string? end_vend_complemento { get; init; }
        public string? end_vend_bairro { get; init; }
        public string? end_vend_cep { get; init; }
        public string? end_vend_cidade { get; init; }
        public string? end_vend_uf { get; init; }
        public string? fone_vendedor { get; init; }
        public string? mail_vendedor { get; init; }
        public string? dt_upd { get; init; }
        public string? cpf_vendedor { get; init; }
        public string? ativo { get; init; }
        public string? data_admissao { get; init; }
        public string? data_saida { get; init; }
        public string? timestamp { get; init; }
        public string? matricula { get; init; }
        public string? id_tipo_venda { get; init; }
        public string? descricao_tipo_venda { get; init; }
        public string? cargo { get; init; }
        public string? funcao { get; init; }
        public string? nome_exibicao { get; init; }
        public string? empresa { get; init; }
        public string? codigo_integracao_ws { get; init; }
        public string? meta_peso { get; init; }
    }
}
