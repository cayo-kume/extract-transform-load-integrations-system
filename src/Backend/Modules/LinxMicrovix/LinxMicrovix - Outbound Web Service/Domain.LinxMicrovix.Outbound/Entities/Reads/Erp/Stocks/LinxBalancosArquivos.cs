using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stocks
{
    /// <summary>
    /// LinxBalancosArquivos, tabela linxmicrovix.LinxBalancosArquivos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_balancos_arquivos (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxBalancosArquivos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_balancos_arquivos { get; private set; }
        public long? codigoproduto { get; private set; }
        public decimal? qtdeproduto { get; private set; }
        public string? lote { get; private set; }
        public long timestamp { get; private set; }

        public LinxBalancosArquivos() { }

        public LinxBalancosArquivos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stocks.LinxBalancosArquivos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_balancos_arquivos = MicrovixConverters.ToInt32(dto.id_balancos_arquivos);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            qtdeproduto = MicrovixConverters.ToNullableDecimal(dto.qtdeproduto);
            lote = MicrovixConverters.ToNullableString(dto.lote, 60);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
