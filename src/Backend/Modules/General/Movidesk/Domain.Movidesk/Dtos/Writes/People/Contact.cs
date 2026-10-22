namespace Domain.Movidesk.Dtos.Writes.People
{
    /// <summary>
    /// contato da pessoa (telefone, celular etc.)
    /// </summary>
    public class Contact
    {
        public string? contactType { get; init; }

        public string? contact { get; init; }

        // só um contato principal
        public bool? isDefault { get; init; }
    }
}
