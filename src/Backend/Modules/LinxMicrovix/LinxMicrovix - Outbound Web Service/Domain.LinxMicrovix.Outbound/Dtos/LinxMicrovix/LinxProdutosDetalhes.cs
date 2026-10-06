

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxProdutosDetalhes
    {
        public string? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? cod_produto { get; private set; }
        public string? cod_barra { get; private set; }
        public string? quantidade { get; private set; }
        public string? preco_custo { get; private set; }
        public string? preco_venda { get; private set; }
        public string? custo_medio { get; private set; }
        public string? id_config_tributaria { get; private set; }
        public string? desc_config_tributaria { get; private set; }
        public string? despesas1 { get; private set; }
        public string? qtde_minima { get; private set; }
        public string? qtde_maxima { get; private set; }
        public string? ipi { get; private set; }
        public string? timestamp { get; private set; }
        public string? empresa { get; private set; }

        public LinxProdutosDetalhes() { }

        public LinxProdutosDetalhes(
            string? portal,
            string? cnpj_emp,
            string? cod_produto,
            string? cod_barra,
            string? quantidade,
            string? preco_custo,
            string? preco_venda,
            string? custo_medio,
            string? id_config_tributaria,
            string? desc_config_tributaria,
            string? despesas1,
            string? qtde_minima,
            string? qtde_maxima,
            string? ipi,
            string? timestamp,
            string? empresa
        )
        {
            this.portal = portal;
            this.cnpj_emp = cnpj_emp;
            this.cod_produto = cod_produto;
            this.cod_barra = cod_barra;
            this.quantidade = quantidade;
            this.preco_custo = preco_custo;
            this.preco_venda = preco_venda;
            this.custo_medio = custo_medio;
            this.id_config_tributaria = id_config_tributaria;
            this.desc_config_tributaria = desc_config_tributaria;
            this.despesas1 = despesas1;
            this.qtde_minima = qtde_minima;
            this.qtde_maxima = qtde_maxima;
            this.ipi = ipi;
            this.timestamp = timestamp;
            this.empresa = empresa;
        }
    }
}