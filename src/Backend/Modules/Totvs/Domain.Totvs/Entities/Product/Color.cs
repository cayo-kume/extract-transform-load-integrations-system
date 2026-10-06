using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class Color
    {
        public int referenceId { get; private set; }
        public string code { get; private set; }
        public string name { get; private set; }
        public string groupName { get; private set; }
        public string auxiliaryCode { get; private set; }
        public string auxiliaryName { get; private set; }
        public string pantoneCode { get; private set; }
        public string configurableField { get; private set; }

        [SkipProperty]
        public List<Product> products { get; private set; } = [];

        [SkipProperty]
        public List<MeasurementTableOmni> measurementTableOmni { get; private set; } = [];

        public Color() { }

        public Color(int referenceId, Dtos.Product.Color colorDto)
        {
            this.referenceId = referenceId;
            code = colorDto.code?.Length > 60 ? colorDto.code.Substring(0, 60) : colorDto.code;
            name = colorDto.name?.Length > 60 ? colorDto.name.Substring(0, 60) : colorDto.name;
            groupName = colorDto.groupName?.Length > 60 ? colorDto.groupName.Substring(0, 60) : colorDto.groupName;
            auxiliaryCode = colorDto.auxiliaryCode?.Length > 60 ? colorDto.auxiliaryCode.Substring(0, 60) : colorDto.auxiliaryCode;
            auxiliaryName = colorDto.auxiliaryName?.Length > 60 ? colorDto.auxiliaryName.Substring(0, 60) : colorDto.auxiliaryName;
            pantoneCode = colorDto.pantoneCode?.Length > 60 ? colorDto.pantoneCode.Substring(0, 60) : colorDto.pantoneCode;
            configurableField = colorDto.configurableField?.Length > 60 ? colorDto.configurableField.Substring(0, 60) : colorDto.configurableField;

            colorDto.products?.ForEach(product =>
                products.Add(new Product(referenceId, code, product)
            ));

            colorDto.measurementTableOmni?.ForEach(measurement =>
                measurementTableOmni.Add(new MeasurementTableOmni(referenceId, code, measurement)
            ));
        }
    }
}
