using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stocks
{
    /// <summary>
    /// LinxBalancos, tabela linxmicrovix.LinxBalancos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_balancos (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxBalancos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public int id_balancos { get; private set; }
        public string? descricao { get; private set; }
        public DateTime? data_lancamento { get; private set; }
        public string? status { get; private set; }
        public int? id_deposito { get; private set; }
        public long timestamp { get; private set; }

        public LinxBalancos() { }

        public LinxBalancos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stocks.LinxBalancos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_balancos = MicrovixConverters.ToInt32(dto.id_balancos);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 250);
            data_lancamento = MicrovixConverters.ToNullableDateTime(dto.data_lancamento);
            status = MicrovixConverters.ToNullableString(dto.status, 100);
            id_deposito = MicrovixConverters.ToNullableInt32(dto.id_deposito);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
