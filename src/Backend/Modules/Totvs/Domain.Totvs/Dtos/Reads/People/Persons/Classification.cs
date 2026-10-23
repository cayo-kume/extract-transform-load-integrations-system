namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// classificação (code nulo vira vazio, faz parte da chave) da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Classification
    {
        public string? typeCode { get; init; }
        public string? code { get; init; }
        public string? typeName { get; init; }
        public string? name { get; init; }
    }
}
