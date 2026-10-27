using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxClasseFiscal, tabela linxmicrovix.LinxClasseFiscal. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_classe_fiscal
    /// </summary>
    public class LinxClasseFiscal
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_classe_fiscal { get; private set; }
        public string? descricao { get; private set; }
        public bool? excluido { get; private set; }
        public long timestamp { get; private set; }

        public LinxClasseFiscal() { }

        public LinxClasseFiscal(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxClasseFiscal dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_classe_fiscal = MicrovixConverters.ToInt32(dto.id_classe_fiscal);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 150);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
