using Domain.AfterSale.Dtos.Reads.Me;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class ReasonValidation : AbstractValidator<Reason>
    {
        public ReasonValidation()
        {
            RuleFor(x => x.deleted_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deleted_at));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.created_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.created_at));

            RuleFor(x => x.description)
                .MaximumLength(300)
                .WithMessage("O campo 'description' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.ord)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ord));

            RuleFor(x => x.action)
                .MaximumLength(50)
                .WithMessage("O campo 'action' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.action));

            RuleFor(x => x.upload_image)
                .MaximumLength(50)
                .WithMessage("O campo 'upload_image' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.upload_image));

            RuleFor(x => x.should_approve)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.should_approve));

            RuleFor(x => x.show_product_grid)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.show_product_grid));

            RuleFor(x => x.ecommerce_id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ecommerce_id));

            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleForEach(x => x.subreasons)
                .SetValidator(new SubReasonValidation());
        }
    }
}
