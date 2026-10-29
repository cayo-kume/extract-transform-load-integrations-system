using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Customers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Customers
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceCustomers (scripts de tabelas do módulo)
    /// </summary>
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.CustomerID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'CustomerID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'CustomerID' deve ser um número inteiro.");

            RuleFor(x => x.CustomerType)
                .MaximumLength(10)
                .WithMessage("O campo 'CustomerType' deve ter no máximo 10 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CustomerType));

            RuleFor(x => x.CustomerStatusID)
                .MustBeValidInt32()
                .WithMessage("O campo 'CustomerStatusID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.CustomerStatusID));

            RuleFor(x => x.WebSiteID)
                .MaximumLength(50)
                .WithMessage("O campo 'WebSiteID' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.WebSiteID));

            RuleFor(x => x.Name)
                .MaximumLength(250)
                .WithMessage("O campo 'Name' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Surname)
                .MaximumLength(250)
                .WithMessage("O campo 'Surname' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Surname));

            RuleFor(x => x.TradingName)
                .MaximumLength(250)
                .WithMessage("O campo 'TradingName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.TradingName));

            RuleFor(x => x.Email)
                .MaximumLength(250)
                .WithMessage("O campo 'Email' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Cpf)
                .MaximumLength(20)
                .WithMessage("O campo 'Cpf' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Cpf));

            RuleFor(x => x.Cnpj)
                .MaximumLength(20)
                .WithMessage("O campo 'Cnpj' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Cnpj));

            RuleFor(x => x.RG)
                .MaximumLength(30)
                .WithMessage("O campo 'RG' deve ter no máximo 30 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.RG));

            RuleFor(x => x.Gender)
                .MaximumLength(10)
                .WithMessage("O campo 'Gender' deve ter no máximo 10 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Gender));

            RuleFor(x => x.BirthDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'BirthDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.BirthDate));

            RuleFor(x => x.CreatedDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'CreatedDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.CreatedDate));

            RuleFor(x => x.Contact!.Phone)
                .MaximumLength(50)
                .WithMessage("O campo 'Contact.Phone' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.Phone));

            RuleFor(x => x.Contact!.Phone2)
                .MaximumLength(50)
                .WithMessage("O campo 'Contact.Phone2' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.Phone2));

            RuleFor(x => x.Contact!.CellPhone)
                .MaximumLength(50)
                .WithMessage("O campo 'Contact.CellPhone' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.CellPhone));

            RuleFor(x => x.Contact!.Fax)
                .MaximumLength(50)
                .WithMessage("O campo 'Contact.Fax' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.Fax));

            RuleFor(x => x.EmailConfirmation!.Status)
                .MaximumLength(50)
                .WithMessage("O campo 'EmailConfirmation.Status' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.EmailConfirmation != null && !String.IsNullOrEmpty(x.EmailConfirmation.Status));

            RuleFor(x => x.EmailConfirmation!.ConfirmationDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'EmailConfirmation.ConfirmationDate' deve ser uma data válida.")
                .When(x => x.EmailConfirmation != null && !String.IsNullOrEmpty(x.EmailConfirmation.ConfirmationDate));

            RuleForEach(x => x.Address)
                .SetValidator(new CustomerAddressValidator());

            RuleForEach(x => x.Groups)
                .SetValidator(new CustomerGroupValidator());
        }
    }
}
