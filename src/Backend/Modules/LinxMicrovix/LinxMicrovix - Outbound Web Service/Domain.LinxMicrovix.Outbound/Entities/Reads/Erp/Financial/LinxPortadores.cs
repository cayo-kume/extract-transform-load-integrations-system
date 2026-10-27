using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxPortadores, tabela linxmicrovix.LinxPortadores. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_portador (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxPortadores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int cod_portador { get; private set; }
        public string? nome_portador { get; private set; }
        public decimal? taxa_acrescimo { get; private set; }
        public bool? aviso_vencimento { get; private set; }
        public bool? reaviso_vencimento { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxPortadores() { }

        public LinxPortadores(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxPortadores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            cod_portador = MicrovixConverters.ToInt32(dto.cod_portador);
            nome_portador = MicrovixConverters.ToNullableString(dto.nome_portador, 50);
            taxa_acrescimo = MicrovixConverters.ToNullableDecimal(dto.taxa_acrescimo);
            aviso_vencimento = MicrovixConverters.ToNullableBoolean(dto.aviso_vencimento);
            reaviso_vencimento = MicrovixConverters.ToNullableBoolean(dto.reaviso_vencimento);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
