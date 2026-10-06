using Domain.Shared.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Parameters
{
    public class Company : CompanyBase
    {
        public Guid Token { get; init; }
    }
}
