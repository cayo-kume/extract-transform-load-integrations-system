using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// e-mail (EmailInDto): tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class EmailInValidator : AbstractValidator<EmailIn>
    {
        public EmailInValidator()
        {
            RuleFor(x => x.typeCode)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'typeCode' deve ter até 2 dígitos (0 zera).")
                .When(x => x.typeCode is not null);

            RuleFor(x => x.email)
                .MaximumLength(60)
                .WithMessage("O campo 'email' aceita até 60 caracteres.");

            RuleFor(x => x.email)
                .EmailAddress()
                .WithMessage("O campo 'email' não é um e-mail válido.")
                .When(x => !String.IsNullOrEmpty(x.email));
        }
    }
}
