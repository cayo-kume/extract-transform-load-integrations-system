using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Dtos.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.GetReverse
{
    public class CustomerValidation : AbstractValidator<Customer>
    {
        public CustomerValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.first_name)
                .MaximumLength(50)
                .WithMessage("O campo 'first_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.first_name));

            RuleFor(x => x.last_name)
                .MaximumLength(250)
                .WithMessage("O campo 'last_name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.last_name));

            RuleFor(x => x.email)
                .MaximumLength(50)
                .WithMessage("O campo 'email' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.email));

            RuleFor(x => x.contact_email)
                .MaximumLength(50)
                .WithMessage("O campo 'contact_email' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.contact_email));

            RuleFor(x => x.document)
                .MaximumLength(14)
                .WithMessage("O campo 'document' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.document));

            RuleFor(x => x.phone)
                .MaximumLength(20)
                .WithMessage("O campo 'phone' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.phone));

            RuleFor(x => x.contact_phone)
                .MaximumLength(20)
                .WithMessage("O campo 'contact_phone' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.contact_phone));

            RuleFor(x => x.address_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.address_id));

            RuleFor(x => x.shipping_address_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.shipping_address_id));

            RuleFor(x => x.is_restrict)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_restrict));

            RuleFor(x => x.is_erased)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_erased));

            RuleFor(x => x.first_name_encrypted)
                .MaximumLength(50)
                .WithMessage("O campo 'first_name_encrypted' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.first_name_encrypted));

            RuleFor(x => x.last_name_encrypted)
                .MaximumLength(250)
                .WithMessage("O campo 'last_name_encrypted' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.last_name_encrypted));

            RuleFor(x => x.document_encrypted)
                .MaximumLength(14)
                .WithMessage("O campo 'document_encrypted' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.document_encrypted));

            RuleFor(x => x.email_encrypted)
                .MaximumLength(50)
                .WithMessage("O campo 'email_encrypted' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.email_encrypted));

            RuleFor(x => x.phone_encrypted)
                .MaximumLength(20)
                .WithMessage("O campo 'phone_encrypted' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.phone_encrypted));

            RuleFor(x => x.contact_phone_encrypted)
                .MaximumLength(20)
                .WithMessage("O campo 'contact_phone_encrypted' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.contact_phone_encrypted));

            RuleFor(x => x.order_email)
                .MaximumLength(50)
                .WithMessage("O campo 'order_email' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.order_email));

            RuleFor(x => x.order_phone)
                .MaximumLength(20)
                .WithMessage("O campo 'order_phone' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.order_phone));

            RuleFor(x => x.address)
                .SetValidator(new AddressValidation());

            RuleFor(x => x.shipping_address)
                .SetValidator(new ShippingAddressValidation());
        }
    }
}
