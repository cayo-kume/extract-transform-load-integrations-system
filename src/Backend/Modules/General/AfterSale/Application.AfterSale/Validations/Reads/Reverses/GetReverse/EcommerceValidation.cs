using Domain.AfterSale.Dtos.Reads.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Reverses.GetReverse
{
    public class EcommerceValidation : AbstractValidator<Ecommerce>
    {
        public EcommerceValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.uuid)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.uuid));

            RuleFor(x => x.company_name)
                .MaximumLength(250)
                .WithMessage("O campo 'company_name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.company_name));

            RuleFor(x => x.trade_name)
                .MaximumLength(250)
                .WithMessage("O campo 'trade_name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.trade_name));

            RuleFor(x => x.display_name)
                .MaximumLength(250)
                .WithMessage("O campo 'display_name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.display_name));

            RuleFor(x => x.url)
                .MaximumLength(300)
                .WithMessage("O campo 'url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.url));

            RuleFor(x => x.is_active)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_active));

            RuleFor(x => x.phone)
                .MaximumLength(20)
                .WithMessage("O campo 'phone' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.phone));

            RuleFor(x => x.email)
                .MaximumLength(50)
                .WithMessage("O campo 'email' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.email));

            RuleFor(x => x.document)
                .MaximumLength(14)
                .WithMessage("O campo 'document' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.document));

            RuleFor(x => x.address_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.address_id));

            RuleFor(x => x.maintenance_mode_global)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.maintenance_mode_global));

            RuleFor(x => x.is_test)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_test));

            RuleFor(x => x.last_order_report_date)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.last_order_report_date));
        }
    }
}
