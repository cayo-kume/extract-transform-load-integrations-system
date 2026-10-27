using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxCategoriasFinanceiras, tabela linxmicrovix.LinxCategoriasFinanceiras. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_categorias_financeiras
    /// </summary>
    public class LinxCategoriasFinanceiras
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_categorias_financeiras { get; private set; }
        public string? descricao { get; private set; }
        public bool? ativo { get; private set; }
        public long? historico { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxCategoriasFinanceiras() { }

        public LinxCategoriasFinanceiras(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxCategoriasFinanceiras dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_categorias_financeiras = MicrovixConverters.ToInt32(dto.id_categorias_financeiras);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 50);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            historico = MicrovixConverters.ToNullableInt64(dto.historico);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
