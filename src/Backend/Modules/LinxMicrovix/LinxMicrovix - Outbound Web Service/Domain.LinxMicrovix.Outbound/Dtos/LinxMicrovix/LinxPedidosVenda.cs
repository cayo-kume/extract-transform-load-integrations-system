

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxPedidosVenda
    {
        public string? cnpj_emp { get; private set; }
        public string? cod_pedido { get; private set; }
        public string? data_lancamento { get; private set; }
        public string? hora_lancamento { get; private set; }
        public string? transacao { get; private set; }
        public string? usuario { get; private set; }
        public string? codigo_cliente { get; private set; }
        public string? cod_produto { get; private set; }
        public string? quantidade { get; private set; }
        public string? valor_unitario { get; private set; }
        public string? cod_vendedor { get; private set; }
        public string? valor_frete { get; private set; }
        public string? valor_total { get; private set; }
        public string? desconto_item { get; private set; }
        public string? cod_plano_pagamento { get; private set; }
        public string? plano_pagamento { get; private set; }
        public string? obs { get; private set; }
        public string? aprovado { get; private set; }
        public string? cancelado { get; private set; }
        public string? data_aprovacao { get; private set; }
        public string? data_alteracao { get; private set; }
        public string? tipo_frete { get; private set; }
        public string? natureza_operacao { get; private set; }
        public string? tabela_preco { get; private set; }
        public string? nome_tabela_preco { get; private set; }
        public string? previsao_entrega { get; private set; }
        public string? realizado_por { get; private set; }
        public string? pontuacao_ser { get; private set; }
        public string? venda_externa { get; private set; }
        public string? nf_gerada { get; private set; }
        public string? status { get; private set; }
        public string? numero_projeto_officina { get; private set; }
        public string? cod_natureza_operacao { get; private set; }
        public string? margem_contribuicao { get; private set; }
        public string? doc_origem { get; private set; }
        public string? posicao_item { get; private set; }
        public string? orcamento_origem { get; private set; }
        public string? transacao_origem { get; private set; }
        public string? timestamp { get; private set; }
        public string? desconto { get; private set; }
        public string? transacao_ws { get; private set; }
        public string? empresa { get; private set; }
        public string? transportador { get; private set; }
        public string? deposito { get; private set; }
        public string? portal { get; private set; }

        public LinxPedidosVenda() { }

        public LinxPedidosVenda(
            string? cnpj_emp,
            string? cod_pedido,
            string? data_lancamento,
            string? hora_lancamento,
            string? transacao,
            string? usuario,
            string? codigo_cliente,
            string? cod_produto,
            string? quantidade,
            string? valor_unitario,
            string? cod_vendedor,
            string? valor_frete,
            string? valor_total,
            string? desconto_item,
            string? cod_plano_pagamento,
            string? plano_pagamento,
            string? obs,
            string? aprovado,
            string? cancelado,
            string? data_aprovacao,
            string? data_alteracao,
            string? tipo_frete,
            string? natureza_operacao,
            string? tabela_preco,
            string? nome_tabela_preco,
            string? previsao_entrega,
            string? realizado_por,
            string? pontuacao_ser,
            string? venda_externa,
            string? nf_gerada,
            string? status,
            string? numero_projeto_officina,
            string? cod_natureza_operacao,
            string? margem_contribuicao,
            string? doc_origem,
            string? posicao_item,
            string? orcamento_origem,
            string? transacao_origem,
            string? timestamp,
            string? desconto,
            string? transacao_ws,
            string? empresa,
            string? transportador,
            string? portal,
            string? deposito
        )
        {
            this.cnpj_emp = cnpj_emp;
            this.cod_pedido = cod_pedido;
            this.data_lancamento = data_lancamento;
            this.transacao = transacao;
            this.hora_lancamento = hora_lancamento;
            this.usuario = usuario;
            this.codigo_cliente = codigo_cliente;
            this.cod_produto = cod_produto;
            this.quantidade = quantidade;
            this.valor_unitario = valor_unitario;
            this.cod_vendedor = cod_vendedor;
            this.valor_frete = valor_frete;
            this.valor_total = valor_total;
            this.desconto_item = desconto_item;
            this.cod_plano_pagamento = cod_plano_pagamento;
            this.plano_pagamento = plano_pagamento;
            this.obs = obs?.Trim();
            this.aprovado = aprovado;
            this.cancelado = cancelado;
            this.data_aprovacao = data_aprovacao;
            this.data_alteracao = data_alteracao;
            this.tipo_frete = tipo_frete;
            this.natureza_operacao = natureza_operacao;
            this.tabela_preco = tabela_preco;
            this.nome_tabela_preco = nome_tabela_preco;
            this.previsao_entrega = previsao_entrega;
            this.realizado_por = realizado_por;
            this.pontuacao_ser = pontuacao_ser;
            this.venda_externa = venda_externa;
            this.nf_gerada = nf_gerada;
            this.status = status;
            this.numero_projeto_officina = numero_projeto_officina;
            this.cod_natureza_operacao = cod_natureza_operacao;
            this.margem_contribuicao = margem_contribuicao;
            this.doc_origem = doc_origem;
            this.posicao_item = posicao_item;
            this.orcamento_origem = orcamento_origem;
            this.transacao_origem = transacao_origem;
            this.timestamp = timestamp;
            this.desconto = desconto;
            this.transacao_ws = transacao_ws;
            this.empresa = empresa;
            this.transportador = transportador;
            this.portal = portal;
            this.deposito = deposito;
        }
    }
}