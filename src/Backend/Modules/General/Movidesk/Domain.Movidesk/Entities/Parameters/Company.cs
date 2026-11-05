using Domain.Shared.Entities.Base;

namespace Domain.Movidesk.Entities.Parameters
{
    public class Company : CompanyBase
    {
        public Guid Token { get; init; }

        // endereço da API da empresa; nulo usa o Movidesk:BaseUrl do appsettings (IApiClient.UseBaseAddress)
        public string? base_url { get; init; }
    }
}
