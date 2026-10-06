using Application.AfterSale.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Api;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Services
{
    public class AfterSaleRefundsService : IAfterSaleRefundsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly IAfterSaleRepository _afterSaleRepository;
        private readonly IValidator<Domain.AfterSale.Dtos.Refunds.GetRefund.Refund> _refundValidator;

        private const string GetRefundHashesKey = "AfterSale:GetRefundHashes";

        public AfterSaleRefundsService(
            [FromKeyedServices("AfterSaleAPI")] IApiClient apiClient, 
            ILoggerService logger, 
            IAfterSaleRepository afterSaleRepository, 
            IValidator<Domain.AfterSale.Dtos.Refunds.GetRefund.Refund> refundValidator, 
            IMemoryCache cache
        ) =>
        (_apiClient, _logger, _afterSaleRepository, _refundValidator, _cache) = (apiClient, logger, afterSaleRepository, refundValidator, cache);

        /// <summary>
        /// returns all refunds
        /// </summary>
        public Task<bool?> GetRefunds()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// returns the possible actions of a reverse
        /// </summary>
        public Task<bool?> GetActions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// returns the possible banks of a refund
        /// </summary>
        public Task<bool?> GetBanks()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get a refund
        /// </summary>
        public Task<bool?> GetRefundById(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// returns the possible status of a refund
        /// </summary>
        public Task<bool?> GetStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// returns the possible types of a refund
        /// </summary>
        public Task<bool?> GetTypes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// cancel reverse
        /// </summary>
        public Task<bool?> PostCancelRefundById(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// change total amount of refund
        /// </summary>
        public Task<bool?> PostChangeTotalAmountRefundById(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// pay refund
        /// </summary>
        public Task<bool?> PostPayRefundById(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// approve reverse
        /// </summary>
        /// <param name="id"></param>
        public Task<bool?> PostApproveRefundById(long id)
        {
            throw new NotImplementedException();
        }
    }
}
