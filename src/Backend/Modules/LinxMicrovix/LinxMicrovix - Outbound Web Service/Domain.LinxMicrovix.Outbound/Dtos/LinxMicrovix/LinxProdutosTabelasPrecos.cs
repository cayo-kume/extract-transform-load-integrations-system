namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxProdutosTabelasPrecos
    {
        public string? cod_produto { get; private set; }
        public string? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? id_tabela { get; private set; }
        public string? precovenda { get; private set; }
        public string? timestamp { get; private set; }

        public LinxProdutosTabelasPrecos() { }

        public LinxProdutosTabelasPrecos(
            string? cod_produto,
            string? portal,
            string? cnpj_emp,
            string? id_tabela,
            string? precovenda,
            string? timestamp
        )
        {
            this.cod_produto = cod_produto;
            this.cnpj_emp = cnpj_emp;
            this.id_tabela = id_tabela;
            this.precovenda = precovenda;
            this.timestamp = timestamp;
            this.portal = portal;
        }
    }
}