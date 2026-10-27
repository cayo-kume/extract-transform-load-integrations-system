using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products
{
    /// <summary>
    /// LinxProdutosCamposAdicionais, tabela linxmicrovix.LinxProdutosCamposAdicionais. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_produto + campo
    /// </summary>
    public class LinxProdutosCamposAdicionais
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long cod_produto { get; private set; }
        public string campo { get; private set; } = String.Empty;
        public string? valor { get; private set; }
        public long timestamp { get; private set; }
        public int? id_campo_valor { get; private set; }

        public LinxProdutosCamposAdicionais() { }

        public LinxProdutosCamposAdicionais(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Products.LinxProdutosCamposAdicionais dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_produto = MicrovixConverters.ToInt64(dto.cod_produto);
            campo = MicrovixConverters.ToNullableString(dto.campo, 50) ?? String.Empty;   //fora da documentação
            valor = MicrovixConverters.ToNullableString(dto.valor, 30);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            id_campo_valor = MicrovixConverters.ToNullableInt32(dto.id_campo_valor);
        }
    }
}
