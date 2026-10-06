using Application.Core.Abstractions.Services;
using Application.Jadlog.Abstractions.Services;
using Domain.Core.Abstractions.Api;
using Domain.Core.Entities.Exceptions;
using Domain.Core.Enums;
using Domain.Jadlog.Abstractions.Repositorys;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Application.Jadlog.Services
{
    public class JadlogService : IJadlogService
    {
        private readonly IApiClient _apiClient;
        private readonly IApiClient _apiClientTech;
        private readonly ILoggerService _logger;
        private readonly IValidator<Domain.Jadlog.DTOs.Consulta> _validator;
        private readonly IJadlogRepository _jadlogRepository;

        public JadlogService(
            IJadlogRepository jadlogRepository,
            ILoggerService logger,
            IValidator<Domain.Jadlog.DTOs.Consulta> validator,
            [FromKeyedServices("JadlogAPI")] IApiClient apiClient,
            [FromKeyedServices("JadlogTechAPI")] IApiClient apiClientTech) =>
            (_jadlogRepository, _logger, _validator, _apiClient, _apiClientTech) = (jadlogRepository, logger, validator, apiClient, apiClientTech);

        public async Task<bool?> CancelOrder()
        {
            //Refatorar Aqui (retirar try catches)

            try
            {
                return true;
            }
            catch (SQLCommandException ex)
            {
                _logger.AddMessage(
                    //stage: ex.Stage,
                    //error: ex.Error,
                    //log//level: ex.MessageLevel,
                    message: ex.Message
                    //exceptionMessage: ex.ExceptionMessage
                    //commandSQL: ex.CommandSQL
                );

                throw;
            }
            catch (GeneralException ex)
            {
                _logger.AddMessage(
                    //stage: ex.stage,
                    //error: ex.Error,
                    //log//level: ex.MessageLevel,
                    message: ex.Message
                    //exceptionMessage: ex.ExceptionMessage
                );

                throw;
            }
            catch (Exception ex)
            {
                _logger.AddMessage(
                    message: "Error when executing GetRecords method"
                    //exceptionMessage: ex.Message
                );
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }

            return true;
        }

        public async Task<bool?> InsertOrder()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.JadlogSendOrders);

            var _listSomenteNovos = new List<Domain.Jadlog.Entities.OrderSent>();
            var orders = await _jadlogRepository.GetInvoicedOrders();
               
            if (orders.Count() > 0)
            {
                foreach (var order in orders)
                {
                    try
                    {
                        var parameter = await _jadlogRepository.GetParameters(doc_tomador: order.tomador.Doc, doc_remetente: order.company.Doc, tipo_servico: order.TIPO_SERVICO);
                        var jObject = BuildJObject(parameter, order);
                        var response = await _apiClient.PostAsync(route: "embarcador/api/pedido/incluir", body: jObject, headers: new Dictionary<string, string> { { "Authorization", $"Bearer {parameter.token}" } });
                        var pedido = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Jadlog.DTOs.Response.Rootobject>(response);
                        _listSomenteNovos.Add(new Domain.Jadlog.Entities.OrderSent(pedido: pedido, cod_pedido: order.Number, response: response, request: jObject));
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }                
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _jadlogRepository.InsertResponse(_listSomenteNovos, _logger.GetExecutionGuid());

                    for (int i = 0; i < _listSomenteNovos.Count; i++)
                    {
                        _logger.AddRecord(_listSomenteNovos[i].recordKey, _listSomenteNovos[i].recordXml);
                    }
                }

                _logger.AddMessage(
                    $"Concluída com sucesso: {_listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
                );
            }

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public Task<bool?> InsertTreatment()
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SearchDACTEXml()
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SearchPickupPoints()
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SearchQRCodePickupDropoff()
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SearchShippingValue()
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> SearchTrackingHistory()
        {
            try
            {
                _logger
                   .Clear()
                   .AddLog(EnumJob.JadlogSearchTrackingHistory);

                var consultas = new List<Domain.Jadlog.DTOs.Consulta>();
                var trackings = new List<Domain.Jadlog.Entities.Consulta>();
                var parameters = await _jadlogRepository.GetParameters();
                var shipments = await _jadlogRepository.GetShipmentIds();

                foreach (var parameter in parameters)
                {
                    var listExistingShipments = shipments.Where(x => x.product == parameter.product && x.doc_company == parameter.doc_company).ToList();

                    for (int i = 0; i < listExistingShipments.Count(); i += 100)
                    {
                        var lote = listExistingShipments.Skip(i).Take(100).ToList();
                        var consultaObj = new JObject { 
                            { 
                                "consulta", JArray.FromObject(lote.Select(s => new { shipmentId = s.shipment_id })) 
                            } 
                        };

                        var response = await _apiClientTech.PostAsync(
                                        route: "embarcador/api/tracking/consultar",
                                        body: consultaObj,
                                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {parameter.token}" } }
                                    );

                        var results = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Jadlog.DTOs.Root>(response);

                        consultas.AddRange(results.consulta.Where(x => !listExistingShipments.Any(y => 
                                y.shipment_id == x.shipmentId &&
                                y.eventos.Count() == x.tracking.eventos
                                                                .GroupBy(i => new { i.data, i.status, i.unidade })
                                                                .Select(g => g.First())
                                                                .Count())
                            )
                        );
                    }
                }

                foreach (var consulta in consultas)
                {
                    var validations = _validator.Validate(consulta);

                    foreach (var error in validations.Errors)
                    {
                        _logger.AddMessage(
                            //stage: EnumStages.DeserializeXMLToObject,
                            //error: EnumError.Validation,
                            //log//level: EnumMessageLevel.Warning,
                            message: $"Error when convert record - shipmentId: {consulta.shipmentId} | tracking_code: {consulta.tracking.codigo}\n{error}"
                        );
                    }

                    trackings.Add(new Domain.Jadlog.Entities.Consulta(consulta, Newtonsoft.Json.JsonConvert.SerializeObject(consulta.tracking)));
                }

                if (trackings.Count() > 0)
                {
                    await _jadlogRepository.BulkInsertIntoTableRaw(trackings, _logger.GetExecutionGuid());

                    trackings.ForEach(s =>
                            _logger.AddRecord(
                                s.shipmentId.ToString(),
                                s.Responses
                                    .Where(pair => pair.Key == s.shipmentId)
                                    .Select(pair => pair.Value)
                                    .FirstOrDefault()
                            )
                        );

                    _logger.AddMessage(
                                $"Concluída com sucesso: {trackings.Count()} registro(s) novo(s) inserido(s)!"
                            );
                }
                else
                    _logger.AddMessage(
                        $"Concluída com sucesso: {trackings.Count()} registro(s) novo(s) inserido(s)!"
                    );

                return true;
            }
            catch (SQLCommandException ex)
            {
                _logger.AddMessage(
                    //stage: ex.Stage,
                    //error: ex.Error,
                    //log//level: ex.MessageLevel,
                    message: ex.Message
                    //exceptionMessage: ex.ExceptionMessage
                    //commandSQL: ex.CommandSQL
                );

                throw;
            }
            catch (GeneralException ex)
            {
                _logger.AddMessage(
                    //stage: ex.stage,
                    //error: ex.Error,
                    //log//level: ex.MessageLevel,
                    message: ex.Message
                    //exceptionMessage: ex.ExceptionMessage
                );

                throw;
            }
            catch (Exception ex)
            {
                _logger.AddMessage(
                    message: "Error when executing GetRecords method"
                    //exceptionMessage: ex.Message
                );
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }

            return true;
        }

        private JObject? BuildJObject(Domain.Jadlog.Entities.Parameters parameter, Domain.Jadlog.Entities.Order order)
        {
            return new JObject
            {
                { "codCliente", parameter.cod_client },
                { "conteudo", order.itens.First().Description.Replace("Ç","C").Replace("Ú","U").PadRight(25).Substring(0, order.itens.First().Description.Length > 24 ? 24 : order.itens.First().Description.Length) },
                { "pedido", new JArray (order.Number) },
                { "totPeso", order.itens.First().weight_product },
                { "totValor", order.itens.First().Amount },
                { "obs", null },
                { "modalidade", parameter.modality },
                { "contaCorrente", parameter.account },
                { "tpColeta", "K" },
                { "tipoFrete", null },
                { "cdUnidadeOri", parameter.cdUnitOri },
                { "cdUnidadeDes", null },
                { "cdPickupOri", null },
                { "cdPickupDes", null },
                { "nrContrato", null },
                { "servico", null },
                { "shipmentId", null },
                { "vlColeta", null },
                { "des", new JObject {
                        { "nome", order.client.Reason },
                        { "cnpjCpf", order.client.Doc },
                        { "ie", order.client.StateRegistration },
                        { "endereco", order.client.Street },
                        { "numero", order.client.StreetNumber },
                        { "compl", order.client.ComplementAddress?.Length > 40 ? order.client.ComplementAddress?.Substring(0, 39) : order.client.ComplementAddress },
                        { "bairro", order.client.Neighborhood },
                        { "cidade", order.client.City },
                        { "uf", order.client.State },
                        { "cep", order.client.ZipCode },
                        { "fone", null },
                        { "cel", order.client.Phone },
                        { "email", order.client.Email },
                        { "contato", order.client.Reason?.Length > 40 ? order.client.Reason?.Substring(0, 39) : order.client.Reason }
                    }
                },
                { "rem", new JObject {
                        { "nome", order.company.Reason },
                        { "cnpjCpf", order.company.Doc },
                        { "ie", order.company.StateRegistration },
                        { "endereco", order.company.Street },
                        { "numero", order.company.StreetNumber },
                        { "compl", order.company.ComplementAddress },
                        { "bairro", order.company.Neighborhood },
                        { "cidade", order.company.City },
                        { "uf", order.company.State },
                        { "cep", order.company.ZipCode },
                        { "fone", null },
                        { "cel", order.company.Phone },
                        { "email", order.company.Email },
                        { "contato", order.company.Name }
                    }
                },
                { "tomador", new JObject {
                        { "nome", order.tomador.Reason },
                        { "cnpjCpf", order.tomador.Doc },
                        { "ie", order.tomador.StateRegistration },
                        { "endereco", order.tomador.Street },
                        { "numero", order.tomador.StreetNumber },
                        { "compl", order.tomador.ComplementAddress },
                        { "bairro", order.tomador.Neighborhood },
                        { "cidade", order.tomador.City },
                        { "uf", order.tomador.State },
                        { "cep", order.tomador.ZipCode },
                        { "fone", null },
                        { "cel", order.tomador.Phone },
                        { "email", order.tomador.Email },
                        { "contato", order.tomador.Name }
                    }
                },
                { "dfe", new JArray(
                    new JObject {
                        { "cfop", order.Cfop },
                        { "danfeCte", order.invoice.KeyNfe },
                        { "nrDoc", order.invoice.Number },
                        { "serie", order.invoice.Serie },
                        { "tpDocumento", 2 },
                        { "valor", order.invoice.Amount },
                    })
                },
                { "volume", new JArray(
                    new JObject {
                        { "altura", 1 },
                        { "comprimento", 1 },
                        { "identificador", order.Number },
                        { "largura", 1 },
                        { "peso", 1 }
                    })
                },
            };
        }
    }
}
