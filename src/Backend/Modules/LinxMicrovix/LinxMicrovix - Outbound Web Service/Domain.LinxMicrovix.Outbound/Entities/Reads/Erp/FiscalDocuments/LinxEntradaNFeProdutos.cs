using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxEntradaNFeProdutos, tabela linxmicrovix.LinxEntradaNFeProdutos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_entrada_nfe_produtos
    /// </summary>
    public class LinxEntradaNFeProdutos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_entrada_nfe_produtos { get; private set; }
        public int? id_entrada_nfe { get; private set; }
        public int? numero_item { get; private set; }
        public string? codigo_produto { get; private set; }
        public string? descricao_produto { get; private set; }
        public string? gtin { get; private set; }
        public decimal? qtde_tributavel { get; private set; }
        public decimal? qtde_comercial { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxEntradaNFeProdutos() { }

        public LinxEntradaNFeProdutos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxEntradaNFeProdutos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_entrada_nfe_produtos = MicrovixConverters.ToInt32(dto.id_entrada_nfe_produtos);
            id_entrada_nfe = MicrovixConverters.ToNullableInt32(dto.id_entrada_nfe);
            numero_item = MicrovixConverters.ToNullableInt32(dto.numero_item);
            codigo_produto = MicrovixConverters.ToNullableString(dto.codigo_produto, 60);
            descricao_produto = MicrovixConverters.ToNullableString(dto.descricao_produto, 250);
            gtin = MicrovixConverters.ToNullableString(dto.gtin, 25);
            qtde_tributavel = MicrovixConverters.ToNullableDecimal(dto.qtde_tributavel);
            qtde_comercial = MicrovixConverters.ToNullableDecimal(dto.qtde_comercial);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
