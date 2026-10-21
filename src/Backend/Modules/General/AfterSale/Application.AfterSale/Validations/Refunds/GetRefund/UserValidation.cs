using Domain.AfterSale.Dtos.Reads.Refunds.GetRefund;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.GetRefund
{
    public class UserValidation : AbstractValidator<User>
    {
        public UserValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.name)
                .MaximumLength(50)
                .WithMessage("O campo 'name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.first_name)
                .MaximumLength(50)
                .WithMessage("O campo 'first_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.first_name));

            RuleFor(x => x.last_name)
                .MaximumLength(100)
                .WithMessage("O campo 'last_name' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.last_name));

            RuleFor(x => x.email)
                .MaximumLength(50)
                .WithMessage("O campo 'email' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.email));

            RuleFor(x => x.mobile_phone)
                .MaximumLength(20)
                .WithMessage("O campo 'mobile_phone' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.mobile_phone));

            RuleFor(x => x.phone)
                .MaximumLength(20)
                .WithMessage("O campo 'phone' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.phone));

            RuleFor(x => x.document)
                .MaximumLength(14)
                .WithMessage("O campo 'document' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.document));

            RuleFor(x => x.wallet)
                .MaximumLength(50)
                .WithMessage("O campo 'wallet' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.wallet));
        }
    }
}
