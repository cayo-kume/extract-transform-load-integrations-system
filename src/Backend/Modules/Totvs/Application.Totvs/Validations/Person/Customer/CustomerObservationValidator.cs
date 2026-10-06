using Domain.Totvs.Dtos.Person.Customer;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.Customer
{
    public class CustomerObservationValidator : AbstractValidator<Customerobservation>
    {
        public CustomerObservationValidator()
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

            RuleFor(x => x.LastChangeDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.LastChangeDate));
        }
    }
}
