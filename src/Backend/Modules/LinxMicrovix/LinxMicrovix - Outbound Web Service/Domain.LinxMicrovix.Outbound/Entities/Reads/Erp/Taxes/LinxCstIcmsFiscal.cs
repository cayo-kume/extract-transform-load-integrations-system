using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxCstIcmsFiscal, tabela linxmicrovix.LinxCstIcmsFiscal. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_cst_icms_fiscal
    /// </summary>
    public class LinxCstIcmsFiscal
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_cst_icms_fiscal { get; private set; }
        public string? cst_icms_fiscal { get; private set; }
        public string? descricao { get; private set; }
        public bool? substituicao_tributaria { get; private set; }
        public bool? excluido { get; private set; }
        public long timestamp { get; private set; }

        public LinxCstIcmsFiscal() { }

        public LinxCstIcmsFiscal(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxCstIcmsFiscal dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_cst_icms_fiscal = MicrovixConverters.ToInt32(dto.id_cst_icms_fiscal);
            cst_icms_fiscal = MicrovixConverters.ToNullableString(dto.cst_icms_fiscal, 4);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 150);
            substituicao_tributaria = MicrovixConverters.ToNullableBoolean(dto.substituicao_tributaria);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
