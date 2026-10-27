using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields
{
    /// <summary>
    /// B2CConsultaB2CProdutosCamposAdicionais, tabela linxmicrovix.B2CConsultaB2CProdutosCamposAdicionais. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id_b2c_produtos_campos_adicionais (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionais
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_b2c_produtos_campos_adicionais { get; private set; }
        public string? nome_campo { get; private set; }
        public string? tipo { get; private set; }
        public int? decimais { get; private set; }
        public bool? multipla_escolha { get; private set; }
        public int? ordem { get; private set; }
        public bool? ativo { get; private set; }
        public int? nivel_id { get; private set; }
        public string? nivel_descricao { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public B2CConsultaB2CProdutosCamposAdicionais() { }

        public B2CConsultaB2CProdutosCamposAdicionais(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionais dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_b2c_produtos_campos_adicionais = MicrovixConverters.ToInt32(dto.id_b2c_produtos_campos_adicionais);
            nome_campo = String.IsNullOrEmpty(dto.nome_campo) ? null : dto.nome_campo;
            tipo = String.IsNullOrEmpty(dto.tipo) ? null : dto.tipo;
            decimais = MicrovixConverters.ToNullableInt32(dto.decimais);
            multipla_escolha = MicrovixConverters.ToNullableBoolean(dto.multipla_escolha);
            ordem = MicrovixConverters.ToNullableInt32(dto.ordem);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            nivel_id = MicrovixConverters.ToNullableInt32(dto.nivel_id);
            nivel_descricao = String.IsNullOrEmpty(dto.nivel_descricao) ? null : dto.nivel_descricao;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
