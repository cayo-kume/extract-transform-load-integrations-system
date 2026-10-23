using Application.AfterSale.Abstractions.Services;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.AfterSale.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Services
{
    /// <summary>
    /// operations shared by the AfterSale services (reverses and refunds)
    /// </summary>
    public class AfterSaleService : IAfterSaleService
    {
        private readonly IAfterSaleRepository _afterSaleRepository;

        public AfterSaleService(IAfterSaleRepository afterSaleRepository) =>
            _afterSaleRepository = afterSaleRepository;

        /// <summary>
        /// logs a call made on demand to the AfterSale API (POST/PUT actions and GET by id) in aftersale.AfterSaleRequests
        /// </summary>
        /// <param name="execution">execution of the log (auditing.Logs) opened by the caller</param>
        public bool SaveRequest(Guid? execution, EnumJob job, string user_name, string httpMethod, string route, string doc_company, long id, string? requestBody, string? responseBody, bool isSuccess) =>
            _afterSaleRepository.InsertIntoAfterSaleRequests(
                new Domain.AfterSale.Entities.Writes.Requests.Request(
                    execution: execution ?? Guid.Empty,
                    job: job,
                    user_name: user_name,
                    http_method: httpMethod,
                    route: route,
                    doc_company: doc_company,
                    field_key_value: id.ToString(),
                    request_body: requestBody,
                    response_body: responseBody,
                    is_success: isSuccess
                )
            );
    }
}
