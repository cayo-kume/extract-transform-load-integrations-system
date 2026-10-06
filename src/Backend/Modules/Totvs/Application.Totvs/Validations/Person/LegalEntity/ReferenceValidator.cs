using Domain.Totvs.Dtos.Person.LegalEntity;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.LegalEntity
{
    public class ReferenceValidator : AbstractValidator<Reference>
    {
        public ReferenceValidator()
        {
            RuleFor(x => x.sequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequence));

            RuleFor(x => x.type)
                .MaximumLength(60)
                .WithMessage("O campo 'type' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.description)
                .MaximumLength(250)
                .WithMessage("O campo 'description' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.phoneNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'phoneNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.phoneNumber));

            RuleFor(x => x.responsiblePersonName)
                .MaximumLength(250)
                .WithMessage("O campo 'responsiblePersonName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.responsiblePersonName));

            RuleFor(x => x.isInactive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isInactive));
        }
    }
}
