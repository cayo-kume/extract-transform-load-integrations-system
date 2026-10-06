using Domain.Totvs.Dtos.SalesOrder;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrder
{
    public class RepresentativeObservationValidator : AbstractValidator<RepresentativeObservation>
    {
        public RepresentativeObservationValidator()
        {
            RuleFor(x => x.observation)
                .MaximumLength(800)
                .WithMessage("O campo 'observation' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));

            RuleFor(x => x.visualizationType)
                .MaximumLength(60)
                .WithMessage("O campo 'visualizationType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.visualizationType));
        }
    }
}
