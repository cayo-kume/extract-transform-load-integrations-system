using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Payments
{
    /// <summary>
    /// LinxPlanosBandeiras, tabela linxmicrovix.LinxPlanosBandeiras. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + plano + bandeira
    /// </summary>
    public class LinxPlanosBandeiras
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int plano { get; private set; }
        public string bandeira { get; private set; } = String.Empty;
        public string? tipo_bandeira { get; private set; }
        public int? adquirente { get; private set; }
        public string? nome_adquirente { get; private set; }
        public string? codigo_bandeira_sitef { get; private set; }
        public long timestamp { get; private set; }

        public LinxPlanosBandeiras() { }

        public LinxPlanosBandeiras(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Payments.LinxPlanosBandeiras dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            plano = MicrovixConverters.ToInt32(dto.plano);
            bandeira = MicrovixConverters.ToNullableString(dto.bandeira, 30) ?? String.Empty;
            tipo_bandeira = MicrovixConverters.ToNullableString(dto.tipo_bandeira, 100);
            adquirente = MicrovixConverters.ToNullableInt32(dto.adquirente);
            nome_adquirente = MicrovixConverters.ToNullableString(dto.nome_adquirente, 60);
            codigo_bandeira_sitef = MicrovixConverters.ToNullableString(dto.codigo_bandeira_sitef, 10);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
