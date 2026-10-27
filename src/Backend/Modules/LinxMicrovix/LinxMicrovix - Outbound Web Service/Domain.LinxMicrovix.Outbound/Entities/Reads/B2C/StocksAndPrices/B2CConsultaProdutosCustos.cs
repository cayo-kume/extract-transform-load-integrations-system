using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// B2CConsultaProdutosCustos, tabela linxmicrovix.B2CConsultaProdutosCustos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigoproduto
    /// </summary>
    public class B2CConsultaProdutosCustos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int? id_produtos_custos { get; private set; }
        public long codigoproduto { get; private set; }
        public int? empresa { get; private set; }
        public decimal? custoicms1 { get; private set; }
        public decimal? ipi1 { get; private set; }
        public decimal? markup { get; private set; }
        public decimal? customedio { get; private set; }
        public decimal? frete1 { get; private set; }
        public int? precisao { get; private set; }
        public decimal? precominimo { get; private set; }
        public DateTime? dt_update { get; private set; }
        public decimal? custoliquido { get; private set; }
        public decimal? precovenda { get; private set; }
        public decimal? custototal { get; private set; }
        public decimal? precocompra { get; private set; }
        public long timestamp { get; private set; }
        public decimal? desconto_especial { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaProdutosCustos() { }

        public B2CConsultaProdutosCustos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.StocksAndPrices.B2CConsultaProdutosCustos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_produtos_custos = MicrovixConverters.ToNullableInt32(dto.id_produtos_custos);
            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            custoicms1 = MicrovixConverters.ToNullableDecimal(dto.custoicms1);
            ipi1 = MicrovixConverters.ToNullableDecimal(dto.ipi1);
            markup = MicrovixConverters.ToNullableDecimal(dto.markup);
            customedio = MicrovixConverters.ToNullableDecimal(dto.customedio);
            frete1 = MicrovixConverters.ToNullableDecimal(dto.frete1);
            precisao = MicrovixConverters.ToNullableInt32(dto.precisao);
            precominimo = MicrovixConverters.ToNullableDecimal(dto.precominimo);
            dt_update = MicrovixConverters.ToNullableDateTime(dto.dt_update);   //fora da documentação
            custoliquido = MicrovixConverters.ToNullableDecimal(dto.custoliquido);
            precovenda = MicrovixConverters.ToNullableDecimal(dto.precovenda);
            custototal = MicrovixConverters.ToNullableDecimal(dto.custototal);
            precocompra = MicrovixConverters.ToNullableDecimal(dto.precocompra);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            desconto_especial = MicrovixConverters.ToNullableDecimal(dto.desconto_especial);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
