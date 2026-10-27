using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// B2CConsultaProdutosTabelasPrecos, tabela linxmicrovix.B2CConsultaProdutosTabelasPrecos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_tabela + codigoproduto
    /// </summary>
    public class B2CConsultaProdutosTabelasPrecos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int? id_prod_tab_preco { get; private set; }
        public int id_tabela { get; private set; }
        public long codigoproduto { get; private set; }
        public decimal? precovenda { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaProdutosTabelasPrecos() { }

        public B2CConsultaProdutosTabelasPrecos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.StocksAndPrices.B2CConsultaProdutosTabelasPrecos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_prod_tab_preco = MicrovixConverters.ToNullableInt32(dto.id_prod_tab_preco);
            id_tabela = MicrovixConverters.ToInt32(dto.id_tabela);
            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            precovenda = MicrovixConverters.ToNullableDecimal(dto.precovenda);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
