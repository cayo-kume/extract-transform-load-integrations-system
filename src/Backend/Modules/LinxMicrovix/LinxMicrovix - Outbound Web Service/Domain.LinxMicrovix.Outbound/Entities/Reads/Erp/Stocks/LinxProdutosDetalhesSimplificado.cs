using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stocks
{
    /// <summary>
    /// LinxProdutosDetalhesSimplificado, tabela linxmicrovix.LinxProdutosDetalhesSimplificado. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_produto
    /// </summary>
    public class LinxProdutosDetalhesSimplificado
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public long cod_produto { get; private set; }
        public decimal? quantidade { get; private set; }
        public int? id_config_tributaria { get; private set; }
        public long timestamp { get; private set; }

        public LinxProdutosDetalhesSimplificado() { }

        public LinxProdutosDetalhesSimplificado(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stocks.LinxProdutosDetalhesSimplificado dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            cod_produto = MicrovixConverters.ToInt64(dto.cod_produto);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            id_config_tributaria = MicrovixConverters.ToNullableInt32(dto.id_config_tributaria);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
