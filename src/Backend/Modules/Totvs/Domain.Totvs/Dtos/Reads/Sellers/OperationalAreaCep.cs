namespace Domain.Totvs.Dtos.Reads.Sellers
{
    /// <summary>
    /// CEP de uma região de atuação do GET seller/v2/operational-area-cep (OperationalAreaCepModel)
    /// </summary>
    public class OperationalAreaCep
    {
        public string? operationalAreaCode { get; init; }
        public string? operationalAreaName { get; init; }
        public string? cep { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
