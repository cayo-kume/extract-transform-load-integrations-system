using Domain.AfterSale.Dtos.Reads.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Reverses.GetReverse
{
    public class CourierDataValidation : AbstractValidator<CourierData>
    {
        public CourierDataValidation()
        {
            RuleFor(x => x.posting_card)
                .MaximumLength(50)
                .WithMessage("O campo 'posting_card' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.posting_card));

            RuleFor(x => x.is_store_seller_contract)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_store_seller_contract));
        }
    }
}
