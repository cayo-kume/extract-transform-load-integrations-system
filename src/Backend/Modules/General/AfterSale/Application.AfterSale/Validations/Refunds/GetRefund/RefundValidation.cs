using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Dtos.Reads.Refunds.GetRefund;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.GetRefund
{
    public class RefundValidation : AbstractValidator<Refund>
    {
        public RefundValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.type)
                .MaximumLength(50)
                .WithMessage("O campo 'type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.action)
                .MaximumLength(50)
                .WithMessage("O campo 'action' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.action));

            RuleFor(x => x.requested_shipping_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.requested_shipping_amount));

            RuleFor(x => x.shipping_method)
                .MaximumLength(50)
                .WithMessage("O campo 'shipping_method' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shipping_method));

            RuleFor(x => x.added_taxes_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.added_taxes_amount));

            RuleFor(x => x.requested_raw_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.requested_raw_amount));

            RuleFor(x => x.requested_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.requested_amount));

            RuleFor(x => x.requested_total_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.requested_total_amount));

            RuleFor(x => x.received_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.received_amount));

            RuleFor(x => x.received_raw_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.received_raw_amount));

            RuleFor(x => x.total_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.total_amount));

            RuleFor(x => x.free_shipping)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.free_shipping));

            RuleFor(x => x.created_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.created_at));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.seller_info)
                .MaximumLength(50)
                .WithMessage("O campo 'seller_info' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.seller_info));

            RuleFor(x => x.refunded_shipping_type)
                .MaximumLength(50)
                .WithMessage("O campo 'refunded_shipping_type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.refunded_shipping_type));

            RuleFor(x => x.can_edit_wire_transfer)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.can_edit_wire_transfer));

            RuleFor(x => x.can_edit_email)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.can_edit_email));

            RuleFor(x => x.has_wire_transfer_account)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.has_wire_transfer_account));

            RuleFor(x => x.customer_retention_method_id)
               .MaximumLength(50)
               .WithMessage("O campo 'customer_retention_method_id' deve ter no máximo 50 caracteres.")
               .When(x => !string.IsNullOrEmpty(x.customer_retention_method_id));

            RuleFor(x => x.external_order_url)
               .MaximumLength(300)
               .WithMessage("O campo 'external_order_url' deve ter no máximo 300 caracteres.")
               .When(x => !string.IsNullOrEmpty(x.external_order_url));

            RuleFor(x => x.cashback_account)
               .MaximumLength(50)
               .WithMessage("O campo 'cashback_account' deve ter no máximo 50 caracteres.")
               .When(x => !string.IsNullOrEmpty(x.cashback_account));

            RuleFor(x => x.should_ask_voucher_code)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.should_ask_voucher_code));

            RuleFor(x => x.comments_count)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.comments_count));

            RuleFor(x => x.enable_payment)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.enable_payment));

            RuleFor(x => x.insurance_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.insurance_amount));

            RuleFor(x => x.bonus_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.bonus_amount));

            RuleFor(x => x.bonus_amount_percent)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.bonus_amount_percent));

            RuleFor(x => x.customer)
                .SetValidator(new CustomerValidation());

            RuleFor(x => x.status)
                .SetValidator(new StatusValidation());

            RuleFor(x => x.voucher)
                .SetValidator(new VoucherValidation());

            RuleFor(x => x.reverse)
                .SetValidator(new ReverseValidation());

            RuleFor(x => x.product_price_type)
                .SetValidator(new ProductPriceTypeValidation());

            RuleForEach(x => x.status_histories)
                .SetValidator(new StatusHistoriesValidation());

            RuleForEach(x => x.total_amount_histories)
                .SetValidator(new TotalAmountHistoriesValidation());

            RuleForEach(x => x.products)
                .SetValidator(new ProductValidation());

            RuleForEach(x => x.order_payment_methods)
               .MaximumLength(50)
               .WithMessage("O campo 'order_payment_methods' deve ter no máximo 50 caracteres.")
               .When(x => x.order_payment_methods is { Count: > 0 });

            RuleForEach(x => x.could_change_status_for)
               .MaximumLength(50)
               .WithMessage("O campo 'could_change_status_for' deve ter no máximo 50 caracteres.")
               .When(x => x.could_change_status_for  is { Count: > 0 });
        }
    }
}
