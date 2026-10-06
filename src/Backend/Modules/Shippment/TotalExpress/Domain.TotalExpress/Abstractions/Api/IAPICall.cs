using Newtonsoft.Json.Linq;

namespace Domain.TotalExpress.Abstractions.Api
{
    public interface IAPICall
    {
        public Task<HttpResponseMessage?> PostAsync(string? rote, Dictionary<string?, string?> parameters, string? clientName, JObject jToken);
        public Task<string?> PostAsync(JToken jToken, string? rote, Dictionary<string?, string?> parameters, string? clientName);
        public Task<string?> PostAsync(JObject jObject, string? rote, Dictionary<string?, string?> parameters, string? clientName);
    }
}
