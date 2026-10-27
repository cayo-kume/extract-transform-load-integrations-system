using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaGrade2, tabela linxmicrovix.B2CConsultaGrade2. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo_grade2
    /// </summary>
    public class B2CConsultaGrade2
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int codigo_grade2 { get; private set; }
        public string? nome_grade2 { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaGrade2() { }

        public B2CConsultaGrade2(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaGrade2 dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigo_grade2 = MicrovixConverters.ToInt32(dto.codigo_grade2);
            nome_grade2 = MicrovixConverters.ToNullableString(dto.nome_grade2, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
