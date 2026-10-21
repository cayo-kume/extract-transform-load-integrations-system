using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.Types
{
    public class TypeValidation : AbstractValidator<Domain.AfterSale.Dtos.Reads.Refunds.Types.Type>
    {
        public TypeValidation()
        {
            RuleFor(x => x.name)
                .MaximumLength(50)
                .WithMessage("O campo 'name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.description)
                .MaximumLength(250)
                .WithMessage("O campo 'description' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));
        }
    }
}
