using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Dtos.Reads.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.GetReverse
{
    public class StatusHistoriesValidation : AbstractValidator<StatusHistories>
    {
        public StatusHistoriesValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.reverse_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.reverse_id));

            RuleFor(x => x.status_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.status_id));

            RuleFor(x => x.user_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.user_id));

            RuleFor(x => x.customer_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.customer_id));

            RuleFor(x => x.date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.date));

            RuleFor(x => x.comments)
                .MaximumLength(300)
                .WithMessage("O campo 'comments' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.comments));

            RuleFor(x => x.billing_message)
                .MaximumLength(800)
                .WithMessage("O campo 'billing_message' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.billing_message));

            RuleFor(x => x.status)
                .SetValidator(new StatusValidation());

            RuleFor(x => x.user)
                .SetValidator(new UserValidation());
        }
    }
}
