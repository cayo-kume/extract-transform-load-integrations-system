using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices
{
    /// <summary>
    /// LinxProdutosTabelasPrecos, tabela linxmicrovix.LinxProdutosTabelasPrecos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_tabela + cod_produto
    /// </summary>
    public class LinxProdutosTabelasPrecos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_tabela { get; private set; }
        public long cod_produto { get; private set; }
        public decimal? precovenda { get; private set; }
        public long timestamp { get; private set; }

        public LinxProdutosTabelasPrecos() { }

        public LinxProdutosTabelasPrecos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Prices.LinxProdutosTabelasPrecos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_tabela = MicrovixConverters.ToInt32(dto.id_tabela);
            cod_produto = MicrovixConverters.ToInt64(dto.cod_produto);
            precovenda = MicrovixConverters.ToNullableDecimal(dto.precovenda);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
