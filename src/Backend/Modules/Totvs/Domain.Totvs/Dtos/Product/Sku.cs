namespace Domain.Totvs.Dtos.Product
{
    public class Sku
    {
        public string ReferenceCode { get; init; }
        public string referenceId { get; init; }
        public string lastReferenceCode { get; init; }
        public string insertDate { get; init; }
        public string maxChangeFilterDate { get; init; }
        public string name { get; init; }
        public string description { get; init; }
        public string descriptive { get; init; }
        public string gridCode { get; init; }
        public string weight { get; init; }
        public string height { get; init; }
        public string width { get; init; }
        public string length { get; init; }
        public string packWeight { get; init; }
        public string packHeight { get; init; }
        public string packWidth { get; init; }
        public string packLength { get; init; }
        public List<string> grid { get; init; }
        public List<Observation>? observations { get; init; }
        public List<Detail>? details { get; init; }
        public List<Composition>? composition { get; init; }
        public List<ReferenceCategory>? referenceCategories { get; init; }
        public List<Color>? colors { get; init; }
    }
}
