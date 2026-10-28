namespace Domain.Totvs.Dtos.Writes.People.Messages
{
    /// <summary>
    /// uma mensagem do InsertMessagePerson (MessageModel)
    /// </summary>
    public class MessageModel
    {
        // até 200 caracteres
        public string? description { get; init; }

        // código da prioridade, cadastrada no PESFL151
        public int? priority { get; init; }
    }
}
