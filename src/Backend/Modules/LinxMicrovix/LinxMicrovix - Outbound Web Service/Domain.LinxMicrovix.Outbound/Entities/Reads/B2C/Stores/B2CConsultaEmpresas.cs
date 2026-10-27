using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Stores
{
    /// <summary>
    /// B2CConsultaEmpresas, tabela linxmicrovix.B2CConsultaEmpresas. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + empresa (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaEmpresas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int empresa { get; private set; }
        public string? nome_emp { get; private set; }
        public string? cnpjEmp { get; private set; }
        public string? end_unidade { get; private set; }
        public string? complemento_end_unidade { get; private set; }
        public string? nr_rua_unidade { get; private set; }
        public string? bairro_unidade { get; private set; }
        public string? cep_unidade { get; private set; }
        public string? cidade_unidade { get; private set; }
        public string? uf_unidade { get; private set; }
        public string? email_unidade { get; private set; }
        public long timestamp { get; private set; }
        public DateTime? data_criacao { get; private set; }
        public bool? centro_distribuicao { get; private set; }
        public int? portal { get; private set; }
        public bool? envia_produtos_b2c { get; private set; }
        public bool? envia_estoque_b2c { get; private set; }
        public bool? integracao_oms { get; private set; }

        public B2CConsultaEmpresas() { }

        public B2CConsultaEmpresas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Stores.B2CConsultaEmpresas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            empresa = MicrovixConverters.ToInt32(dto.empresa);
            nome_emp = MicrovixConverters.ToNullableString(dto.nome_emp, 50);
            cnpjEmp = MicrovixConverters.ToNullableString(dto.cnpjEmp, 14);
            end_unidade = MicrovixConverters.ToNullableString(dto.end_unidade, 250);
            complemento_end_unidade = MicrovixConverters.ToNullableString(dto.complemento_end_unidade, 60);
            nr_rua_unidade = MicrovixConverters.ToNullableString(dto.nr_rua_unidade, 20);
            bairro_unidade = MicrovixConverters.ToNullableString(dto.bairro_unidade, 60);
            cep_unidade = MicrovixConverters.ToNullableString(dto.cep_unidade, 9);
            cidade_unidade = MicrovixConverters.ToNullableString(dto.cidade_unidade, 50);
            uf_unidade = MicrovixConverters.ToNullableString(dto.uf_unidade, 2);
            email_unidade = MicrovixConverters.ToNullableString(dto.email_unidade, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            data_criacao = MicrovixConverters.ToNullableDateTime(dto.data_criacao);
            centro_distribuicao = MicrovixConverters.ToNullableBoolean(dto.centro_distribuicao);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            envia_produtos_b2c = MicrovixConverters.ToNullableBoolean(dto.envia_produtos_b2c);
            envia_estoque_b2c = MicrovixConverters.ToNullableBoolean(dto.envia_estoque_b2c);
            integracao_oms = MicrovixConverters.ToNullableBoolean(dto.integracao_oms);
        }
    }
}
