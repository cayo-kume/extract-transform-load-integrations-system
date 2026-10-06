using Domain.Totvs.Dtos.Person.LegalEntity;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.LegalEntity
{
    public class PhoneValidator : AbstractValidator<Phone>
    {
        public PhoneValidator()
        {
            RuleFor(x => x.Sequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.Sequence));

            RuleFor(x => x.typeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.typeCode));

            RuleFor(x => x.typeName)
                .MaximumLength(60)
                .WithMessage("O campo 'typeName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeName));

            RuleFor(x => x.number)
                .MaximumLength(20)
                .WithMessage("O campo 'number' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.number));

            RuleFor(x => x.branchLine)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchLine));
        }
    }
}
