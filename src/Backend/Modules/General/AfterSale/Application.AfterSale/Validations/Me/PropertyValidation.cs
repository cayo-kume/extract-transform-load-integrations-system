using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;

namespace Application.AfterSale.Validations.Me
{
    public class PropertyValidation : AbstractValidator<Property>
    {
        public PropertyValidation()
        {
            RuleFor(x => x.borderradius)
                .MaximumLength(50)
                .WithMessage("O campo 'borderradius' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.borderradius));

            RuleFor(x => x.background)
                .MaximumLength(50)
                .WithMessage("O campo 'background' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.background));

            RuleFor(x => x.border)
                .MaximumLength(50)
                .WithMessage("O campo 'border' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.border));

            RuleFor(x => x.text)
                .MaximumLength(50)
                .WithMessage("O campo 'text' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.text));

            RuleFor(x => x.primary)
                .MaximumLength(50)
                .WithMessage("O campo 'primary' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.primary));

            RuleFor(x => x.boxshadow)
                .MaximumLength(50)
                .WithMessage("O campo 'boxshadow' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.boxshadow));
        }
    }
}
