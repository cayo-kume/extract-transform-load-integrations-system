using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class WebData
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public int sequence { get; private set; }
        public string description { get; private set; }
        public string descriptive { get; private set; }
        public decimal weight { get; private set; }
        public decimal height { get; private set; }
        public decimal width { get; private set; }
        public decimal length { get; private set; }
        public decimal packWeight { get; private set; }
        public decimal packHeight { get; private set; }
        public decimal packWidth { get; private set; }
        public decimal packLength { get; private set; }
        public decimal productPackWeight { get; private set; }
        public decimal packQuantity { get; private set; }
        public string link { get; private set; }
        public string url { get; private set; }
        public string status { get; private set; }
        public bool isUpdate { get; private set; }
        public string definition1 { get; private set; }
        public string definition2 { get; private set; }
        public int index { get; private set; }
        public decimal bonusQuantity { get; private set; }

        public WebData() { }

        public WebData(int sequence, int referenceId, string colorCode, int productCode, string productSku, Dtos.Product.WebData webdataDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            this.sequence = sequence;
            description = webdataDto.description?.Length > 300 ? webdataDto.description.Substring(0, 300) : webdataDto.description;
            descriptive = webdataDto.descriptive?.Length > 300 ? webdataDto.descriptive.Substring(0, 300) : webdataDto.descriptive;
            weight = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.weight);
            height = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.height);
            width = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.width);
            length = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.length);
            packWeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.packWeight);
            packHeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.packHeight);
            packWidth = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.packWidth);
            packLength = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.packLength);
            productPackWeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.productPackWeight);
            packQuantity = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.packQuantity);
            link = webdataDto.link?.Length > 8000 ? webdataDto.link.Substring(0, 8000) : webdataDto.link;
            url = webdataDto.url?.Length > 8000 ? webdataDto.url.Substring(0, 8000) : webdataDto.url;
            status = webdataDto.status?.Length > 60 ? webdataDto.status.Substring(0, 60) : webdataDto.status;
            isUpdate = ConverterValidatorsExtensions.ConvertToBooleanValidation(webdataDto.isUpdate);
            definition1 = webdataDto.definition1?.Length > 300 ? webdataDto.definition1.Substring(0, 300) : webdataDto.definition1;
            definition2 = webdataDto.definition2?.Length > 300 ? webdataDto.definition2.Substring(0, 300) : webdataDto.definition2;
            index = ConverterValidatorsExtensions.ConvertToInt32Validation(webdataDto.index);
            bonusQuantity = ConverterValidatorsExtensions.ConvertToDecimalValidation(webdataDto.bonusQuantity);
        }
    }
}
