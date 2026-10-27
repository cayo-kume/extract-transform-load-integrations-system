using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Sellers
{
    /// <summary>
    /// LinxComissoesVendedores, tabela linxmicrovix.LinxComissoesVendedores. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + vendedor + empresa + data_origem + valor_base + valor_comissao + cancelado
    /// </summary>
    public class LinxComissoesVendedores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int vendedor { get; private set; }
        public int empresa { get; private set; }
        public DateTime data_origem { get; private set; }
        public decimal valor_base { get; private set; }
        public decimal valor_comissao { get; private set; }
        public string cancelado { get; private set; } = String.Empty;
        public string? disponivel { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxComissoesVendedores() { }

        public LinxComissoesVendedores(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Sellers.LinxComissoesVendedores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            vendedor = MicrovixConverters.ToInt32(dto.vendedor);
            empresa = MicrovixConverters.ToInt32(dto.empresa);
            data_origem = MicrovixConverters.ToNullableDateTime(dto.data_origem) ?? new DateTime(1900, 1, 1);
            valor_base = MicrovixConverters.ToNullableDecimal(dto.valor_base) ?? 0;
            valor_comissao = MicrovixConverters.ToNullableDecimal(dto.valor_comissao) ?? 0;
            cancelado = MicrovixConverters.ToNullableString(dto.cancelado, 1) ?? String.Empty;
            disponivel = MicrovixConverters.ToNullableString(dto.disponivel, 1);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
