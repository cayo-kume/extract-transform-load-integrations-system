using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stocks
{
    /// <summary>
    /// LinxBalancosSecoes, tabela linxmicrovix.LinxBalancosSecoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_balancos_secoes (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxBalancosSecoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_balancos_secoes { get; private set; }
        public int? id_balancos { get; private set; }
        public int? id_balancos_arquivos { get; private set; }
        public string? status { get; private set; }
        public long timestamp { get; private set; }

        public LinxBalancosSecoes() { }

        public LinxBalancosSecoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stocks.LinxBalancosSecoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_balancos_secoes = MicrovixConverters.ToInt32(dto.id_balancos_secoes);
            id_balancos = MicrovixConverters.ToNullableInt32(dto.id_balancos);
            id_balancos_arquivos = MicrovixConverters.ToNullableInt32(dto.id_balancos_arquivos);
            status = MicrovixConverters.ToNullableString(dto.status, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
