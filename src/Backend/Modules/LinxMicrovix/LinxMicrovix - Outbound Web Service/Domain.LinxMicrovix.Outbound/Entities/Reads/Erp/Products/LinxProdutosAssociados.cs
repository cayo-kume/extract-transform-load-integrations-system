using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products
{
    /// <summary>
    /// LinxProdutosAssociados, tabela linxmicrovix.LinxProdutosAssociados. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + codigoproduto_origem + codigoproduto_associado (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxProdutosAssociados
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public string? portal { get; private set; }
        public long codigoproduto_origem { get; private set; }
        public long codigoproduto_associado { get; private set; }
        public decimal? coeficiente_desconto { get; private set; }
        public long timestamp { get; private set; }
        public decimal? qtde_item { get; private set; }
        public bool? item_obrigatorio { get; private set; }

        public LinxProdutosAssociados() { }

        public LinxProdutosAssociados(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Products.LinxProdutosAssociados dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableString(dto.portal, 50);
            codigoproduto_origem = MicrovixConverters.ToInt64(dto.codigoproduto_origem);
            codigoproduto_associado = MicrovixConverters.ToInt64(dto.codigoproduto_associado);
            coeficiente_desconto = MicrovixConverters.ToNullableDecimal(dto.coeficiente_desconto);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            qtde_item = MicrovixConverters.ToNullableDecimal(dto.qtde_item);
            item_obrigatorio = MicrovixConverters.ToNullableBoolean(dto.item_obrigatorio);
        }
    }
}
