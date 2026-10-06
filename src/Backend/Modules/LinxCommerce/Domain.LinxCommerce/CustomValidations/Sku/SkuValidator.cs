using Domain.Core.Extensions;
using Domain.LinxCommerce.CustomValidations.Validations;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Sku
{
    public class SkuValidator : AbstractValidator<Domain.LinxCommerce.Entities.Sku.Sku>
    {
        public SkuValidator()
        {
            RuleFor(x => x.UPC)
                .TruncateIfExceeds(20, (x, val) => x.UPC = val);

            RuleFor(x => string.Join(", ", x.SuppliersID))
                .MaximumLength(50)
                .WithMessage(x => $"Property: SuppliersIDs | Value: {string.Join(", ", x.SuppliersID)}, Tamanho do texto: {string.Join(", ", x.SuppliersID).Length} excede ao permitido: 50")
                .Must((x, SuppliersID) =>
                {
                    if (SuppliersID != null && string.Join(", ", SuppliersID).Length > 50)
                        x.SuppliersIDs = string.Join(", ", SuppliersID).Substring(0, 50);
                    else
                        x.SuppliersIDs = string.Join(", ", SuppliersID);
                    return true;
                });

            RuleFor(x => string.Join(", ", x.ParentsID))
                .MaximumLength(50)
                .WithMessage(x => $"Property: ParentsIDs | Value: {string.Join(", ", x.ParentsID)}, Tamanho do texto: {string.Join(", ", x.ParentsID).Length} excede ao permitido: 50")
                .Must((x, ParentsID) =>
                {
                    if (ParentsID != null && string.Join(", ", ParentsID).Length > 50)
                        x.ParentsIDs = string.Join(", ", ParentsID).Substring(0, 50);
                    else
                        x.ParentsIDs = string.Join(", ", ParentsID);
                    return true;
                });

            RuleFor(x => x.Name)
                .TruncateIfExceeds(160, (x, val) => x.Name = val);

            RuleFor(x => x.SKU)
                .TruncateIfExceeds(20, (x, val) => x.SKU = val);

            RuleFor(x => x.CreatedBy)
                .TruncateIfExceeds(50, (x, val) => x.CreatedBy = val);

            RuleFor(x => x.ModifiedBy)
                .TruncateIfExceeds(50, (x, val) => x.ModifiedBy = val);

            RuleFor(x => x.IntegrationID)
                .TruncateIfExceeds(20, (x, val) => x.IntegrationID = val);

            RuleFor(x => x.PreorderDate)
                .NormalizeDateIfInvalid((x, val) => x.PreorderDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: PreorderDate | Value: {x.PreorderDate}, Error when trying to convert value: {x.PreorderDate.ToString()} to DateTime");

            RuleFor(x => x.VisibleFrom)
                .NormalizeDateIfInvalid((x, val) => x.VisibleFrom = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: VisibleFrom | Value: {x.VisibleFrom}, Error when trying to convert value: {x.VisibleFrom.ToString()} to DateTime");

            RuleFor(x => x.VisibleTo)
                .NormalizeDateIfInvalid((x, val) => x.VisibleTo = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: VisibleTo | Value: {x.VisibleTo}, Error when trying to convert value: {x.VisibleTo.ToString()} to DateTime");

            RuleFor(x => x.CreatedDate)
                .NormalizeDateIfInvalid((x, val) => x.CreatedDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: CreatedDate | Value: {x.CreatedDate}, Error when trying to convert value: {x.CreatedDate.ToString()} to DateTime");

            RuleFor(x => x.ModifiedDate)
                .NormalizeDateIfInvalid((x, val) => x.ModifiedDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: ModifiedDate | Value: {x.ModifiedDate}, Error when trying to convert value: {x.ModifiedDate.ToString()} to DateTime");

            RuleForEach(x => x.MetadataValues)
                .SetValidator(new MetadataValueValidator());

            RuleForEach(x => x.ParentRelations)
                .SetValidator(new ParentRelationValidator());
        }
    }
}
