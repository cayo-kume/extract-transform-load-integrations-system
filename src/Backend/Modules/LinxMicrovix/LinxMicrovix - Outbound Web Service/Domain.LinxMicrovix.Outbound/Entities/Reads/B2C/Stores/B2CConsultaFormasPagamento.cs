using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Stores
{
    /// <summary>
    /// B2CConsultaFormasPagamento, tabela linxmicrovix.B2CConsultaFormasPagamento. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_forma_pgto (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaFormasPagamento
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int cod_forma_pgto { get; private set; }
        public string? forma_pgto { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaFormasPagamento() { }

        public B2CConsultaFormasPagamento(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Stores.B2CConsultaFormasPagamento dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            cod_forma_pgto = MicrovixConverters.ToInt32(dto.cod_forma_pgto);
            forma_pgto = MicrovixConverters.ToNullableString(dto.forma_pgto, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
