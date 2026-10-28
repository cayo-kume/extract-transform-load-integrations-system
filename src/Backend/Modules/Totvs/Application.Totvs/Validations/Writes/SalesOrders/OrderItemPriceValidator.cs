using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// item com o preço novo: produto por código ou SKU e preço positivo com até 9 inteiros e 6 decimais
    /// </summary>
    public class OrderItemPriceValidator : AbstractValidator<OrderItemPrice>
    {
        public OrderItemPriceValidator()
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

            RuleFor(x => x.price)
                .NotNull()
                .WithMessage("O campo 'price' é obrigatório.")
                .GreaterThan(0)
                .WithMessage("O campo 'price' deve ser maior que zero.")
                .LessThanOrEqualTo(999_999_999.999999m)
                .WithMessage("O campo 'price' aceita até 9 inteiros.")
                .PrecisionScale(15, 6, true)
                .WithMessage("O campo 'price' aceita até 6 casas decimais.");
        }
    }
}
