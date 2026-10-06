using Application.Core.Abstractions.Services;
using Application.Movidesk.Interfaces.Services;
using Domain.Core.Abstractions.Api;
using Domain.Core.Enums;
using Domain.Movidesk.Interfaces.Repositorys;
using Domain.Movidesk.Models.Tickets;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Application.Movidesk.Services
{
    public class MovideskService : IMovideskService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMovideskRepository _movideskRepository;

        private readonly IValidator<Domain.Movidesk.Dtos.Person.Person> _personValidator;
        private readonly IValidator<Domain.Movidesk.Dtos.Tickets.Ticket> _ticketValidator;

        private readonly IMemoryCache _cache;
        private const string PersonsHashesKey = "Movidesk:PersonsHashes";
        private const string TicketsHashKey = "Movidesk:TicketsHash";

        public MovideskService([FromKeyedServices("MovideskAPI")] IApiClient apiClient, ILoggerService logger, IMovideskRepository movideskRepository, IValidator<Domain.Movidesk.Dtos.Person.Person> personValidator, IValidator<Domain.Movidesk.Dtos.Tickets.Ticket> ticketValidator, IMemoryCache cache) =>
            (_apiClient, _logger, _movideskRepository, _personValidator, _ticketValidator, _cache) = (apiClient, logger, movideskRepository, personValidator, ticketValidator, cache);

        public Task<bool> GetPerson()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> GetPersons()
        {
            _logger
              .Clear()
              .AddLog(EnumJob.MovideskGetPersons);

            var _listSomenteNovos = new List<Domain.Movidesk.Models.Persons.Person>();

            string[] profileTypes = { "2", "3" };

            var token = await _movideskRepository.GetTokenAsync();

            var processedHashes = _cache.GetOrCreate(PersonsHashesKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return new HashSet<string>();
            });

            for (int i = 0; i < profileTypes.Length; i++)
            {
                var filters = $"profileType eq {profileTypes[i]}&$orderby=id desc";

                var response = await _apiClient.GetAsync(
                    route: $"persons?token={token.Token}&$filter={filters}"
                );

                var hash = _logger.ComputeSha256Hash(response);

                if (!processedHashes.Contains(hash))
                {
                    var persons = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Person.Person>>(response);

                    foreach (var person in persons)
                    {
                        var validations = _personValidator.Validate(person);

                        if (validations.Errors.Count() > 0)
                        {
                            validations.Errors.ForEach(x =>
                                _logger.AddMessage(
                                    message: $"Error when convert record - person_id: {person.id} | document: {person.cpfCnpj}\n" +
                                                $"{x}"
                                )
                            );

                            continue;
                        }

                        _listSomenteNovos.Add(new Domain.Movidesk.Models.Persons.Person(person: person));
                    }
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _movideskRepository.BulkInsertIntoTableRaw(_listSomenteNovos, _logger.GetExecutionGuid());

                    _listSomenteNovos.ForEach(s =>
                        _logger.AddRecord(s.RecordKey, s.RecordResponse)
                    );

                    processedHashes.Add(hash);
                }
            }

            _logger.AddMessage(
                    $"Concluída com sucesso: {_listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public Task<bool> GetService()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> GetServices()
        {
            var token = await _movideskRepository.GetTokenAsync();

            var response = await _apiClient.GetAsync(
                route: $"services?token={token.Token}"
            );

            var services = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Services.Service>>(response);

            return true;
        }

        public Task<bool> GetTicket()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> GetTickets()
        {
            _logger
              .Clear()
              .AddLog(EnumJob.MovideskGetTickets);

            var _listSomenteNovos = new List<Domain.Movidesk.Models.Tickets.Ticket>();

            var token = await _movideskRepository.GetTokenAsync();

            var filters = $"createdDate ge {DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}T00:00:00.00z and createdDate le {DateTime.Today.ToString("yyyy-MM-dd")}T00:00:00.00z";

            var selectItens = "id, protocol, type, subject, category, urgency, status, baseStatus, justification," +
                                "origin, createdDate, originEmailAccount, ownerTeam, serviceFirstLevelId, serviceFirstLevel," +
                                "serviceSecondLevel, serviceThirdLevel, contactForm, serviceFull, tags, cc, resolvedIn," +
                                "reopenedIn, closedIn, lastActionDate, actionCount, lastUpdate, lifeTimeWorkingTime," +
                                "stoppedTime, stoppedTimeWorkingTime, resolvedInFirstCall, chatWidget, chatGroup, chatTalkTime," +
                                "chatWaitingTime, sequence, slaAgreement, slaAgreementRule, slaSolutionTime, slaResponseTime," +
                                "slaSolutionChangedByUser, slaSolutionDate, slaSolutionDateIsPaused, slaResponseDate, slaRealResponseDate";

            var expandItens = "owner($select=id, personType, profileType, businessName, email, phone)," +
                                "createdBy($select=id, personType, profileType, businessName, email, phone)," +
                                "slaSolutionChangedBy($select=id, personType, profileType, businessName, email, phone)," +
                                "clients($select=id, personType, profileType, businessName, email, phone, isDeleted, organization)," +
                                "actions($expand=createdBy, timeAppointments, expenses, attachments)," +
                                "customFieldValues($expand=items)";

            var response = await _apiClient.GetAsync(
                route: $"tickets?token={token.Token}&$filter={filters}&$select={selectItens}&$expand={expandItens}"
            );

            var hash = _logger.ComputeSha256Hash(response);
            var cachedHash = _cache.Get<string>(TicketsHashKey) ?? String.Empty;

            if (!cachedHash.Equals(hash))
            {
                try
                {
                    var tickets = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Tickets.Ticket>>(response);

                    foreach (var ticket in tickets)
                    {
                        var validations = _ticketValidator.Validate(ticket);

                        if (validations.Errors.Count() > 0)
                        {
                            validations.Errors.ForEach(x =>
                                _logger.AddMessage(
                                    message: $"Error when convert record - ticket_id: {ticket.id} | subject: {ticket.subject}\n" +
                                                $"{x}"
                                )
                            );
                        }

                        _listSomenteNovos.Add(new Domain.Movidesk.Models.Tickets.Ticket(ticket));
                    }

                    if (_listSomenteNovos.Count() > 0)
                    {
                        await _movideskRepository.BulkInsertIntoTableRaw(_listSomenteNovos, _logger.GetExecutionGuid());

                        _listSomenteNovos.ForEach(s =>
                            _logger.AddRecord(s.RecordKey, s.RecordResponse)
                        );

                        _cache.Set(TicketsHashKey, hash, TimeSpan.FromHours(2));
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {_listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /*************************************************************************************************************************************************************************/

        public async Task BuildInvoicesFromTickets()
        {
            try
            {
                var filter = String.Empty;
                var response = String.Empty;

                var selectItens = "id, subject, createdDate, resolvedIn, serviceFirstLevel";
                var expandItens = "createdBy($select=businessName, email, phone), customFieldValues($expand=items), actions($select=id, description)";

                var tickets = new List<TicketAntigo?>();
                var token = await _movideskRepository.GetTokenAsync();

                tickets = await _movideskRepository.GetTicketsFromDatabase();

                if (tickets.Count() > 0)
                {
                    for (int i = 0; i < tickets.Count(); i++)
                    {
                        if (i != tickets.Count() - 1)
                            filter += $"id eq {tickets[i].id} or ";
                        else
                            filter += $"id eq {tickets[i].id}";
                    }

                    response = await _apiClient.GetAsync(
                        route: $"tickets?token={token.Token}&$filter={filter}&$select={selectItens}&$expand={expandItens}"
                    );

                    var ticketsToBeBuild = JsonConvert.DeserializeObject<List<TicketAntigo>>(response.Replace("'", "''"));
                    var faturas = BuildInvoicesFromTickets(ticketsToBeBuild);

                    foreach (var fatura in faturas)
                    {
                        if (await _movideskRepository.InsertInvoice(fatura))
                        {
                            await _movideskRepository.UpdateTicketStatusFromDatabase(fatura);
                        }
                    }
                }
            }
            catch (Exception ex) when (ex.Message.Contains(" - "))
            {
                string[] subs = ex.Message.Split(" - ");
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertTicket(string request)
        {
            try
            {
                var ticketWebhook = JsonConvert.DeserializeObject<TicketAntigo>(request);
                await _movideskRepository.InsertTicket(ticketWebhook);
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateTicket()
        {
            try
            {
                var filter = String.Empty;
                var response = String.Empty;

                var selectItens = "id, subject, createdDate, resolvedIn, serviceFirstLevel";
                var expandItens = "createdBy($select=businessName, email, phone), customFieldValues($expand=items), actions($select=id, description)";

                var tickets = new List<TicketAntigo?>();
                var token = await _movideskRepository.GetTokenAsync();

                tickets = await _movideskRepository.GetBlankTicketsFromDatabase();

                if (tickets.Count() > 0)
                {
                    for (int i = 0; i < tickets.Count(); i++)
                    {
                        if (i != tickets.Count() - 1)
                            filter += $"id eq {tickets[i].id} or ";
                        else
                            filter += $"id eq {tickets[i].id}";
                    }

                    response = await _apiClient.GetAsync(
                        route: $"tickets?token={token.Token}&$filter={filter}&$select={selectItens}&$expand={expandItens}"
                    );

                    var updatedTickets = JsonConvert.DeserializeObject<List<TicketAntigo>>(response.Replace("'", "''"));
                    await _movideskRepository.UpdateTicketFromDatabase(updatedTickets);
                }

                return true;
            }
            catch (Exception ex) when (ex.Message.Contains(" - "))
            {
                string[] subs = ex.Message.Split(" - ");
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<Invoice> BuildInvoicesFromTickets(IEnumerable<TicketAntigo> tickets)
        {
            var listFaturas = new List<Invoice>();
            foreach (var ticket in tickets)
            {
                try
                {
                    var fatura = new Invoice();
                    foreach (var customField in ticket.customFieldValues)
                    {
                        try
                        {
                            if (customField.customFieldId == 90569 && ticket.customFieldValues.Where(c => c.customFieldId == 90569).First().value is not null)
                                fatura.codigo_barras_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 90569).First().value;

                            if (customField.customFieldId == 90557 && ticket.customFieldValues.Where(c => c.customFieldId == 90557).First().value is not null)
                                fatura.nome_fantasia_fornecedor = ticket.customFieldValues.Where(c => c.customFieldId == 90557).First().value;

                            if (customField.customFieldId == 90584 && ticket.customFieldValues.Where(c => c.customFieldId == 90584).First().value is not null)
                                fatura.cidade_fornecedor = Regex.Replace(ticket.customFieldValues.Where(c => c.customFieldId == 90584).First().value, @"[^\w\.@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));

                            if (customField.customFieldId == 90581 && ticket.customFieldValues.Where(c => c.customFieldId == 90581).First().value is not null)
                                fatura.endereco_fornecedor = ticket.customFieldValues.Where(c => c.customFieldId == 90581).First().value;

                            if (customField.customFieldId == 133070 && ticket.customFieldValues.Where(c => c.customFieldId == 133070).First().value is not null)
                                fatura.numero_endereco_fornecedor = Regex.Replace(ticket.customFieldValues.Where(c => c.customFieldId == 133070).First().value, @"[^\w\.@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));

                            if (customField.customFieldId == 90585 && ticket.customFieldValues.Where(c => c.customFieldId == 90585).First().items.Count() > 0)
                                fatura.uf_fornecedor = ticket.customFieldValues.Where(c => c.customFieldId == 90585).First().items.First().customFieldItem;

                            if (customField.customFieldId == 90580 && ticket.customFieldValues.Where(c => c.customFieldId == 90580).First().value is not null)
                                fatura.cep_fornecedor = ticket.customFieldValues.Where(c => c.customFieldId == 90580).First().value;

                            if (customField.customFieldId == 90560 && ticket.customFieldValues.Where(c => c.customFieldId == 90560).First().value is not null)
                                fatura.agencia_fornecedor = Regex.Replace(ticket.customFieldValues.Where(c => c.customFieldId == 90560).First().value, @"[^0-9a-zA-Z]+", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));

                            if (customField.customFieldId == 90559 && ticket.customFieldValues.Where(c => c.customFieldId == 90559).First().items.Count() > 0)
                                fatura.banco_fornecedor = Regex.Replace(ticket.customFieldValues.Where(c => c.customFieldId == 90559).First().items.First().customFieldItem, @"[^0-9a-zA-Z]+", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));

                            if (customField.customFieldId == 90561 && ticket.customFieldValues.Where(c => c.customFieldId == 90561).First().value is not null)
                                fatura.conta_corrente_fornecedor = Regex.Replace(ticket.customFieldValues.Where(c => c.customFieldId == 90561).First().value, @"[^0-9a-zA-Z]+", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));

                            if (customField.customFieldId == 131957 && ticket.customFieldValues.Where(c => c.customFieldId == 131957).First().items.Count() > 0)
                                fatura.forma_pagamento_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 131957).First().items.First().customFieldItem;

                            if (customField.customFieldId == 139723 && ticket.customFieldValues.Where(c => c.customFieldId == 139723).First().value is not null)
                                fatura.pix_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 139723).First().value;

                            if (customField.customFieldId == 132100 && ticket.customFieldValues.Where(c => c.customFieldId == 132100).First().value is not null)
                                fatura.numero_documento_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 132100).First().value == String.Empty ? 0 : Convert.ToInt64(Regex.Replace(ticket.customFieldValues.Where(c => c.customFieldId == 132100).First().value, @"[^\w\.@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5)));

                            if (customField.customFieldId == 132101 && ticket.customFieldValues.Where(c => c.customFieldId == 132101).First().value is not null)
                                fatura.serie_documento_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 132101).First().value == String.Empty ? 0 : Convert.ToInt64(Regex.Replace(ticket.customFieldValues.Where(c => c.customFieldId == 132101).First().value, @"[^\w\.@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5)));

                            if (customField.customFieldId == 90562 && ticket.customFieldValues.Where(c => c.customFieldId == 90562).First().value is not null && ticket.customFieldValues.Where(c => c.customFieldId == 90562).First().value != "")
                                fatura.data_vencimento_fatura = Convert.ToDateTime(ticket.customFieldValues.Where(c => c.customFieldId == 90562).First().value.Split("T")[0]);

                            if (customField.customFieldId == 124596 && ticket.customFieldValues.Where(c => c.customFieldId == 124596).First().value is not null)
                                fatura.numero_parcela_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 124596).First().value == String.Empty ? 0 : Convert.ToInt32(Regex.Replace(ticket.customFieldValues.Where(c => c.customFieldId == 124596).First().value, @"[^\w\.@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5)));

                            if (customField.customFieldId == 132102 && ticket.customFieldValues.Where(c => c.customFieldId == 132102).First().value is not null)
                                fatura.total_parcelas_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 132102).First().value == String.Empty ? 0 : Convert.ToInt32(Regex.Replace(ticket.customFieldValues.Where(c => c.customFieldId == 132102).First().value, @"[^\w\.@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5)));

                            if (customField.customFieldId == 107789 && ticket.customFieldValues.Where(c => c.customFieldId == 107789).First().value is not null)
                                fatura.valor_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 107789).First().value == String.Empty ? 0 : decimal.Parse(ticket.customFieldValues.Where(c => c.customFieldId == 107789).First().value.Replace(".", ","), new CultureInfo("pt-BR"));

                            if (ticket.serviceFirstLevel == "01 - Misha")
                            {
                                if (customField.customFieldId == 133016 && ticket.customFieldValues.Where(c => c.customFieldId == 133016).First().items.Count() > 0)
                                    fatura.centro_custo_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 133016).First().items.First().customFieldItem;
                            }
                            else if (ticket.serviceFirstLevel == "03 - Open Era")
                            {
                                if (customField.customFieldId == 133017 && ticket.customFieldValues.Where(c => c.customFieldId == 133017).First().items.Count() > 0)
                                    fatura.centro_custo_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 133017).First().items.First().customFieldItem;
                            }
                            else if (ticket.serviceFirstLevel == "04 - Margaux")
                            {
                                if (customField.customFieldId == 133018 && ticket.customFieldValues.Where(c => c.customFieldId == 133018).First().items.Count() > 0)
                                    fatura.centro_custo_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 133018).First().items.First().customFieldItem;
                            }
                            else if (ticket.serviceFirstLevel == "10 - New Bloomers")
                            {
                                if (customField.customFieldId == 133019 && ticket.customFieldValues.Where(c => c.customFieldId == 133019).First().items.Count() > 0)
                                    fatura.centro_custo_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 133019).First().items.First().customFieldItem;
                            }

                            if (customField.customFieldId == 134368 && ticket.customFieldValues.Where(c => c.customFieldId == 134368).First().items.Count() > 0)
                                fatura.historico_contabil_fatura = ticket.customFieldValues.Where(c => c.customFieldId == 134368).First().items.First().customFieldItem;

                            if (customField.customFieldId == 90558 && ticket.customFieldValues.Where(c => c.customFieldId == 90558).First().value is not null)
                                fatura.cnpj_fornecedor = Regex.Replace(ticket.customFieldValues.Where(c => c.customFieldId == 90558).First().value, @"[^\w\.@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
                        }
                        catch
                        {
                            throw new Exception(@$"{customField}");
                        }
                    }

                    fatura.nome_empresa = ticket.serviceFirstLevel;
                    fatura.id_ticket = Convert.ToInt32(ticket.id);
                    fatura.data_emissao_fatura = ticket.resolvedIn;
                    fatura.data_lancamento_fatura = ticket.resolvedIn;

                    if (fatura.numero_parcela_fatura == 0)
                        fatura.numero_parcela_fatura = 1;

                    if (fatura.total_parcelas_fatura == 0)
                        fatura.total_parcelas_fatura = 1;

                    if (ticket.serviceFirstLevel == "01 - Misha")
                        fatura.cnpj_empresa = "38367316000199";
                    else if (ticket.serviceFirstLevel == "03 - Open Era")
                        fatura.cnpj_empresa = "42538267000187";
                    else if (ticket.serviceFirstLevel == "04 - Margaux")
                        fatura.cnpj_empresa = "33713331000128";
                    else if (ticket.serviceFirstLevel == "10 - New Bloomers")
                        fatura.cnpj_empresa = "44065634000106";

                    if (fatura.data_vencimento_fatura.ToString() == "01/01/0001 00:00:00" || fatura.data_vencimento_fatura.ToString() == "1/1/0001 12:00:00 AM")
                        fatura.data_vencimento_fatura = DateTime.Now;

                    if (fatura.data_emissao_fatura.ToString() == "01/01/0001 00:00:00" || fatura.data_emissao_fatura.ToString() == "1/1/0001 12:00:00 AM")
                        fatura.data_emissao_fatura = ticket.createdDate;

                    if (fatura.data_lancamento_fatura.ToString() == "01/01/0001 00:00:00" || fatura.data_lancamento_fatura.ToString() == "1/1/0001 12:00:00 AM")
                        fatura.data_lancamento_fatura = ticket.createdDate;

                    fatura.obs = ticket.actions[0].description;

                    listFaturas.Add(fatura);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return listFaturas;
        }
    }
}
