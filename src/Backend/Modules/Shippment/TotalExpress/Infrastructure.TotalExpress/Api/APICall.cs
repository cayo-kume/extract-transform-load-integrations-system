using Domain.TotalExpress.Abstractions.Api;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace Infrastructure.TotalExpress.Api
{
    public class APICall : IAPICall
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public APICall(IHttpClientFactory httpClientFactory) =>
            (_httpClientFactory) = (httpClientFactory);

        public async Task<HttpResponseMessage?> PostAsync(string? rote, Dictionary<string?, string?> parameters, string? clientName, JObject jObject)
        {
            var client = CreateClient(clientName, parameters);

            if (rote == "previsao_entrega_atualizada.php")
                Thread.Sleep(1000);

            return await client.PostAsync(
                client.BaseAddress + rote,
                new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(jObject),
                Encoding.UTF8, "application/json")
            );
        }

        public async Task<string?> PostAsync(JObject jObject, string? rote, Dictionary<string?, string?> parameters, string? clientName)
        {
            var client = CreateClient(clientName, parameters);

            if (rote == "previsao_entrega_atualizada.php")
                Thread.Sleep(1000);

            var response = await client.PostAsync(
                client.BaseAddress + rote,
                new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(jObject),
                Encoding.UTF8, "application/json")
            );

            if (response.StatusCode == HttpStatusCode.OK)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception($"{response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }

        public async Task<string?> PostAsync(JToken jToken, string? rote, Dictionary<string?, string?> parameters, string? clientName)
        {
            var client = CreateClient(clientName, parameters);

            var response = await client.PostAsync(
                client.BaseAddress + rote,
                new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(jToken),
                Encoding.UTF8, "application/json")
            );

            if (response.StatusCode == HttpStatusCode.OK)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception($"{response.StatusCode} - {await response.Content.ReadAsStringAsync()}");

        }

        private HttpClient CreateClient(string? clientName, Dictionary<string?, string?> parameters)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            foreach (var parameter in parameters)
            {
                client.DefaultRequestHeaders.Add(parameter.Key, parameter.Value);
            }

            return client;
        }
    }
}

