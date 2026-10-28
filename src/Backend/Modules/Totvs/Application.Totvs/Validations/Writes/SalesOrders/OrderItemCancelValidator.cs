using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// item a cancelar: produto por código ou SKU, quantidade positiva (delta, 9 inteiros e 3 decimais) e o motivo
    /// </summary>
    public class OrderItemCancelValidator : AbstractValidator<OrderItemCancel>
    {
        public OrderItemCancelValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.productCode is null) != String.IsNullOrEmpty(x.productSku))
                .WithName("productCode")
                .WithMessage("Informe exatamente um entre 'productCode' e 'productSku'.");

            RuleFor(x => x.productCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'productCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.productCode is not null);

            RuleFor(x => x.productSku)
                .MaximumLength(40)
                .WithMessage("O campo 'productSku' aceita até 40 caracteres.");

            RuleFor(x => x.cancelQuantity)
                .NotNull()
                .WithMessage("O campo 'cancelQuantity' é obrigatório.")
                .GreaterThan(0)
                .WithMessage("O campo 'cancelQuantity' deve ser maior que zero.")
                .LessThanOrEqualTo(999_999_999.999m)
                .WithMessage("O campo 'cancelQuantity' aceita até 9 inteiros.")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'cancelQuantity' aceita até 3 casas decimais.");

            RuleFor(x => x.reasonCancellationCode)
                .NotNull()
                .WithMessage("O campo 'reasonCancellationCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'reasonCancellationCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.reasonCancellationDescription)
                .MaximumLength(80)
                .WithMessage("O campo 'reasonCancellationDescription' aceita até 80 caracteres.");
        }
    }
}
