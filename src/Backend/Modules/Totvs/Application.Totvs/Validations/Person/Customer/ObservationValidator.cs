using Domain.Totvs.Dtos.Person.Customer;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.Customer
{
    public class ObservationValidator : AbstractValidator<Observation>
    {
        public ObservationValidator()
        {
            RuleFor(x => x.sequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequence));

            RuleFor(x => x.observation)
                .MaximumLength(300)
                .WithMessage("O campo 'observation' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));

            RuleFor(x => x.isMaintenance)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isMaintenance));
        }
    }
}
