using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stocks
{
    /// <summary>
    /// LinxProdutosDepositos, tabela linxmicrovix.LinxProdutosDepositos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_deposito
    /// </summary>
    public class LinxProdutosDepositos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int cod_deposito { get; private set; }
        public string? nome_deposito { get; private set; }
        public bool? disponivel { get; private set; }
        public bool? disponivel_transferencia { get; private set; }
        public long timestamp { get; private set; }
        public bool? outlet { get; private set; }

        public LinxProdutosDepositos() { }

        public LinxProdutosDepositos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stocks.LinxProdutosDepositos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            cod_deposito = MicrovixConverters.ToInt32(dto.cod_deposito);
            nome_deposito = MicrovixConverters.ToNullableString(dto.nome_deposito, 50);
            disponivel = MicrovixConverters.ToNullableBoolean(dto.disponivel);
            disponivel_transferencia = MicrovixConverters.ToNullableBoolean(dto.disponivel_transferencia);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            outlet = MicrovixConverters.ToNullableBoolean(dto.outlet);   //fora da documentação
        }
    }
}
