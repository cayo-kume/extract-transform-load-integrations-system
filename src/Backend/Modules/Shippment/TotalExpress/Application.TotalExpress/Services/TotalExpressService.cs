using Application.Core.Abstractions.Services;
using Application.TotalExpress.Abstractions.Services;
using Domain.Core.Enums;
using Domain.TotalExpress.Abstractions.Api;
using Domain.TotalExpress.Abstractions.Repository;
using Domain.TotalExpress.Dtos;
using Domain.TotalExpress.Entities;
using Domain.TotalExpress.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace Application.TotalExpress.Services
{
    public class TotalExpressService : ITotalExpressService
    {
        private readonly IAPICall _apiCall;
        private readonly ILoggerService _logger;
        private readonly ITotalExpressRepository _totalExpressRepository;

        public TotalExpressService(IAPICall apiCall, ITotalExpressRepository totalExpressRepository, ILoggerService logger) =>
            (_apiCall, _totalExpressRepository, _logger) = (apiCall, totalExpressRepository, logger);

        public async Task<bool> SendOrder(string? orderNumber)
        {
            try
            {
                var order = await _totalExpressRepository.GetInvoicedOrder(orderNumber);

                if (order != null)
                {
                    var parameters = await _totalExpressRepository.GetParameters(order.company.Doc, order.shippingCompany.Method);
                    var jArray = BuildJObject(order, parameters);
                    var token = await GenerateToken();
                    var headers = new Dictionary<string?, string?>
                    {
                        { "ContentType", "application/json" },
                        { "Authorization", token.token_type + " " + token.access_token }
                    };

                    for (int j = 0; j < jArray.Count(); j++)
                    {
                        await _totalExpressRepository.GenerateRequestLog(
                                            order.Number,
                                            JsonConvert.SerializeObject(jArray[j])
                                        );

                        string? response = await _apiCall.PostAsync(
                            jArray[j],
                            "ics-edi-lv/v1/coleta/smartlabel/registrar",
                            headers,
                            "TotalExpressAPI"
                        );

                        //await _totalExpressRepository.GenerateResponseLog(
                        //    order.number,
                        //    parameters.sender_id,
                        //    response
                        //);
                    }

                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> SendOrderAsEtur(string? orderNumber)
        {
            try
            {
                var order = await _totalExpressRepository.GetInvoicedOrder(orderNumber);

                if (order != null)
                {
                    var parameters = await _totalExpressRepository.GetParameters(order.company.Doc, "ETUR");
                    var jArray = BuildJObject(order, parameters);
                    var token = await GenerateToken();
                    var headers = new Dictionary<string?, string?>
                    {
                        { "ContentType", "application/json" },
                        { "Authorization", token.token_type + " " + token.access_token }
                    };

                    for (int j = 0; j < jArray.Count(); j++)
                    {
                        await _totalExpressRepository.GenerateRequestLog(
                                            order.Number,
                                            JsonConvert.SerializeObject(jArray[j])
                                        );

                        string? response = await _apiCall.PostAsync(
                            jArray[j],
                            "ics-edi-lv/v1/coleta/smartlabel/registrar",
                            headers,
                            "TotalExpressAPI"
                        );

                        //await _totalExpressRepository.GenerateResponseLog(
                        //    order.number,
                        //    parameters.sender_id,
                        //    response
                        //);
                    }

                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> SendOrders()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.TotalExpressSendOrders);

            var _listSomenteNovos = new List<Encomenda>();
            var orders = await _totalExpressRepository.GetInvoicedOrders();

            if (orders.Count() > 0)
            {
                var headers = new Dictionary<string?, string?>
                {
                    { "Authorization", "Basic" + " " + "YXBpLW5ld2Jsb29tZXJzOjdUVkNHS0BmcWU=" }
                };

                foreach (var order in orders)
                {
                    try
                    {
                        var parameters = await _totalExpressRepository.GetParameters(order.company.Doc, order.shippingCompany.Method);
                        var jArray = BuildJObject(order, parameters);

                        for (int j = 0; j < jArray.Count(); j++)
                        {
                            await _totalExpressRepository.GenerateRequestLog(
                                                order.Number,
                                                JsonConvert.SerializeObject(jArray[j])
                                            );

                            string? response = await _apiCall.PostAsync(
                                jArray[j],
                                "ics-edi-lv/v1/coleta/smartlabel/registrar",
                                headers,
                                "TotalExpressAPI"
                            );

                            var retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<Return.Rootobject>(response);

                            foreach (var encomenda in retorno.retorno.encomendas)
                            {
                                _listSomenteNovos.Add(new Encomenda(encomenda, response, parameters.sender_id, request: jArray[j]));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _totalExpressRepository.GenerateResponseLog(encomendas: _listSomenteNovos, _logger.GetExecutionGuid());

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

            return false;
        }

        public async Task<bool> InsertLogOrdersByDateInterval()
        {
            try
            {
                var parameters = await _totalExpressRepository.GetSenderIds();

                if (parameters.Count() > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        var jObject = new JObject
                        {
                            { "remetenteId", 47812 },
                            //{ "data_inicial", DateTime.Now.Date.ToString("yyyy-MM-dd") }
                            //{ "remetenteId", "remetenteId" },
                            //{ "pedido", "OA-35289" }
                            { "awb", "TXAS614593325tx" }
                        };

                        var headers = new Dictionary<string?, string?>
                        {
                            { "ContentType", "application/xml" },
                            { "Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("apistatusnew-prod:GttTBS8x")) } //transformar em variável
                        };

                        try
                        {
                            string? response = await _apiCall.PostAsync(
                                                jObject,
                                                "previsao_entrega_atualizada.php",
                                                headers,
                                                "TotalExpressEdiAPI"
                                             );

                            var statusList = JsonConvert.DeserializeObject<IEnumerable<Status>>(response);
                            //var status = JsonConvert.DeserializeObject<Status>(response);

                            //foreach (var status in statusList)
                            //{
                            //    if (status.detalhes != null)
                            //    {
                            //        var lastStatus = status.detalhes.statusDeEncomenda.LastOrDefault();

                            //        var lastStatusDescription = lastStatus == null ?
                            //                                    null : $"{lastStatus.statusid} - {lastStatus.status}";

                            //        var lastStatusDate = lastStatus == null ?
                            //                             null : $"{lastStatus.data}";

                            //        var deliveryForecastDate = status.detalhes.dataPrev == null ?
                            //                           null : status.detalhes.dataPrev.PrevEntrega;

                            //        var collectionDate = status.detalhes.statusDeEncomenda.Where(p => p.status == "COLETA REALIZADA").FirstOrDefault() == null ?
                            //                         null : status.detalhes.statusDeEncomenda.Where(p => p.status == "COLETA REALIZADA").FirstOrDefault().data;

                            //        var deliveryMadeDate = status.detalhes.statusDeEncomenda.Where(p => p.status == "ENTREGA REALIZADA").FirstOrDefault() == null ?
                            //                         null : status.detalhes.statusDeEncomenda.Where(p => p.status == "ENTREGA REALIZADA").FirstOrDefault().data;

                            //        await _totalExpressRepository.UpdateDeliveryDates(
                            //            deliveryMadeDate,
                            //            collectionDate,
                            //            deliveryForecastDate,
                            //            lastStatusDate,
                            //            lastStatusDescription,
                            //            status.pedido
                            //        );
                            //    }
                            //}
                        }
                        catch (Exception ex) when (ex.Message.Contains("BadRequest"))
                        {
                            continue;
                        }
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertLogOrdersByAWBs()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.TotalExpressTrackingHistory);

            var listStatus = new List<Status>();
            var listErrors = new List<Error>();

            var pedidos = await _totalExpressRepository.GetSenderAwbs();

            if (pedidos.Count() > 0)
            {
                foreach (var pedido in pedidos)
                {
                    var jObject = new JObject
                    {
                        { "remetenteId", pedido.remetenteid },
                        { "pedido", $"{pedido.pedido}" }
                    };

                    var headers = new Dictionary<string?, string?>
                    {
                        { "ContentType", "application/xml" },
                        { "Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("apistatusnew-prod:GttTBS8x")) } //transformar em variável
                    };

                    var response = await _apiCall.PostAsync(
                        "previsao_entrega_atualizada.php",
                        headers,
                        "TotalExpressEdiAPI",
                        jObject
                    );

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var res = await response.Content.ReadAsStringAsync();

                        if (res.IndexOf("{") == 0)
                        {
                            var status = JsonConvert.DeserializeObject<Status>(res);
                            listStatus.Add(new Status(status, res));
                        }
                        else
                        {
                            var statusList = JsonConvert.DeserializeObject<List<Status>>(res);

                            statusList
                                .ForEach(status =>
                                    listStatus.Add(new Status(status, res))
                            );
                        }
                    }
                    else
                        listErrors.Add(new Error(pedido: pedido.pedido, erro: await response.Content.ReadAsStringAsync()));
                }
            }

            if (listStatus.Count() > 0 || listErrors.Count() > 0)
            {
                await _totalExpressRepository.InsertStatus(listStatus, listErrors, new Guid());

                listStatus.ForEach(s =>
                    _logger.AddRecord(
                        s.pedido.ToString(),
                        s.Responses
                            .Where(pair => pair.Key == s.pedido)
                            .Select(pair => pair.Value)
                            .FirstOrDefault()
                    )
                );

                listErrors.ForEach(s =>
                    _logger.AddRecord(
                        s.pedido.ToString(),
                        s.Responses
                            .Where(pair => pair.Key == s.pedido)
                            .Select(pair => pair.Value)
                            .FirstOrDefault()
                    )
                );
            }

            _logger.AddMessage(
                    $"Concluída com sucesso: {listStatus.Count} registro(s) novo(s) inserido(s), {listErrors.Count} erro(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        private JArray BuildJObject(Order order, Parameters parameters)
        {
            var jArrayObj = new JArray();

            foreach (var item in order.itens)
            {
                var jObject = new JObject
                {
                    { "remetenteId", parameters.sender_id },
                    { "cnpj", order.company.Doc },
                    { "encomendas", new JArray(
                        new JObject
                        {
                            { "servicoTipoInfo", null },
                            { "servicoTipo", parameters.service_type },
                            { "coletaInfo", String.Empty },
                            { "condFrete", "CIF" },
                            { "entregaTipo", 0 },
                            { "icmsIsencao", 0 },
                            { "natureza", item.Description.Replace("Ç","C").Replace("Ú","U").PadRight(25).Substring(0, item.Description.Length > 24 ? 24 : item.Description.Length) },
                            { "pedido", order.Number.Trim() },
                            { "peso", 0 },
                            { "volumes", order.Volumes },
                            { "volumesTipo", "CX" },
                            { "clienteCodigo", int.Parse(order.client.Code) },
                            { "destinatario", new JObject
                                {
                                    { "nome", order.client.Reason },
                                    { "cpfCnpj", order.client.Doc },
                                    { "ie", order.client.StateRegistration },
                                    { "email", order.client.Email },
                                    { "telefone1", order.client.Phone.Replace("(","").Replace(")","").Replace("-","").Replace(" ","") },
                                    { "telefone2", "" },
                                    { "telefone3", "" },
                                    { "endereco", new JObject
                                        {
                                            { "bairro", order.client.Neighborhood },
                                            { "cep", order.client.ZipCode },
                                            { "cidade", order.client.City },
                                            { "complemento", order.client.ComplementAddress },
                                            { "estado", order.client.State },
                                            { "logradouro", order.client.Street },
                                            { "numero", order.client.StreetNumber },
                                            { "pais", "BR" },
                                            { "pontoReferencia", "" }
                                        }
                                    }
                                }
                            },
                            { "docFiscal", new JObject
                                {
                                    { "nfe", new JArray(
                                            new JObject
                                            {
                                                { "nfeCfop", order.Cfop?.Replace(".","") },
                                                { "nfeChave", order.invoice.KeyNfe },
                                                { "nfeData", order.invoice.DateEmission.ToString("dd-MM-yyyy") },
                                                { "nfeNumero", int.Parse(order.invoice.Number) },
                                                { "nfeSerie", order.invoice.Serie },
                                                { "nfeValProd", order.invoice.Amount },
                                                { "nfeValTotal", order.invoice.Amount }
                                            }
                                        )
                                    }
                                }
                            },
                        })
                    },
                };

                jArrayObj.Add(jObject);
            }

            return jArrayObj;
        }

        private async Task<Token?> GenerateToken()
        {
            var jObject = new JObject
            {
                { "grant_type", "password" },
                { "username", "api-newbloomers" }, //transformar em variável
                { "password", "7TVCGK@fqe" } //transformar em variável
            };

            var headers = new Dictionary<string?, string?>
            {
                { "ContentType", "application/json" },
                { "Authorization", "Basic " + "SUNTOnRvdGFs" } //transformar em variável
            };

            var result = await _apiCall.PostAsync(
                jObject,
                "ics-seguranca/v1/oauth2/tokenGerar",
                headers,
                "TotalExpressAPI"
            );

            return JsonConvert.DeserializeObject<Token>(result);
        }

        public async Task<string> GetSendOrder(string orderNumber)
        {
            var pedido = await _totalExpressRepository.GetSendOrder(orderNumber);
            return Newtonsoft.Json.JsonConvert.SerializeObject(pedido);
        }
    }
}