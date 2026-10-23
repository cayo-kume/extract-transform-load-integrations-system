namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// observação de cliente (só as 10 últimas por padrão; sequence anulável: nulo vira a posição) da pessoa física ou jurídica (API Person)
    /// </summary>
    public class CustomerObservation
    {
        public string? sequence { get; init; }
        public string? observation { get; init; }
        public string? isMaintenance { get; init; }
        public string? LastChangeDate { get; init; }
    }
}
