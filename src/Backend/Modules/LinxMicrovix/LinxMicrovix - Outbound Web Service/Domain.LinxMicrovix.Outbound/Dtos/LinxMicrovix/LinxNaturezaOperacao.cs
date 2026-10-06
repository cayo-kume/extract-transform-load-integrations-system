

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxNaturezaOperacao
    {
        public string? portal { get; private set; }
        public string? cod_natureza_operacao { get; private set; }
        public string? descricao { get; private set; }
        public string? soma_relatorios { get; private set; }
        public string? operacao { get; private set; }
        public string? inativa { get; private set; }
        public string? timestamp { get; private set; }
        public string? calcula_ipi { get; private set; }
        public string? calcula_iss { get; private set; }
        public string? calcula_irrf { get; private set; }
        public string? tipo_preco { get; private set; }
        public string? atualiza_custo { get; private set; }
        public string? transferencia { get; private set; }
        public string? baixar_estoque { get; private set; }
        public string? consumo_proprio { get; private set; }
        public string? contabiliza_cmv { get; private set; }
        public string? despesa { get; private set; }
        public string? atualiza_custo_medio { get; private set; }
        public string? exige_nf_origem { get; private set; }
        public string? integra_contabilidade { get; private set; }
        public string? id_obs { get; private set; }
        public string? venda_futura { get; private set; }
        public string? base_icms_considera_ipi { get; private set; }
        public string? permite_escolha_historico { get; private set; }
        public string? import_produtos { get; private set; }
        public string? deposito_reserva_venda { get; private set; }
        public string? exibe_nfe { get; private set; }
        public string? faturamento_antecipado { get; private set; }
        public string? exibir_informacoes_imposto { get; private set; }
        public string? gera_garantia_nacional { get; private set; }
        public string? transferencia_deposito { get; private set; }
        public string? venda_diferencial_aliquota { get; private set; }
        public string? insere_obs_pis_cofins { get; private set; }
        public string? diferencial_ativo_consumo { get; private set; }
        public string? recusa_de { get; private set; }
        public string? codigo_ws { get; private set; }

        public LinxNaturezaOperacao() { }

        public LinxNaturezaOperacao(
            string? portal,
            string? cod_natureza_operacao,
            string? descricao,
            string? soma_relatorios,
            string? operacao,
            string? inativa,
            string? timestamp,
            string? calcula_ipi,
            string? calcula_iss,
            string? calcula_irrf,
            string? tipo_preco,
            string? atualiza_custo,
            string? transferencia,
            string? baixar_estoque,
            string? consumo_proprio,
            string? contabiliza_cmv,
            string? despesa,
            string? atualiza_custo_medio,
            string? exige_nf_origem,
            string? integra_contabilidade,
            string? id_obs,
            string? venda_futura,
            string? base_icms_considera_ipi,
            string? permite_escolha_historico,
            string? import_produtos,
            string? deposito_reserva_venda,
            string? exibe_nfe,
            string? faturamento_antecipado,
            string? exibir_informacoes_imposto,
            string? gera_garantia_nacional,
            string? transferencia_deposito,
            string? venda_diferencial_aliquota,
            string? insere_obs_pis_cofins,
            string? diferencial_ativo_consumo,
            string? recusa_de,
            string? codigo_ws
        )
        {
            this.portal = portal;
            this.cod_natureza_operacao = cod_natureza_operacao;
            this.descricao = descricao;
            this.soma_relatorios = soma_relatorios;
            this.operacao = operacao;
            this.inativa = inativa;
            this.timestamp = timestamp;
            this.calcula_ipi = calcula_ipi;
            this.calcula_iss = calcula_iss;
            this.calcula_irrf = calcula_irrf;
            this.tipo_preco = tipo_preco;
            this.atualiza_custo = atualiza_custo;
            this.transferencia = transferencia;
            this.baixar_estoque = baixar_estoque;
            this.consumo_proprio = consumo_proprio;
            this.contabiliza_cmv = contabiliza_cmv;
            this.despesa = despesa;
            this.atualiza_custo_medio = atualiza_custo_medio;
            this.exige_nf_origem = exige_nf_origem;
            this.integra_contabilidade = integra_contabilidade;
            this.id_obs = id_obs;
            this.venda_futura = venda_futura;
            this.base_icms_considera_ipi = base_icms_considera_ipi;
            this.permite_escolha_historico = permite_escolha_historico;
            this.import_produtos = import_produtos;
            this.deposito_reserva_venda = deposito_reserva_venda;
            this.exibe_nfe = exibe_nfe;
            this.faturamento_antecipado = faturamento_antecipado;
            this.exibir_informacoes_imposto = exibir_informacoes_imposto;
            this.gera_garantia_nacional = gera_garantia_nacional;
            this.transferencia_deposito = transferencia_deposito;
            this.venda_diferencial_aliquota = venda_diferencial_aliquota;
            this.insere_obs_pis_cofins = insere_obs_pis_cofins;
            this.diferencial_ativo_consumo = diferencial_ativo_consumo;
            this.recusa_de = recusa_de;
            this.codigo_ws = codigo_ws;
        }
    }
}