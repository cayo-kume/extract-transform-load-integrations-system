namespace Domain.Movidesk.Dtos.Writes.Tickets
{
    /// <summary>
    /// cliente do ticket: pessoa, empresa ou departamento pelo Cód. Ref.
    /// </summary>
    public class Client
    {
        public string? id { get; init; }

        // organização do cliente; o id é o único campo que pode ser escrito
        public OrganizationReference? organization { get; init; }
    }
}
