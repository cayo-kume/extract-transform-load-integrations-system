using Application.LinxCommerce.Validations.Reads.Base;
using Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Reads.SalesRepresentatives
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceSalesRepresentatives (scripts de tabelas do módulo)
    /// </summary>
    public class SalesRepresentativeValidator : AbstractValidator<SalesRepresentative>
    {
        public SalesRepresentativeValidator()
        {
            RuleFor(x => x.SalesRepresentativeID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'SalesRepresentativeID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'SalesRepresentativeID' deve ser um número inteiro.");

            RuleFor(x => x.Name)
                .MaximumLength(250)
                .WithMessage("O campo 'Name' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Name));

            RuleFor(x => x.FriendlyCode)
                .MaximumLength(100)
                .WithMessage("O campo 'FriendlyCode' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.FriendlyCode));

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1000)
                .WithMessage("O campo 'ImageUrl' deve ter no máximo 1000 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.IntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'IntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.IntegrationID));

            RuleFor(x => x.AllowQuoteDeletion)
                .MustBeValidBoolean()
                .WithMessage("O campo 'AllowQuoteDeletion' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.AllowQuoteDeletion));

            RuleFor(x => x.BusinessContractID)
                .MustBeValidInt32()
                .WithMessage("O campo 'BusinessContractID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.BusinessContractID));

            RuleFor(x => x.SalesRepresentativeType)
                .MaximumLength(50)
                .WithMessage("O campo 'SalesRepresentativeType' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.SalesRepresentativeType));

            RuleFor(x => x.Status)
                .MaximumLength(50)
                .WithMessage("O campo 'Status' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Status));

            RuleFor(x => x.Identification!.Type)
                .MaximumLength(50)
                .WithMessage("O campo 'Identification.Type' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.Identification != null && !String.IsNullOrEmpty(x.Identification.Type));

            RuleFor(x => x.Identification!.DocumentNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'Identification.DocumentNumber' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => x.Identification != null && !String.IsNullOrEmpty(x.Identification.DocumentNumber));

            RuleFor(x => x.Commission!.TotalCommission)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Commission.TotalCommission' deve ser um número decimal.")
                .When(x => x.Commission != null && !String.IsNullOrEmpty(x.Commission.TotalCommission));

            RuleFor(x => x.Commission!.DeliveryCommission)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Commission.DeliveryCommission' deve ser um número decimal.")
                .When(x => x.Commission != null && !String.IsNullOrEmpty(x.Commission.DeliveryCommission));

            RuleFor(x => x.Contact!.Email)
                .MaximumLength(250)
                .WithMessage("O campo 'Contact.Email' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.Email));

            RuleFor(x => x.Contact!.Phone)
                .MaximumLength(50)
                .WithMessage("O campo 'Contact.Phone' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.Phone));

            RuleFor(x => x.Contact!.CellPhone)
                .MaximumLength(50)
                .WithMessage("O campo 'Contact.CellPhone' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.CellPhone));

            RuleFor(x => x.ShippingRegion!.SelectedMode)
                .MaximumLength(50)
                .WithMessage("O campo 'ShippingRegion.SelectedMode' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.ShippingRegion != null && !String.IsNullOrEmpty(x.ShippingRegion.SelectedMode));

            RuleFor(x => x.ShippingRegion!.ShippingRegionID)
                .MustBeValidInt32()
                .WithMessage("O campo 'ShippingRegion.ShippingRegionID' deve ser um número inteiro.")
                .When(x => x.ShippingRegion != null && !String.IsNullOrEmpty(x.ShippingRegion.ShippingRegionID));

            RuleFor(x => x.MaxDiscount!.Type)
                .MaximumLength(50)
                .WithMessage("O campo 'MaxDiscount.Type' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.MaxDiscount != null && !String.IsNullOrEmpty(x.MaxDiscount.Type));

            RuleFor(x => x.MaxDiscount!.Amount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'MaxDiscount.Amount' deve ser um número decimal.")
                .When(x => x.MaxDiscount != null && !String.IsNullOrEmpty(x.MaxDiscount.Amount));

            RuleFor(x => x.WebSiteSettings!.WebSiteFilter)
                .MaximumLength(50)
                .WithMessage("O campo 'WebSiteSettings.WebSiteFilter' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.WebSiteSettings != null && !String.IsNullOrEmpty(x.WebSiteSettings.WebSiteFilter));

            RuleFor(x => x.Portfolio!.HasPortfolio)
                .MustBeValidBoolean()
                .WithMessage("O campo 'Portfolio.HasPortfolio' deve ser true ou false.")
                .When(x => x.Portfolio != null && !String.IsNullOrEmpty(x.Portfolio.HasPortfolio));

            RuleFor(x => x.Portfolio!.PortfolioAssociationType)
                .MaximumLength(50)
                .WithMessage("O campo 'Portfolio.PortfolioAssociationType' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.Portfolio != null && !String.IsNullOrEmpty(x.Portfolio.PortfolioAssociationType));

            RuleForEach(x => x.Addresses)
                .SetValidator(new SalesRepresentativeAddressValidator());

            RuleForEach(x => x.Portfolio!.Customers)
                .SetValidator(new SalesRepresentativeCustomerValidator())
                .When(x => x.Portfolio != null);
        }
    }
}
