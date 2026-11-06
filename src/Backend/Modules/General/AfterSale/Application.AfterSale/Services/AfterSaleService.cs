using Application.AfterSale.Abstractions.Services;
using Application.Shared.Abstractions.Services;
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
        private readonly IInitialLoad _initialLoad;

        // banco vazio: desde 01/01/2025 (como as notas e os pedidos da Totvs e da LinxCommerce), com o teto da carga inicial
        private static readonly DateTime InitialLoad = new(2025, 1, 1);

        public AfterSaleService(IAfterSaleRepository afterSaleRepository, IInitialLoad initialLoad) =>
            (_afterSaleRepository, _initialLoad) = (afterSaleRepository, initialLoad);

        public DateTime GetSearchStartDate(DateTime? lastUpdate) =>
            lastUpdate.HasValue ? DateTime.Now.AddMonths(-1) : _initialLoad.Since(InitialLoad, DateTime.Now);

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
