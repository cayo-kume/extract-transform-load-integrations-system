using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PurchaseOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.PurchaseOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script055_Create_LinxMicrovix_Purchase_Orders_Tables
    /// </summary>
    public class LinxPedidosCompraValidator : AbstractValidator<LinxPedidosCompra>
    {
        public LinxPedidosCompraValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.cod_pedido)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_pedido));

            RuleFor(x => x.data_pedido)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_pedido));

            RuleFor(x => x.transacao)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'transacao' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.usuario)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.usuario));

            RuleFor(x => x.codigo_fornecedor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_fornecedor));

            RuleFor(x => x.cod_produto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.cod_produto));

            RuleFor(x => x.quantidade)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantidade));

            RuleFor(x => x.valor_unitario)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_unitario));

            RuleFor(x => x.cod_comprador)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_comprador));

            RuleFor(x => x.valor_frete)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_frete));

            RuleFor(x => x.valor_total)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_total));

            RuleFor(x => x.cod_plano_pagamento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_plano_pagamento));

            RuleFor(x => x.plano_pagamento)
                .MaximumLength(100)
                .WithMessage("O campo 'plano_pagamento' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.plano_pagamento));

            RuleFor(x => x.aprovado)
                .MaximumLength(1)
                .WithMessage("O campo 'aprovado' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.aprovado));

            RuleFor(x => x.cancelado)
                .MaximumLength(1)
                .WithMessage("O campo 'cancelado' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cancelado));

            RuleFor(x => x.encerrado)
                .MaximumLength(1)
                .WithMessage("O campo 'encerrado' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.encerrado));

            RuleFor(x => x.data_aprovacao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_aprovacao));

            RuleFor(x => x.numero_ped_fornec)
                .MaximumLength(15)
                .WithMessage("O campo 'numero_ped_fornec' deve ter no máximo 15 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_ped_fornec));

            RuleFor(x => x.tipo_frete)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_frete' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_frete));

            RuleFor(x => x.natureza_operacao)
                .MaximumLength(73)
                .WithMessage("O campo 'natureza_operacao' deve ter no máximo 73 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.natureza_operacao));

            RuleFor(x => x.previsao_entrega)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.previsao_entrega));

            RuleFor(x => x.numero_projeto_officina)
                .MaximumLength(50)
                .WithMessage("O campo 'numero_projeto_officina' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_projeto_officina));

            RuleFor(x => x.status_pedido)
                .MaximumLength(1)
                .WithMessage("O campo 'status_pedido' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status_pedido));

            RuleFor(x => x.qtde_entregue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_entregue));

            RuleFor(x => x.descricao_frete)
                .MaximumLength(40)
                .WithMessage("O campo 'descricao_frete' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_frete));

            RuleFor(x => x.integrado_linx)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.integrado_linx));

            RuleFor(x => x.nf_gerada)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.nf_gerada));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.nf_origem_ws)
                .MaximumLength(30)
                .WithMessage("O campo 'nf_origem_ws' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nf_origem_ws));

            RuleFor(x => x.id_compras_pedido_status)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_compras_pedido_status));

            RuleFor(x => x.qtde_desejada)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_desejada));

            RuleFor(x => x.motivo_ajuste)
                .MaximumLength(500)
                .WithMessage("O campo 'motivo_ajuste' deve ter no máximo 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.motivo_ajuste));

            RuleFor(x => x.saldo_disponivel_api)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.saldo_disponivel_api));
        }
    }
}
