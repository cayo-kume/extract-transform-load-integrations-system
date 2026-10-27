using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores
{
    /// <summary>
    /// parâmetros do portal para uma loja (LinxLojasParametros), tabela linxmicrovix.LinxLojasParametros. A resposta não traz
    /// o CNPJ: a chave é cnpj_emp, a loja consultada
    /// </summary>
    public class LinxLojasParametros
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public decimal? percentual_minimo_antecipacao { get; private set; }
        public long timestamp { get; private set; }

        public LinxLojasParametros() { }

        public LinxLojasParametros(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stores.LinxLojasParametros dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            percentual_minimo_antecipacao = MicrovixConverters.ToNullableDecimal(dto.percentual_minimo_antecipacao);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
