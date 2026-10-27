using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Optics
{
    /// <summary>
    /// LinxPosicaoOsRamoOptico, tabela linxmicrovix.LinxPosicaoOsRamoOptico. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_posicao_os_ramo_optico (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxPosicaoOsRamoOptico
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_posicao_os_ramo_optico { get; private set; }
        public string? descricao { get; private set; }
        public int? codigo_status_padrao { get; private set; }
        public bool? ativo { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxPosicaoOsRamoOptico() { }

        public LinxPosicaoOsRamoOptico(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Optics.LinxPosicaoOsRamoOptico dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_posicao_os_ramo_optico = MicrovixConverters.ToInt32(dto.id_posicao_os_ramo_optico);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 50);
            codigo_status_padrao = MicrovixConverters.ToNullableInt32(dto.codigo_status_padrao);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
