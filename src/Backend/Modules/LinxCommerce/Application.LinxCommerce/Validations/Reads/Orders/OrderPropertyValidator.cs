using Application.LinxCommerce.Validations.Reads.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Reads.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrdersProperties (scripts de tabelas do módulo)
    /// </summary>
    public class OrderPropertyValidator : AbstractValidator<OrderProperty>
    {
        public OrderPropertyValidator()
        {
            RuleFor(x => x.OrderPropertyID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'OrderPropertyID' é obrigatório.")
                .MustBeValidInt64()
                .WithMessage("O campo 'OrderPropertyID' deve ser um número inteiro.");

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

            RuleFor(x => x.Source)
                .MaximumLength(100)
                .WithMessage("O campo 'Source' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Source));

            RuleFor(x => x.Type)
                .MaximumLength(100)
                .WithMessage("O campo 'Type' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Type));
        }
    }
}
