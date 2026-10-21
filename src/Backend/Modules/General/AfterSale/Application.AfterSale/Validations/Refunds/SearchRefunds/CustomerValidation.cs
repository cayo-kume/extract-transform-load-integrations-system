using Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.SearchRefunds
{
    public class CustomerValidation : AbstractValidator<Customer>
    {
        public CustomerValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.first_name)
                .MaximumLength(50)
                .WithMessage("O campo 'first_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.first_name));

            RuleFor(x => x.last_name)
                .MaximumLength(100)
                .WithMessage("O campo 'last_name' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.last_name));
        }
    }
}
