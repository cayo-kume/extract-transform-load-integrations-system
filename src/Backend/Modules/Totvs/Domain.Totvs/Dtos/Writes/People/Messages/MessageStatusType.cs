namespace Domain.Totvs.Dtos.Writes.People.Messages
{
    /// <summary>
    /// situação da mensagem (MessageStatusType), com os números da descrição do swagger: vai como número (D6/T3)
    /// </summary>
    public enum MessageStatusType
    {
        Unread = 1,     // não lida
        Read = 2,       // lida
        Canceled = 3    // cancelada
    }
}
