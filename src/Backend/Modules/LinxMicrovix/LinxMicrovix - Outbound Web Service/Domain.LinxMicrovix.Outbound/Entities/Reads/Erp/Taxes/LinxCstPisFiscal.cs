using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxCstPisFiscal, tabela linxmicrovix.LinxCstPisFiscal. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_cst_pis_fiscal
    /// </summary>
    public class LinxCstPisFiscal
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_cst_pis_fiscal { get; private set; }
        public string? cst_pis_fiscal { get; private set; }
        public string? descricao { get; private set; }
        public bool? excluido { get; private set; }
        public DateTime? inicio_vigencia { get; private set; }
        public DateTime? termino_vigencia { get; private set; }
        public long timestamp { get; private set; }

        public LinxCstPisFiscal() { }

        public LinxCstPisFiscal(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxCstPisFiscal dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_cst_pis_fiscal = MicrovixConverters.ToInt32(dto.id_cst_pis_fiscal);
            cst_pis_fiscal = MicrovixConverters.ToNullableString(dto.cst_pis_fiscal, 4);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 150);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            inicio_vigencia = MicrovixConverters.ToNullableDateTime(dto.inicio_vigencia);
            termino_vigencia = MicrovixConverters.ToNullableDateTime(dto.termino_vigencia);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
