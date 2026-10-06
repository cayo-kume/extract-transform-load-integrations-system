using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Dtos.Refunds.GetRefund;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.GetRefund
{
    public class TotalAmountHistoriesValidation : AbstractValidator<TotalAmountHistories>
    {
        public TotalAmountHistoriesValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.date));

            RuleFor(x => x.total_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.total_amount));

            RuleFor(x => x.user)
                .SetValidator(new UserValidation());

            RuleFor(x => x.type)
                .SetValidator(new TypeValidation());
        }
    }
}
