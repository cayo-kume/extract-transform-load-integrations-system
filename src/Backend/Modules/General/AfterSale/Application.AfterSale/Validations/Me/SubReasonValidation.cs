using Domain.AfterSale.Dtos.Me;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class SubReasonValidation : AbstractValidator<SubReason>
    {
        public SubReasonValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.reason_id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.reason_id));

            RuleFor(x => x.description)
                .MaximumLength(50)
                .WithMessage("O campo 'description' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.ord)
                .MaximumLength(50)
                .WithMessage("O campo 'ord' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ord));

            RuleFor(x => x.created_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.created_at));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.deleted_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deleted_at));
        }
    }
}
