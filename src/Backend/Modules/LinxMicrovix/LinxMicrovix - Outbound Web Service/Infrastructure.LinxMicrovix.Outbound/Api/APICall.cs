using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Domain.Core.Entities.Exceptions;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using System.Text;

namespace Infrastructure.LinxMicrovix.Outbound.WebService.Api
{
    public class APICall : IAPICall
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public APICall(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string?> PostAsync(LinxAPIParam jobParameter, string? body)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LinxMicrovixAPI");
                var content = new StringContent(body ?? string.Empty, Encoding.ASCII, "text/xml");
                var response = await client.PostAsync(jobParameter.jobName, content);

                if (response.IsSuccessStatusCode)
                    return (await response.Content.ReadAsStringAsync()).Replace("'", "");
                else
                    throw new APIException(
                        message: $"Error when querying endpoint: {jobParameter.jobName} on microvix webservice",
                        exceptionMessage: $"API return HttpStatusCode:'{response.StatusCode}' when it was expected '200 (OK)'\n",
                        apiRequestResponse: $"Request: {body}"
                    );
            }
            catch (APIException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new APIException(
                    message: $"Error when querying endpoint: {jobParameter.jobName} on microvix webservice - {ex.Message}",
                    exceptionMessage: ex.StackTrace,
                    apiRequestResponse: body
                );
            }
        }
    }
}
