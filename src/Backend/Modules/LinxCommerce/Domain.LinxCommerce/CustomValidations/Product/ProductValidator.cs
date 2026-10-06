using Domain.Core.Extensions;
using Domain.LinxCommerce.CustomValidations.Validations;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Product
{
    public class ProductValidator : AbstractValidator<Domain.LinxCommerce.Entities.Product.Product>
    {
        public ProductValidator()
        {
            RuleFor(x => string.Join(", ", x.SkusID))
                .MaximumLength(3000)
                .WithMessage(x => $"Property: SkusIDs | Value: {string.Join(", ", x.SkusID)}, Tamanho do texto: {string.Join(", ", x.SkusID).Length} excede ao permitido: 3000")
                .Must((x, SkusID) =>
                {
                    if (SkusID != null && string.Join(", ", SkusID).Length > 3000)
                        x.SkusIDs = string.Join(", ", SkusID).Substring(0, 3000);
                    else
                        x.SkusIDs = string.Join(", ", SkusID);
                    return true;
                });

            RuleFor(x => string.Join(", ", x.FlagsID))
                .MaximumLength(3000)
                .WithMessage(x => $"Property: FlagsIDs | Value: {string.Join(", ", x.FlagsID)}, Tamanho do texto: {string.Join(", ", x.FlagsID).Length} excede ao permitido: 3000")
                .Must((x, FlagsID) =>
                {
                    if (FlagsID != null && string.Join(", ", FlagsID).Length > 3000)
                        x.FlagsIDs = string.Join(", ", FlagsID).Substring(0, 3000);
                    else
                        x.FlagsIDs = string.Join(", ", FlagsID);
                    return true;
                });

            RuleFor(x => string.Join(", ", x.MediasID))
                .MaximumLength(3000)
                .WithMessage(x => $"Property: MediasIDs | Value: {string.Join(", ", x.MediasID)}, Tamanho do texto: {string.Join(", ", x.MediasID).Length} excede ao permitido: 3000")
                .Must((x, MediasID) =>
                {
                    if (MediasID != null && string.Join(", ", MediasID).Length > 3000)
                        x.MediasIDs = string.Join(", ", MediasID).Substring(0, 3000);
                    else
                        x.MediasIDs = string.Join(", ", MediasID);
                    return true;
                });

            RuleFor(x => string.Join(", ", x.CategoriesID))
                .MaximumLength(3000)
                .WithMessage(x => $"Property: CategoriesIDs | Value: {string.Join(", ", x.CategoriesID)}, Tamanho do texto: {string.Join(", ", x.CategoriesID).Length} excede ao permitido: 3000")
                .Must((x, CategoriesID) =>
                {
                    if (CategoriesID != null && string.Join(", ", CategoriesID).Length > 3000)
                        x.CategoriesIDs = string.Join(", ", CategoriesID).Substring(0, 3000);
                    else
                        x.CategoriesIDs = string.Join(", ", CategoriesID);
                    return true;
                });

            RuleFor(x => string.Join(", ", x.ShippingRegionsID))
                .MaximumLength(3000)
                .WithMessage(x => $"Property: ShippingRegionsIDs | Value: {string.Join(", ", x.ShippingRegionsID)}, Tamanho do texto: {string.Join(", ", x.ShippingRegionsID).Length} excede ao permitido: 3000")
                .Must((x, ShippingRegionsID) =>
                {
                    if (ShippingRegionsID != null && string.Join(", ", ShippingRegionsID).Length > 3000)
                        x.ShippingRegionsIDs = string.Join(", ", ShippingRegionsID).Substring(0, 3000);
                    else
                        x.ShippingRegionsIDs = string.Join(", ", ShippingRegionsID);
                    return true;
                });

            RuleFor(x => x.SearchKeywords)
                .TruncateIfExceeds(300, (x, val) => x.SearchKeywords = val);

            RuleFor(x => x.PageTitle)
                .TruncateIfExceeds(60, (x, val) => x.PageTitle = val);

            RuleFor(x => x.UrlFriendly)
                .TruncateIfExceeds(160, (x, val) => x.UrlFriendly = val);

            RuleFor(x => x.MetaDescription)
                .TruncateIfExceeds(160, (x, val) => x.MetaDescription = val);

            RuleFor(x => x.MetaKeywords)
                .TruncateIfExceeds(300, (x, val) => x.MetaKeywords = val);

            RuleFor(x => x.ShortDescription)
                .TruncateIfExceeds(160, (x, val) => x.ShortDescription = val);

            RuleFor(x => x.WarrantyDescription)
                .TruncateIfExceeds(60, (x, val) => x.WarrantyDescription = val);

            RuleFor(x => string.Join(", ", x.TagsID))
                .MaximumLength(3000)
                .WithMessage(x => $"Property: TagsIDs | Value: {string.Join(", ", x.TagsID)}, Tamanho do texto: {string.Join(", ", x.TagsID).Length} excede ao permitido: 3000")
                .Must((x, TagsID) =>
                {
                    if (TagsID != null && string.Join(", ", TagsID).Length > 3000)
                        x.TagsIDs = string.Join(", ", TagsID).Substring(0, 3000);
                    else
                        x.TagsIDs = string.Join(", ", TagsID);
                    return true;
                });

            RuleFor(x => x.DisplayAvailability)
                .TruncateIfExceeds(1, (x, val) => x.DisplayAvailability = val);

            RuleFor(x => x.DisplayPrice)
                .TruncateIfExceeds(1, (x, val) => x.DisplayPrice = val);

            RuleFor(x => x.Name)
                .TruncateIfExceeds(160, (x, val) => x.Name = val);

            RuleFor(x => x.SKU)
                .TruncateIfExceeds(60, (x, val) => x.SKU = val);

            RuleFor(x => x.CreatedBy)
                .TruncateIfExceeds(60, (x, val) => x.CreatedBy = val);

            RuleFor(x => x.ModifiedBy)
                .TruncateIfExceeds(60, (x, val) => x.ModifiedBy = val);

            RuleFor(x => x.IntegrationID)
                .TruncateIfExceeds(60, (x, val) => x.IntegrationID = val);

            RuleFor(x => x.VisibleFrom)
                .NormalizeDateIfInvalid((x, val) => x.VisibleFrom = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: VisibleFrom | Value: {x.VisibleFrom}, Error when trying to convert value: {x.VisibleFrom.ToString()} to DateTime");

            RuleFor(x => x.VisibleTo)
                .NormalizeDateIfInvalid((x, val) => x.VisibleTo = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: VisibleTo | Value: {x.VisibleTo}, Error when trying to convert value: {x.VisibleTo.ToString()} to DateTime");

            RuleFor(x => x.NewFrom)
                .NormalizeDateIfInvalid((x, val) => x.NewFrom = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: NewFrom | Value: {x.NewFrom}, Error when trying to convert value: {x.NewFrom.ToString()} to DateTime");

            RuleFor(x => x.NewTo)
                .NormalizeDateIfInvalid((x, val) => x.NewTo = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: NewTo | Value: {x.NewTo}, Error when trying to convert value: {x.NewTo.ToString()} to DateTime");

            RuleFor(x => x.CreatedDate)
                .NormalizeDateIfInvalid((x, val) => x.CreatedDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: CreatedDate | Value: {x.CreatedDate}, Error when trying to convert value: {x.CreatedDate.ToString()} to DateTime");

            RuleFor(x => x.ModifiedDate)
                .NormalizeDateIfInvalid((x, val) => x.ModifiedDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: ModifiedDate | Value: {x.ModifiedDate}, Error when trying to convert value: {x.ModifiedDate.ToString()} to DateTime");

            RuleForEach(x => x.Medias)
                .SetValidator(new MidiaValidator());

            RuleForEach(x => x.MetadataValues)
                .SetValidator(new MetadataValueValidator());
        }
    }
}
