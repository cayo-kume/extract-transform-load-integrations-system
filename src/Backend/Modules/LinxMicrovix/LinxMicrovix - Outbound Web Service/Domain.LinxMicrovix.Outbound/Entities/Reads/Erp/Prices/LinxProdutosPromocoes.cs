using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices
{
    /// <summary>
    /// LinxProdutosPromocoes, tabela linxmicrovix.LinxProdutosPromocoes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_produto + data_inicio_promocao + id_campanha
    /// </summary>
    public class LinxProdutosPromocoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long cod_produto { get; private set; }
        public decimal? preco_promocao { get; private set; }
        public DateTime data_inicio_promocao { get; private set; }
        public DateTime? data_termino_promocao { get; private set; }
        public DateTime? data_cadastro_promocao { get; private set; }
        public string? promocao_ativa { get; private set; }
        public long id_campanha { get; private set; }
        public string? nome_campanha { get; private set; }
        public bool? promocao_opcional { get; private set; }
        public decimal? custo_total_campanha { get; private set; }
        public long timestamp { get; private set; }

        public LinxProdutosPromocoes() { }

        public LinxProdutosPromocoes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Prices.LinxProdutosPromocoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_produto = MicrovixConverters.ToInt64(dto.cod_produto);
            preco_promocao = MicrovixConverters.ToNullableDecimal(dto.preco_promocao);
            data_inicio_promocao = MicrovixConverters.ToNullableDateTime(dto.data_inicio_promocao) ?? new DateTime(1900, 1, 1);
            data_termino_promocao = MicrovixConverters.ToNullableDateTime(dto.data_termino_promocao);
            data_cadastro_promocao = MicrovixConverters.ToNullableDateTime(dto.data_cadastro_promocao);
            promocao_ativa = MicrovixConverters.ToNullableString(dto.promocao_ativa, 1);
            id_campanha = MicrovixConverters.ToInt64(dto.id_campanha);
            nome_campanha = MicrovixConverters.ToNullableString(dto.nome_campanha, 60);
            promocao_opcional = MicrovixConverters.ToNullableBoolean(dto.promocao_opcional);
            custo_total_campanha = MicrovixConverters.ToNullableDecimal(dto.custo_total_campanha);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
