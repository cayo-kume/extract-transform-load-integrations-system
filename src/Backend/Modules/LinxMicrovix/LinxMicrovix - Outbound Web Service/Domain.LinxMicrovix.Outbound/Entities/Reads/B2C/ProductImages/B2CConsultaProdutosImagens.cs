using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages
{
    /// <summary>
    /// B2CConsultaProdutosImagens, tabela linxmicrovix.B2CConsultaProdutosImagens. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_imagem_produto
    /// </summary>
    public class B2CConsultaProdutosImagens
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_imagem_produto { get; private set; }
        public int? id_imagem { get; private set; }
        public long? codigoproduto { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }
        public bool? principal { get; private set; }

        public B2CConsultaProdutosImagens() { }

        public B2CConsultaProdutosImagens(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductImages.B2CConsultaProdutosImagens dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_imagem_produto = MicrovixConverters.ToInt32(dto.id_imagem_produto);
            id_imagem = MicrovixConverters.ToNullableInt32(dto.id_imagem);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
            principal = MicrovixConverters.ToNullableBoolean(dto.principal);
        }
    }
}
