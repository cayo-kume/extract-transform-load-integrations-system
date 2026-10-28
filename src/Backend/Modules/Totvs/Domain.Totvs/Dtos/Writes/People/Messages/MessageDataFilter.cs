namespace Domain.Totvs.Dtos.Writes.People.Messages
{
    /// <summary>
    /// mensagem a alterar (MessageDataFilter): a pessoa (exatamente um dos dois) e a sequence devolvida pelo POST
    /// </summary>
    public class MessageDataFilter
    {
        public int? personCode { get; init; }
        public string? personCpfCnpj { get; init; }

        public int? sequence { get; init; }
    }
}
