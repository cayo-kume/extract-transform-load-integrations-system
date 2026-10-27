namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxNaturezaOperacao: as naturezas de operação. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxNaturezaOperacao
    {
        public string? portal { get; init; }
        public string? cod_natureza_operacao { get; init; }
        public string? descricao { get; init; }
        public string? soma_relatorios { get; init; }
        public string? operacao { get; init; }
        public string? inativa { get; init; }
        public string? timestamp { get; init; }
        public string? calcula_ipi { get; init; }
        public string? calcula_iss { get; init; }
        public string? calcula_irrf { get; init; }
        public string? tipo_preco { get; init; }
        public string? atualiza_custo { get; init; }
        public string? transferencia { get; init; }
        public string? baixar_estoque { get; init; }
        public string? consumo_proprio { get; init; }
        public string? contabiliza_cmv { get; init; }
        public string? despesa { get; init; }
        public string? atualiza_custo_medio { get; init; }
        public string? exige_nf_origem { get; init; }
        public string? integra_contabilidade { get; init; }
        public string? id_obs { get; init; }
        public string? venda_futura { get; init; }
        public string? base_icms_considera_ipi { get; init; }
        public string? permite_escolha_historico { get; init; }
        public string? import_produtos { get; init; }
        public string? deposito_reserva_venda { get; init; }
        public string? exibe_nfe { get; init; }
        public string? faturamento_antecipado { get; init; }
        public string? exibir_informacoes_imposto { get; init; }
        public string? gera_garantia_nacional { get; init; }
        public string? transferencia_deposito { get; init; }
        public string? venda_diferencial_aliquota { get; init; }
        public string? insere_obs_pis_cofins { get; init; }
        public string? diferencial_ativo_consumo { get; init; }
        public string? recusa_de { get; init; }
        public string? codigo_ws { get; init; }
    }
}
