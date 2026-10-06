namespace Domain.Totvs.Dtos.Product
{
    public class MeasurementTableOmni
    {
        public string sku { get; init; }
        public string tamanho { get; init; }
        public List<Medida> medidas { get; init; }
    }
}
