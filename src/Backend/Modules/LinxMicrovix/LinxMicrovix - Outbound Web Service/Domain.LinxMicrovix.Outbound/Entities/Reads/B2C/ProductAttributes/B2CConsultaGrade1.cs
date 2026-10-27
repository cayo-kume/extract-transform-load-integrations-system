using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// B2CConsultaGrade1, tabela linxmicrovix.B2CConsultaGrade1. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo_grade1
    /// </summary>
    public class B2CConsultaGrade1
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int codigo_grade1 { get; private set; }
        public string? nome_grade1 { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaGrade1() { }

        public B2CConsultaGrade1(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.ProductAttributes.B2CConsultaGrade1 dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigo_grade1 = MicrovixConverters.ToInt32(dto.codigo_grade1);
            nome_grade1 = MicrovixConverters.ToNullableString(dto.nome_grade1, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
