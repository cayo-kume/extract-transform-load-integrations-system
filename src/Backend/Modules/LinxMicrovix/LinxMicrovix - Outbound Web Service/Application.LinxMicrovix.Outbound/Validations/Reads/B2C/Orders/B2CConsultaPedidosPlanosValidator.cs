using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Orders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script088_Create_LinxMicrovix_B2C_Orders_Tables
    /// </summary>
    public class B2CConsultaPedidosPlanosValidator : AbstractValidator<B2CConsultaPedidosPlanos>
    {
        public B2CConsultaPedidosPlanosValidator()
        {
            RuleFor(x => x.id_pedido_planos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_pedido_planos' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.id_pedido)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_pedido));

            RuleFor(x => x.plano_pagamento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano_pagamento));

            RuleFor(x => x.valor_plano)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_plano));

            RuleFor(x => x.nsu_sitef)
                .MaximumLength(20)
                .WithMessage("O campo 'nsu_sitef' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nsu_sitef));

            RuleFor(x => x.cod_autorizacao)
                .MaximumLength(50)
                .WithMessage("O campo 'cod_autorizacao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cod_autorizacao));

            RuleFor(x => x.cod_loja_sitef)
                .MaximumLength(10)
                .WithMessage("O campo 'cod_loja_sitef' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cod_loja_sitef));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
