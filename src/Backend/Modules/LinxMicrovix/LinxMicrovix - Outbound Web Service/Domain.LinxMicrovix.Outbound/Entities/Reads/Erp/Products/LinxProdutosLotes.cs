using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products
{
    /// <summary>
    /// LinxProdutosLotes, tabela linxmicrovix.LinxProdutosLotes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + codigoproduto + empresa + lote + deposito (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxProdutosLotes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long codigoproduto { get; private set; }
        public int empresa { get; private set; }
        public string lote { get; private set; } = String.Empty;
        public int deposito { get; private set; }
        public decimal? saldo_disponivel { get; private set; }
        public long timestamp { get; private set; }

        public LinxProdutosLotes() { }

        public LinxProdutosLotes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Products.LinxProdutosLotes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            empresa = MicrovixConverters.ToInt32(dto.empresa);
            lote = MicrovixConverters.ToNullableString(dto.lote, 60) ?? String.Empty;
            deposito = MicrovixConverters.ToInt32(dto.deposito);
            saldo_disponivel = MicrovixConverters.ToNullableDecimal(dto.saldo_disponivel);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
