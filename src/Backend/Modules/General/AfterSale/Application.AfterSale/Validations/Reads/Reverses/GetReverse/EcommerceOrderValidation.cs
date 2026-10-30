using Domain.AfterSale.Dtos.Reads.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Reverses.GetReverse
{
    public class EcommerceOrderValidation : AbstractValidator<EcommerceOrder>
    {
        public EcommerceOrderValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.order_id)
                .MaximumLength(50)
                .WithMessage("O campo 'order_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.order_id));

            RuleFor(x => x.invoice_number)
                .MaximumLength(50)
                .WithMessage("O campo 'invoice_number' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.invoice_number));

            RuleFor(x => x.uuid)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.uuid));

            RuleFor(x => x.shipping_value)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.shipping_value));

            RuleFor(x => x.ordered_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.ordered_at));

            RuleFor(x => x.invoiced_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.invoiced_at));

            RuleFor(x => x.delivered_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.delivered_at));

            RuleFor(x => x.value)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.value));

            RuleFor(x => x.ecommerce_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.ecommerce_id));

            RuleFor(x => x.is_online)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_online));

            RuleFor(x => x.is_marketplace)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_marketplace));

            RuleFor(x => x.customer_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.customer_id));

            RuleFor(x => x.marketplace_name)
                .MaximumLength(250)
                .WithMessage("O campo 'marketplace_name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.marketplace_name));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.deleted_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deleted_at));

            RuleFor(x => x.is_erased)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_erased));

            RuleFor(x => x.is_restrict)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_restrict));

            RuleForEach(x => x.payment_methods)
                .MaximumLength(50)
                .WithMessage("O campo 'payment_methods' deve ter no máximo 50 caracteres.")
                .When(x => x.payment_methods is { Count: > 0 }); ;
        }
    }
}
