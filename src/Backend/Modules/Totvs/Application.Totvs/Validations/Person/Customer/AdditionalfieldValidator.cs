using Domain.Totvs.Dtos.Person.Customer;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.Customer
{
    public class AdditionalfieldValidator : AbstractValidator<Additionalfield>
    {
        public AdditionalfieldValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.type)
                .MaximumLength(60)
                .WithMessage("O campo 'type' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.value)
                .MaximumLength(250)
                .WithMessage("O campo 'value' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.value));
        }
    }
}
