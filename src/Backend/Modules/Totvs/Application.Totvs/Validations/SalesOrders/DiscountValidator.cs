using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class DiscountValidator : AbstractValidator<Discount>
    {
        public DiscountValidator()
        {
            RuleFor(x => x.typeDiscountCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'typeDiscountCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(40)
                .WithMessage("O campo 'description' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.discountPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountPercentage));

            RuleFor(x => x.discountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountValue));
        }
    }
}
