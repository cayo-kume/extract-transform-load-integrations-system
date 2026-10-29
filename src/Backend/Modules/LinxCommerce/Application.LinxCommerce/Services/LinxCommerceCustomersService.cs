using Application.LinxCommerce.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Enums;
using FluentValidation;

namespace Application.LinxCommerce.Services
{
    /// <summary>
    /// clientes do Linx Commerce (API Profile). Cada método faz as próprias chamadas; o comum vem do ILinxCommerceService
    /// </summary>
    public class LinxCommerceCustomersService : ILinxCommerceCustomersService
    {
        private readonly ILoggerService _logger;
        private readonly ILinxCommerceService _linxCommerceService;
        private readonly ILinxCommerceCustomersRepository _linxCommerceCustomersRepository;
        private readonly IValidator<Domain.LinxCommerce.Dtos.Reads.Customers.Customer> _customerValidator;

        // carga inicial (banco vazio): tudo (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §10.2, pendência 3)
        private static readonly DateTime CustomersInitialLoad = new(2000, 1, 1);

        public LinxCommerceCustomersService(
            ILoggerService logger,
            ILinxCommerceService linxCommerceService,
            ILinxCommerceCustomersRepository linxCommerceCustomersRepository,
            IValidator<Domain.LinxCommerce.Dtos.Reads.Customers.Customer> customerValidator
        ) =>
            (_logger, _linxCommerceService, _linxCommerceCustomersRepository, _customerValidator) =
            (logger, linxCommerceService, linxCommerceCustomersRepository, customerValidator);

        public async Task<bool?> SearchCustomer()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxCommerceSearchCustomer);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxCommerceService.GetCompanys())
                {
                    var batchTime = _linxCommerceService.GetBatchTime();

                    // o cliente não tem data de alteração gravada (o código antigo não lia): a janela sai do último lote, com uma hora de
                    // folga. O filtro por ModifiedDate no SearchCustomer está a conferir: o código antigo filtrava por CreatedDate e
                    // nunca lia alterações (4.2, item 3)
                    var lastBatch = await _linxCommerceCustomersRepository.GetCustomersLastBatchTime(company.doc_company!);
                    var since = lastBatch.HasValue ? lastBatch.Value.AddHours(-1) : CustomersInitialLoad;

                    var found = await _linxCommerceService.Search<Domain.LinxCommerce.Dtos.Reads.Customers.SearchCustomerResult>(
                        company,
                        "Profile/API.svc/web/SearchCustomer",
                        _linxCommerceService.ModifiedDateWindow(since, batchTime),
                        "CustomerID"
                    );

                    if (found is null)
                        continue;

                    var changed = found
                        .Select(x => (id: Domain.Shared.Extensions.ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.CustomerID), type: x.dto.CustomerType))
                        .Where(x => x.id != 0)
                        .GroupBy(x => x.id)
                        .Select(g => g.Last())
                        .ToList();

                    if (changed.Count == 0)
                        continue;

                    var customers = new List<Domain.LinxCommerce.Dtos.Reads.Customers.Customer>();
                    var customersJson = new Dictionary<int, string>();
                    var failed = false;

                    foreach (var (id, type) in changed)
                    {
                        // pessoa física no GetPerson, jurídica no GetCompany, como no código antigo ("P" na busca)
                        var route = type == "P" ? "Profile/API.svc/web/GetPerson" : "Profile/API.svc/web/GetCompany";
                        var customer = await _linxCommerceService.Get<Domain.LinxCommerce.Dtos.Reads.Customers.Customer>(company, route, id, $"cliente {id}");

                        if (customer is null || Domain.Shared.Extensions.ConverterValidatorsExtensions.ConvertToInt32Validation(customer.Value.dto.CustomerID) != id)
                        {
                            if (customer is not null)
                                _logger.AddMessage($"{route} do cliente {id} (empresa {company.doc_company}) devolveu outro CustomerID: {customer.Value.dto.CustomerID}.");

                            failed = true;
                            break;
                        }

                        customers.Add(customer.Value.dto);
                        customersJson[id] = customer.Value.json;
                    }

                    // um cliente que falhou ficaria para trás da próxima janela: nada da empresa é gravado
                    if (failed)
                        continue;

                    totalInseridos += await SaveCustomers(company.doc_company!, batchTime, customers, customersJson);
                }

                _logger.AddMessage(
                    $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!"
                );

                return true;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Devolve a quantidade de clientes gravados.
        /// </summary>
        private async Task<int> SaveCustomers(
            string doc_company,
            DateTime batchTime,
            List<Domain.LinxCommerce.Dtos.Reads.Customers.Customer> customers,
            IReadOnlyDictionary<int, string> customersJson)
        {
            var _listSomenteNovos = new List<Domain.LinxCommerce.Entities.Reads.Customers.Customer>();

            foreach (var customer in customers)
            {
                var validations = _customerValidator.Validate(customer);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"cliente {customer.CustomerID}, {s.PropertyName}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.LinxCommerce.Entities.Reads.Customers.Customer(doc_company, batchTime, customer));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxCommerceCustomersRepository.BulkInsertIntoLinxCommerceCustomers(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
                if (customersJson.TryGetValue(record.CustomerID, out var json))
                    _logger.AddRecord(record.CustomerID.ToString(), json);

            return _listSomenteNovos.Count;
        }
    }
}
