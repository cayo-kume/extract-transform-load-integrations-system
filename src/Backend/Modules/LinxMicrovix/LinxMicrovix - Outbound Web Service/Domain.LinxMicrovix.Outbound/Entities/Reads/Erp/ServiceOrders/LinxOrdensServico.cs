using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// LinxOrdensServico, tabela linxmicrovix.LinxOrdensServico. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + numero_os (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxOrdensServico
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int numero_os { get; private set; }
        public int? empresa { get; private set; }
        public DateTime? data_os { get; private set; }
        public DateTime? data_envio_laboratorio { get; private set; }
        public string? cancelado { get; private set; }
        public int? id_laboratorio { get; private set; }
        public int? id_posicao_os_ramo_optico { get; private set; }
        public bool? compartilhar_hub_laboratorios { get; private set; }
        public int? cod_cliente_os { get; private set; }
        public int? cod_vendedor { get; private set; }
        public int? numero_sequencial_antecipacao_financeira { get; private set; }
        public string? chave_nfe_laboratorio { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }
        public DateTime? previsao_entrega_os { get; private set; }

        public LinxOrdensServico() { }

        public LinxOrdensServico(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ServiceOrders.LinxOrdensServico dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            numero_os = MicrovixConverters.ToInt32(dto.numero_os);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            data_os = MicrovixConverters.ToNullableDateTime(dto.data_os);
            data_envio_laboratorio = MicrovixConverters.ToNullableDateTime(dto.data_envio_laboratorio);
            cancelado = MicrovixConverters.ToNullableString(dto.cancelado, 1);
            id_laboratorio = MicrovixConverters.ToNullableInt32(dto.id_laboratorio);
            id_posicao_os_ramo_optico = MicrovixConverters.ToNullableInt32(dto.id_posicao_os_ramo_optico);
            compartilhar_hub_laboratorios = MicrovixConverters.ToNullableBoolean(dto.compartilhar_hub_laboratorios);
            cod_cliente_os = MicrovixConverters.ToNullableInt32(dto.cod_cliente_os);
            cod_vendedor = MicrovixConverters.ToNullableInt32(dto.cod_vendedor);
            numero_sequencial_antecipacao_financeira = MicrovixConverters.ToNullableInt32(dto.numero_sequencial_antecipacao_financeira);
            chave_nfe_laboratorio = MicrovixConverters.ToNullableString(dto.chave_nfe_laboratorio, 44);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            previsao_entrega_os = MicrovixConverters.ToNullableDateTime(dto.previsao_entrega_os);
        }
    }
}
