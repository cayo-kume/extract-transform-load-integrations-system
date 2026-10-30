using Domain.AfterSale.Dtos.Reads.Me;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class PageBodyContentValidation : AbstractValidator<PageBodyContent>
    {
        public PageBodyContentValidation()
        {
            RuleFor(x => x.title)
                .MaximumLength(800)
                .WithMessage("O campo 'title' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.title));

            RuleFor(x => x.description)
                .MaximumLength(800)
                .WithMessage("O campo 'description' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.label_commentary)
                .MaximumLength(800)
                .WithMessage("O campo 'label_commentary' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.label_commentary));

            RuleFor(x => x.description_commentary)
                .MaximumLength(800)
                .WithMessage("O campo 'description_commentary' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description_commentary));

            RuleFor(x => x.image_upload_title)
                .MaximumLength(800)
                .WithMessage("O campo 'image_upload_title' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.image_upload_title));

            RuleFor(x => x.image_upload_description)
                .MaximumLength(800)
                .WithMessage("O campo 'image_upload_description' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.image_upload_description));

            RuleFor(x => x.checkbox)
                .MaximumLength(800)
                .WithMessage("O campo 'checkbox' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.checkbox));

            RuleFor(x => x.deny_action)
                .MaximumLength(800)
                .WithMessage("O campo 'deny_action' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.deny_action));

            RuleFor(x => x.observation)
                .MaximumLength(800)
                .WithMessage("O campo 'observation' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));

            RuleFor(x => x.enabled_observations)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.enabled_observations));

            RuleFor(x => x.request_pix_key_disabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.request_pix_key_disabled));

            RuleFor(x => x.required_invoice_number)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.required_invoice_number));

            RuleFor(x => x.required_invoice_file)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.required_invoice_file));

            RuleFor(x => x.required_commentary)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.required_commentary));
        }
    }
}
