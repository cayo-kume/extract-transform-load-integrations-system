using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class ObservationValidator : AbstractValidator<Observation>
    {
        public ObservationValidator()
        {
            RuleFor(x => x.observation)
                .MaximumLength(300)
                .WithMessage("O campo 'observation' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));
        }
    }
}
