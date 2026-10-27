using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stocks
{
    /// <summary>
    /// LinxProdutosDetalhes, tabela linxmicrovix.LinxProdutosDetalhes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_produto
    /// </summary>
    public class LinxProdutosDetalhes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long cod_produto { get; private set; }
        public string? cod_barra { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? preco_custo { get; private set; }
        public decimal? preco_venda { get; private set; }
        public decimal? custo_medio { get; private set; }
        public int? id_config_tributaria { get; private set; }
        public string? desc_config_tributaria { get; private set; }
        public decimal? despesas1 { get; private set; }
        public decimal? qtde_minima { get; private set; }
        public decimal? qtde_maxima { get; private set; }
        public decimal? ipi { get; private set; }
        public long timestamp { get; private set; }
        public decimal? custototal { get; private set; }
        public int? empresa { get; private set; }
        public int? id_produtos_status_compra { get; private set; }
        public string? localizacao { get; private set; }

        public LinxProdutosDetalhes() { }

        public LinxProdutosDetalhes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stocks.LinxProdutosDetalhes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_produto = MicrovixConverters.ToInt64(dto.cod_produto);
            cod_barra = MicrovixConverters.ToNullableString(dto.cod_barra, 20);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            preco_custo = MicrovixConverters.ToNullableDecimal(dto.preco_custo);
            preco_venda = MicrovixConverters.ToNullableDecimal(dto.preco_venda);
            custo_medio = MicrovixConverters.ToNullableDecimal(dto.custo_medio);
            id_config_tributaria = MicrovixConverters.ToNullableInt32(dto.id_config_tributaria);
            desc_config_tributaria = MicrovixConverters.ToNullableString(dto.desc_config_tributaria, 100);
            despesas1 = MicrovixConverters.ToNullableDecimal(dto.despesas1);
            qtde_minima = MicrovixConverters.ToNullableDecimal(dto.qtde_minima);
            qtde_maxima = MicrovixConverters.ToNullableDecimal(dto.qtde_maxima);
            ipi = MicrovixConverters.ToNullableDecimal(dto.ipi);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            custototal = MicrovixConverters.ToNullableDecimal(dto.custototal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_produtos_status_compra = MicrovixConverters.ToNullableInt32(dto.id_produtos_status_compra);
            localizacao = MicrovixConverters.ToNullableString(dto.localizacao, 50);
        }
    }
}
