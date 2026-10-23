namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// observação (sequence anulável na API: nulo vira a posição) da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Observation
    {
        public string? sequence { get; init; }
        public string? observation { get; init; }
        public string? isMaintenance { get; init; }
    }
}
