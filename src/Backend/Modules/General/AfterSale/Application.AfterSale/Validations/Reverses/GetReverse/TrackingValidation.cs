using Domain.AfterSale.Dtos.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.GetReverse
{
    public class TrackingValidation : AbstractValidator<Tracking>
    {
        public TrackingValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.reverse_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.reverse_id));

            RuleFor(x => x.courier_contract_id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.courier_contract_id));

            RuleFor(x => x.authorization_code)
                .MaximumLength(50)
                .WithMessage("O campo 'authorization_code' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.authorization_code));

            RuleFor(x => x.tracking_code)
                .MaximumLength(50)
                .WithMessage("O campo 'tracking_code' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tracking_code));

            RuleFor(x => x.shipping_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.shipping_amount));

            RuleFor(x => x.package_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.package_amount));

            RuleFor(x => x.courier_name)
                .MaximumLength(250)
                .WithMessage("O campo 'courier_name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.courier_name));

            RuleFor(x => x.tracking_url)
                .MaximumLength(300)
                .WithMessage("O campo 'tracking_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tracking_url));

            RuleFor(x => x.expire_date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.expire_date));

            RuleFor(x => x.collect_date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.collect_date));

            RuleFor(x => x.requested_collect_date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.requested_collect_date));

            RuleFor(x => x.status)
                .MaximumLength(50)
                .WithMessage("O campo 'status' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.message)
                .MaximumLength(800)
                .WithMessage("O campo 'message' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.message));

            RuleFor(x => x.status_updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.status_updated_at));

            RuleFor(x => x.is_change_collect_to_post)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_change_collect_to_post));

            RuleFor(x => x.type)
                .MaximumLength(50)
                .WithMessage("O campo 'type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.qr_code)
                .MaximumLength(800)
                .WithMessage("O campo 'qr_code' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.qr_code));

            RuleFor(x => x.service_type)
                .MaximumLength(50)
                .WithMessage("O campo 'service_type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.service_type));

            RuleFor(x => x.cte)
                .MaximumLength(50)
                .WithMessage("O campo 'cte' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cte));

            RuleFor(x => x.delivery_deadline)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.delivery_deadline));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.deleted_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deleted_at));
        }
    }
}
