

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxProdutosPromocoes
    {
        public string? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? cod_produto { get; private set; }
        public string? preco_promocao { get; private set; }
        public string? data_inicio_promocao { get; private set; }
        public string? data_termino_promocao { get; private set; }
        public string? data_cadastro_promocao { get; private set; }
        public string? promocao_ativa { get; private set; }
        public string? id_campanha { get; private set; }
        public string? nome_campanha { get; private set; }
        public string? promocao_opcional { get; private set; }
        public string? custo_total_campanha { get; private set; }

        public LinxProdutosPromocoes() { }

        public LinxProdutosPromocoes(
            string? portal,
            string? cnpj_emp,
            string? cod_produto,
            string? preco_promocao,
            string? data_inicio_promocao,
            string? data_termino_promocao,
            string? data_cadastro_promocao,
            string? promocao_ativa,
            string? id_campanha,
            string? nome_campanha,
            string? promocao_opcional,
            string? custo_total_campanha
        )
        {
            this.portal = portal;
            this.cnpj_emp = cnpj_emp;
            this.cod_produto = cod_produto;
            this.preco_promocao = preco_promocao;
            this.data_inicio_promocao = data_inicio_promocao;
            this.data_termino_promocao = data_termino_promocao;
            this.data_cadastro_promocao = data_cadastro_promocao;
            this.promocao_ativa = promocao_ativa;
            this.id_campanha = id_campanha;
            this.nome_campanha = nome_campanha;
            this.promocao_opcional = promocao_opcional;
            this.custo_total_campanha = custo_total_campanha;
        }
    }
}