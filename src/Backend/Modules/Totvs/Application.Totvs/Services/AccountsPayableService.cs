using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Enums;
using Domain.Shared.Extensions;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Application.Totvs.Services
{
    public class AccountsPayableService : IAccountsPayableService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IValidator<Domain.Totvs.Dtos.AccountsPayable.Duplicate> _duplicateValidator;
        private readonly IValidator<Domain.Totvs.Dtos.AccountsPayable.ComissionPaid> _comissionPaidValidator;
        private readonly IAccountsPayableRepository _accountsPayableRepository;
        private static List<Domain.Totvs.Dtos.AccountsPayable.Duplicate?> _duplicatesCache { get; set; } = new List<Domain.Totvs.Dtos.AccountsPayable.Duplicate?>();
        private static List<Domain.Totvs.Dtos.AccountsPayable.ComissionPaid?> _comissionsPaidCache { get; set; } = new List<Domain.Totvs.Dtos.AccountsPayable.ComissionPaid?>();

        public AccountsPayableService(
            [FromKeyedServices("TotvsHmg")] IApiClient apiClient,
            ILoggerService logger,
            IValidator<Domain.Totvs.Dtos.AccountsPayable.Duplicate> duplicateValidator,
            IValidator<Domain.Totvs.Dtos.AccountsPayable.ComissionPaid> comissionPaidValidator,
            IAccountsPayableRepository accountsPayableRepository) =>
            (_apiClient, _logger, _duplicateValidator, _comissionPaidValidator, _accountsPayableRepository) =
                (apiClient, logger, duplicateValidator, comissionPaidValidator, accountsPayableRepository);

        public async Task<bool?> PostSearchDuplicates()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.TotvsAccountsPayableDuplicates);

            int pageIndex = 1, pageCount = 1;
            var duplicatesList = new List<Domain.Totvs.Entities.AccountsPayable.Duplicate>();
            var itens = new List<Domain.Totvs.Dtos.AccountsPayable.Duplicate>();

            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "newbloomerstrnapiv2" },
                { "client_secret", "4860687042" },
                { "username", "INT" },
                { "password", "INTV3" }
            };

            var response = await _apiClient.PostFormAsync(
                route: "/authorization/v2/token",
                formData: parameters
            );

            var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(response);

            while (pageIndex <= pageCount)
            {
                var objectRequest = new
                {
                    filter = new
                    {
                        change = new
                        {
                            startDate = $"{DateTime.Now.AddMonths(-10).Date:yyyy-MM-dd}T00:00:00.000Z",
                            endDate = $"{DateTime.Now.Date:yyyy-MM-dd}T23:59:59.000Z"
                        }
                    },
                    order = "maxChangeFilterDate",
                    page = pageIndex,
                    pageSize = 100,
                    expand = "expense"
                };

                var _response = await _apiClient.PostAsync(
                    route: "/accounts-payable/v2/duplicates/search",
                    body: objectRequest,
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                );

                try
                {
                    var duplicates = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Totvs.Dtos.AccountsPayable.Root>(_response);

                    if (pageCount == 1)
                        pageCount = duplicates.totalPages;

                    pageIndex++;

                    itens.AddRange(duplicates.items);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (_duplicatesCache.Count() == 0)
                _duplicatesCache = await _accountsPayableRepository.GetExistingDuplicates(
                    itens.Select(x => x.duplicateCode).ToList()
                );

            itens.RemoveAll(x => _duplicatesCache.Any(y =>
               x.duplicateCode == y.duplicateCode &&
               ConverterValidatorsExtensions.ConvertToDateTimeValidation<string?>(x.maxChangeFilterDate) <= DateTime.Parse(y.maxChangeFilterDate, new CultureInfo("en-US"))
            ));

            foreach (var duplicate in itens)
            {
                var validations = _duplicateValidator.Validate(duplicate);

                if (validations.Errors.Count() > 0)
                {
                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        _logger.AddMessage(
                            message: $"Error when convert record - duplicateCode: {duplicate.duplicateCode} | Supplier: {duplicate.supplierCode}\n" +
                                     $"{validations.Errors[j]}"
                        );
                    }
                }

                duplicatesList.Add(new Domain.Totvs.Entities.AccountsPayable.Duplicate(duplicate));
            }

            if (duplicatesList.Count() > 0)
            {
                await _accountsPayableRepository.BulkInsertIntoTableDuplicatesRaw(duplicatesList, _logger.GetExecutionGuid());

                foreach (var item in itens.Where(x => x != null))
                    _duplicatesCache.Add(item);

                duplicatesList.ForEach(d =>
                    _logger.AddRecord(
                        d.duplicateCode.ToString(),
                        d.Responses
                            .Where(pair => pair.Key == d.duplicateCode.ToString())
                            .Select(pair => pair.Value)
                            .FirstOrDefault()
                    )
                );

                _logger.AddMessage(
                    $"Concluída com sucesso: {duplicatesList.Count()} registro(s) novo(s) inserido(s)!"
                );
            }
            else
                _logger.AddMessage(
                    $"Concluída com sucesso: {duplicatesList.Count()} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool?> PostSearchComissionsPaid()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.TotvsAccountsPayableComissionsPaid);

            int pageIndex = 1, pageCount = 1;
            var comissionsPaidList = new List<Domain.Totvs.Entities.AccountsPayable.ComissionPaid>();
            var itens = new List<Domain.Totvs.Dtos.AccountsPayable.ComissionPaid>();

            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "newbloomerstrnapiv2" },
                { "client_secret", "4860687042" },
                { "username", "INT" },
                { "password", "INTV3" }
            };

            var response = await _apiClient.PostFormAsync(
                route: "/authorization/v2/token",
                formData: parameters
            );

            var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(response);

            while (pageIndex <= pageCount)
            {
                var objectRequest = new
                {
                    filter = new
                    {
                        change = new
                        {
                            startDate = $"{DateTime.Now.AddMonths(-10).Date:yyyy-MM-dd}T00:00:00.000Z",
                            endDate = $"{DateTime.Now.Date:yyyy-MM-dd}T23:59:59.000Z"
                        }
                    },
                    order = "maxChangeFilterDate",
                    page = pageIndex,
                    pageSize = 500
                };

                var _response = await _apiClient.PostAsync(
                    route: "/accounts-payable/v2/comissions-paid/search",
                    body: objectRequest,
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                );

                try
                {
                    var comissionsPaid = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Totvs.Dtos.AccountsPayable.ComissionPaidRoot>(_response);

                    if (pageCount == 1)
                        pageCount = comissionsPaid.totalPages;

                    pageIndex++;

                    itens.AddRange(comissionsPaid.items);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (_comissionsPaidCache.Count() == 0)
                _comissionsPaidCache = await _accountsPayableRepository.GetExistingComissionsPaid(
                    itens.Select(x => x.commissionCode).ToList()
                );

            itens.RemoveAll(x => _comissionsPaidCache.Any(y =>
               x.commissionCode == y.commissionCode &&
               ConverterValidatorsExtensions.ConvertToDateTimeValidation<string?>(x.maxChangeFilterDate) <= DateTime.Parse(y.maxChangeFilterDate, new CultureInfo("en-US"))
            ));

            foreach (var comissionPaid in itens)
            {
                var validations = _comissionPaidValidator.Validate(comissionPaid);

                if (validations.Errors.Count() > 0)
                {
                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        _logger.AddMessage(
                            message: $"Error when convert record - commissionCode: {comissionPaid.commissionCode} | Representative: {comissionPaid.representativeCode}\n" +
                                     $"{validations.Errors[j]}"
                        );
                    }
                }

                comissionsPaidList.Add(new Domain.Totvs.Entities.AccountsPayable.ComissionPaid(comissionPaid));
            }

            if (comissionsPaidList.Count() > 0)
            {
                await _accountsPayableRepository.BulkInsertIntoTableComissionsPaidRaw(comissionsPaidList, _logger.GetExecutionGuid());

                foreach (var item in itens.Where(x => x != null))
                    _comissionsPaidCache.Add(item);

                comissionsPaidList.ForEach(c =>
                    _logger.AddRecord(
                        c.commissionCode.ToString(),
                        c.Responses
                            .Where(pair => pair.Key == c.commissionCode.ToString())
                            .Select(pair => pair.Value)
                            .FirstOrDefault()
                    )
                );

                _logger.AddMessage(
                    $"Concluída com sucesso: {comissionsPaidList.Count()} registro(s) novo(s) inserido(s)!"
                );
            }
            else
                _logger.AddMessage(
                    $"Concluída com sucesso: {comissionsPaidList.Count()} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
    }
}
