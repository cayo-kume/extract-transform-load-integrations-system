using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Products
{
    /// <summary>
    /// B2CConsultaProdutosStatus, tabela linxmicrovix.B2CConsultaProdutosStatus. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigoproduto
    /// </summary>
    public class B2CConsultaProdutosStatus
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public long codigoproduto { get; private set; }
        public string? referencia { get; private set; }
        public bool? ativo { get; private set; }
        public bool? b2c { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaProdutosStatus() { }

        public B2CConsultaProdutosStatus(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Products.B2CConsultaProdutosStatus dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            referencia = MicrovixConverters.ToNullableString(dto.referencia, 20);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            b2c = MicrovixConverters.ToNullableBoolean(dto.b2c);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
