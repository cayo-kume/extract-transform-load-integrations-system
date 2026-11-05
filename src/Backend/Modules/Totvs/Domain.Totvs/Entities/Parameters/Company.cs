using Domain.Shared.Entities.Base;

namespace Domain.Totvs.Entities.Parameters
{
    /// <summary>
    /// linha de totvs.TotvsParameters: a holding Newbloomers e as credenciais do fluxo password da Totvs.
    /// As lojas da holding vêm de totvs.TotvsBranches, não daqui
    /// </summary>
    public class Company : CompanyBase
    {
        public string client_id { get; init; } = String.Empty;
        public string client_secret { get; init; } = String.Empty;
        public string username { get; init; } = String.Empty;
        public string password { get; init; } = String.Empty;

        // endereço da API da empresa; nulo usa o Totvs:BaseUrl do appsettings (IApiClient.UseBaseAddress)
        public string? base_url { get; init; }
    }
}
