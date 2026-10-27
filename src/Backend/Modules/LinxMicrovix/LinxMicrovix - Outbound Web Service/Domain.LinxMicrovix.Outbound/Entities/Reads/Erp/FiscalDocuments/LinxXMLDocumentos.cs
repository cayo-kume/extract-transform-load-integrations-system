using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxXMLDocumentos, tabela linxmicrovix.LinxXMLDocumentos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador_microvix
    /// </summary>
    public class LinxXMLDocumentos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? documento { get; private set; }
        public string? serie { get; private set; }
        public DateTime? data_emissao { get; private set; }
        public string? chave_nfe { get; private set; }
        public int? situacao { get; private set; }
        public string? xml { get; private set; }
        public bool? excluido { get; private set; }
        public Guid identificador_microvix { get; private set; }
        public DateTime? dt_insert { get; private set; }
        public long timestamp { get; private set; }
        public string? nProtCanc { get; private set; }
        public string? nProtInut { get; private set; }
        public string? xmlDistribuicao { get; private set; }
        public string? nProtDeneg { get; private set; }
        public string? cStat { get; private set; }
        public int? id_nfe { get; private set; }
        public string? justificativa { get; private set; }
        public int? cod_cliente { get; private set; }

        public LinxXMLDocumentos() { }

        public LinxXMLDocumentos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxXMLDocumentos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            documento = MicrovixConverters.ToNullableInt32(dto.documento);
            serie = MicrovixConverters.ToNullableString(dto.serie, 10);
            data_emissao = MicrovixConverters.ToNullableDateTime(dto.data_emissao);
            chave_nfe = MicrovixConverters.ToNullableString(dto.chave_nfe, 44);
            situacao = MicrovixConverters.ToNullableInt32(dto.situacao);
            xml = String.IsNullOrEmpty(dto.xml) ? null : dto.xml;
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            identificador_microvix = MicrovixConverters.ToNullableGuid(dto.identificador_microvix) ?? Guid.Empty;
            dt_insert = MicrovixConverters.ToNullableDateTime(dto.dt_insert);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            nProtCanc = MicrovixConverters.ToNullableString(dto.nProtCanc, 20);
            nProtInut = MicrovixConverters.ToNullableString(dto.nProtInut, 20);
            xmlDistribuicao = String.IsNullOrEmpty(dto.xmlDistribuicao) ? null : dto.xmlDistribuicao;
            nProtDeneg = MicrovixConverters.ToNullableString(dto.nProtDeneg, 15);
            cStat = MicrovixConverters.ToNullableString(dto.cStat, 5);
            id_nfe = MicrovixConverters.ToNullableInt32(dto.id_nfe);
            justificativa = MicrovixConverters.ToNullableString(dto.justificativa, 255);
            cod_cliente = MicrovixConverters.ToNullableInt32(dto.cod_cliente);
        }
    }
}
