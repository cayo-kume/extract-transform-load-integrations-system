using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxMovimentoVendaConjugada, tabela linxmicrovix.LinxMovimentoVendaConjugada. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador_produto + identificador_servico (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxMovimentoVendaConjugada
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public Guid identificador_produto { get; private set; }
        public Guid identificador_servico { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxMovimentoVendaConjugada() { }

        public LinxMovimentoVendaConjugada(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxMovimentoVendaConjugada dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            identificador_produto = MicrovixConverters.ToNullableGuid(dto.identificador_produto) ?? Guid.Empty;
            identificador_servico = MicrovixConverters.ToNullableGuid(dto.identificador_servico) ?? Guid.Empty;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
