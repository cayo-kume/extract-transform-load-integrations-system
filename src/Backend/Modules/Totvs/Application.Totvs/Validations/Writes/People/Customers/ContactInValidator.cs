using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// contato da pessoa jurídica (ContactInDto): sequence só na alteração: tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class ContactInValidator : AbstractValidator<ContactIn>
    {
        public ContactInValidator()
        {
            RuleFor(x => x.sequence)
                .InclusiveBetween(0, 999)
                .WithMessage("O campo 'sequence' deve ter até 3 dígitos (0 zera).")
                .When(x => x.sequence is not null);

            RuleFor(x => x.typeCode)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'typeCode' deve ter até 2 dígitos (0 zera).")
                .When(x => x.typeCode is not null);

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' aceita até 60 caracteres.");

            RuleFor(x => x.function)
                .MaximumLength(20)
                .WithMessage("O campo 'function' aceita até 20 caracteres.");

            RuleFor(x => x.phoneNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'phoneNumber' aceita até 20 caracteres.");

            RuleFor(x => x.cellNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'cellNumber' aceita até 20 caracteres.");

            RuleFor(x => x.email)
                .MaximumLength(60)
                .WithMessage("O campo 'email' aceita até 60 caracteres.");

            RuleFor(x => x.email)
                .EmailAddress()
                .WithMessage("O campo 'email' do contato não é um e-mail válido.")
                .When(x => !String.IsNullOrEmpty(x.email));
        }
    }
}
