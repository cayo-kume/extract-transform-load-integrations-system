using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxNFeEvento, tabela linxmicrovix.LinxNFeEvento. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_nfe_evento
    /// </summary>
    public class LinxNFeEvento
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_nfe_evento { get; private set; }
        public int? id_nfe { get; private set; }
        public string? codigo_tipo { get; private set; }
        public string? xml { get; private set; }
        public long timestamp { get; private set; }
        public DateTime? data_emissao { get; private set; }
        public string? hora_emissao { get; private set; }

        public LinxNFeEvento() { }

        public LinxNFeEvento(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxNFeEvento dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_nfe_evento = MicrovixConverters.ToInt32(dto.id_nfe_evento);
            id_nfe = MicrovixConverters.ToNullableInt32(dto.id_nfe);
            codigo_tipo = MicrovixConverters.ToNullableString(dto.codigo_tipo, 6);
            xml = String.IsNullOrEmpty(dto.xml) ? null : dto.xml;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            data_emissao = MicrovixConverters.ToNullableDateTime(dto.data_emissao);
            hora_emissao = MicrovixConverters.ToNullableString(dto.hora_emissao, 20);   //5 na documentação; a resposta real tem até 8
        }
    }
}
