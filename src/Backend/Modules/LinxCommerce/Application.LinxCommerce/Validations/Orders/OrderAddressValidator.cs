using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrdersAddresses (scripts de tabelas do módulo)
    /// </summary>
    public class OrderAddressValidator : AbstractValidator<OrderAddress>
    {
        public OrderAddressValidator()
        {
            RuleFor(x => x.OrderAddressID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'OrderAddressID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'OrderAddressID' deve ser um número inteiro.");

            RuleFor(x => x.AddressType)
                .MustBeValidInt32()
                .WithMessage("O campo 'AddressType' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.AddressType));

            RuleFor(x => x.Name)
                .MaximumLength(250)
                .WithMessage("O campo 'Name' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Name));

            RuleFor(x => x.AddressLine)
                .MaximumLength(400)
                .WithMessage("O campo 'AddressLine' deve ter no máximo 400 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.AddressLine));

            RuleFor(x => x.Number)
                .MaximumLength(50)
                .WithMessage("O campo 'Number' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Number));

            RuleFor(x => x.Neighbourhood)
                .MaximumLength(250)
                .WithMessage("O campo 'Neighbourhood' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Neighbourhood));

            RuleFor(x => x.City)
                .MaximumLength(250)
                .WithMessage("O campo 'City' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.City));

            RuleFor(x => x.State)
                .MaximumLength(50)
                .WithMessage("O campo 'State' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.State));

            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .WithMessage("O campo 'PostalCode' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.PostalCode));

            RuleFor(x => x.AddressNotes)
                .MaximumLength(400)
                .WithMessage("O campo 'AddressNotes' deve ter no máximo 400 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.AddressNotes));

            RuleFor(x => x.Landmark)
                .MaximumLength(400)
                .WithMessage("O campo 'Landmark' deve ter no máximo 400 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Landmark));

            RuleFor(x => x.ContactName)
                .MaximumLength(250)
                .WithMessage("O campo 'ContactName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ContactName));

            RuleFor(x => x.ContactDocumentNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'ContactDocumentNumber' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ContactDocumentNumber));

            RuleFor(x => x.ContactPhone)
                .MaximumLength(50)
                .WithMessage("O campo 'ContactPhone' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ContactPhone));

            RuleFor(x => x.PointOfSaleID)
                .MustBeValidInt32()
                .WithMessage("O campo 'PointOfSaleID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.PointOfSaleID));
        }
    }
}
