using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class ConservationInstructionsItem
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public int conservationInstructionsCode { get; private set; }
        public int order { get; private set; }
        public int item { get; private set; }

        public ConservationInstructionsItem() { }

        public ConservationInstructionsItem(int referenceId, string colorCode, int productCode, string productSku, int conservationInstructionsCode, Dtos.Product.ConservationInstructionsItem conservationInstructionsItemDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            this.conservationInstructionsCode = conservationInstructionsCode;
            order = ConverterValidatorsExtensions.ConvertToInt32Validation(conservationInstructionsItemDto.order);
            item = ConverterValidatorsExtensions.ConvertToInt32Validation(conservationInstructionsItemDto.item);
        }
    }
}
