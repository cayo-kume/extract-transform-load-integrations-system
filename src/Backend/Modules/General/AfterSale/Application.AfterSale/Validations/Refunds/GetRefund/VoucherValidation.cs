using Domain.AfterSale.Dtos.Refunds.GetRefund;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.GetRefund
{
    public class VoucherValidation : AbstractValidator<Voucher>
    {
        public VoucherValidation()
        {
            RuleFor(x => x.expiring_date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.expiring_date));

            RuleFor(x => x.redemption_code)
                .MaximumLength(50)
                .WithMessage("O campo 'redemption_code' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.redemption_code));

            RuleFor(x => x.giftcard_id)
                .MaximumLength(50)
                .WithMessage("O campo 'giftcard_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.giftcard_id));
        }
    }
}
