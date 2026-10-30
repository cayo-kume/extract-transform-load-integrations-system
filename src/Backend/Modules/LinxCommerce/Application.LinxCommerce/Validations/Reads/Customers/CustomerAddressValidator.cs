using Application.LinxCommerce.Validations.Reads.Base;
using Domain.LinxCommerce.Dtos.Reads.Customers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Reads.Customers
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceCustomersAddresses (scripts de tabelas do módulo)
    /// </summary>
    public class CustomerAddressValidator : AbstractValidator<CustomerAddress>
    {
        public CustomerAddressValidator()
        {
            RuleFor(x => x.ID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'ID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'ID' deve ser um número inteiro.");

            RuleFor(x => x.MainAddress)
                .MustBeValidBoolean()
                .WithMessage("O campo 'MainAddress' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.MainAddress));

            RuleFor(x => x.Name)
                .MaximumLength(250)
                .WithMessage("O campo 'Name' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Name));

            RuleFor(x => x.ContactName)
                .MaximumLength(250)
                .WithMessage("O campo 'ContactName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ContactName));

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
        }
    }
}
