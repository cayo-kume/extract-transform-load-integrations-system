namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxGrupoLojas
    {
        public string? cnpj { get; set; }
        public string? nome_empresa { get; set; }
        public string? id_empresas_rede { get; set; }
        public string? rede { get; set; }
        public string? portal { get; set; }
        public string? nome_portal { get; set; }
        public string? empresa { get; set; }
        public string? classificacao_portal { get; set; }

        public LinxGrupoLojas()
        {
        }

        public LinxGrupoLojas(string? cnpj, string? nome_empresa, string? id_empresas_rede, string? rede, string? portal, string? nome_portal, string? empresa, string? classificacao_portal)
        {
            this.cnpj = cnpj;
            this.nome_empresa = nome_empresa;
            this.id_empresas_rede = id_empresas_rede;
            this.rede = rede;
            this.portal = portal;
            this.nome_portal = nome_portal;
            this.empresa = empresa;
            this.classificacao_portal = classificacao_portal;
        }
    }
}
