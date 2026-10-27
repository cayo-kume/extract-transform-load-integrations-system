using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxCstIpiFiscal, tabela linxmicrovix.LinxCstIpiFiscal. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_cst_ipi_fiscal (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxCstIpiFiscal
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_cst_ipi_fiscal { get; private set; }
        public string? cst_ipi_fiscal { get; private set; }
        public string? descricao { get; private set; }
        public bool? excluido { get; private set; }
        public DateTime? inicio_vigencia { get; private set; }
        public DateTime? termino_vigencia { get; private set; }
        public string? cst_ipi_fiscal_inverso { get; private set; }
        public long timestamp { get; private set; }

        public LinxCstIpiFiscal() { }

        public LinxCstIpiFiscal(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxCstIpiFiscal dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_cst_ipi_fiscal = MicrovixConverters.ToInt32(dto.id_cst_ipi_fiscal);
            cst_ipi_fiscal = MicrovixConverters.ToNullableString(dto.cst_ipi_fiscal, 4);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 150);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            inicio_vigencia = MicrovixConverters.ToNullableDateTime(dto.inicio_vigencia);
            termino_vigencia = MicrovixConverters.ToNullableDateTime(dto.termino_vigencia);
            cst_ipi_fiscal_inverso = MicrovixConverters.ToNullableString(dto.cst_ipi_fiscal_inverso, 4);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
