using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxRotinaOrigem, tabela linxmicrovix.LinxRotinaOrigem. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo_rotina
    /// </summary>
    public class LinxRotinaOrigem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int codigo_rotina { get; private set; }
        public string? descricao_rotina { get; private set; }
        public long timestamp { get; private set; }

        public LinxRotinaOrigem() { }

        public LinxRotinaOrigem(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxRotinaOrigem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            codigo_rotina = MicrovixConverters.ToInt32(dto.codigo_rotina);
            descricao_rotina = MicrovixConverters.ToNullableString(dto.descricao_rotina, 150);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
