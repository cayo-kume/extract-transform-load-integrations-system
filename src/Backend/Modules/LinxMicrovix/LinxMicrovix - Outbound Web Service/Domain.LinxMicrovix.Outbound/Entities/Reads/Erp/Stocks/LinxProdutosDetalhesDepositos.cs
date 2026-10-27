using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stocks
{
    /// <summary>
    /// LinxProdutosDetalhesDepositos, tabela linxmicrovix.LinxProdutosDetalhesDepositos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_deposito + cod_produto
    /// </summary>
    public class LinxProdutosDetalhesDepositos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long cod_produto { get; private set; }
        public int? empresa { get; private set; }
        public int cod_deposito { get; private set; }
        public decimal? saldo { get; private set; }
        public long timestamp { get; private set; }

        public LinxProdutosDetalhesDepositos() { }

        public LinxProdutosDetalhesDepositos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stocks.LinxProdutosDetalhesDepositos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_produto = MicrovixConverters.ToInt64(dto.cod_produto);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            cod_deposito = MicrovixConverters.ToInt32(dto.cod_deposito);
            saldo = MicrovixConverters.ToNullableDecimal(dto.saldo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
