using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings
{
    /// <summary>
    /// LinxConfiguracoesTributariasEmpresas, tabela linxmicrovix.LinxConfiguracoesTributariasEmpresas. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_config_tributaria + empresa
    /// </summary>
    public class LinxConfiguracoesTributariasEmpresas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_config_tributaria { get; private set; }
        public int empresa { get; private set; }
        public long timestamp { get; private set; }

        public LinxConfiguracoesTributariasEmpresas() { }

        public LinxConfiguracoesTributariasEmpresas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasEmpresas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_config_tributaria = MicrovixConverters.ToInt32(dto.id_config_tributaria);
            empresa = MicrovixConverters.ToInt32(dto.empresa);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
