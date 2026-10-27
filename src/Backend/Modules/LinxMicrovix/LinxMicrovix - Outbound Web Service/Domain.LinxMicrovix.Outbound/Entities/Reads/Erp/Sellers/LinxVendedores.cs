using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Sellers
{
    /// <summary>
    /// LinxVendedores, tabela linxmicrovix.LinxVendedores. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_vendedor
    /// </summary>
    public class LinxVendedores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int cod_vendedor { get; private set; }
        public string? nome_vendedor { get; private set; }
        public string? tipo_vendedor { get; private set; }
        public string? end_vend_rua { get; private set; }
        public int? end_vend_numero { get; private set; }
        public string? end_vend_complemento { get; private set; }
        public string? end_vend_bairro { get; private set; }
        public string? end_vend_cep { get; private set; }
        public string? end_vend_cidade { get; private set; }
        public string? end_vend_uf { get; private set; }
        public string? fone_vendedor { get; private set; }
        public string? mail_vendedor { get; private set; }
        public DateTime? dt_upd { get; private set; }
        public string? cpf_vendedor { get; private set; }
        public string? ativo { get; private set; }
        public DateTime? data_admissao { get; private set; }
        public DateTime? data_saida { get; private set; }
        public long timestamp { get; private set; }
        public string? matricula { get; private set; }
        public int? id_tipo_venda { get; private set; }
        public string? descricao_tipo_venda { get; private set; }
        public string? cargo { get; private set; }
        public string? funcao { get; private set; }
        public string? nome_exibicao { get; private set; }
        public int? empresa { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public decimal? meta_peso { get; private set; }

        public LinxVendedores() { }

        public LinxVendedores(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Sellers.LinxVendedores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_vendedor = MicrovixConverters.ToInt32(dto.cod_vendedor);
            nome_vendedor = MicrovixConverters.ToNullableString(dto.nome_vendedor, 50);
            tipo_vendedor = MicrovixConverters.ToNullableString(dto.tipo_vendedor, 1);
            end_vend_rua = MicrovixConverters.ToNullableString(dto.end_vend_rua, 250);
            end_vend_numero = MicrovixConverters.ToNullableInt32(dto.end_vend_numero);
            end_vend_complemento = MicrovixConverters.ToNullableString(dto.end_vend_complemento, 60);
            end_vend_bairro = MicrovixConverters.ToNullableString(dto.end_vend_bairro, 60);
            end_vend_cep = MicrovixConverters.ToNullableString(dto.end_vend_cep, 9);
            end_vend_cidade = MicrovixConverters.ToNullableString(dto.end_vend_cidade, 60);
            end_vend_uf = MicrovixConverters.ToNullableString(dto.end_vend_uf, 2);
            fone_vendedor = MicrovixConverters.ToNullableString(dto.fone_vendedor, 30);
            mail_vendedor = MicrovixConverters.ToNullableString(dto.mail_vendedor, 50);
            dt_upd = MicrovixConverters.ToNullableDateTime(dto.dt_upd);
            cpf_vendedor = MicrovixConverters.ToNullableString(dto.cpf_vendedor, 11);
            ativo = MicrovixConverters.ToNullableString(dto.ativo, 1);
            data_admissao = MicrovixConverters.ToNullableDateTime(dto.data_admissao);
            data_saida = MicrovixConverters.ToNullableDateTime(dto.data_saida);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            matricula = MicrovixConverters.ToNullableString(dto.matricula, 30);
            id_tipo_venda = MicrovixConverters.ToNullableInt32(dto.id_tipo_venda);
            descricao_tipo_venda = MicrovixConverters.ToNullableString(dto.descricao_tipo_venda, 100);
            cargo = MicrovixConverters.ToNullableString(dto.cargo, 20);
            funcao = MicrovixConverters.ToNullableString(dto.funcao, 100);
            nome_exibicao = MicrovixConverters.ToNullableString(dto.nome_exibicao, 15);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            codigo_integracao_ws = MicrovixConverters.ToNullableString(dto.codigo_integracao_ws, 50);
            meta_peso = MicrovixConverters.ToNullableDecimal(dto.meta_peso);
        }
    }
}
