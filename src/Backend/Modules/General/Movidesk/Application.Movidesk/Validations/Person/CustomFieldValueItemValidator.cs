using Domain.Core.Extensions;
using Domain.Movidesk.Dtos.Person;
using FluentValidation;

namespace Application.Movidesk.Validations.Person
{
    public class CustomFieldValueItemValidator : AbstractValidator<CustomFieldValueItem>
    {
        public CustomFieldValueItemValidator()
        {
            RuleFor(x => x.personId)
               .MustBeValidInt32()
               .When(x => !string.IsNullOrEmpty(x.personId));

            RuleFor(x => x.clientId)
               .MustBeValidInt32()
               .When(x => !string.IsNullOrEmpty(x.clientId));

            RuleFor(x => x.team)
                .MaximumLength(128)
                .WithMessage("O campo 'team' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.team));

            RuleFor(x => x.customFieldItem)
                .MaximumLength(256)
                .WithMessage("O campo 'customFieldItem' deve ter no máximo 256 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customFieldItem));
        }
    }
}
