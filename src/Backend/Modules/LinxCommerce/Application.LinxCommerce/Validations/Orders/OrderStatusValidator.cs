using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrderStatus (scripts de tabelas do módulo)
    /// </summary>
    public class OrderStatusValidator : AbstractValidator<OrderStatus>
    {
        public OrderStatusValidator()
        {
            RuleFor(x => x.OrderStatusID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'OrderStatusID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'OrderStatusID' deve ser um número inteiro.");

            RuleFor(x => x.Status)
                .MaximumLength(100)
                .WithMessage("O campo 'Status' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Status));

            RuleFor(x => x.Color)
                .MaximumLength(50)
                .WithMessage("O campo 'Color' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Color));

            RuleFor(x => x.IsDefault)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsDefault' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsDefault));

            RuleFor(x => x.IntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'IntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.IntegrationID));
        }
    }
}
