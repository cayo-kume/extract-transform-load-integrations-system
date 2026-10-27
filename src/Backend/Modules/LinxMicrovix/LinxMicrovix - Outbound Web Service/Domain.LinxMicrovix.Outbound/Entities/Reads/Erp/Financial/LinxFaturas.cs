using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxFaturas, tabela linxmicrovix.LinxFaturas. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + codigo_fatura + ordem_parcela
    /// </summary>
    public class LinxFaturas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long codigo_fatura { get; private set; }
        public DateTime? data_emissao { get; private set; }
        public int? cod_cliente { get; private set; }
        public string? nome_cliente { get; private set; }
        public DateTime? data_vencimento { get; private set; }
        public DateTime? data_baixa { get; private set; }
        public decimal? valor_fatura { get; private set; }
        public decimal? valor_pago { get; private set; }
        public decimal? valor_desconto { get; private set; }
        public decimal? valor_juros { get; private set; }
        public int? documento { get; private set; }
        public string? serie { get; private set; }
        public int? ecf { get; private set; }
        public string? observacao { get; private set; }
        public int? qtde_parcelas { get; private set; }
        public int ordem_parcela { get; private set; }
        public string? receber_pagar { get; private set; }
        public int? vendedor { get; private set; }
        public string? excluido { get; private set; }
        public string? cancelado { get; private set; }
        public Guid? identificador { get; private set; }
        public string? NSU { get; private set; }
        public string? cod_autorizacao { get; private set; }
        public string? documento_sem_tef { get; private set; }
        public string? autorizacao_sem_tef { get; private set; }
        public int? plano { get; private set; }
        public long? conta_credito { get; private set; }
        public long? conta_debito { get; private set; }
        public long? conta_fluxo { get; private set; }
        public long? cod_historico { get; private set; }
        public string? forma_pgto { get; private set; }
        public int? ordem_cartao { get; private set; }
        public string? banco_codigo { get; private set; }
        public string? banco_agencia { get; private set; }
        public string? banco_conta { get; private set; }
        public string? banco_autorizacao_garantidora { get; private set; }
        public string? numero_bilhete_seguro { get; private set; }
        public long timestamp { get; private set; }
        public int? empresa { get; private set; }
        public int? id_categorias_financeiras { get; private set; }
        public decimal? taxa_financeira { get; private set; }
        public decimal? valor_abatimento { get; private set; }
        public decimal? valor_multa { get; private set; }
        public int? centrocusto { get; private set; }
        public decimal? perc_taxa_adquirente { get; private set; }
        public string? fatura_origem_importacao_erp { get; private set; }
        public int? cod_portador { get; private set; }
        public string? nsu_host { get; private set; }
        public string? nsu_sitef { get; private set; }
        public string? previsao { get; private set; }
        public DateTime? data_lanc { get; private set; }
        public DateTime? dt_insert { get; private set; }
        public int? plano_pagamento_baixa { get; private set; }

        public LinxFaturas() { }

        public LinxFaturas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxFaturas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            codigo_fatura = MicrovixConverters.ToInt64(dto.codigo_fatura);
            data_emissao = MicrovixConverters.ToNullableDateTime(dto.data_emissao);
            cod_cliente = MicrovixConverters.ToNullableInt32(dto.cod_cliente);
            nome_cliente = MicrovixConverters.ToNullableString(dto.nome_cliente, 60);
            data_vencimento = MicrovixConverters.ToNullableDateTime(dto.data_vencimento);
            data_baixa = MicrovixConverters.ToNullableDateTime(dto.data_baixa);
            valor_fatura = MicrovixConverters.ToNullableDecimal(dto.valor_fatura);
            valor_pago = MicrovixConverters.ToNullableDecimal(dto.valor_pago);
            valor_desconto = MicrovixConverters.ToNullableDecimal(dto.valor_desconto);
            valor_juros = MicrovixConverters.ToNullableDecimal(dto.valor_juros);
            documento = MicrovixConverters.ToNullableInt32(dto.documento);
            serie = MicrovixConverters.ToNullableString(dto.serie, 10);
            ecf = MicrovixConverters.ToNullableInt32(dto.ecf);
            observacao = String.IsNullOrEmpty(dto.observacao) ? null : dto.observacao;
            qtde_parcelas = MicrovixConverters.ToNullableInt32(dto.qtde_parcelas);
            ordem_parcela = MicrovixConverters.ToInt32(dto.ordem_parcela);
            receber_pagar = MicrovixConverters.ToNullableString(dto.receber_pagar, 1);
            vendedor = MicrovixConverters.ToNullableInt32(dto.vendedor);
            excluido = MicrovixConverters.ToNullableString(dto.excluido, 1);
            cancelado = MicrovixConverters.ToNullableString(dto.cancelado, 1);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            NSU = MicrovixConverters.ToNullableString(dto.NSU, 50);
            cod_autorizacao = MicrovixConverters.ToNullableString(dto.cod_autorizacao, 50);
            documento_sem_tef = MicrovixConverters.ToNullableString(dto.documento_sem_tef, 350);
            autorizacao_sem_tef = MicrovixConverters.ToNullableString(dto.autorizacao_sem_tef, 30);
            plano = MicrovixConverters.ToNullableInt32(dto.plano);
            conta_credito = MicrovixConverters.ToNullableInt64(dto.conta_credito);
            conta_debito = MicrovixConverters.ToNullableInt64(dto.conta_debito);
            conta_fluxo = MicrovixConverters.ToNullableInt64(dto.conta_fluxo);
            cod_historico = MicrovixConverters.ToNullableInt64(dto.cod_historico);
            forma_pgto = MicrovixConverters.ToNullableString(dto.forma_pgto, 50);
            ordem_cartao = MicrovixConverters.ToNullableInt32(dto.ordem_cartao);
            banco_codigo = MicrovixConverters.ToNullableString(dto.banco_codigo, 10);
            banco_agencia = MicrovixConverters.ToNullableString(dto.banco_agencia, 30);
            banco_conta = MicrovixConverters.ToNullableString(dto.banco_conta, 30);
            banco_autorizacao_garantidora = MicrovixConverters.ToNullableString(dto.banco_autorizacao_garantidora, 30);
            numero_bilhete_seguro = MicrovixConverters.ToNullableString(dto.numero_bilhete_seguro, 30);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_categorias_financeiras = MicrovixConverters.ToNullableInt32(dto.id_categorias_financeiras);
            taxa_financeira = MicrovixConverters.ToNullableDecimal(dto.taxa_financeira);
            valor_abatimento = MicrovixConverters.ToNullableDecimal(dto.valor_abatimento);
            valor_multa = MicrovixConverters.ToNullableDecimal(dto.valor_multa);
            centrocusto = MicrovixConverters.ToNullableInt32(dto.centrocusto);
            perc_taxa_adquirente = MicrovixConverters.ToNullableDecimal(dto.perc_taxa_adquirente);
            fatura_origem_importacao_erp = MicrovixConverters.ToNullableString(dto.fatura_origem_importacao_erp, 50);
            cod_portador = MicrovixConverters.ToNullableInt32(dto.cod_portador);
            nsu_host = MicrovixConverters.ToNullableString(dto.nsu_host, 50);
            nsu_sitef = MicrovixConverters.ToNullableString(dto.nsu_sitef, 50);
            previsao = MicrovixConverters.ToNullableString(dto.previsao, 1);
            data_lanc = MicrovixConverters.ToNullableDateTime(dto.data_lanc);
            dt_insert = MicrovixConverters.ToNullableDateTime(dto.dt_insert);
            plano_pagamento_baixa = MicrovixConverters.ToNullableInt32(dto.plano_pagamento_baixa);
        }
    }
}
