using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Dtos.Reads.Refunds.Status
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
