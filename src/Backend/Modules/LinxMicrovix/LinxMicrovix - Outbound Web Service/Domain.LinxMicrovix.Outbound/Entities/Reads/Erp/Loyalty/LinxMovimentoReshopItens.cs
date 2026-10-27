using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Loyalty
{
    /// <summary>
    /// LinxMovimentoReshopItens, tabela linxmicrovix.LinxMovimentoReshopItens. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_movimento_campanha_reshop_item
    /// </summary>
    public class LinxMovimentoReshopItens
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_movimento_campanha_reshop_item { get; private set; }
        public int? id_campanha { get; private set; }
        public Guid? identificador { get; private set; }
        public decimal? valor_unitario { get; private set; }
        public decimal? valor_desconto_item { get; private set; }
        public decimal? valor_total_desconto_item { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? valor_original { get; private set; }
        public long timestamp { get; private set; }
        public int? ordem { get; private set; }
        public int portal { get; private set; }

        public LinxMovimentoReshopItens() { }

        public LinxMovimentoReshopItens(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Loyalty.LinxMovimentoReshopItens dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_movimento_campanha_reshop_item = MicrovixConverters.ToInt32(dto.id_movimento_campanha_reshop_item);
            id_campanha = MicrovixConverters.ToNullableInt32(dto.id_campanha);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            valor_unitario = MicrovixConverters.ToNullableDecimal(dto.valor_unitario);
            valor_desconto_item = MicrovixConverters.ToNullableDecimal(dto.valor_desconto_item);
            valor_total_desconto_item = MicrovixConverters.ToNullableDecimal(dto.valor_total_desconto_item);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            valor_original = MicrovixConverters.ToNullableDecimal(dto.valor_original);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            ordem = MicrovixConverters.ToNullableInt32(dto.ordem);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
