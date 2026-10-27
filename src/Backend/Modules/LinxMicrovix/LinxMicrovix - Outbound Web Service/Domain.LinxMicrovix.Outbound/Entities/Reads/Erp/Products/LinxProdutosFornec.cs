using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products
{
    /// <summary>
    /// LinxProdutosFornec, tabela linxmicrovix.LinxProdutosFornec. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_prod_forn
    /// </summary>
    public class LinxProdutosFornec
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_prod_forn { get; private set; }
        public long? codigoproduto { get; private set; }
        public int? cod_fornecedor { get; private set; }
        public decimal? custo { get; private set; }
        public string? moeda { get; private set; }
        public string? unidade_compra { get; private set; }
        public decimal? fator_conversao_compra { get; private set; }
        public string? codauxiliar { get; private set; }
        public decimal? qtde_embalagem { get; private set; }
        public int? prazo_entrega_padrao { get; private set; }
        public int? empresa { get; private set; }
        public decimal? desconto1 { get; private set; }
        public decimal? desconto2 { get; private set; }
        public decimal? desconto3 { get; private set; }
        public decimal? desconto { get; private set; }
        public decimal? ipi1 { get; private set; }
        public decimal? diferencial_icms { get; private set; }
        public decimal? despesas1 { get; private set; }
        public decimal? acrescimo { get; private set; }
        public decimal? valor_custo_substituicao { get; private set; }
        public decimal? frete1 { get; private set; }
        public string? fornecedor_principal { get; private set; }
        public string? excluido { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxProdutosFornec() { }

        public LinxProdutosFornec(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Products.LinxProdutosFornec dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_prod_forn = MicrovixConverters.ToInt32(dto.id_prod_forn);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            cod_fornecedor = MicrovixConverters.ToNullableInt32(dto.cod_fornecedor);
            custo = MicrovixConverters.ToNullableDecimal(dto.custo);
            moeda = MicrovixConverters.ToNullableString(dto.moeda, 10);
            unidade_compra = MicrovixConverters.ToNullableString(dto.unidade_compra, 50);
            fator_conversao_compra = MicrovixConverters.ToNullableDecimal(dto.fator_conversao_compra);
            codauxiliar = MicrovixConverters.ToNullableString(dto.codauxiliar, 40);
            qtde_embalagem = MicrovixConverters.ToNullableDecimal(dto.qtde_embalagem);
            prazo_entrega_padrao = MicrovixConverters.ToNullableInt32(dto.prazo_entrega_padrao);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            desconto1 = MicrovixConverters.ToNullableDecimal(dto.desconto1);
            desconto2 = MicrovixConverters.ToNullableDecimal(dto.desconto2);
            desconto3 = MicrovixConverters.ToNullableDecimal(dto.desconto3);
            desconto = MicrovixConverters.ToNullableDecimal(dto.desconto);
            ipi1 = MicrovixConverters.ToNullableDecimal(dto.ipi1);
            diferencial_icms = MicrovixConverters.ToNullableDecimal(dto.diferencial_icms);
            despesas1 = MicrovixConverters.ToNullableDecimal(dto.despesas1);
            acrescimo = MicrovixConverters.ToNullableDecimal(dto.acrescimo);
            valor_custo_substituicao = MicrovixConverters.ToNullableDecimal(dto.valor_custo_substituicao);
            frete1 = MicrovixConverters.ToNullableDecimal(dto.frete1);
            fornecedor_principal = MicrovixConverters.ToNullableString(dto.fornecedor_principal, 1);
            excluido = MicrovixConverters.ToNullableString(dto.excluido, 1);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
