using Domain.Totvs.Dtos.SalesOrder;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrder
{
    public class ObservationValidator : AbstractValidator<Observation>
    {
        public ObservationValidator()
        {
            RuleFor(x => x.observation)
                .MaximumLength(800)
                .WithMessage("O campo 'observation' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));
        }
    }
}
