namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// campo adicional (AdditionalFieldInDto): o value vai como veio (texto até 80, número com até 12 inteiros e 6 decimais ou
    /// data DD/MM/YYYY em texto)
    /// </summary>
    public class AdditionalFieldIn
    {
        public int? code { get; init; }
        // JsonElement quando vem do controller: vai à Totvs como veio (texto, número ou data DD/MM/YYYY)
        [Newtonsoft.Json.JsonConverter(typeof(RawJsonConverter))]
        public object? value { get; init; }
    }
}
