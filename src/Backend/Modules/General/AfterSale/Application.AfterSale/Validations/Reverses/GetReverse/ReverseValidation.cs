using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Dtos.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.GetReverse
{
    public class ReverseValidation : AbstractValidator<Reverse>
    {
        public ReverseValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.service_type_change)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.service_type_change));

            RuleFor(x => x.ecommerce_order_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.ecommerce_order_id));

            RuleFor(x => x.courier_contract_id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.courier_contract_id));

            RuleFor(x => x.invoice)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.invoice));

            RuleFor(x => x.courier_collect)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.courier_collect));

            RuleFor(x => x.is_awaiting_resend_webhook)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_awaiting_resend_webhook));

            RuleFor(x => x.is_erased)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_erased));

            RuleFor(x => x.freight_by_customer)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.freight_by_customer));

            RuleFor(x => x.skip_process_step)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.skip_process_step));

            RuleFor(x => x.billing_item_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.billing_item_id));

            RuleFor(x => x.status_id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.status_id));

            RuleFor(x => x.ecommerce_id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ecommerce_id));

            RuleFor(x => x.insurance_manual_needed)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.insurance_manual_needed));

            RuleFor(x => x.created_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.created_at));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.deleted_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deleted_at));

            RuleFor(x => x.origin_seller_id)
                .MaximumLength(50)
                .WithMessage("O campo 'origin_seller_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.origin_seller_id));

            RuleFor(x => x.reverse_type)
                .MaximumLength(50)
                .WithMessage("O campo 'reverse_type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reverse_type));

            RuleFor(x => x.courier_service_type)
                .MaximumLength(50)
                .WithMessage("O campo 'courier_service_type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.courier_service_type));

            RuleFor(x => x.store_id)
                .MaximumLength(50)
                .WithMessage("O campo 'store_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.store_id));

            RuleFor(x => x.total_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.total_amount));

            RuleFor(x => x.store_expire_date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.store_expire_date));

            RuleFor(x => x.origin)
                .MaximumLength(50)
                .WithMessage("O campo 'origin' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.origin));

            RuleFor(x => x.external_source)
                .MaximumLength(50)
                .WithMessage("O campo 'external_source' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.external_source));

            RuleFor(x => x.type)
                .MaximumLength(50)
                .WithMessage("O campo 'type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.billing_date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.billing_date));

            RuleFor(x => x.must_treat_refund)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.must_treat_refund));

            RuleFor(x => x.customer_nfd_attached)
                .MaximumLength(50)
                .WithMessage("O campo 'customer_nfd_attached' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customer_nfd_attached));

            RuleFor(x => x.returned_invoice)
                .MaximumLength(50)
                .WithMessage("O campo 'returned_invoice' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.returned_invoice));

            RuleFor(x => x.reverse_type_name)
                .MaximumLength(50)
                .WithMessage("O campo 'reverse_type_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reverse_type_name));

            RuleFor(x => x.is_generic_courier)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_generic_courier));

            RuleFor(x => x.can_generate_voucher)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.can_generate_voucher));

            RuleFor(x => x.could_cancel)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.could_cancel));

            RuleFor(x => x.can_send_correction_letter)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.can_send_correction_letter));

            RuleFor(x => x.correction_letter_link)
                .MaximumLength(300)
                .WithMessage("O campo 'correction_letter_link' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.correction_letter_link));

            RuleFor(x => x.customer_url)
                .MaximumLength(300)
                .WithMessage("O campo 'customer_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customer_url));

            RuleFor(x => x.timeline_url)
                .MaximumLength(300)
                .WithMessage("O campo 'timeline_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.timeline_url));

            RuleFor(x => x.collect_scheduling_link)
                .MaximumLength(300)
                .WithMessage("O campo 'collect_scheduling_link' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.collect_scheduling_link));

            RuleFor(x => x.courier_data)
                .SetValidator(new CourierDataValidation());

            RuleFor(x => x.destination_data)
                .SetValidator(new DestinationDataValidation());

            RuleFor(x => x.courier_data_encrypted)
                .SetValidator(new CourierDataEncryptedValidation());

            RuleFor(x => x.ecommerce)
                .SetValidator(new EcommerceValidation());

            RuleFor(x => x.ecommerce_order)
                .SetValidator(new EcommerceOrderValidation());

            RuleFor(x => x.status)
                .SetValidator(new StatusValidation());

            RuleFor(x => x.tracking)
                .SetValidator(new TrackingValidation());

            RuleFor(x => x.ecommerce_order_seller)
                .SetValidator(new EcommerceOrderSellerValidation());

            RuleFor(x => x.brand)
                .SetValidator(new BrandValidation());

            RuleFor(x => x.retry_options)
                .SetValidator(new RetryOptionsValidation());

            RuleFor(x => x.reverse_destination)
                .SetValidator(new ReverseDestinationValidation());

            RuleForEach(x => x.products)
                .SetValidator(new ProductValidation());

            RuleForEach(x => x.status_histories)
                .SetValidator(new StatusHistoriesValidation());
        }
    }
}
