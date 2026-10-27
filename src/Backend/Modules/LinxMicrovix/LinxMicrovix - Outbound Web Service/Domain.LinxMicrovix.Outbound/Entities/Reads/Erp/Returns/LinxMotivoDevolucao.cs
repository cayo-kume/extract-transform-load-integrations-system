using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxMotivoDevolucao, tabela linxmicrovix.LinxMotivoDevolucao. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_motivo (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxMotivoDevolucao
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int cod_motivo { get; private set; }
        public string? descricao_motivo { get; private set; }
        public int? cod_deposito { get; private set; }
        public bool? ativo { get; private set; }
        public long timestamp { get; private set; }

        public LinxMotivoDevolucao() { }

        public LinxMotivoDevolucao(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxMotivoDevolucao dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            cod_motivo = MicrovixConverters.ToInt32(dto.cod_motivo);
            descricao_motivo = MicrovixConverters.ToNullableString(dto.descricao_motivo, 50);
            cod_deposito = MicrovixConverters.ToNullableInt32(dto.cod_deposito);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
