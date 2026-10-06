using Domain.Totvs.Dtos.Person.Customer;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.Customer
{
    public class FamiliarValidator : AbstractValidator<Familiar>
    {
        public FamiliarValidator()
        {
            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.birthDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.birthDate));

            RuleFor(x => x.gender)
                .MaximumLength(60)
                .WithMessage("O campo 'gender' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.gender));

            RuleFor(x => x.kinshipDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'kinshipDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.kinshipDescription));
        }
    }
}
