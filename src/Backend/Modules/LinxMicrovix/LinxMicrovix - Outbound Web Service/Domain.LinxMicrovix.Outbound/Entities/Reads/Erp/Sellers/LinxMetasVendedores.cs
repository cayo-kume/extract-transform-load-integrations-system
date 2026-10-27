using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Sellers
{
    /// <summary>
    /// LinxMetasVendedores, tabela linxmicrovix.LinxMetasVendedores. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_meta + cod_vendedor (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxMetasVendedores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_meta { get; private set; }
        public string? descricao_meta { get; private set; }
        public DateTime? data_inicial_meta { get; private set; }
        public DateTime? data_final_meta { get; private set; }
        public decimal? valor_meta_loja { get; private set; }
        public decimal? valor_meta_vendedor { get; private set; }
        public int cod_vendedor { get; private set; }
        public long timestamp { get; private set; }

        public LinxMetasVendedores() { }

        public LinxMetasVendedores(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Sellers.LinxMetasVendedores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_meta = MicrovixConverters.ToInt32(dto.id_meta);
            descricao_meta = MicrovixConverters.ToNullableString(dto.descricao_meta, 50);
            data_inicial_meta = MicrovixConverters.ToNullableDateTime(dto.data_inicial_meta);
            data_final_meta = MicrovixConverters.ToNullableDateTime(dto.data_final_meta);
            valor_meta_loja = MicrovixConverters.ToNullableDecimal(dto.valor_meta_loja);
            valor_meta_vendedor = MicrovixConverters.ToNullableDecimal(dto.valor_meta_vendedor);
            cod_vendedor = MicrovixConverters.ToInt32(dto.cod_vendedor);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
