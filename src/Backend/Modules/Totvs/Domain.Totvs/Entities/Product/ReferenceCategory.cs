using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class ReferenceCategory
    {
        public int referenceId { get; private set; }
        public int code { get; private set; }
        public string name { get; private set; }
        public string type { get; private set; }

        public ReferenceCategory() { }

        public ReferenceCategory(int referenceId, Dtos.Product.ReferenceCategory referencecategoryDto)
        {
            this.referenceId = referenceId;
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(referencecategoryDto.code);
            name = referencecategoryDto.name?.Length > 250 ? referencecategoryDto.name.Substring(0, 250) : referencecategoryDto.name;
            type = referencecategoryDto.type?.Length > 60 ? referencecategoryDto.type.Substring(0, 60) : referencecategoryDto.type;
        }
    }
}
