using Application.Totvs.Validations.Writes.People.Customers;
using Domain.Totvs.Dtos.Writes.Products.Batches;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Batches
{
    /// <summary>
    /// campo adicional do item de lote: código (4) e valor (texto, número ou data DD/MM/YYYY)
    /// </summary>
    public class BatchAdditionalFieldValidator : AbstractValidator<BatchAdditionalField>
    {
        public BatchAdditionalFieldValidator()
        {
            RuleFor(x => x.code)
                .NotNull()
                .WithMessage("O campo 'code' do campo adicional é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'code' do campo adicional deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.value)
                .NotNull()
                .WithMessage("O campo 'value' do campo adicional é obrigatório.")
                .Must(CustomerRules.IsAdditionalValue)
                .WithMessage("O campo 'value' aceita texto até 80 caracteres, número com até 12 inteiros e 6 decimais ou data DD/MM/YYYY.");
        }
    }
}
