using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores
{
    /// <summary>
    /// loja do grupo (LinxGrupoLojas), tabela linxmicrovix.LinxGrupoLojas. Chave: portal + empresa (o CNPJ não serve: uma
    /// loja vem com ele vazio, testado). O método não tem timestamp: o grupo é relido inteiro a cada execução
    /// </summary>
    public class LinxGrupoLojas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int empresa { get; private set; }
        public string? CNPJ { get; private set; }
        public string? nome_empresa { get; private set; }
        public int? id_empresas_rede { get; private set; }
        public string? rede { get; private set; }
        public string? nome_portal { get; private set; }
        public bool? lojas_proprias { get; private set; }
        public string? classificacao_portal { get; private set; }
        public bool? emp_mod_linx_promo { get; private set; }

        public LinxGrupoLojas() { }

        public LinxGrupoLojas(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stores.LinxGrupoLojas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            empresa = MicrovixConverters.ToInt32(dto.empresa);
            CNPJ = MicrovixConverters.ToNullableString(dto.CNPJ, 14);
            nome_empresa = MicrovixConverters.ToNullableString(dto.nome_empresa, 50);
            id_empresas_rede = MicrovixConverters.ToNullableInt32(dto.id_empresas_rede);
            rede = MicrovixConverters.ToNullableString(dto.rede, 100);
            nome_portal = MicrovixConverters.ToNullableString(dto.nome_portal, 50);
            lojas_proprias = MicrovixConverters.ToNullableBoolean(dto.lojas_proprias);
            classificacao_portal = MicrovixConverters.ToNullableString(dto.classificacao_portal, 50);
            emp_mod_linx_promo = MicrovixConverters.ToNullableBoolean(dto.emp_mod_linx_promo);   //fora da v274
        }
    }
}
