using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.GetRefund
{
    public class TypeValidation : AbstractValidator<Domain.AfterSale.Dtos.Refunds.GetRefund.Type>
    {
        public TypeValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.name)
                .MaximumLength(50)
                .WithMessage("O campo 'name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.description)
                .MaximumLength(150)
                .WithMessage("O campo 'description' deve ter no máximo 150 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));
        }
    }
}
