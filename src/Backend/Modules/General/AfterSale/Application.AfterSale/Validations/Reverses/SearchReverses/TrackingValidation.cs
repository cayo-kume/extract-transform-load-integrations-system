using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.SearchReverses
{
    public class TrackingValidation : AbstractValidator<Domain.AfterSale.Dtos.Reverses.SearchReverses.Tracking>
    {
        public TrackingValidation()
        {
            RuleFor(x => x.authorization_code)
                .MaximumLength(50)
                .WithMessage("O campo 'authorization_code' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.authorization_code));

            RuleFor(x => x.delivery_deadline)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.delivery_deadline));

            RuleFor(x => x.shipping_amount)
                .MaximumLength(50)
                .WithMessage("O campo 'shipping_amount' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shipping_amount));

            RuleFor(x => x.tracking_code)
                .MaximumLength(50)
                .WithMessage("O campo 'tracking_code' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tracking_code));
        }
    }
}
