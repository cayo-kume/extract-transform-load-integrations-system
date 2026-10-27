using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxNaturezaOperacao, tabela linxmicrovix.LinxNaturezaOperacao. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_natureza_operacao
    /// </summary>
    public class LinxNaturezaOperacao
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public string cod_natureza_operacao { get; private set; } = String.Empty;
        public string? descricao { get; private set; }
        public string? soma_relatorios { get; private set; }
        public string? operacao { get; private set; }
        public string? inativa { get; private set; }
        public long timestamp { get; private set; }
        public bool? calcula_ipi { get; private set; }
        public bool? calcula_iss { get; private set; }
        public bool? calcula_irrf { get; private set; }
        public string? tipo_preco { get; private set; }
        public string? atualiza_custo { get; private set; }
        public bool? transferencia { get; private set; }
        public bool? baixar_estoque { get; private set; }
        public bool? consumo_proprio { get; private set; }
        public bool? contabiliza_cmv { get; private set; }
        public bool? despesa { get; private set; }
        public bool? atualiza_custo_medio { get; private set; }
        public bool? exige_nf_origem { get; private set; }
        public bool? integra_contabilidade { get; private set; }
        public int? id_obs { get; private set; }
        public bool? venda_futura { get; private set; }
        public bool? base_icms_considera_ipi { get; private set; }
        public bool? permite_escolha_historico { get; private set; }
        public bool? import_produtos { get; private set; }
        public int? deposito_reserva_venda { get; private set; }
        public bool? exibe_nfe { get; private set; }
        public bool? faturamento_antecipado { get; private set; }
        public bool? exibir_informacoes_imposto { get; private set; }
        public bool? gera_garantia_nacional { get; private set; }
        public bool? transferencia_deposito { get; private set; }
        public bool? venda_diferencial_aliquota { get; private set; }
        public bool? insere_obs_pis_cofins { get; private set; }
        public bool? diferencial_ativo_consumo { get; private set; }
        public bool? recusa_de { get; private set; }
        public string? codigo_ws { get; private set; }

        public LinxNaturezaOperacao() { }

        public LinxNaturezaOperacao(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxNaturezaOperacao dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            cod_natureza_operacao = MicrovixConverters.ToNullableString(dto.cod_natureza_operacao, 10) ?? String.Empty;
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 60);
            soma_relatorios = MicrovixConverters.ToNullableString(dto.soma_relatorios, 1);
            operacao = MicrovixConverters.ToNullableString(dto.operacao, 2);
            inativa = MicrovixConverters.ToNullableString(dto.inativa, 1);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            calcula_ipi = MicrovixConverters.ToNullableBoolean(dto.calcula_ipi);
            calcula_iss = MicrovixConverters.ToNullableBoolean(dto.calcula_iss);
            calcula_irrf = MicrovixConverters.ToNullableBoolean(dto.calcula_irrf);
            tipo_preco = MicrovixConverters.ToNullableString(dto.tipo_preco, 2);
            atualiza_custo = MicrovixConverters.ToNullableString(dto.atualiza_custo, 1);
            transferencia = MicrovixConverters.ToNullableBoolean(dto.transferencia);
            baixar_estoque = MicrovixConverters.ToNullableBoolean(dto.baixar_estoque);
            consumo_proprio = MicrovixConverters.ToNullableBoolean(dto.consumo_proprio);
            contabiliza_cmv = MicrovixConverters.ToNullableBoolean(dto.contabiliza_cmv);
            despesa = MicrovixConverters.ToNullableBoolean(dto.despesa);
            atualiza_custo_medio = MicrovixConverters.ToNullableBoolean(dto.atualiza_custo_medio);
            exige_nf_origem = MicrovixConverters.ToNullableBoolean(dto.exige_nf_origem);
            integra_contabilidade = MicrovixConverters.ToNullableBoolean(dto.integra_contabilidade);
            id_obs = MicrovixConverters.ToNullableInt32(dto.id_obs);
            venda_futura = MicrovixConverters.ToNullableBoolean(dto.venda_futura);
            base_icms_considera_ipi = MicrovixConverters.ToNullableBoolean(dto.base_icms_considera_ipi);
            permite_escolha_historico = MicrovixConverters.ToNullableBoolean(dto.permite_escolha_historico);
            import_produtos = MicrovixConverters.ToNullableBoolean(dto.import_produtos);
            deposito_reserva_venda = MicrovixConverters.ToNullableInt32(dto.deposito_reserva_venda);
            exibe_nfe = MicrovixConverters.ToNullableBoolean(dto.exibe_nfe);
            faturamento_antecipado = MicrovixConverters.ToNullableBoolean(dto.faturamento_antecipado);
            exibir_informacoes_imposto = MicrovixConverters.ToNullableBoolean(dto.exibir_informacoes_imposto);
            gera_garantia_nacional = MicrovixConverters.ToNullableBoolean(dto.gera_garantia_nacional);
            transferencia_deposito = MicrovixConverters.ToNullableBoolean(dto.transferencia_deposito);
            venda_diferencial_aliquota = MicrovixConverters.ToNullableBoolean(dto.venda_diferencial_aliquota);
            insere_obs_pis_cofins = MicrovixConverters.ToNullableBoolean(dto.insere_obs_pis_cofins);
            diferencial_ativo_consumo = MicrovixConverters.ToNullableBoolean(dto.diferencial_ativo_consumo);
            recusa_de = MicrovixConverters.ToNullableBoolean(dto.recusa_de);
            codigo_ws = MicrovixConverters.ToNullableString(dto.codigo_ws, 50);
        }
    }
}
