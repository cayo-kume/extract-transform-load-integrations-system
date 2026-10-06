using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class CompositionValidator : AbstractValidator<Composition>
    {
        public CompositionValidator()
        {
            RuleFor(x => x.percentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentage));

            RuleFor(x => x.material)
                .MaximumLength(60)
                .WithMessage("O campo 'material' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.material));
        }
    }
}
