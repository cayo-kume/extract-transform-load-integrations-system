

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxPedidosCompra
    {
        public string? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? cod_pedido { get; private set; }
        public string? data_pedido { get; private set; }
        public string? transacao { get; private set; }
        public string? usuario { get; private set; }
        public string? codigo_fornecedor { get; private set; }
        public string? cod_produto { get; private set; }
        public string? quantidade { get; private set; }
        public string? valor_unitario { get; private set; }
        public string? cod_comprador { get; private set; }
        public string? valor_frete { get; private set; }
        public string? valor_total { get; private set; }
        public string? cod_plano_pagamento { get; private set; }
        public string? plano_pagamento { get; private set; }
        public string? obs { get; private set; }
        public string? aprovado { get; private set; }
        public string? cancelado { get; private set; }
        public string? encerrado { get; private set; }
        public string? data_aprovacao { get; private set; }
        public string? numero_ped_fornec { get; private set; }
        public string? tipo_frete { get; private set; }
        public string? nf_origem_ws { get; private set; }
        public string? natureza_operacao { get; private set; }
        public string? previsao_entrega { get; private set; }
        public string? numero_projeto_officina { get; private set; }
        public string? status_pedido { get; private set; }
        public string? qtde_entregue { get; private set; }
        public string? descricao_frete { get; private set; }
        public string? integrado_linx { get; private set; }
        public string? nf_gerada { get; private set; }
        public string? timestamp { get; private set; }
        public string? empresa { get; private set; }

        public LinxPedidosCompra() { }

        public LinxPedidosCompra(
            string? portal,
            string? cnpj_emp,
            string? cod_pedido,
            string? data_pedido,
            string? transacao,
            string? usuario,
            string? codigo_fornecedor,
            string? cod_produto,
            string? quantidade,
            string? valor_unitario,
            string? cod_comprador,
            string? valor_frete,
            string? valor_total,
            string? cod_plano_pagamento,
            string? plano_pagamento,
            string? obs,
            string? aprovado,
            string? cancelado,
            string? encerrado,
            string? data_aprovacao,
            string? numero_ped_fornec,
            string? tipo_frete,
            string? nf_origem_ws,
            string? natureza_operacao,
            string? previsao_entrega,
            string? numero_projeto_officina,
            string? status_pedido,
            string? qtde_entregue,
            string? descricao_frete,
            string? integrado_linx,
            string? nf_gerada,
            string? timestamp,
            string? empresa
        )
        {
            this.portal = portal;
            this.cnpj_emp = cnpj_emp;
            this.cod_pedido = cod_pedido;
            this.data_aprovacao = data_aprovacao;
            this.data_pedido = data_pedido;
            this.transacao = transacao;
            this.usuario = usuario;
            this.codigo_fornecedor = codigo_fornecedor;
            this.cod_produto = cod_produto;
            this.quantidade = quantidade;
            this.valor_unitario = valor_unitario;
            this.cod_comprador = cod_comprador;
            this.valor_frete = valor_frete;
            this.valor_total = valor_total;
            this.cod_plano_pagamento = cod_plano_pagamento;
            this.plano_pagamento = plano_pagamento;
            this.obs = obs;
            this.aprovado = aprovado;
            this.cancelado = cancelado;
            this.encerrado = encerrado;
            this.numero_ped_fornec = numero_ped_fornec;
            this.tipo_frete = tipo_frete;
            this.nf_origem_ws = nf_origem_ws;
            this.natureza_operacao = natureza_operacao;
            this.previsao_entrega = previsao_entrega;
            this.numero_projeto_officina = numero_projeto_officina;
            this.status_pedido = status_pedido;
            this.qtde_entregue = qtde_entregue;
            this.descricao_frete = descricao_frete;
            this.integrado_linx = integrado_linx;
            this.nf_gerada = nf_gerada;
            this.timestamp = timestamp;
            this.empresa = empresa;
        }
    }
}