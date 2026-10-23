namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// observação do representante (sem chave na API: a posição é a chave)
    /// </summary>
    public class RepresentativeObservation
    {
        public string? observation { get; init; }
        public string? visualizationType { get; init; }
    }
}
