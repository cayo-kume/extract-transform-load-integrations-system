using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class Manufacturer
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public int code { get; private set; }
        public string cpfCnpj { get; private set; }
        public string name { get; private set; }

        public Manufacturer() { }

        public Manufacturer(int referenceId, string colorCode, int productCode, string productSku, Dtos.Product.Manufacturer manufacturerDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(manufacturerDto.code);
            cpfCnpj = manufacturerDto.cpfCnpj?.Length > 14 ? manufacturerDto.cpfCnpj.Substring(0, 14) : manufacturerDto.cpfCnpj;
            name = manufacturerDto.name?.Length > 250 ? manufacturerDto.name.Substring(0, 250) : manufacturerDto.name;
        }
    }
}
