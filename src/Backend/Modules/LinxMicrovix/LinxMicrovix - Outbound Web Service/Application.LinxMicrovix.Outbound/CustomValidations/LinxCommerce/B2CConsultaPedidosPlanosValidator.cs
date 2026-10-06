using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaPedidosPlanosValidator : AbstractValidator<B2CConsultaPedidosPlanos>
    {
        public B2CConsultaPedidosPlanosValidator()
        {
            RuleFor(x => x.id_pedido_planos)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id_pedido_planos));

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
