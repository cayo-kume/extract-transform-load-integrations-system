using Domain.AfterSale.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Abstractions.Services
{
    /// <summary>
    /// operations shared by the AfterSale services (reverses and refunds)
    /// </summary>
    public interface IAfterSaleService
    {
        /// <summary>
        /// logs a call made on demand to the AfterSale API (POST/PUT actions and GET by id) in general.AfterSaleRequests
        /// </summary>
        public bool SaveRequest(Guid? execution, EnumJob job, string user_name, string httpMethod, string route, string doc_company, long id, string? requestBody, string? responseBody, bool isSuccess);
    }
}
