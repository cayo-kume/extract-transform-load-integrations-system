namespace Domain.Totvs.Dtos.Writes.People.Messages
{
    /// <summary>
    /// corpo do POST person/v2/messages (InsertMessagePersonCommand): inclui mensagens para uma pessoa (PESFM152), exibidas ao atender o
    /// cliente. Recebido pela rota do controller e enviado à Totvs como está; nulos não são enviados
    /// </summary>
    public class InsertMessagePerson
    {
        // pessoa: exatamente um dos dois
        public int? personCode { get; init; }
        public string? personCpfCnpj { get; init; }

        public List<MessageModel>? messages { get; init; }
    }
}
