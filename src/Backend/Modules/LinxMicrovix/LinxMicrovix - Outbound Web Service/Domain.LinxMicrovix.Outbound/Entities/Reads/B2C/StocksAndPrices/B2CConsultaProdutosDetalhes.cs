using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// B2CConsultaProdutosDetalhes, tabela linxmicrovix.B2CConsultaProdutosDetalhes. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + id_prod_det
    /// </summary>
    public class B2CConsultaProdutosDetalhes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public long id_prod_det { get; private set; }
        public long? codigoproduto { get; private set; }
        public int? empresa { get; private set; }
        public decimal? saldo { get; private set; }
        public bool? controla_lote { get; private set; }
        public string? nomeproduto_alternativo { get; private set; }
        public long timestamp { get; private set; }
        public string? referencia { get; private set; }
        public string? localizacao { get; private set; }
        public int? portal { get; private set; }
        public int? tempo_preparacao_estoque { get; private set; }

        public B2CConsultaProdutosDetalhes() { }

        public B2CConsultaProdutosDetalhes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.StocksAndPrices.B2CConsultaProdutosDetalhes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_prod_det = MicrovixConverters.ToInt64(dto.id_prod_det);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            saldo = MicrovixConverters.ToNullableDecimal(dto.saldo);
            controla_lote = MicrovixConverters.ToNullableBoolean(dto.controla_lote);
            nomeproduto_alternativo = MicrovixConverters.ToNullableString(dto.nomeproduto_alternativo, 250);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            referencia = MicrovixConverters.ToNullableString(dto.referencia, 50);   //fora da documentação
            localizacao = MicrovixConverters.ToNullableString(dto.localizacao, 50);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            tempo_preparacao_estoque = MicrovixConverters.ToNullableInt32(dto.tempo_preparacao_estoque);
        }
    }
}
