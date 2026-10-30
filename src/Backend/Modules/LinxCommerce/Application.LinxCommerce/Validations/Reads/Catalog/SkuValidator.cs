using Application.LinxCommerce.Validations.Reads.Base;
using Domain.LinxCommerce.Dtos.Reads.Catalog;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Reads.Catalog
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceSkus (scripts de tabelas do módulo)
    /// </summary>
    public class SkuValidator : AbstractValidator<Sku>
    {
        public SkuValidator()
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

            RuleFor(x => x.UPC)
                .MaximumLength(50)
                .WithMessage("O campo 'UPC' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.UPC));

            RuleFor(x => x.DisplayCondition)
                .MustBeValidBoolean()
                .WithMessage("O campo 'DisplayCondition' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.DisplayCondition));

            RuleFor(x => x.DefinitionID)
                .MustBeValidInt32()
                .WithMessage("O campo 'DefinitionID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.DefinitionID));

            RuleFor(x => x.ConditionID)
                .MustBeValidInt32()
                .WithMessage("O campo 'ConditionID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.ConditionID));

            RuleFor(x => x.UnitOfMeasureID)
                .MustBeValidInt32()
                .WithMessage("O campo 'UnitOfMeasureID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.UnitOfMeasureID));

            RuleFor(x => x.PurchasingPolicyID)
                .MustBeValidInt32()
                .WithMessage("O campo 'PurchasingPolicyID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.PurchasingPolicyID));

            RuleFor(x => x.ManageStock)
                .MustBeValidBoolean()
                .WithMessage("O campo 'ManageStock' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.ManageStock));

            RuleFor(x => x.MinimumQtyAllowed)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'MinimumQtyAllowed' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.MinimumQtyAllowed));

            RuleFor(x => x.MaximumQtyAllowed)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'MaximumQtyAllowed' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.MaximumQtyAllowed));

            RuleFor(x => x.Preorderable)
                .MustBeValidBoolean()
                .WithMessage("O campo 'Preorderable' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.Preorderable));

            RuleFor(x => x.PreorderDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'PreorderDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.PreorderDate));

            RuleFor(x => x.PreorderLimit)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'PreorderLimit' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.PreorderLimit));

            RuleFor(x => x.Backorderable)
                .MustBeValidBoolean()
                .WithMessage("O campo 'Backorderable' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.Backorderable));

            RuleFor(x => x.BackorderLimit)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'BackorderLimit' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.BackorderLimit));

            RuleFor(x => x.WrappingQty)
                .MaximumLength(50)
                .WithMessage("O campo 'WrappingQty' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.WrappingQty));

            RuleFor(x => x.Weight)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Weight' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Weight));

            RuleFor(x => x.Height)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Height' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Height));

            RuleFor(x => x.Width)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Width' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Width));

            RuleFor(x => x.Depth)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Depth' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Depth));

            RuleFor(x => x.Cost)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Cost' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Cost));

            RuleFor(x => x.Tax)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Tax' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Tax));

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

            RuleFor(x => x.IsDeleted)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsDeleted' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsDeleted));

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
                .SetValidator(new SkuMetadataValueValidator());

            RuleForEach(x => x.ParentRelations)
                .SetValidator(new SkuParentRelationValidator());
        }
    }
}
