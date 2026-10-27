using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Loyalty
{
    /// <summary>
    /// LinxMovimentoReshop, tabela linxmicrovix.LinxMovimentoReshop. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_movimento_campanha_reshop
    /// </summary>
    public class LinxMovimentoReshop
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_movimento_campanha_reshop { get; private set; }
        public int? id_campanha { get; private set; }
        public Guid? identificador { get; private set; }
        public string? nome { get; private set; }
        public string? descricao { get; private set; }
        public bool? aplicar_desconto_venda { get; private set; }
        public decimal? valor_desconto_subtotal { get; private set; }
        public decimal? valor_desconto_completo { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxMovimentoReshop() { }

        public LinxMovimentoReshop(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Loyalty.LinxMovimentoReshop dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_movimento_campanha_reshop = MicrovixConverters.ToInt32(dto.id_movimento_campanha_reshop);
            id_campanha = MicrovixConverters.ToNullableInt32(dto.id_campanha);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            nome = MicrovixConverters.ToNullableString(dto.nome, 100);   //fora da documentação
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 200);
            aplicar_desconto_venda = MicrovixConverters.ToNullableBoolean(dto.aplicar_desconto_venda);
            valor_desconto_subtotal = MicrovixConverters.ToNullableDecimal(dto.valor_desconto_subtotal);
            valor_desconto_completo = MicrovixConverters.ToNullableDecimal(dto.valor_desconto_completo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
