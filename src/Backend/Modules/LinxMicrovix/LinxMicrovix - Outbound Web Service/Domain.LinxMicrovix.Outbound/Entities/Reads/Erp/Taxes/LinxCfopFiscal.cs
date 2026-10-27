using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxCfopFiscal, tabela linxmicrovix.LinxCfopFiscal. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_cfop_fiscal
    /// </summary>
    public class LinxCfopFiscal
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_cfop_fiscal { get; private set; }
        public string? cfop_fiscal { get; private set; }
        public string? descricao { get; private set; }
        public bool? excluido { get; private set; }
        public long timestamp { get; private set; }

        public LinxCfopFiscal() { }

        public LinxCfopFiscal(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxCfopFiscal dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_cfop_fiscal = MicrovixConverters.ToInt32(dto.id_cfop_fiscal);
            cfop_fiscal = MicrovixConverters.ToNullableString(dto.cfop_fiscal, 5);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 300);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
