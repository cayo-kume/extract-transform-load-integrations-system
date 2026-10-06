using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Product
{
    public class MeasurementTableOmni
    {
        public int referenceId { get; private set; }
        public string colorCode { get; private set; }
        public string sku { get; private set; }
        public string tamanho { get; private set; }

        [SkipProperty]
        public List<Medida> medidas { get; private set; } = [];

        public MeasurementTableOmni() { }

        public MeasurementTableOmni(int referenceId, string colorCode, Dtos.Product.MeasurementTableOmni measurementTableOmniDto)
        {
            this.referenceId = referenceId;
            this.colorCode = colorCode;
            sku = measurementTableOmniDto.sku?.Length > 60 ? measurementTableOmniDto.sku.Substring(0, 60) : measurementTableOmniDto.sku;
            tamanho = measurementTableOmniDto.tamanho?.Length > 60 ? measurementTableOmniDto.tamanho.Substring(0, 60) : measurementTableOmniDto.tamanho;

            measurementTableOmniDto.medidas?.ForEach(medida =>
                medidas.Add(new Medida(referenceId, colorCode, sku, medida)
            ));
        }
    }
}
