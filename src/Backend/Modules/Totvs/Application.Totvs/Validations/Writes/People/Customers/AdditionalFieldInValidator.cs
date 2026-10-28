using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// campo adicional (AdditionalFieldInDto): o value vai como veio (texto até 80, número com até 12 inteiros e 6 decimais ou: tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class AdditionalFieldInValidator : AbstractValidator<AdditionalFieldIn>
    {
        public AdditionalFieldInValidator()
        {
            RuleFor(x => x.code)
                .InclusiveBetween(0, 999_999_999)
                .WithMessage("O campo 'code' deve ter até 9 dígitos (0 zera).")
                .When(x => x.code is not null);

            RuleFor(x => x.code)
                .NotNull()
                .WithMessage("O campo 'code' do campo adicional é obrigatório.")
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'code' do campo adicional deve ter de 1 a 9 dígitos.");

            RuleFor(x => x.value)
                .Must(CustomerRules.IsAdditionalValue)
                .WithMessage("O campo 'value' aceita texto até 80 caracteres, número com até 12 inteiros e 6 decimais ou data DD/MM/YYYY.")
                .When(x => x.value is not null);
        }
    }
}
