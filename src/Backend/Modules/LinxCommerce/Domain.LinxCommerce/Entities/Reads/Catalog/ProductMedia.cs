using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Catalog
{
    /// <summary>
    /// mídia do produto, com Image e Video (1:1) em colunas, nulas quando a mídia não é daquele tipo, tabela linxcommerce.LinxCommerceProductsMedias
    /// </summary>
    public class ProductMedia
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int ProductID { get; private set; }
        public int MediaID { get; private set; }
        public string Type { get; private set; } = String.Empty;
        public string Index { get; private set; } = String.Empty;
        public bool? IsDeleted { get; private set; }
        public DateTimeOffset? CreatedDate { get; private set; }
        public int? ParentMediaID { get; private set; }
        public string OriginalFileName { get; private set; } = String.Empty;
        public string ImageRelativePath { get; private set; } = String.Empty;
        public string ImageAbsolutePath { get; private set; } = String.Empty;
        public string ImageExtension { get; private set; } = String.Empty;
        public decimal? ImageMaxWidth { get; private set; }
        public decimal? ImageMaxHeight { get; private set; }
        public decimal? ImageWidth { get; private set; }
        public decimal? ImageHeight { get; private set; }
        public int? ImageMediaSizeType { get; private set; }
        public string VideoTitle { get; private set; } = String.Empty;
        public string VideoUrl { get; private set; } = String.Empty;

        public ProductMedia() { }

        public ProductMedia(string doc_company, DateTime integrated_or_updated_at, int ProductID, Dtos.Reads.Catalog.ProductMedia dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ProductID = ProductID;

            MediaID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.MediaID);
            Type = ConverterValidatorsExtensions.StringLengthValidation(dto.Type, 50);
            Index = ConverterValidatorsExtensions.StringLengthValidation(dto.Index, 50);
            IsDeleted = NullableConverters.ToNullableBoolean(dto.IsDeleted);
            CreatedDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.CreatedDate);
            ParentMediaID = NullableConverters.ToNullableInt32(dto.ParentMediaID);
            OriginalFileName = ConverterValidatorsExtensions.StringLengthValidation(dto.OriginalFileName, 400);
            ImageRelativePath = ConverterValidatorsExtensions.StringLengthValidation(dto.Image?.RelativePath, 1000);
            ImageAbsolutePath = ConverterValidatorsExtensions.StringLengthValidation(dto.Image?.AbsolutePath, 1000);
            ImageExtension = ConverterValidatorsExtensions.StringLengthValidation(dto.Image?.Extension, 20);
            ImageMaxWidth = NullableConverters.ToNullableDecimal(dto.Image?.MaxWidth);
            ImageMaxHeight = NullableConverters.ToNullableDecimal(dto.Image?.MaxHeight);
            ImageWidth = NullableConverters.ToNullableDecimal(dto.Image?.Width);
            ImageHeight = NullableConverters.ToNullableDecimal(dto.Image?.Height);
            ImageMediaSizeType = NullableConverters.ToNullableInt32(dto.Image?.MediaSizeType);
            VideoTitle = ConverterValidatorsExtensions.StringLengthValidation(dto.Video?.Title, 400);
            VideoUrl = ConverterValidatorsExtensions.StringLengthValidation(dto.Video?.Url, 1000);
        }
    }
}
