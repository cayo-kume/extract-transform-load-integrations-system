using Application.AfterSale.Validations.Reads.Me;
using Domain.AfterSale.Dtos.Reads.Refunds.GetRefund;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Refunds.GetRefund
{
    public class ProductValidation : AbstractValidator<Product>
    {
        public ProductValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.reverse_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.reverse_id));

            RuleFor(x => x.refund_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.refund_id));

            RuleFor(x => x.motive_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.motive_id));

            RuleFor(x => x.ecommerce_order_product_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.ecommerce_order_product_id));

            RuleFor(x => x.qty)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qty));

            RuleFor(x => x.requested_qty)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.requested_qty));

            RuleFor(x => x.received_qty)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.received_qty));

            RuleFor(x => x.product_received_comment)
                .MaximumLength(300)
                .WithMessage("O campo 'product_received_comment' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.product_received_comment));

            RuleFor(x => x.comments)
                .MaximumLength(300)
                .WithMessage("O campo 'comments' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.comments));

            RuleFor(x => x.reverse_action)
                .MaximumLength(50)
                .WithMessage("O campo 'reverse_action' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reverse_action));

            RuleFor(x => x.customer_retention_method_id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.customer_retention_method_id));

            RuleFor(x => x.protocol)
                .MaximumLength(50)
                .WithMessage("O campo 'protocol' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.protocol));

            RuleFor(x => x.product_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.product_id));

            RuleFor(x => x.is_included_by_admin)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_included_by_admin));

            RuleFor(x => x.is_bundle)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_bundle));

            RuleFor(x => x.hash)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.hash));

            RuleFor(x => x.name)
                .MaximumLength(300)
                .WithMessage("O campo 'name' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.sku)
                .MaximumLength(50)
                .WithMessage("O campo 'sku' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sku));

            RuleFor(x => x.reference)
                .MaximumLength(50)
                .WithMessage("O campo 'reference' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reference));

            RuleFor(x => x.price)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.price));

            RuleFor(x => x.selling_price)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.selling_price));

            RuleFor(x => x.shipping_price)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.shipping_price));

            RuleFor(x => x.weight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.weight));

            RuleFor(x => x.order_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.order_id));

            RuleFor(x => x.returned_invoice)
                .MaximumLength(50)
                .WithMessage("O campo 'returned_invoice' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.returned_invoice));

            RuleFor(x => x.returned_invoice_url)
                .MaximumLength(300)
                .WithMessage("O campo 'returned_invoice_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.returned_invoice_url));

            RuleFor(x => x.returned_invoice_url_xml)
                .MaximumLength(300)
                .WithMessage("O campo 'returned_invoice_url_xml' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.returned_invoice_url_xml));

            RuleFor(x => x.invoice)
                .MaximumLength(50)
                .WithMessage("O campo 'invoice' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.invoice));

            RuleFor(x => x.original_qty)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.original_qty));

            RuleFor(x => x.brand)
                .MaximumLength(50)
                .WithMessage("O campo 'brand' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.brand));

            RuleFor(x => x.situation)
                .MaximumLength(50)
                .WithMessage("O campo 'situation' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.situation));

            RuleFor(x => x.bundle_hash)
                .MaximumLength(50)
                .WithMessage("O campo 'bundle_hash' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bundle_hash));

            RuleFor(x => x.reason)
                .SetValidator(new ReasonValidation());

            RuleFor(x => x.subreason)
                .SetValidator(new SubReasonValidation());

            RuleForEach(x => x.images)
                .SetValidator(new ImageValidation());

            RuleForEach(x => x.image_url)
                .MaximumLength(300)
                .WithMessage("O campo 'image_url' deve ter no máximo 300 caracteres.")
                .When(x => x.image_url is { Count: > 0 });
        }
    }
}
