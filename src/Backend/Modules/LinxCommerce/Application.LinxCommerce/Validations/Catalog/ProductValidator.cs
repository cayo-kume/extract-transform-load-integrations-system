using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Catalog;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Catalog
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceProducts (scripts de tabelas do módulo)
    /// </summary>
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.ProductID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'ProductID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'ProductID' deve ser um número inteiro.");

            RuleFor(x => x.Name)
                .MaximumLength(400)
                .WithMessage("O campo 'Name' deve ter no máximo 400 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Name));

            RuleFor(x => x.SKU)
                .MaximumLength(100)
                .WithMessage("O campo 'SKU' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.SKU));

            RuleFor(x => x.IntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'IntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.IntegrationID));

            RuleFor(x => x.BrandID)
                .MustBeValidInt32()
                .WithMessage("O campo 'BrandID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.BrandID));

            RuleFor(x => x.RatingSetID)
                .MustBeValidInt32()
                .WithMessage("O campo 'RatingSetID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.RatingSetID));

            RuleFor(x => x.CatalogItemType)
                .MustBeValidInt32()
                .WithMessage("O campo 'CatalogItemType' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.CatalogItemType));

            RuleFor(x => x.PurchasingPolicyID)
                .MustBeValidInt32()
                .WithMessage("O campo 'PurchasingPolicyID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.PurchasingPolicyID));

            RuleFor(x => x.DefinitionID)
                .MustBeValidInt32()
                .WithMessage("O campo 'DefinitionID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.DefinitionID));

            RuleFor(x => x.AcceptanceTermID)
                .MustBeValidInt32()
                .WithMessage("O campo 'AcceptanceTermID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.AcceptanceTermID));

            RuleFor(x => x.IsFreeShipping)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsFreeShipping' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsFreeShipping));

            RuleFor(x => x.IsVisible)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsVisible' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsVisible));

            RuleFor(x => x.VisibleFrom)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'VisibleFrom' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.VisibleFrom));

            RuleFor(x => x.VisibleTo)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'VisibleTo' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.VisibleTo));

            RuleFor(x => x.IsSearchable)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsSearchable' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsSearchable));

            RuleFor(x => x.IsUponRequest)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsUponRequest' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsUponRequest));

            RuleFor(x => x.DisplayAvailability)
                .MaximumLength(50)
                .WithMessage("O campo 'DisplayAvailability' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.DisplayAvailability));

            RuleFor(x => x.DisplayStockQty)
                .MustBeValidBoolean()
                .WithMessage("O campo 'DisplayStockQty' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.DisplayStockQty));

            RuleFor(x => x.DisplayPrice)
                .MaximumLength(50)
                .WithMessage("O campo 'DisplayPrice' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.DisplayPrice));

            RuleFor(x => x.IsNew)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsNew' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsNew));

            RuleFor(x => x.NewFrom)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'NewFrom' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.NewFrom));

            RuleFor(x => x.NewTo)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'NewTo' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.NewTo));

            RuleFor(x => x.IsGift)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsGift' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsGift));

            RuleFor(x => x.UseAcceptanceTerm)
                .MustBeValidBoolean()
                .WithMessage("O campo 'UseAcceptanceTerm' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.UseAcceptanceTerm));

            RuleFor(x => x.IsDeleted)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsDeleted' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsDeleted));

            RuleFor(x => x.SendToMarketplace)
                .MaximumLength(50)
                .WithMessage("O campo 'SendToMarketplace' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.SendToMarketplace));

            RuleFor(x => x.PageTitle)
                .MaximumLength(400)
                .WithMessage("O campo 'PageTitle' deve ter no máximo 400 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.PageTitle));

            RuleFor(x => x.UrlFriendly)
                .MaximumLength(400)
                .WithMessage("O campo 'UrlFriendly' deve ter no máximo 400 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.UrlFriendly));

            RuleFor(x => x.CreatedDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'CreatedDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.CreatedDate));

            RuleFor(x => x.CreatedBy)
                .MaximumLength(100)
                .WithMessage("O campo 'CreatedBy' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CreatedBy));

            RuleFor(x => x.ModifiedDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'ModifiedDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.ModifiedDate));

            RuleFor(x => x.ModifiedBy)
                .MaximumLength(100)
                .WithMessage("O campo 'ModifiedBy' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ModifiedBy));

            RuleForEach(x => x.MetadataValues)
                .SetValidator(new ProductMetadataValueValidator());

            RuleForEach(x => x.Medias)
                .SetValidator(new ProductMediaValidator());

            RuleForEach(x => x.MediaAssociations)
                .SetValidator(new ProductMediaAssociationValidator());
        }
    }
}
