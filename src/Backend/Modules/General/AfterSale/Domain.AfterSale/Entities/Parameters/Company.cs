using Domain.Shared.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Parameters
{
    public class Company : CompanyBase
    {
        public Guid Token { get; init; }

        // endereço da API da empresa; nulo usa o AfterSale:BaseUrl do appsettings (IApiClient.UseBaseAddress)
        public string? base_url { get; init; }
    }
}
