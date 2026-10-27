using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxEntradaNFe, tabela linxmicrovix.LinxEntradaNFe. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_entrada_nfe
    /// </summary>
    public class LinxEntradaNFe
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_entrada_nfe { get; private set; }
        public int? numero_nfe { get; private set; }
        public bool? lancada_no_microvix { get; private set; }
        public int? empresa { get; private set; }
        public string? documento_emitente { get; private set; }
        public string? razao_social_emitente { get; private set; }
        public int? serie_nfe { get; private set; }
        public DateTime? data_emissao_nfe { get; private set; }
        public string? chave_nfe { get; private set; }
        public bool? excluida { get; private set; }
        public DateTime? dt_insert { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxEntradaNFe() { }

        public LinxEntradaNFe(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxEntradaNFe dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_entrada_nfe = MicrovixConverters.ToInt32(dto.id_entrada_nfe);
            numero_nfe = MicrovixConverters.ToNullableInt32(dto.numero_nfe);
            lancada_no_microvix = MicrovixConverters.ToNullableBoolean(dto.lancada_no_microvix);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            documento_emitente = MicrovixConverters.ToNullableString(dto.documento_emitente, 14);
            razao_social_emitente = MicrovixConverters.ToNullableString(dto.razao_social_emitente, 60);
            serie_nfe = MicrovixConverters.ToNullableInt32(dto.serie_nfe);
            data_emissao_nfe = MicrovixConverters.ToNullableDateTime(dto.data_emissao_nfe);
            chave_nfe = MicrovixConverters.ToNullableString(dto.chave_nfe, 44);
            excluida = MicrovixConverters.ToNullableBoolean(dto.excluida);
            dt_insert = MicrovixConverters.ToNullableDateTime(dto.dt_insert);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
