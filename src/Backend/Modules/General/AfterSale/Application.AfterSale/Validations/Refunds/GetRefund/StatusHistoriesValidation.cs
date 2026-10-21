using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Dtos.Reads.Refunds.GetRefund;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.GetRefund
{
    public class StatusHistoriesValidation : AbstractValidator<StatusHistories>
    {
        public StatusHistoriesValidation()
        {
            RuleFor(x => x.date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.date));

            RuleFor(x => x.comments)
                .MaximumLength(300)
                .WithMessage("O campo 'comments' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.comments));

            RuleFor(x => x.user)
                .SetValidator(new UserValidation());

            RuleFor(x => x.status)
                .SetValidator(new StatusValidation());
        }
    }
}
