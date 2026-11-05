using Domain.Shared.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Parameters
{
    public class Company : CompanyBase
    {
        public Guid Token { get; init; }

        // endereços da API da empresa (produção e homologação, IApiClient.UseBaseAddress); nulo usa o AfterSale:BaseUrl do appsettings
        public string? base_url { get; init; }
        public string? homolog_url { get; init; }
    }
}
