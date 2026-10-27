using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Payments
{
    /// <summary>
    /// LinxPlanos, tabela linxmicrovix.LinxPlanos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + plano
    /// </summary>
    public class LinxPlanos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int plano { get; private set; }
        public string? desc_plano { get; private set; }
        public int? qtde_parcelas { get; private set; }
        public int? prazo_entre_parcelas { get; private set; }
        public string? tipo_plano { get; private set; }
        public decimal? indice_plano { get; private set; }
        public int? cod_forma_pgto { get; private set; }
        public string? forma_pgto { get; private set; }
        public int? conta_central { get; private set; }
        public string? tipo_transacao { get; private set; }
        public decimal? taxa_financeira { get; private set; }
        public DateTime? dt_upd { get; private set; }
        public string? desativado { get; private set; }
        public string? usa_tef { get; private set; }
        public long timestamp { get; private set; }
        public string? codigo_ws { get; private set; }
        public decimal? valor_minimo_parcela { get; private set; }
        public bool? gerado_config_receb_vendas { get; private set; }

        public LinxPlanos() { }

        public LinxPlanos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Payments.LinxPlanos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            plano = MicrovixConverters.ToInt32(dto.plano);
            desc_plano = MicrovixConverters.ToNullableString(dto.desc_plano, 100);
            qtde_parcelas = MicrovixConverters.ToNullableInt32(dto.qtde_parcelas);
            prazo_entre_parcelas = MicrovixConverters.ToNullableInt32(dto.prazo_entre_parcelas);
            tipo_plano = MicrovixConverters.ToNullableString(dto.tipo_plano, 1);
            indice_plano = MicrovixConverters.ToNullableDecimal(dto.indice_plano);
            cod_forma_pgto = MicrovixConverters.ToNullableInt32(dto.cod_forma_pgto);
            forma_pgto = MicrovixConverters.ToNullableString(dto.forma_pgto, 50);
            conta_central = MicrovixConverters.ToNullableInt32(dto.conta_central);
            tipo_transacao = MicrovixConverters.ToNullableString(dto.tipo_transacao, 1);
            taxa_financeira = MicrovixConverters.ToNullableDecimal(dto.taxa_financeira);
            dt_upd = MicrovixConverters.ToNullableDateTime(dto.dt_upd);
            desativado = MicrovixConverters.ToNullableString(dto.desativado, 1);
            usa_tef = MicrovixConverters.ToNullableString(dto.usa_tef, 1);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            codigo_ws = MicrovixConverters.ToNullableString(dto.codigo_ws, 50);
            valor_minimo_parcela = MicrovixConverters.ToNullableDecimal(dto.valor_minimo_parcela);   //fora da documentação
            gerado_config_receb_vendas = MicrovixConverters.ToNullableBoolean(dto.gerado_config_receb_vendas);
        }
    }
}
