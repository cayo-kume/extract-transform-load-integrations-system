using Application.LinxCommerce.Validations.Reads.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Reads.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrdersShipments (scripts de tabelas do módulo)
    /// </summary>
    public class OrderShipmentValidator : AbstractValidator<OrderShipment>
    {
        public OrderShipmentValidator()
        {
            RuleFor(x => x.OrderShipmentID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'OrderShipmentID' é obrigatório.")
                .MustBeValidGuid()
                .WithMessage("O campo 'OrderShipmentID' deve ser um Guid válido.");

            RuleFor(x => x.DeliveryMethodID)
                .MustBeValidInt32()
                .WithMessage("O campo 'DeliveryMethodID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.DeliveryMethodID));

            RuleFor(x => x.ShipmentNumber)
                .MaximumLength(50)
                .WithMessage("O campo 'ShipmentNumber' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ShipmentNumber));

            RuleFor(x => x.ShipmentStatus)
                .MaximumLength(50)
                .WithMessage("O campo 'ShipmentStatus' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ShipmentStatus));

            RuleFor(x => x.AssignUserId)
                .MustBeValidInt32()
                .WithMessage("O campo 'AssignUserId' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.AssignUserId));

            RuleFor(x => x.AssignUserName)
                .MaximumLength(250)
                .WithMessage("O campo 'AssignUserName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.AssignUserName));

            RuleFor(x => x.DockID)
                .MustBeValidInt32()
                .WithMessage("O campo 'DockID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.DockID));

            RuleForEach(x => x.Packages)
                .SetValidator(new OrderPackageValidator());
        }
    }
}
