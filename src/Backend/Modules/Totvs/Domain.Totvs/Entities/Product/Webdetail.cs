using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class WebDetail
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public int descriptionType { get; private set; }
        public string type { get; private set; }
        public string title { get; private set; }
        public string description { get; private set; }
        
        public WebDetail() { }

        public WebDetail(int referenceId, string colorCode, int productCode, string productSku, Dtos.Product.WebDetail webdetailDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            descriptionType = ConverterValidatorsExtensions.ConvertToInt32Validation(webdetailDto.descriptionType);
            type = webdetailDto.type?.Length > 60 ? webdetailDto.type.Substring(0, 60) : webdetailDto.type;
            title = webdetailDto.title?.Length > 60 ? webdetailDto.title.Substring(0, 60) : webdetailDto.title;
            description = webdetailDto.description?.Length > 300 ? webdetailDto.description.Substring(0, 300) : webdetailDto.description;
        }
    }
}
