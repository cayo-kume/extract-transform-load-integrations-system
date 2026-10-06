using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class Composition
    {
        public int referenceId { get; private set; }
        public string material { get; private set; }
        public decimal percentage { get; private set; }

        public Composition() { }

        public Composition(int referenceId, Dtos.Product.Composition compositionDto)
        {
            this.referenceId = referenceId;
            material = compositionDto.material?.Length > 60 ? compositionDto.material.Substring(0, 60) : compositionDto.material;
            percentage = ConverterValidatorsExtensions.ConvertToDecimalValidation(compositionDto.percentage);
        }
    }
}
