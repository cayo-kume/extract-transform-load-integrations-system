using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores
{
    /// <summary>
    /// cadastro de uma loja (LinxLojas), tabela linxmicrovix.LinxLojas. Chave: cnpj_emp, a loja consultada
    /// </summary>
    public class LinxLojas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public string? nome_emp { get; private set; }
        public string? razao_emp { get; private set; }
        public string? inscricao_emp { get; private set; }
        public string? endereco_emp { get; private set; }
        public int? num_emp { get; private set; }
        public string? complement_emp { get; private set; }
        public string? bairro_emp { get; private set; }
        public string? cep_emp { get; private set; }
        public string? cidade_emp { get; private set; }
        public string? estado_emp { get; private set; }
        public string? fone_emp { get; private set; }
        public string? email_emp { get; private set; }
        public int? cod_ibge_municipio { get; private set; }
        public DateTime? data_criacao_emp { get; private set; }
        public DateTime? data_criacao_portal { get; private set; }
        public string? sistema_tributacao { get; private set; }
        public int? regime_tributario { get; private set; }
        public decimal? area_empresa { get; private set; }
        public long timestamp { get; private set; }
        public string? sigla_empresa { get; private set; }
        public int? id_classe_fiscal { get; private set; }
        public bool? centro_distribuicao { get; private set; }
        public string? inscricao_municipal_emp { get; private set; }
        public string? cnae_emp { get; private set; }
        public string? cod_cliente_linx { get; private set; }
        public int? tabela_preco_preferencial { get; private set; }
        public string? subrede { get; private set; }
        public bool? produto_disponivel_empresa { get; private set; }
        public int? id_grupo { get; private set; }

        public LinxLojas() { }

        public LinxLojas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stores.LinxLojas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            nome_emp = MicrovixConverters.ToNullableString(dto.nome_emp, 50);
            razao_emp = MicrovixConverters.ToNullableString(dto.razao_emp, 200);
            inscricao_emp = MicrovixConverters.ToNullableString(dto.inscricao_emp, 20);
            endereco_emp = MicrovixConverters.ToNullableString(dto.endereco_emp, 250);
            num_emp = MicrovixConverters.ToNullableInt32(dto.num_emp);
            complement_emp = MicrovixConverters.ToNullableString(dto.complement_emp, 60);
            bairro_emp = MicrovixConverters.ToNullableString(dto.bairro_emp, 50);
            cep_emp = MicrovixConverters.ToNullableString(dto.cep_emp, 9);
            cidade_emp = MicrovixConverters.ToNullableString(dto.cidade_emp, 50);
            estado_emp = MicrovixConverters.ToNullableString(dto.estado_emp, 2);
            fone_emp = MicrovixConverters.ToNullableString(dto.fone_emp, 70);
            email_emp = MicrovixConverters.ToNullableString(dto.email_emp, 50);
            cod_ibge_municipio = MicrovixConverters.ToNullableInt32(dto.cod_ibge_municipio);
            data_criacao_emp = MicrovixConverters.ToNullableDateTime(dto.data_criacao_emp);
            data_criacao_portal = MicrovixConverters.ToNullableDateTime(dto.data_criacao_portal);
            sistema_tributacao = MicrovixConverters.ToNullableString(dto.sistema_tributacao, 1);
            regime_tributario = MicrovixConverters.ToNullableInt32(dto.regime_tributario);
            area_empresa = MicrovixConverters.ToNullableDecimal(dto.area_empresa);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            sigla_empresa = MicrovixConverters.ToNullableString(dto.sigla_empresa, 10);
            id_classe_fiscal = MicrovixConverters.ToNullableInt32(dto.id_classe_fiscal);
            centro_distribuicao = MicrovixConverters.ToNullableBoolean(dto.centro_distribuicao);
            inscricao_municipal_emp = MicrovixConverters.ToNullableString(dto.inscricao_municipal_emp, 30);   //10 na v274; 30 no código antigo
            cnae_emp = MicrovixConverters.ToNullableString(dto.cnae_emp, 7);
            cod_cliente_linx = MicrovixConverters.ToNullableString(dto.cod_cliente_linx, 6);
            tabela_preco_preferencial = MicrovixConverters.ToNullableInt32(dto.tabela_preco_preferencial);
            subrede = MicrovixConverters.ToNullableString(dto.subrede, 100);
            produto_disponivel_empresa = MicrovixConverters.ToNullableBoolean(dto.produto_disponivel_empresa);
            id_grupo = MicrovixConverters.ToNullableInt32(dto.id_grupo);
        }
    }
}
