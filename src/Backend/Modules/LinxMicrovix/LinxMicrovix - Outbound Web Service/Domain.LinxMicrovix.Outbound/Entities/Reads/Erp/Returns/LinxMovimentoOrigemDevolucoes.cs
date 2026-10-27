using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxMovimentoOrigemDevolucoes, tabela linxmicrovix.LinxMovimentoOrigemDevolucoes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador + nota_origem
    /// </summary>
    public class LinxMovimentoOrigemDevolucoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public Guid identificador { get; private set; }
        public int nota_origem { get; private set; }
        public int? ecf_origem { get; private set; }
        public DateTime? data_origem { get; private set; }
        public string? serie_origem { get; private set; }
        public int? empresa { get; private set; }
        public long timestamp { get; private set; }

        public LinxMovimentoOrigemDevolucoes() { }

        public LinxMovimentoOrigemDevolucoes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxMovimentoOrigemDevolucoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador) ?? Guid.Empty;
            nota_origem = MicrovixConverters.ToInt32(dto.nota_origem);
            ecf_origem = MicrovixConverters.ToNullableInt32(dto.ecf_origem);
            data_origem = MicrovixConverters.ToNullableDateTime(dto.data_origem);
            serie_origem = MicrovixConverters.ToNullableString(dto.serie_origem, 10);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
