using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxCstCofinsFiscal, tabela linxmicrovix.LinxCstCofinsFiscal. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_cst_cofins_fiscal
    /// </summary>
    public class LinxCstCofinsFiscal
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_cst_cofins_fiscal { get; private set; }
        public string? cst_cofins_fiscal { get; private set; }
        public string? descricao { get; private set; }
        public bool? excluido { get; private set; }
        public DateTime? inicio_vigencia { get; private set; }
        public DateTime? termino_vigencia { get; private set; }
        public long timestamp { get; private set; }

        public LinxCstCofinsFiscal() { }

        public LinxCstCofinsFiscal(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxCstCofinsFiscal dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_cst_cofins_fiscal = MicrovixConverters.ToInt32(dto.id_cst_cofins_fiscal);
            cst_cofins_fiscal = MicrovixConverters.ToNullableString(dto.cst_cofins_fiscal, 4);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 150);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            inicio_vigencia = MicrovixConverters.ToNullableDateTime(dto.inicio_vigencia);
            termino_vigencia = MicrovixConverters.ToNullableDateTime(dto.termino_vigencia);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
