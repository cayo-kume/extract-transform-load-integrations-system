using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxTrocaUnificadaResumoVendas, tabela linxmicrovix.LinxTrocaUnificadaResumoVendas. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_troca_unificada_resumo_vendas
    /// </summary>
    public class LinxTrocaUnificadaResumoVendas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public long id_troca_unificada_resumo_vendas { get; private set; }
        public string? token { get; private set; }
        public Guid? identificador { get; private set; }
        public int? documento { get; private set; }
        public string? serie { get; private set; }
        public DateTime? data_venda { get; private set; }
        public string? documento_cliente { get; private set; }
        public bool? venda_cancelada { get; private set; }
        public long timestamp { get; private set; }

        public LinxTrocaUnificadaResumoVendas() { }

        public LinxTrocaUnificadaResumoVendas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxTrocaUnificadaResumoVendas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_troca_unificada_resumo_vendas = MicrovixConverters.ToInt64(dto.id_troca_unificada_resumo_vendas);
            token = MicrovixConverters.ToNullableString(dto.token, 30);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            documento = MicrovixConverters.ToNullableInt32(dto.documento);
            serie = MicrovixConverters.ToNullableString(dto.serie, 10);
            data_venda = MicrovixConverters.ToNullableDateTime(dto.data_venda);
            documento_cliente = MicrovixConverters.ToNullableString(dto.documento_cliente, 14);
            venda_cancelada = MicrovixConverters.ToNullableBoolean(dto.venda_cancelada);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
