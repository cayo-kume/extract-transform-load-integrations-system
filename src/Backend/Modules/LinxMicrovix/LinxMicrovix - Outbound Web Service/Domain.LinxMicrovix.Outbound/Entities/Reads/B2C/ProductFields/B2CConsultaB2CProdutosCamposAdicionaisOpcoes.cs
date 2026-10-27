using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields
{
    /// <summary>
    /// B2CConsultaB2CProdutosCamposAdicionaisOpcoes, tabela linxmicrovix.B2CConsultaB2CProdutosCamposAdicionaisOpcoes. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id_b2c_produtos_campos_adicionais_opcoes (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaB2CProdutosCamposAdicionaisOpcoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_b2c_produtos_campos_adicionais_opcoes { get; private set; }
        public int? id_b2c_produtos_campos_adicionais { get; private set; }
        public string? valor { get; private set; }
        public int? ordem { get; private set; }
        public bool? ativo { get; private set; }
        public long? categoria_nivel { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public B2CConsultaB2CProdutosCamposAdicionaisOpcoes() { }

        public B2CConsultaB2CProdutosCamposAdicionaisOpcoes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisOpcoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_b2c_produtos_campos_adicionais_opcoes = MicrovixConverters.ToInt32(dto.id_b2c_produtos_campos_adicionais_opcoes);
            id_b2c_produtos_campos_adicionais = MicrovixConverters.ToNullableInt32(dto.id_b2c_produtos_campos_adicionais);
            valor = String.IsNullOrEmpty(dto.valor) ? null : dto.valor;
            ordem = MicrovixConverters.ToNullableInt32(dto.ordem);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            categoria_nivel = MicrovixConverters.ToNullableInt64(dto.categoria_nivel);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
