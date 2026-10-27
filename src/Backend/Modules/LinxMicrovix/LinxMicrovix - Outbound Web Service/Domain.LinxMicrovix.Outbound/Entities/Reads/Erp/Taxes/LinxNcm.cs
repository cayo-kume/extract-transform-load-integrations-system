using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxNcm, tabela linxmicrovix.LinxNcm. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigo
    /// </summary>
    public class LinxNcm
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public string codigo { get; private set; } = String.Empty;
        public string? descricao { get; private set; }
        public int? codigo_genero { get; private set; }
        public bool? ativo { get; private set; }
        public int? id_ncm { get; private set; }
        public long timestamp { get; private set; }

        public LinxNcm() { }

        public LinxNcm(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxNcm dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            codigo = MicrovixConverters.ToNullableString(dto.codigo, 20) ?? String.Empty;
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 60);
            codigo_genero = MicrovixConverters.ToNullableInt32(dto.codigo_genero);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            id_ncm = MicrovixConverters.ToNullableInt32(dto.id_ncm);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
