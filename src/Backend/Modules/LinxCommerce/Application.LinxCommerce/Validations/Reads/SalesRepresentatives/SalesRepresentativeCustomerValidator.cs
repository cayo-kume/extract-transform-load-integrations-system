using Application.LinxCommerce.Validations.Reads.Base;
using Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Reads.SalesRepresentatives
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceSalesRepresentativesCustomers (scripts de tabelas do módulo)
    /// </summary>
    public class SalesRepresentativeCustomerValidator : AbstractValidator<SalesRepresentativeCustomer>
    {
        public SalesRepresentativeCustomerValidator()
        {
            RuleFor(x => x.CustomerID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'CustomerID' é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O campo 'CustomerID' deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Status)
                .MaximumLength(50)
                .WithMessage("O campo 'Status' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Status));

            RuleFor(x => x.IsMaxDiscountEnabled)
                .MaximumLength(10)
                .WithMessage("O campo 'IsMaxDiscountEnabled' deve ter no máximo 10 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.IsMaxDiscountEnabled));
        }
    }
}
