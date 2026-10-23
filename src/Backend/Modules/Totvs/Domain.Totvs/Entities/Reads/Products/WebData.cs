using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// dados de e-commerce do produto (index é a chave), tabela totvs.TotvsProductsWebData
    /// </summary>
    public class WebData
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int productCode { get; private set; }
        public int index { get; private set; }
        public string? description { get; private set; }   // nvarchar(max)
        public string? descriptive { get; private set; }   // nvarchar(max)
        public decimal? weight { get; private set; }
        public decimal? height { get; private set; }
        public decimal? width { get; private set; }
        public decimal? length { get; private set; }
        public decimal? packWeight { get; private set; }
        public decimal? packHeight { get; private set; }
        public decimal? packWidth { get; private set; }
        public decimal? packLength { get; private set; }
        public decimal? productPackWeight { get; private set; }
        public decimal? packQuantity { get; private set; }
        public string? link { get; private set; }   // nvarchar(max)
        public string? url { get; private set; }   // nvarchar(max)
        public string status { get; private set; } = String.Empty;
        public bool? isUpdate { get; private set; }
        public string definition1 { get; private set; } = String.Empty;
        public string definition2 { get; private set; } = String.Empty;
        public int? bonusQuantity { get; private set; }

        public WebData() { }

        public WebData(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int productCode, Dtos.Reads.Products.WebData dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.productCode = productCode;

            index = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.index);
            description = dto.description;
            descriptive = dto.descriptive;
            weight = NullableConverters.ToNullableDecimal(dto.weight);
            height = NullableConverters.ToNullableDecimal(dto.height);
            width = NullableConverters.ToNullableDecimal(dto.width);
            length = NullableConverters.ToNullableDecimal(dto.length);
            packWeight = NullableConverters.ToNullableDecimal(dto.packWeight);
            packHeight = NullableConverters.ToNullableDecimal(dto.packHeight);
            packWidth = NullableConverters.ToNullableDecimal(dto.packWidth);
            packLength = NullableConverters.ToNullableDecimal(dto.packLength);
            productPackWeight = NullableConverters.ToNullableDecimal(dto.productPackWeight);
            packQuantity = NullableConverters.ToNullableDecimal(dto.packQuantity);
            link = dto.link;
            url = dto.url;
            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 20);
            isUpdate = NullableConverters.ToNullableBoolean(dto.isUpdate);
            definition1 = ConverterValidatorsExtensions.StringLengthValidation(dto.definition1, 250);
            definition2 = ConverterValidatorsExtensions.StringLengthValidation(dto.definition2, 250);
            bonusQuantity = NullableConverters.ToNullableInt32(dto.bonusQuantity);
        }
    }
}
