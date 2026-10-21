using Domain.AfterSale.Dtos.Reads.Refunds.GetRefund;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.GetRefund
{
    public class ProductPriceTypeValidation : AbstractValidator<ProductPriceType>
    {
        public ProductPriceTypeValidation()
        {
            RuleFor(x => x.type)
                .MaximumLength(50)
                .WithMessage("O campo 'type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.label)
                .MaximumLength(50)
                .WithMessage("O campo 'label' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.label));
        }
    }
}
