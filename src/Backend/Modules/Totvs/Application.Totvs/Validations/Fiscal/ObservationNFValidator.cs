using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class ObservationNFValidator : AbstractValidator<ObservationNF>
    {
        public ObservationNFValidator()
        {
            RuleFor(x => x.sequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequence));

            RuleFor(x => x.observation)
                .MaximumLength(8000)
                .WithMessage("O campo 'observation' deve ter no máximo 8000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));
        }
    }
}
