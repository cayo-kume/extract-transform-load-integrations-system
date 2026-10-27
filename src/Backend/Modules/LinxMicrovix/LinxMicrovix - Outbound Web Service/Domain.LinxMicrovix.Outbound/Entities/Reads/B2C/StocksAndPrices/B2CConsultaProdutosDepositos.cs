using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// B2CConsultaProdutosDepositos, tabela linxmicrovix.B2CConsultaProdutosDepositos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_deposito
    /// </summary>
    public class B2CConsultaProdutosDepositos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_deposito { get; private set; }
        public string? nome_deposito { get; private set; }
        public string? disponivel { get; private set; }
        public bool? disponivel_transferencia { get; private set; }
        public bool? disponivel_franquias { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }
        public bool? outlet { get; private set; }

        public B2CConsultaProdutosDepositos() { }

        public B2CConsultaProdutosDepositos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.StocksAndPrices.B2CConsultaProdutosDepositos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_deposito = MicrovixConverters.ToInt32(dto.id_deposito);
            nome_deposito = MicrovixConverters.ToNullableString(dto.nome_deposito, 50);
            disponivel = MicrovixConverters.ToNullableString(dto.disponivel, 1);
            disponivel_transferencia = MicrovixConverters.ToNullableBoolean(dto.disponivel_transferencia);
            disponivel_franquias = MicrovixConverters.ToNullableBoolean(dto.disponivel_franquias);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
            outlet = MicrovixConverters.ToNullableBoolean(dto.outlet);
        }
    }
}
