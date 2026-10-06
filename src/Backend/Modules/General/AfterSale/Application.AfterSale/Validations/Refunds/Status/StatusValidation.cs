using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.Status
{
    public class StatusValidation : AbstractValidator<Domain.AfterSale.Dtos.Refunds.Status.Status>
    {
        public StatusValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

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
