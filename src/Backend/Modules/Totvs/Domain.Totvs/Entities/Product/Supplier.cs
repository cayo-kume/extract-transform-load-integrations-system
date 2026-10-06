using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class Supplier
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public int code { get; private set; }
        public string cpfCnpj { get; private set; }
        public string name { get; private set; }
        public string originalCode { get; private set; }
        public bool isDefault { get; private set; }

        public Supplier() { }

        public Supplier(int referenceId, string colorCode, int productCode, string productSku, Dtos.Product.Supplier supplierDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(supplierDto.code);
            cpfCnpj = supplierDto.cpfCnpj?.Length > 14 ? supplierDto.cpfCnpj.Substring(0, 14) : supplierDto.cpfCnpj;
            name = supplierDto.name?.Length > 250 ? supplierDto.name.Substring(0, 250) : supplierDto.name;
            originalCode = supplierDto.originalCode?.Length > 60 ? supplierDto.originalCode.Substring(0, 60) : supplierDto.originalCode;
            isDefault = ConverterValidatorsExtensions.ConvertToBooleanValidation(supplierDto.isDefault);
        }
    }
}
