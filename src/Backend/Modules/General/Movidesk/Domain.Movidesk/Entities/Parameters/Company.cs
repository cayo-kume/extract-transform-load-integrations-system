using Domain.Shared.Entities.Base;

namespace Domain.Movidesk.Entities.Parameters
{
    public class Company : CompanyBase
    {
        public Guid Token { get; init; }

        // endereços da API da empresa (produção e homologação, IApiClient.UseBaseAddress); nulo usa o Movidesk:BaseUrl do appsettings
        public string? base_url { get; init; }
        public string? homolog_url { get; init; }
    }
}
