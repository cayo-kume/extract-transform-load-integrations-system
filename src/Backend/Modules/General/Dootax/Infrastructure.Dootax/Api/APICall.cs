using Domain.Dootax.Entities;
using Domain.Dootax.Interfaces.Apis;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace Infrastructure.Dootax.Api
{
    public class APICall : IAPICall
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public APICall(IHttpClientFactory httpClientFactory) =>
            (_httpClientFactory) = (httpClientFactory);

        public async Task<XML> PostAsync(XML xml)
        {
            try
            {
                //select in database to take token and user
                //var client = CreateClient("78391c28-0d30-4d8d-bbcd-735006165ecf", "newbloomers"); //homolog
                var client = CreateClient("87e2c224-fb6a-48c2-82da-8c57584b3169", "newbloomers"); //prod
                var xmlText = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml.DsXml)); //convert xml to base64
                var jObject = new JObject
                {
                    { "filename", xml.Documento + ".txt" },
                    { "content", xmlText },
                };

                var response = await client.PostAsync(client.BaseAddress + "/api/v2/doodoc/pagtrib/upload/import", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(jObject), Encoding.UTF8, "application/json"));

                if (response.StatusCode == HttpStatusCode.OK)
                    return xml;
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception(@$"Dootax - PostXmlAsync - Falha ao executar request - {ex.Message}");
            }
        }

        private HttpClient CreateClient(string token, string userName)
        {
            var client = _httpClientFactory.CreateClient("DootaxAPI");
            client.DefaultRequestHeaders.Add("oauth-token", token);
            client.DefaultRequestHeaders.Add("tenant-alias", userName);

            return client;
        }
    }
}
