using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products
{
    /// <summary>
    /// LinxLotesProdutos, tabela linxmicrovix.LinxLotesProdutos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_lote (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxLotesProdutos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_lote { get; private set; }
        public long? codigo_produto { get; private set; }
        public string? lote { get; private set; }
        public string? identificador { get; private set; }
        public int? transacao { get; private set; }
        public DateTime? data_fabricacao { get; private set; }
        public DateTime? data_vencimento { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxLotesProdutos() { }

        public LinxLotesProdutos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Products.LinxLotesProdutos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_lote = MicrovixConverters.ToInt32(dto.id_lote);
            codigo_produto = MicrovixConverters.ToNullableInt64(dto.codigo_produto);
            lote = MicrovixConverters.ToNullableString(dto.lote, 60);
            identificador = MicrovixConverters.ToNullableString(dto.identificador, 1000);   //fora da documentação
            transacao = MicrovixConverters.ToNullableInt32(dto.transacao);
            data_fabricacao = MicrovixConverters.ToNullableDateTime(dto.data_fabricacao);
            data_vencimento = MicrovixConverters.ToNullableDateTime(dto.data_vencimento);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
