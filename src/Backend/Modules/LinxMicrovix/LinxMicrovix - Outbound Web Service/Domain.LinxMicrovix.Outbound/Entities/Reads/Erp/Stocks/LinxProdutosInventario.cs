using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stocks
{
    /// <summary>
    /// LinxProdutosInventario, tabela linxmicrovix.LinxProdutosInventario. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_produto
    /// </summary>
    public class LinxProdutosInventario
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long cod_produto { get; private set; }
        public string? cod_barra { get; private set; }
        public decimal? quantidade { get; private set; }
        public int? empresa { get; private set; }
        public long timestamp { get; private set; }

        public LinxProdutosInventario() { }

        public LinxProdutosInventario(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stocks.LinxProdutosInventario dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_produto = MicrovixConverters.ToInt64(dto.cod_produto);
            cod_barra = MicrovixConverters.ToNullableString(dto.cod_barra, 20);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
