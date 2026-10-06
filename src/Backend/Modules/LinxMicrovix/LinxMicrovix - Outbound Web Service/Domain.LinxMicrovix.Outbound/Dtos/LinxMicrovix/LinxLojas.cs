namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxLojas
    {
        public string? portal { get; set; }
        public string? empresa { get; set; }
        public string? nome_emp { get; set; }
        public string? razao_emp { get; set; }
        public string? cnpj_emp { get; set; }
        public string? inscricao_emp { get; set; }
        public string? endereco_emp { get; set; }
        public string? num_emp { get; set; }
        public string? complement_emp { get; set; }
        public string? bairro_emp { get; set; }
        public string? cep_emp { get; set; }
        public string? cidade_emp { get; set; }
        public string? estado_emp { get; set; }
        public string? fone_emp { get; set; }
        public string? email_emp { get; set; }
        public string? cod_ibge_municipio { get; set; }
        public string? data_criacao_emp { get; set; }
        public string? data_criacao_portal { get; set; }
        public string? sistema_tributacao { get; set; }
        public string? regime_tributario { get; set; }
        public string? area_empresa { get; set; }
        public string? timestamp { get; set; }
        public string? sigla_empresa { get; set; }
        public string? id_classe_fiscal { get; set; }
        public string? centro_distribuicao { get; set; }
        public string? inscricao_municipal_emp { get; set; }
        public string? cnae_emp { get; set; }
        public string? cod_cliente_linx { get; set; }
        public string? tabela_preco_preferencial { get; set; }

        public LinxLojas()
        {
        }

        public LinxLojas(string? portal, string? empresa, string? nome_emp, string? razao_emp, string? cnpj_emp, string? inscricao_emp, string? endereco_emp, string? num_emp, string? complement_emp, string? bairro_emp, string? cep_emp, string? cidade_emp, string? estado_emp, string? fone_emp, string? email_emp, string? cod_ibge_municipio, string? data_criacao_emp, string? data_criacao_portal, string? sistema_tributacao, string? regime_tributario, string? area_empresa, string? timestamp, string? sigla_empresa, string? id_classe_fiscal, string? centro_distribuicao, string? inscricao_municipal_emp, string? cnae_emp, string? cod_cliente_linx, string? tabela_preco_preferencial)
        {
            this.portal = portal;
            this.empresa = empresa;
            this.nome_emp = nome_emp;
            this.razao_emp = razao_emp;
            this.cnpj_emp = cnpj_emp;
            this.inscricao_emp = inscricao_emp;
            this.endereco_emp = endereco_emp;
            this.num_emp = num_emp;
            this.complement_emp = complement_emp;
            this.bairro_emp = bairro_emp;
            this.cep_emp = cep_emp;
            this.cidade_emp = cidade_emp;
            this.estado_emp = estado_emp;
            this.fone_emp = fone_emp;
            this.email_emp = email_emp;
            this.cod_ibge_municipio = cod_ibge_municipio;
            this.data_criacao_emp = data_criacao_emp;
            this.data_criacao_portal = data_criacao_portal;
            this.sistema_tributacao = sistema_tributacao;
            this.regime_tributario = regime_tributario;
            this.area_empresa = area_empresa;
            this.timestamp = timestamp;
            this.sigla_empresa = sigla_empresa;
            this.id_classe_fiscal = id_classe_fiscal;
            this.centro_distribuicao = centro_distribuicao;
            this.inscricao_municipal_emp = inscricao_municipal_emp;
            this.cnae_emp = cnae_emp;
            this.cod_cliente_linx = cod_cliente_linx;
            this.tabela_preco_preferencial = tabela_preco_preferencial;
        }
    }
}
