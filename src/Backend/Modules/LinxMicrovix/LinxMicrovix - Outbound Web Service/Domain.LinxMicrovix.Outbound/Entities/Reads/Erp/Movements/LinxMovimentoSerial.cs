using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxMovimentoSerial, tabela linxmicrovix.LinxMovimentoSerial. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador + transacao + serial (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxMovimentoSerial
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public Guid identificador { get; private set; }
        public int transacao { get; private set; }
        public string serial { get; private set; } = String.Empty;
        public long timestamp { get; private set; }

        public LinxMovimentoSerial() { }

        public LinxMovimentoSerial(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxMovimentoSerial dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador) ?? Guid.Empty;
            transacao = MicrovixConverters.ToInt32(dto.transacao);
            serial = MicrovixConverters.ToNullableString(dto.serial, 50) ?? String.Empty;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
