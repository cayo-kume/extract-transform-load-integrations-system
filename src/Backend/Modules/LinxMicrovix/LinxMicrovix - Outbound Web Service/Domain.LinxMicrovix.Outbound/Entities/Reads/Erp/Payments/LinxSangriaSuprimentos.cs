using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Payments
{
    /// <summary>
    /// LinxSangriaSuprimentos, tabela linxmicrovix.LinxSangriaSuprimentos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_sangria_suprimentos (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxSangriaSuprimentos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? usuario { get; private set; }
        public string? data { get; private set; }
        public decimal? valor { get; private set; }
        public string? obs { get; private set; }
        public string? cancelado { get; private set; }
        public bool? conferido { get; private set; }
        public long? cod_historico { get; private set; }
        public string? desc_historico { get; private set; }
        public int id_sangria_suprimentos { get; private set; }
        public long timestamp { get; private set; }

        public LinxSangriaSuprimentos() { }

        public LinxSangriaSuprimentos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Payments.LinxSangriaSuprimentos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            usuario = MicrovixConverters.ToNullableInt32(dto.usuario);
            data = MicrovixConverters.ToNullableString(dto.data, 1000);   //fora da documentação
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            obs = String.IsNullOrEmpty(dto.obs) ? null : dto.obs;
            cancelado = MicrovixConverters.ToNullableString(dto.cancelado, 1);
            conferido = MicrovixConverters.ToNullableBoolean(dto.conferido);
            cod_historico = MicrovixConverters.ToNullableInt64(dto.cod_historico);
            desc_historico = MicrovixConverters.ToNullableString(dto.desc_historico, 50);
            id_sangria_suprimentos = MicrovixConverters.ToInt32(dto.id_sangria_suprimentos);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
