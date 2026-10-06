using Application.Totvs.Validations.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class ProductValidator : AbstractValidator<Domain.Totvs.Dtos.Product.Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.ipi)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.ipi));

            RuleFor(x => x.weight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.weight));

            RuleFor(x => x.isActive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isActive));

            RuleFor(x => x.insertDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.insertDate));

            RuleFor(x => x.isFinishedProduct)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isFinishedProduct));

            RuleFor(x => x.isRawMaterial)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isRawMaterial));

            RuleFor(x => x.isBulkMaterial)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isBulkMaterial));

            RuleFor(x => x.isOwnProduction)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isOwnProduction));

            RuleFor(x => x.minimumStockAmount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.minimumStockAmount));

            RuleFor(x => x.maximumStockAmount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.maximumStockAmount));

            RuleFor(x => x.idealStockAmount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.idealStockAmount));

            RuleFor(x => x.SalesStartDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.SalesStartDate));

            RuleFor(x => x.SalesEndDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.SalesEndDate));

            RuleFor(x => x.isBlocked)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isBlocked));

            RuleFor(x => x.sku)
                .MaximumLength(60)
                .WithMessage("O campo 'sku' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sku));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.ncm)
                .MaximumLength(60)
                .WithMessage("O campo 'ncm' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ncm));

            RuleFor(x => x.cst)
                .MaximumLength(60)
                .WithMessage("O campo 'cst' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cst));

            RuleFor(x => x.cest)
                .MaximumLength(60)
                .WithMessage("O campo 'cest' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cest));

            RuleFor(x => x.prefixEanGtin)
                .MaximumLength(60)
                .WithMessage("O campo 'prefixEanGtin' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.prefixEanGtin));

            RuleFor(x => x.measuredUnit)
                .MaximumLength(60)
                .WithMessage("O campo 'measuredUnit' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.measuredUnit));

            RuleFor(x => x.size)
                .MaximumLength(60)
                .WithMessage("O campo 'size' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.size));

            RuleFor(x => x.balanceValidationType)
                .MaximumLength(60)
                .WithMessage("O campo 'balanceValidationType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.balanceValidationType));

            RuleForEach(x => x.barCodes)
                .SetValidator(new BarcodeValidator());

            RuleForEach(x => x.classifications)
                .SetValidator(new ClassificationValidator());

            RuleForEach(x => x.additionalFields)
                .SetValidator(new AdditionalFieldValidator());

            RuleForEach(x => x.suppliers)
                .SetValidator(new SupplierValidator());

            RuleForEach(x => x.manufacturers)
                .SetValidator(new ManufacturerValidator());

            RuleForEach(x => x.referenceCodeSequences)
                .SetValidator(new ReferenceCodeSequenceValidator());

            RuleForEach(x => x.webData)
                .SetValidator(new WebDataValidator());

            RuleForEach(x => x.webDetail)
                .SetValidator(new WebDetailValidator());

            RuleForEach(x => x.branchesProductBlocked)
                .SetValidator(new BranchesProductBlockedValidator());

            RuleForEach(x => x.conservationInstructions)
                .SetValidator(new ConservationInstructionValidator());
        }
    }
}
