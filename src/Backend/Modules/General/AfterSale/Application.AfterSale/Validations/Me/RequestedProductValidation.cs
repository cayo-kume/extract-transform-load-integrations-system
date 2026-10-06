using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class RequestedProductValidation : AbstractValidator<RequestedProduct>
    {
        public RequestedProductValidation()
        {
            RuleFor(x => x.ref_id)
                .MaximumLength(800)
                .WithMessage("O campo 'ref_id' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ref_id));

            RuleFor(x => x.sku)
                .MaximumLength(800)
                .WithMessage("O campo 'sku' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sku));

            RuleFor(x => x.unity)
                .MaximumLength(800)
                .WithMessage("O campo 'unity' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.unity));

            RuleFor(x => x.unity_plural)
                .MaximumLength(800)
                .WithMessage("O campo 'unity_plural' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.unity_plural));
        }
    }
}
