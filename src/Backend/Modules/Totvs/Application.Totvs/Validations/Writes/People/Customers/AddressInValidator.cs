using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// endereço (AddressInDto). Sem sequence, um endereço de entrega igual a um existente não é incluído de novo: tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class AddressInValidator : AbstractValidator<AddressIn>
    {
        public AddressInValidator()
        {
            RuleFor(x => x.addressType)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.Address))
                .WithMessage($"O campo 'addressType' só aceita {CustomerRules.Names(CustomerEnumNames.Address)}.");

            RuleFor(x => x.sequence)
                .InclusiveBetween(0, 999)
                .WithMessage("O campo 'sequence' deve ter até 3 dígitos (0 zera).")
                .When(x => x.sequence is not null);

            RuleFor(x => x.postOfficeBox)
                .InclusiveBetween(0, 999_999_999)
                .WithMessage("O campo 'postOfficeBox' deve ter até 9 dígitos (0 zera).")
                .When(x => x.postOfficeBox is not null);

            RuleFor(x => x.cep)
                .Matches(@"^(\d{8})?$")
                .WithMessage("O campo 'cep' deve ter 8 dígitos, sem pontuação (ou \"\" para zerar).");

            RuleFor(x => x.cityName)
                .MaximumLength(60)
                .WithMessage("O campo 'cityName' aceita até 60 caracteres.");

            RuleFor(x => x.stateAbbreviation)
                .MaximumLength(3)
                .WithMessage("O campo 'stateAbbreviation' aceita até 3 caracteres.");

            RuleFor(x => x.publicPlace)
                .MaximumLength(10)
                .WithMessage("O campo 'publicPlace' aceita até 10 caracteres.");

            RuleFor(x => x.address)
                .MaximumLength(60)
                .WithMessage("O campo 'address' aceita até 60 caracteres.");

            RuleFor(x => x.number)
                .InclusiveBetween(0, 99_999)
                .WithMessage("O campo 'number' deve ter até 5 dígitos (0 zera).")
                .When(x => x.number is not null);

            RuleFor(x => x.reference)
                .MaximumLength(60)
                .WithMessage("O campo 'reference' aceita até 60 caracteres.");

            RuleFor(x => x.complement)
                .MaximumLength(65)
                .WithMessage("O campo 'complement' aceita até 65 caracteres.");

            RuleFor(x => x.neighborhood)
                .MaximumLength(60)
                .WithMessage("O campo 'neighborhood' aceita até 60 caracteres.");
        }
    }
}
