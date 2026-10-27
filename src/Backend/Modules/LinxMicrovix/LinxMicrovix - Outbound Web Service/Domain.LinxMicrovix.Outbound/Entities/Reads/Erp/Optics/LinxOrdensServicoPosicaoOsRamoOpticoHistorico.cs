using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Optics
{
    /// <summary>
    /// LinxOrdensServicoPosicaoOsRamoOpticoHistorico, tabela linxmicrovix.LinxOrdensServicoPosicaoOsRamoOpticoHistorico. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_ordens_servico_posicao_os_ramo_optico_historico (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxOrdensServicoPosicaoOsRamoOpticoHistorico
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_ordens_servico_posicao_os_ramo_optico_historico { get; private set; }
        public int? numero_os { get; private set; }
        public int? usuario { get; private set; }
        public long timestamp { get; private set; }
        public int? id_posicao_os_ramo_optico { get; private set; }
        public DateTime? data { get; private set; }
        public string? observacao { get; private set; }
        public int? portal { get; private set; }

        public LinxOrdensServicoPosicaoOsRamoOpticoHistorico() { }

        public LinxOrdensServicoPosicaoOsRamoOpticoHistorico(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Optics.LinxOrdensServicoPosicaoOsRamoOpticoHistorico dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_ordens_servico_posicao_os_ramo_optico_historico = MicrovixConverters.ToInt32(dto.id_ordens_servico_posicao_os_ramo_optico_historico);
            numero_os = MicrovixConverters.ToNullableInt32(dto.numero_os);
            usuario = MicrovixConverters.ToNullableInt32(dto.usuario);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            id_posicao_os_ramo_optico = MicrovixConverters.ToNullableInt32(dto.id_posicao_os_ramo_optico);
            data = MicrovixConverters.ToNullableDateTime(dto.data);
            observacao = MicrovixConverters.ToNullableString(dto.observacao, 200);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
