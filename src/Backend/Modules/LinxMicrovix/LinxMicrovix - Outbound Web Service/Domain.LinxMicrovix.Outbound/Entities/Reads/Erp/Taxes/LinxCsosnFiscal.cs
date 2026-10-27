using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxCsosnFiscal, tabela linxmicrovix.LinxCsosnFiscal. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_csosn_fiscal
    /// </summary>
    public class LinxCsosnFiscal
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_csosn_fiscal { get; private set; }
        public string? csosn_fiscal { get; private set; }
        public string? descricao { get; private set; }
        public int? id_csosn_fiscal_substitutiva { get; private set; }
        public long timestamp { get; private set; }

        public LinxCsosnFiscal() { }

        public LinxCsosnFiscal(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxCsosnFiscal dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_csosn_fiscal = MicrovixConverters.ToInt32(dto.id_csosn_fiscal);
            csosn_fiscal = MicrovixConverters.ToNullableString(dto.csosn_fiscal, 5);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 200);
            id_csosn_fiscal_substitutiva = MicrovixConverters.ToNullableInt32(dto.id_csosn_fiscal_substitutiva);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
