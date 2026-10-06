

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxProdutosTabelas
    {
        public string? id_tabela { get; private set; }
        public string? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? nome_tabela { get; private set; }
        public string? ativa { get; private set; }
        public string? timestamp { get; private set; }
        public string? tipo_tabela { get; private set; }
        public string? codigo_integracao_ws { get; private set; }

        public LinxProdutosTabelas() { }

        public LinxProdutosTabelas(
            string? id_tabela,
            string? portal,
            string? cnpj_emp,
            string? nome_tabela,
            string? ativa,
            string? timestamp,
            string? tipo_tabela,
            string? codigo_integracao_ws
        )
        {
            this.id_tabela = id_tabela;
            this.nome_tabela = nome_tabela;
            this.ativa = ativa;
            this.timestamp = timestamp;
            this.portal = portal;
            this.cnpj_emp = cnpj_emp;
            this.tipo_tabela = tipo_tabela;
            this.codigo_integracao_ws = codigo_integracao_ws;
        }
    }
}