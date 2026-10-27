using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// B2CConsultaProdutosDetalhesDepositos, tabela linxmicrovix.B2CConsultaProdutosDetalhesDepositos. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + codigoproduto
    /// </summary>
    public class B2CConsultaProdutosDetalhesDepositos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public long codigoproduto { get; private set; }
        public int? empresa { get; private set; }
        public int? id_deposito { get; private set; }
        public decimal? saldo { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }
        public int? tempo_preparacao_estoque { get; private set; }

        public B2CConsultaProdutosDetalhesDepositos() { }

        public B2CConsultaProdutosDetalhesDepositos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.StocksAndPrices.B2CConsultaProdutosDetalhesDepositos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_deposito = MicrovixConverters.ToNullableInt32(dto.id_deposito);
            saldo = MicrovixConverters.ToNullableDecimal(dto.saldo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            tempo_preparacao_estoque = MicrovixConverters.ToNullableInt32(dto.tempo_preparacao_estoque);
        }
    }
}
