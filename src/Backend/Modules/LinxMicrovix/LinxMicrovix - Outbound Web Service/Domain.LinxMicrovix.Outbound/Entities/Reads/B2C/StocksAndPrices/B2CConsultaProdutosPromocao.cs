using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// B2CConsultaProdutosPromocao, tabela linxmicrovix.B2CConsultaProdutosPromocao. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + codigo_promocao + codigoproduto (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaProdutosPromocao
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public long codigo_promocao { get; private set; }
        public int? empresa { get; private set; }
        public long codigoproduto { get; private set; }
        public decimal? preco { get; private set; }
        public DateTime? data_inicio { get; private set; }
        public DateTime? data_termino { get; private set; }
        public DateTime? data_cadastro { get; private set; }
        public string? ativa { get; private set; }
        public int? codigo_campanha { get; private set; }
        public bool? promocao_opcional { get; private set; }
        public long timestamp { get; private set; }
        public string? referencia { get; private set; }
        public int? portal { get; private set; }

        public B2CConsultaProdutosPromocao() { }

        public B2CConsultaProdutosPromocao(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.StocksAndPrices.B2CConsultaProdutosPromocao dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            codigo_promocao = MicrovixConverters.ToInt64(dto.codigo_promocao);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            preco = MicrovixConverters.ToNullableDecimal(dto.preco);
            data_inicio = MicrovixConverters.ToNullableDateTime(dto.data_inicio);
            data_termino = MicrovixConverters.ToNullableDateTime(dto.data_termino);
            data_cadastro = MicrovixConverters.ToNullableDateTime(dto.data_cadastro);
            ativa = MicrovixConverters.ToNullableString(dto.ativa, 1);
            codigo_campanha = MicrovixConverters.ToNullableInt32(dto.codigo_campanha);
            promocao_opcional = MicrovixConverters.ToNullableBoolean(dto.promocao_opcional);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            referencia = MicrovixConverters.ToNullableString(dto.referencia, 20);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
