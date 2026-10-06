using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class ConservationInstruction
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public int productCode { get; private set; }
        public string productSku { get; private set; }
        public int code { get; private set; }
        public string description { get; private set; }
        public bool _default { get; private set; }
        public int grouperCode { get; private set; }

        [SkipProperty]
        public List<ConservationInstructionsItem> items { get; private set; } = [];

        public ConservationInstruction() { }

        public ConservationInstruction(int referenceId, string colorCode, int productCode, string productSku, Dtos.Product.ConservationInstruction conservationInstructionDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            this.productCode = productCode;
            this.productSku = productSku;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(conservationInstructionDto.code);
            description = conservationInstructionDto.description?.Length > 300 ? conservationInstructionDto.description.Substring(0, 300) : conservationInstructionDto.description;
            _default = ConverterValidatorsExtensions.ConvertToBooleanValidation(conservationInstructionDto._default);
            grouperCode = ConverterValidatorsExtensions.ConvertToInt32Validation(conservationInstructionDto.grouperCode);

            conservationInstructionDto.items?.ForEach(item =>
                items.Add(new ConservationInstructionsItem(referenceId, colorCode, productCode, productSku, code, item)
            ));
        }
    }
}
