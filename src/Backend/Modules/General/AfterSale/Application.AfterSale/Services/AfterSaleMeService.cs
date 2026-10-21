using Application.AfterSale.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.AfterSale.Entities.Reads.Me;
using Domain.Shared.Abstractions.Api;
using Domain.AfterSale.Enums;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Services
{
    public class AfterSaleMeService : IAfterSaleMeService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly IAfterSaleRepository _afterSaleRepository;
        private readonly IAfterSaleMeRepository _afterSaleMeRepository;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Me.Ecommerce> _ecommerceValidator;

        private const string GetMeHashesKey = "AfterSale:GetMeHashes";

        public AfterSaleMeService(
            [FromKeyedServices("AfterSaleAPI")] IApiClient apiClient, 
            ILoggerService logger, 
            IAfterSaleRepository afterSaleRepository,
            IAfterSaleMeRepository afterSaleMeRepository,
            IValidator<Domain.AfterSale.Dtos.Reads.Me.Ecommerce> ecommerceValidator, 
            IMemoryCache cache
        ) =>
        (_apiClient, _logger, _afterSaleRepository, _afterSaleMeRepository, _ecommerceValidator, _cache) = (apiClient, logger, afterSaleRepository, afterSaleMeRepository, ecommerceValidator, cache);

        /// <summary>
        /// return infos about ecommerce, this method fills the tables: [AfterSaleEcommerces], [AfterSaleAddresses] and [AfterSaleReasons]
        /// </summary>
        public async Task<bool?> GetMe()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleEcommerces);

            List<string> sucessfulHashsKeys = [];
            var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Me.Ecommerce>();
            var companys = await _afterSaleRepository.GetCompanys();

            var processedHashes = _cache.GetOrCreate(GetMeHashesKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return new HashSet<string>();
            });

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                    route: "v3/api/me",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var hash = _logger.ComputeSha256Hash(response);
                var hashKey = $"{company.doc_company} - {hash}";
                sucessfulHashsKeys.Add(hashKey);

                if (!processedHashes.Contains(hashKey))
                {
                    var me = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Me.Root>(response);
                    var validations = _ecommerceValidator.Validate(me.data);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Me.Ecommerce(me.data, response));
                }
            }

            if (_listSomenteNovos.Count() > 0)
            {
                await _afterSaleMeRepository.BulkInsertIntoTableAfterSaleEcommerce(_listSomenteNovos, _logger.GetExecutionGuid());

                _listSomenteNovos.ForEach(s =>
                {
                    _logger.AddRecord(s.uuid.ToString(), s.response);
                });

                sucessfulHashsKeys.ForEach(x => processedHashes.Add(x));
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {_listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
    }
}
