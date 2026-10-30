using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Orders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script088_Create_LinxMicrovix_B2C_Orders_Tables
    /// </summary>
    public class B2CConsultaPedidosValidator : AbstractValidator<B2CConsultaPedidos>
    {
        public B2CConsultaPedidosValidator()
        {
            RuleFor(x => x.id_pedido)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_pedido' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.dt_pedido)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_pedido));

            RuleFor(x => x.cod_cliente_erp)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_erp));

            RuleFor(x => x.cod_cliente_b2c)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_b2c));

            RuleFor(x => x.vl_frete)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.vl_frete));

            RuleFor(x => x.forma_pgto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.forma_pgto));

            RuleFor(x => x.plano_pagamento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano_pagamento));

            RuleFor(x => x.anotacao)
                .MaximumLength(400)
                .WithMessage("O campo 'anotacao' deve ter no máximo 400 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.anotacao));

            RuleFor(x => x.taxa_impressao)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.taxa_impressao));

            RuleFor(x => x.finalizado)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.finalizado));

            RuleFor(x => x.valor_frete_gratis)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_frete_gratis));

            RuleFor(x => x.tipo_frete)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tipo_frete));

            RuleFor(x => x.id_status)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_status));

            RuleFor(x => x.cod_transportador)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_transportador));

            RuleFor(x => x.tipo_cobranca_frete)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tipo_cobranca_frete));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_tabela_preco)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_tabela_preco));

            RuleFor(x => x.valor_credito)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_credito));

            RuleFor(x => x.cod_vendedor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_vendedor));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.dt_insert)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_insert));

            RuleFor(x => x.dt_disponivel_faturamento)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_disponivel_faturamento));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.order_id)
                .MaximumLength(40)
                .WithMessage("O campo 'order_id' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.order_id));

            RuleFor(x => x.fulfillment_id)
                .MaximumLength(40)
                .WithMessage("O campo 'fulfillment_id' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fulfillment_id));

            RuleFor(x => x.id_tipo_b2c)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_tipo_b2c));

            RuleFor(x => x.ecommerce_origem)
                .MaximumLength(200)
                .WithMessage("O campo 'ecommerce_origem' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ecommerce_origem));

            RuleFor(x => x.id_endereco_entrega)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_endereco_entrega));

            RuleFor(x => x.marketplace_loja)
                .MaximumLength(50)
                .WithMessage("O campo 'marketplace_loja' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.marketplace_loja));

            RuleFor(x => x.desconto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto));

            RuleFor(x => x.order_id_alternativo)
                .MaximumLength(50)
                .WithMessage("O campo 'order_id_alternativo' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.order_id_alternativo));

            RuleFor(x => x.cnpj_origem_venda)
                .MaximumLength(1000)
                .WithMessage("O campo 'cnpj_origem_venda' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_origem_venda));

            RuleFor(x => x.data_previsao_entrega)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_previsao_entrega));

            RuleFor(x => x.marketplace_full)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.marketplace_full));
        }
    }
}
