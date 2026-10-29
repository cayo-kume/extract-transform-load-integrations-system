using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrdersDiscounts (scripts de tabelas do módulo)
    /// </summary>
    public class OrderDiscountValidator : AbstractValidator<OrderDiscount>
    {
        public OrderDiscountValidator()
        {
            RuleFor(x => x.DiscountID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'DiscountID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'DiscountID' deve ser um número inteiro.");

            RuleFor(x => x.Amount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Amount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Amount));

            RuleFor(x => x.Message)
                .MaximumLength(400)
                .WithMessage("O campo 'Message' deve ter no máximo 400 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Message));

            RuleFor(x => x.Reference)
                .MaximumLength(250)
                .WithMessage("O campo 'Reference' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Reference));

            RuleFor(x => x.Type)
                .MaximumLength(50)
                .WithMessage("O campo 'Type' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Type));
        }
    }
}
