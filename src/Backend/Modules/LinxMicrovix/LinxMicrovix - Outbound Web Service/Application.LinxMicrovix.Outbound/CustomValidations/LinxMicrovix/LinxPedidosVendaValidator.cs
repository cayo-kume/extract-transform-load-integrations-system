using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxPedidosVendaValidator : AbstractValidator<LinxPedidosVenda>
    {
        public LinxPedidosVendaValidator()
        {
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.cod_pedido).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_pedido));
            RuleFor(x => x.data_lancamento).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_lancamento));
            RuleFor(x => x.hora_lancamento).MaximumLength(5).WithMessage("O campo 'hora_lancamento' deve ter no máximo 5 caracteres.").When(x => !string.IsNullOrEmpty(x.hora_lancamento));
            RuleFor(x => x.transacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao));
            RuleFor(x => x.usuario).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.usuario));
            RuleFor(x => x.codigo_cliente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_cliente));
            RuleFor(x => x.cod_produto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_produto));
            RuleFor(x => x.quantidade).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.quantidade));
            RuleFor(x => x.valor_unitario).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_unitario));
            RuleFor(x => x.cod_vendedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_vendedor));
            RuleFor(x => x.valor_frete).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_frete));
            RuleFor(x => x.valor_total).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_total));
            RuleFor(x => x.desconto_item).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.desconto_item));
            RuleFor(x => x.cod_plano_pagamento).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_plano_pagamento));
            RuleFor(x => x.plano_pagamento).MaximumLength(35).WithMessage("O campo 'plano_pagamento' deve ter no máximo 35 caracteres.").When(x => !string.IsNullOrEmpty(x.plano_pagamento));
            RuleFor(x => x.aprovado).MaximumLength(1).WithMessage("O campo 'aprovado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.aprovado));
            RuleFor(x => x.cancelado).MaximumLength(1).WithMessage("O campo 'cancelado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.cancelado));
            RuleFor(x => x.data_aprovacao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_aprovacao));
            RuleFor(x => x.data_alteracao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_alteracao));
            RuleFor(x => x.tipo_frete).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.tipo_frete));
            RuleFor(x => x.natureza_operacao).MaximumLength(60).WithMessage("O campo 'natureza_operacao' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.natureza_operacao));
            RuleFor(x => x.tabela_preco).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.tabela_preco));
            RuleFor(x => x.nome_tabela_preco).MaximumLength(50).WithMessage("O campo 'nome_tabela_preco' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_tabela_preco));
            RuleFor(x => x.previsao_entrega).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.previsao_entrega));
            RuleFor(x => x.realizado_por).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.realizado_por));
            RuleFor(x => x.pontuacao_ser).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.pontuacao_ser));
            RuleFor(x => x.venda_externa).MaximumLength(1).WithMessage("O campo 'venda_externa' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.venda_externa));
            RuleFor(x => x.status).MaximumLength(1).WithMessage("O campo 'status' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.status));
            RuleFor(x => x.numero_projeto_officina).MaximumLength(50).WithMessage("O campo 'numero_projeto_officina' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.numero_projeto_officina));
            RuleFor(x => x.cod_natureza_operacao).MaximumLength(10).WithMessage("O campo 'cod_natureza_operacao' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.cod_natureza_operacao));
            RuleFor(x => x.margem_contribuicao).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.margem_contribuicao));
            RuleFor(x => x.doc_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.doc_origem));
            RuleFor(x => x.posicao_item).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.posicao_item));
            RuleFor(x => x.orcamento_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.orcamento_origem));
            RuleFor(x => x.transacao_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao_origem));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.desconto).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.desconto));
            RuleFor(x => x.transacao_ws).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao_ws));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.transportador).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.transportador));
            RuleFor(x => x.deposito).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.deposito));
        }
    }
}
