using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Stores
{
    /// <summary>
    /// B2CConsultaCNPJsChave, tabela linxmicrovix.B2CConsultaCNPJsChave. Método de portal (sem cnpjEmp): chave portal + CNPJ
    /// </summary>
    public class B2CConsultaCNPJsChave
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string CNPJ { get; private set; } = String.Empty;
        public string? nome_empresa { get; private set; }
        public int? id_empresas_rede { get; private set; }
        public string? rede { get; private set; }
        public int portal { get; private set; }
        public string? nome_portal { get; private set; }
        public int? empresa { get; private set; }
        public string? classificacao_portal { get; private set; }
        public bool? b2c { get; private set; }
        public bool? oms { get; private set; }
        public bool? b2c_marketplace { get; private set; }

        public B2CConsultaCNPJsChave() { }

        public B2CConsultaCNPJsChave(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Stores.B2CConsultaCNPJsChave dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            CNPJ = MicrovixConverters.ToNullableString(dto.CNPJ, 14) ?? String.Empty;
            nome_empresa = MicrovixConverters.ToNullableString(dto.nome_empresa, 300);
            id_empresas_rede = MicrovixConverters.ToNullableInt32(dto.id_empresas_rede);
            rede = MicrovixConverters.ToNullableString(dto.rede, 100);
            portal = MicrovixConverters.ToInt32(dto.portal);
            nome_portal = MicrovixConverters.ToNullableString(dto.nome_portal, 50);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            classificacao_portal = MicrovixConverters.ToNullableString(dto.classificacao_portal, 50);
            b2c = MicrovixConverters.ToNullableBoolean(dto.b2c);
            oms = MicrovixConverters.ToNullableBoolean(dto.oms);
            b2c_marketplace = MicrovixConverters.ToNullableBoolean(dto.b2c_marketplace);
        }
    }
}
