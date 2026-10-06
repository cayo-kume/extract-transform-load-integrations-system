using Domain.Totvs.Dtos.Person.LegalEntity;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.LegalEntity
{
    public class EmailValidator : AbstractValidator<Email>
    {
        public EmailValidator()
        {
            RuleFor(x => x.sequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequence));

            RuleFor(x => x.typeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.typeCode));

            RuleFor(x => x.typeName)
                .MaximumLength(60)
                .WithMessage("O campo 'typeName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeName));

            RuleFor(x => x.email)
                .MaximumLength(60)
                .WithMessage("O campo 'email' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.email));

            RuleFor(x => x.isDefault)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isDefault));
        }
    }
}