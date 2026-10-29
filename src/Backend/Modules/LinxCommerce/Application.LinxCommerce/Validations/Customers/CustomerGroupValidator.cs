using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Customers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Customers
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceCustomersGroups (scripts de tabelas do módulo)
    /// </summary>
    public class CustomerGroupValidator : AbstractValidator<CustomerGroup>
    {
        public CustomerGroupValidator()
        {
            RuleFor(x => x.CustomerGroupID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'CustomerGroupID' é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O campo 'CustomerGroupID' deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Title)
                .MaximumLength(250)
                .WithMessage("O campo 'Title' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Title));
        }
    }
}
