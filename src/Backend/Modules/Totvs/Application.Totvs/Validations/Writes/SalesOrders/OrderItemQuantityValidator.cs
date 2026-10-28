using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// item com a quantidade nova: produto por código ou SKU e a quantidade (sem nullable no swagger: omitida, iria 0)
    /// </summary>
    public class OrderItemQuantityValidator : AbstractValidator<OrderItemQuantity>
    {
        public OrderItemQuantityValidator()
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

            RuleFor(x => x.quantity)
                .NotNull()
                .WithMessage("O campo 'quantity' é obrigatório (a quantidade nova, maior que a atual).")
                .GreaterThan(0)
                .WithMessage("O campo 'quantity' deve ser maior que zero.")
                .LessThanOrEqualTo(999_999_999.999m)
                .WithMessage("O campo 'quantity' aceita até 9 inteiros.")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'quantity' aceita até 3 casas decimais.");
        }
    }
}
