

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxMovimentoTrocas
    {
        public string? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? identificador { get; private set; }
        public string? num_vale { get; private set; }
        public string? valor_vale { get; private set; }
        public string? motivo { get; private set; }
        public string? doc_origem { get; private set; }
        public string? serie_origem { get; private set; }
        public string? doc_venda { get; private set; }
        public string? serie_venda { get; private set; }
        public string? excluido { get; private set; }
        public string? timestamp { get; private set; }
        public string? desfazimento { get; private set; }
        public string? empresa { get; private set; }
        public string? vale_cod_cliente { get; private set; }
        public string? vale_codigoproduto { get; private set; }
        public string? id_vale_ordem_servico_externa { get; private set; }
        public string? doc_venda_origem { get; private set; }
        public string? serie_venda_origem { get; private set; }
        public string? cod_cliente { get; private set; }

        public LinxMovimentoTrocas() { }

        public LinxMovimentoTrocas(
            string? portal,
            string? cnpj_emp,
            string? identificador,
            string? num_vale,
            string? valor_vale,
            string? motivo,
            string? doc_origem,
            string? serie_origem,
            string? doc_venda,
            string? serie_venda,
            string? excluido,
            string? timestamp,
            string? desfazimento,
            string? empresa,
            string? vale_cod_cliente,
            string? vale_codigoproduto,
            string? id_vale_ordem_servico_externa,
            string? doc_venda_origem,
            string? serie_venda_origem,
            string? cod_cliente
        )
        {
            this.portal = portal;
            this.cnpj_emp = cnpj_emp;
            this.identificador = identificador;
            this.num_vale = num_vale;
            this.valor_vale = valor_vale;
            this.motivo = motivo;
            this.doc_origem = doc_origem;
            this.serie_origem = serie_origem;
            this.doc_venda = doc_venda;
            this.serie_venda = serie_venda;
            this.excluido = excluido;
            this.timestamp = timestamp;
            this.desfazimento = desfazimento;
            this.empresa = empresa;
            this.vale_cod_cliente = vale_cod_cliente;
            this.vale_codigoproduto = vale_codigoproduto;
            this.id_vale_ordem_servico_externa = id_vale_ordem_servico_externa;
            this.doc_venda_origem = doc_venda_origem;
            this.serie_venda_origem = serie_venda_origem;
            this.cod_cliente = cod_cliente;
        }
    }
}