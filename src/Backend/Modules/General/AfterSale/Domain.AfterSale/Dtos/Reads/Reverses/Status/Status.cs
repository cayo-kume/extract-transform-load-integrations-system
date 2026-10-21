using Newtonsoft.Json;

namespace Domain.AfterSale.Dtos.Reads.Reverses.Status
{
    public class Status
    {
        public string id { get; init; }
        public string name { get; init; }
        public string description { get; init; }

        [JsonIgnore] // não vem da API; preenchido no service após a desserialização
        public string? doc_company { get; set; }
    }
}
