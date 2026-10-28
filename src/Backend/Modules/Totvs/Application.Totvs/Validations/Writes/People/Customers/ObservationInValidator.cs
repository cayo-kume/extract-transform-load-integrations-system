using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// observação (ObservationInDto): tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class ObservationInValidator : AbstractValidator<ObservationIn>
    {
        public ObservationInValidator()
        {
            RuleFor(x => x.observation)
                .MaximumLength(80)
                .WithMessage("O campo 'observation' aceita até 80 caracteres.");

            RuleFor(x => x.sequence)
                .InclusiveBetween(0, 999_999_999)
                .WithMessage("O campo 'sequence' deve ter até 9 dígitos (0 zera).")
                .When(x => x.sequence is not null);
        }
    }
}
