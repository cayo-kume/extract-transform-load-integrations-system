using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Payments
{
    /// <summary>
    /// LinxMovimentoPlanos, tabela linxmicrovix.LinxMovimentoPlanos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador + plano + ordem_cartao
    /// </summary>
    public class LinxMovimentoPlanos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public Guid identificador { get; private set; }
        public int plano { get; private set; }
        public string? desc_plano { get; private set; }
        public decimal? total { get; private set; }
        public int? qtde_parcelas { get; private set; }
        public decimal? indice_plano { get; private set; }
        public int? cod_forma_pgto { get; private set; }
        public string? forma_pgto { get; private set; }
        public string? tipo_transacao { get; private set; }
        public decimal? taxa_financeira { get; private set; }
        public int ordem_cartao { get; private set; }
        public long timestamp { get; private set; }
        public int? empresa { get; private set; }
        public bool? gerado_config_receb_vendas { get; private set; }

        public LinxMovimentoPlanos() { }

        public LinxMovimentoPlanos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Payments.LinxMovimentoPlanos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador) ?? Guid.Empty;
            plano = MicrovixConverters.ToInt32(dto.plano);
            desc_plano = MicrovixConverters.ToNullableString(dto.desc_plano, 100);
            total = MicrovixConverters.ToNullableDecimal(dto.total);
            qtde_parcelas = MicrovixConverters.ToNullableInt32(dto.qtde_parcelas);
            indice_plano = MicrovixConverters.ToNullableDecimal(dto.indice_plano);
            cod_forma_pgto = MicrovixConverters.ToNullableInt32(dto.cod_forma_pgto);
            forma_pgto = MicrovixConverters.ToNullableString(dto.forma_pgto, 50);
            tipo_transacao = MicrovixConverters.ToNullableString(dto.tipo_transacao, 1);
            taxa_financeira = MicrovixConverters.ToNullableDecimal(dto.taxa_financeira);
            ordem_cartao = MicrovixConverters.ToInt32(dto.ordem_cartao);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            gerado_config_receb_vendas = MicrovixConverters.ToNullableBoolean(dto.gerado_config_receb_vendas);
        }
    }
}
