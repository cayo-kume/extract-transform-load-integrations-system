namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxLojas: o cadastro de uma loja, consultada pelo CNPJ (colunas da resposta real, 26/10/2026)
    /// </summary>
    public class LinxLojas
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? nome_emp { get; init; }
        public string? razao_emp { get; init; }
        public string? cnpj_emp { get; init; }
        public string? inscricao_emp { get; init; }
        public string? endereco_emp { get; init; }
        public string? num_emp { get; init; }
        public string? complement_emp { get; init; }
        public string? bairro_emp { get; init; }
        public string? cep_emp { get; init; }
        public string? cidade_emp { get; init; }
        public string? estado_emp { get; init; }
        public string? fone_emp { get; init; }
        public string? email_emp { get; init; }
        public string? cod_ibge_municipio { get; init; }
        public string? data_criacao_emp { get; init; }
        public string? data_criacao_portal { get; init; }
        public string? sistema_tributacao { get; init; }
        public string? regime_tributario { get; init; }
        public string? area_empresa { get; init; }
        public string? timestamp { get; init; }
        public string? sigla_empresa { get; init; }
        public string? id_classe_fiscal { get; init; }
        public string? centro_distribuicao { get; init; }
        public string? inscricao_municipal_emp { get; init; }
        public string? cnae_emp { get; init; }
        public string? cod_cliente_linx { get; init; }
        public string? tabela_preco_preferencial { get; init; }
        public string? subrede { get; init; }
        public string? produto_disponivel_empresa { get; init; }
        public string? id_grupo { get; init; }
    }
}
