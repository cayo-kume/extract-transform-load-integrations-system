using Domain.AfterSale.Dtos.Reads.Refunds.GetRefund;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Refunds.GetRefund
{
    public class ImageValidation : AbstractValidator<Image>
    {
        public ImageValidation()
        {
            RuleFor(x => x.id)
               .MustBeValidInt32()
               .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.product_id)
                .MaximumLength(50)
                .WithMessage("O campo 'product_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.product_id));

            RuleFor(x => x.image_path)
                .MaximumLength(300)
                .WithMessage("O campo 'image_path' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.image_path));

            RuleFor(x => x.image_url)
                .MaximumLength(300)
                .WithMessage("O campo 'image_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.image_url));
        }
    }
}
