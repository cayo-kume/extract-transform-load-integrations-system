using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Consignments
{
    /// <summary>
    /// LinxMovimentoRemessas, tabela linxmicrovix.LinxMovimentoRemessas. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_remessas
    /// </summary>
    public class LinxMovimentoRemessas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public int id_remessas { get; private set; }
        public int? id_tipo { get; private set; }
        public Guid? identificador_remessa { get; private set; }
        public int? status { get; private set; }
        public string? status_descricao { get; private set; }
        public long timestamp { get; private set; }

        public LinxMovimentoRemessas() { }

        public LinxMovimentoRemessas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Consignments.LinxMovimentoRemessas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_remessas = MicrovixConverters.ToInt32(dto.id_remessas);
            id_tipo = MicrovixConverters.ToNullableInt32(dto.id_tipo);
            identificador_remessa = MicrovixConverters.ToNullableGuid(dto.identificador_remessa);
            status = MicrovixConverters.ToNullableInt32(dto.status);
            status_descricao = MicrovixConverters.ToNullableString(dto.status_descricao, 35);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
