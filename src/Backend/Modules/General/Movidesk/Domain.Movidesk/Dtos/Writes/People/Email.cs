namespace Domain.Movidesk.Dtos.Writes.People
{
    /// <summary>
    /// e-mail da pessoa
    /// </summary>
    public class Email
    {
        // ex.: Pessoal, Profissional
        public string? emailType { get; init; }

        public string? email { get; init; }

        // só um e-mail principal
        public bool? isDefault { get; init; }
    }
}
