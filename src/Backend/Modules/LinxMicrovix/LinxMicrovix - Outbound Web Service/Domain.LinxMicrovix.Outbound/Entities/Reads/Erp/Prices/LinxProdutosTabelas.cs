using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices
{
    /// <summary>
    /// LinxProdutosTabelas, tabela linxmicrovix.LinxProdutosTabelas. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_tabela (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxProdutosTabelas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_tabela { get; private set; }
        public string? nome_tabela { get; private set; }
        public string? ativa { get; private set; }
        public long timestamp { get; private set; }
        public string? tipo_tabela { get; private set; }
        public string? codigo_integracao_ws { get; private set; }

        public LinxProdutosTabelas() { }

        public LinxProdutosTabelas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Prices.LinxProdutosTabelas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_tabela = MicrovixConverters.ToInt32(dto.id_tabela);
            nome_tabela = MicrovixConverters.ToNullableString(dto.nome_tabela, 50);
            ativa = MicrovixConverters.ToNullableString(dto.ativa, 1);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            tipo_tabela = MicrovixConverters.ToNullableString(dto.tipo_tabela, 1);
            codigo_integracao_ws = MicrovixConverters.ToNullableString(dto.codigo_integracao_ws, 50);
        }
    }
}
