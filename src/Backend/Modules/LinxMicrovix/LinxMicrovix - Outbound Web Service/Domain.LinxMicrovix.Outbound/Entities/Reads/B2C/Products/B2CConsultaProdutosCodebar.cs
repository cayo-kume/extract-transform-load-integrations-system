using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Products
{
    /// <summary>
    /// B2CConsultaProdutosCodebar, tabela linxmicrovix.B2CConsultaProdutosCodebar. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_produtos_codebar
    /// </summary>
    public class B2CConsultaProdutosCodebar
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public long? codigoproduto { get; private set; }
        public string? codebar { get; private set; }
        public int id_produtos_codebar { get; private set; }
        public bool? principal { get; private set; }
        public int? empresa { get; private set; }
        public long timestamp { get; private set; }
        public string? tipo_codebar { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaProdutosCodebar() { }

        public B2CConsultaProdutosCodebar(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Products.B2CConsultaProdutosCodebar dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            codebar = String.IsNullOrEmpty(dto.codebar) ? null : dto.codebar;
            id_produtos_codebar = MicrovixConverters.ToInt32(dto.id_produtos_codebar);
            principal = MicrovixConverters.ToNullableBoolean(dto.principal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            tipo_codebar = String.IsNullOrEmpty(dto.tipo_codebar) ? null : dto.tipo_codebar;
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
