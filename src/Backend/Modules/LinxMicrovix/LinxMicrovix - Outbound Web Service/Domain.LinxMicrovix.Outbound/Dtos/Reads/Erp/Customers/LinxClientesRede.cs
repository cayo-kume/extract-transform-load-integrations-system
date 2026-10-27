namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxClientesRede: os registros da tabela clientes_rede no portal centralizador. (colunas da v274: a chave da Newbloomers não tem acesso ao método, a conferir com uma resposta real)
    /// </summary>
    public class LinxClientesRede
    {
        public string? id_clientes_rede { get; init; }
        public string? doc_cliente { get; init; }
        public string? nome_razao_social { get; init; }
        public string? data_cadastro { get; init; }
        public string? pf_pj { get; init; }
        public string? tipo { get; init; }
        public string? cidade { get; init; }
        public string? uf { get; init; }
        public string? pais { get; init; }
        public string? id_estado_civil { get; init; }
        public string? compras_a_prazo { get; init; }
        public string? aceita_boleto_bancario { get; init; }
        public string? nome_fantasia { get; init; }
        public string? numero_endereco { get; init; }
        public string? complemento { get; init; }
        public string? bairro { get; init; }
        public string? cep { get; init; }
        public string? telefone { get; init; }
        public string? celular { get; init; }
        public string? fax { get; init; }
        public string? email { get; init; }
        public string? site { get; init; }
        public string? data_nascimento { get; init; }
        public string? naturalidade { get; init; }
        public string? sexo { get; init; }
        public string? nome_pai { get; init; }
        public string? nome_mae { get; init; }
        public string? identidade_cliente { get; init; }
        public string? data_emissao_identidade_cliente { get; init; }
        public string? uf_emissao_identidade_cliente { get; init; }
        public string? orgao_emissao_identidade_cliente { get; init; }
        public string? nome_empresa_titular { get; init; }
        public string? telefone_empresa_titular { get; init; }
        public string? funcao_titular { get; init; }
        public string? tempo_servico_titular { get; init; }
        public string? renda_titular { get; init; }
        public string? inscricao_estadual { get; init; }
        public string? inscricao_municipal { get; init; }
        public string? cliente_optante_simples_municipal { get; init; }
        public string? cliente_optante_simples_federal { get; init; }
        public string? limite_compras { get; init; }
        public string? cartao_fidelidade { get; init; }
        public string? desabilitado { get; init; }
        public string? motivo_bloqueio { get; init; }
        public string? portal_origem { get; init; }
        public string? empresa_origem { get; init; }
        public string? cod_cliente_portal_origem { get; init; }
        public string? codigo_ws { get; init; }
        public string? integrado_ws { get; init; }
        public string? timestamp { get; init; }
    }
}
