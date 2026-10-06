using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class ShortcodeValidation : AbstractValidator<Shortcode>
    {
        public ShortcodeValidation()
        {
            RuleFor(x => x.app_name)
                .MaximumLength(50)
                .WithMessage("O campo 'app_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.app_name));

            RuleFor(x => x.ecommerce_name)
                .MaximumLength(50)
                .WithMessage("O campo 'ecommerce_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ecommerce_name));

            RuleFor(x => x.ecommerce_phone)
                .MaximumLength(50)
                .WithMessage("O campo 'ecommerce_phone' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ecommerce_phone));

            RuleFor(x => x.ecommerce_url)
                .MaximumLength(300)
                .WithMessage("O campo 'ecommerce_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ecommerce_url));

            RuleFor(x => x.ecommerce_logo_url)
                .MaximumLength(300)
                .WithMessage("O campo 'ecommerce_logo_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ecommerce_logo_url));

            RuleFor(x => x.ecommerce_logo_email_url)
                .MaximumLength(300)
                .WithMessage("O campo 'ecommerce_logo_email_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ecommerce_logo_email_url));

            RuleFor(x => x.ecommerce_display_name)
                .MaximumLength(50)
                .WithMessage("O campo 'ecommerce_display_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ecommerce_display_name));
        }
    }
}
