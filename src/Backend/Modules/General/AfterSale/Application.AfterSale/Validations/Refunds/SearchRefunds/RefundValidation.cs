using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.SearchRefunds
{
    public class RefundValidation : AbstractValidator<Refund>
    {
        public RefundValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.action)
                .MaximumLength(50)
                .WithMessage("O campo 'action' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.action));

            RuleFor(x => x.bonus_amount_percent)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.bonus_amount_percent));

            RuleFor(x => x.order_id)
                .MaximumLength(50)
                .WithMessage("O campo 'order_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.order_id));

            RuleFor(x => x.requested_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.requested_amount));

            RuleFor(x => x.reverse_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.reverse_id));

            RuleFor(x => x.total_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.total_amount));

            RuleFor(x => x.type)
                .MaximumLength(50)
                .WithMessage("O campo 'type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.created_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.created_at));

            RuleFor(x => x.ecommerce_order_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.ecommerce_order_id));

            RuleFor(x => x.bonus_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.bonus_amount));

            RuleFor(x => x.requested_shipping_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.requested_shipping_amount));

            RuleFor(x => x.refund_attachment)
                .MaximumLength(50)
                .WithMessage("O campo 'refund_attachment' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.refund_attachment));

            RuleFor(x => x.shipping_method)
                .MaximumLength(50)
                .WithMessage("O campo 'shipping_method' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shipping_method));

            RuleFor(x => x.requested_raw_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.requested_raw_amount));

            RuleFor(x => x.refunded_shipping_type)
                .MaximumLength(50)
                .WithMessage("O campo 'refunded_shipping_type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.refunded_shipping_type));

            RuleFor(x => x.last_status_history_date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.last_status_history_date));

            RuleFor(x => x.customer)
                .SetValidator(new CustomerValidation());

            RuleFor(x => x.status)
                .SetValidator(new StatusValidation());

            RuleFor(x => x.reverse)
                .SetValidator(new ReverseValidation());

            RuleFor(x => x.product_price_type)
                .SetValidator(new ProductPriceTypeValidation());
        }
    }
}
