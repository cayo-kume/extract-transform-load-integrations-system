using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// telefone (PhoneInDto): tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class PhoneInValidator : AbstractValidator<PhoneIn>
    {
        public PhoneInValidator()
        {
            RuleFor(x => x.typeCode)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'typeCode' deve ter até 2 dígitos (0 zera).")
                .When(x => x.typeCode is not null);

            RuleFor(x => x.number)
                .MaximumLength(20)
                .WithMessage("O campo 'number' aceita até 20 caracteres.");

            RuleFor(x => x.branchLine)
                .InclusiveBetween(0, 9_999)
                .WithMessage("O campo 'branchLine' deve ter até 4 dígitos (0 zera).")
                .When(x => x.branchLine is not null);
        }
    }
}
