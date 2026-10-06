using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaPlanosValidator : AbstractValidator<B2CConsultaPlanos>
    {
        public B2CConsultaPlanosValidator()
        {
            RuleFor(x => x.plano)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano));

            RuleFor(x => x.nome_plano)
                .MaximumLength(30)
                .WithMessage("O campo 'nome_plano' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_plano));

            RuleFor(x => x.forma_pagamento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.forma_pagamento));

            RuleFor(x => x.qtde_parcelas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtde_parcelas));

            RuleFor(x => x.valor_minimo_parcela)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_minimo_parcela));

            RuleFor(x => x.indice)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.indice));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.desativado)
                .MaximumLength(1)
                .WithMessage("O campo 'desativado' deve ter no máximo 1 caractere.")
                .When(x => !string.IsNullOrEmpty(x.desativado));

            RuleFor(x => x.tipo_plano)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_plano' deve ter no máximo 1 caractere.")
                .When(x => !string.IsNullOrEmpty(x.tipo_plano));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
