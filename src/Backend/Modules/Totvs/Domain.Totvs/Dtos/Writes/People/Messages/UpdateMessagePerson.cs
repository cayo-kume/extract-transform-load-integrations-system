namespace Domain.Totvs.Dtos.Writes.People.Messages
{
    /// <summary>
    /// corpo do PUT person/v2/messages/change (UpdateMessagePersonCommand): altera uma mensagem da pessoa. Cancelar (status 3) é o caminho
    /// normal para tirar uma mensagem (PE8); nulos não são enviados (o campo fica como está)
    /// </summary>
    public class UpdateMessagePerson
    {
        public MessageDataFilter? filter { get; init; }

        public string? description { get; init; }
        public MessageStatusType? status { get; init; }
        public int? priority { get; init; }
    }
}
