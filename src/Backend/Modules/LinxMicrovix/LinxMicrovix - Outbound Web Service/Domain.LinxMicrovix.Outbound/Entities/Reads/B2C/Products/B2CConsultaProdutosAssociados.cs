using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Products
{
    /// <summary>
    /// B2CConsultaProdutosAssociados, tabela linxmicrovix.B2CConsultaProdutosAssociados. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaProdutosAssociados
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id { get; private set; }
        public long? codigoproduto { get; private set; }
        public long? codigoproduto_associado { get; private set; }
        public decimal? coeficiente_desconto { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }
        public decimal? qtde_item { get; private set; }
        public bool? item_obrigatorio { get; private set; }

        public B2CConsultaProdutosAssociados() { }

        public B2CConsultaProdutosAssociados(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Products.B2CConsultaProdutosAssociados dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id = MicrovixConverters.ToInt32(dto.id);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            codigoproduto_associado = MicrovixConverters.ToNullableInt64(dto.codigoproduto_associado);
            coeficiente_desconto = MicrovixConverters.ToNullableDecimal(dto.coeficiente_desconto);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            qtde_item = MicrovixConverters.ToNullableDecimal(dto.qtde_item);
            item_obrigatorio = MicrovixConverters.ToNullableBoolean(dto.item_obrigatorio);
        }
    }
}
