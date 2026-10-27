using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// B2CConsultaProdutosTabelas, tabela linxmicrovix.B2CConsultaProdutosTabelas. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_tabela (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaProdutosTabelas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_tabela { get; private set; }
        public string? nome_tabela { get; private set; }
        public string? ativa { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaProdutosTabelas() { }

        public B2CConsultaProdutosTabelas(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.StocksAndPrices.B2CConsultaProdutosTabelas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_tabela = MicrovixConverters.ToInt32(dto.id_tabela);
            nome_tabela = MicrovixConverters.ToNullableString(dto.nome_tabela, 50);
            ativa = MicrovixConverters.ToNullableString(dto.ativa, 1);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
