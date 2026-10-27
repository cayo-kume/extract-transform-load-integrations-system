using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxTrocaUnificadaResumoBaixa, tabela linxmicrovix.LinxTrocaUnificadaResumoBaixa. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_troca_baixa
    /// </summary>
    public class LinxTrocaUnificadaResumoBaixa
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal_baixa { get; private set; }
        public int? empresa_baixa { get; private set; }
        public string? cnpj_empresa_baixa { get; private set; }
        public int id_troca_baixa { get; private set; }
        public long? id_troca_unificada_resumo_vendas_itens { get; private set; }
        public DateTime? data_troca_baixa { get; private set; }
        public int? transacao_baixa { get; private set; }
        public string? descricao_empresa_baixa { get; private set; }
        public long timestamp { get; private set; }

        public LinxTrocaUnificadaResumoBaixa() { }

        public LinxTrocaUnificadaResumoBaixa(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxTrocaUnificadaResumoBaixa dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal_baixa = MicrovixConverters.ToNullableInt32(dto.portal_baixa);
            empresa_baixa = MicrovixConverters.ToNullableInt32(dto.empresa_baixa);
            cnpj_empresa_baixa = MicrovixConverters.ToNullableString(dto.cnpj_empresa_baixa, 14);
            id_troca_baixa = MicrovixConverters.ToInt32(dto.id_troca_baixa);
            id_troca_unificada_resumo_vendas_itens = MicrovixConverters.ToNullableInt64(dto.id_troca_unificada_resumo_vendas_itens);
            data_troca_baixa = MicrovixConverters.ToNullableDateTime(dto.data_troca_baixa);
            transacao_baixa = MicrovixConverters.ToNullableInt32(dto.transacao_baixa);
            descricao_empresa_baixa = MicrovixConverters.ToNullableString(dto.descricao_empresa_baixa, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
