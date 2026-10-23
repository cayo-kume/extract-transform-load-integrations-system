namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// familiar (sem chave na API: a posição é a chave) da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Familiar
    {
        public string? name { get; init; }
        public string? birthDate { get; init; }
        public string? gender { get; init; }
        public string? kinshipDescription { get; init; }
    }
}
