namespace Domain.Totvs.Dtos.Product
{
    public class Color
    {
        public string code { get; init; }
        public string name { get; init; }
        public string groupName { get; init; }
        public string auxiliaryCode { get; init; }
        public string auxiliaryName { get; init; }
        public string pantoneCode { get; init; }
        public string configurableField { get; init; }
        public List<Product>? products { get; init; }
        public List<MeasurementTableOmni>? measurementTableOmni { get; init; }
    }
}
