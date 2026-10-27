using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxMovimentoIndicacoes, tabela linxmicrovix.LinxMovimentoIndicacoes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador + cod_cliente (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxMovimentoIndicacoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public Guid identificador { get; private set; }
        public int cod_cliente { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxMovimentoIndicacoes() { }

        public LinxMovimentoIndicacoes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxMovimentoIndicacoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            identificador = MicrovixConverters.ToNullableGuid(dto.identificador) ?? Guid.Empty;
            cod_cliente = MicrovixConverters.ToInt32(dto.cod_cliente);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
