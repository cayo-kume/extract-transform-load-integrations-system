
using Application.FlashCourier.Interfaces;
using Domain.Core.Abstractions.Api;
using Domain.FlashCourier.Entities;
using Domain.FlashCourier.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Application.FlashCourier.Services
{
    public class FlashCourierService : IFlashCourierService
    {
        private readonly IApiClient _apiClient;
        private readonly IFlashCourierRepository _flashCourierRepository;

        public FlashCourierService([FromKeyedServices("FlashCourierAPI")] IApiClient apiClient, IFlashCourierRepository flashCourierRepository) =>
            (_apiClient, _flashCourierRepository) = (apiClient, flashCourierRepository);

        public async Task<bool> SendOrder(string? orderNumber)
        {
            try
            {
                var order = await _flashCourierRepository.GetInvoicedOrder(orderNumber);
                var parameters = await _flashCourierRepository.GetParameters(order.company.Doc);

                if (order is not null)
                {
                    var jObject = BuildJObject(order, parameters);

                    await _flashCourierRepository.GenerateRequestLog(
                        order.Number,
                        JsonConvert.SerializeObject(jObject)
                    );

                    string? result = await _apiClient.PostAsync(
                        route: "/padrao/importacao",
                        body: jObject,
                        headers: new Dictionary<string, string>
                        {
                            { "Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{parameters.login}:{parameters.senha}"))}" },
                            { "Cookie", "ROUTEID=.1" }
                        }
                    );

                    var postHAWB = JsonConvert.DeserializeObject<List<AWBSucessResponse>>(result);
                    var statusFlash = postHAWB.FirstOrDefault().type.ToUpper() == "SUCESS" ? "Enviado" : "Erro_Flash";

                    await _flashCourierRepository.GenerateResponseLog(
                        orderNumber: order.Number,
                        senderID: order.company.Doc,
                        result,
                        statusFlash: "Enviado",
                        keyNFe: order.invoice.KeyNfe
                    );

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
            try
            {
                var orders = await _flashCourierRepository.GetInvoicedOrders();

                if (orders.Count() > 0)
                {
                    foreach (var order in orders)
                    {
                        var parameters = await _flashCourierRepository.GetParameters(order.company.Doc);

                        var jObject = BuildJObject(order, parameters);

                        await _flashCourierRepository.GenerateRequestLog(
                            order.Number,
                            JsonConvert.SerializeObject(jObject)
                        );

                        string? result = await _apiClient.PostAsync(
                            route: "/padrao/importacao",
                            body: jObject,
                            headers: new Dictionary<string, string>
                            {
                                { "Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{parameters.login}:{parameters.senha}"))}" },
                                { "Cookie", "ROUTEID=.1" }
                            }
                        );

                        var postHAWB = JsonConvert.DeserializeObject<List<AWBSucessResponse>>(result);
                        var statusFlash = postHAWB.FirstOrDefault().type.ToUpper() == "SUCESS" ? "Enviado" : "Erro_Flash";

                        await _flashCourierRepository.GenerateResponseLog(
                            orderNumber: order.Number,
                            senderID: order.company.Doc,
                            result,
                            statusFlash: "Enviado",
                            keyNFe: order.invoice.KeyNfe
                        );
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

        public async Task<bool> UpdateLogOrders()
        {
            try
            {
                var companys = await _flashCourierRepository.GetCompanys();

                foreach (var company in companys)
                {
                    var orders = await _flashCourierRepository.GetShippedOrders(company.Doc);

                    if (orders.Count() > 0)
                    {
                        var parameters = await _flashCourierRepository.GetParameters(company.Doc);
                        var token = "8f1ef4f5cd3989238192cf1e3306d06d88398b8521a7f4ec8c6a9d55c1429f0b"; //HERE

                        var jObject = new JObject
                        {
                            { "login", parameters.login },
                            { "senha", parameters.senha }
                        };

                        var authenticationResponse = await _apiClient.PostAsync(
                            route: "/api/v1/token",
                            body: jObject,
                            headers: new Dictionary<string, string>
                            {
                                { "Authorization", token }
                            }
                        );
                        var authentication = JsonConvert.DeserializeObject<AuthenticationResponse>(authenticationResponse);
                        string?[] numEncCli = orders.Select(a => a.Number).ToArray();

                        var _jObject = new JObject
                        {
                            { "clienteId", parameters.cliente_id },
                            { "cttId", new JArray { parameters.ctt_id } },
                            { "numEncCli", new JArray { numEncCli } }
                        };

                        var result = await _apiClient.PostAsync(
                            route: "/padrao/v2/consulta",
                            body: _jObject,
                            headers: new Dictionary<string, string>
                            {
                                { "Authorization", authentication.access_token }
                            }
                        );

                        var awb = JsonConvert.DeserializeObject<AWBResponse>(result);

                        if (awb.statusRetorno == "00")
                        {
                            foreach (var hawb in awb.hawbs)
                            {
                                var currentStatusHawb = hawb.historico.Last().evento;
                                var currentHistoricoHawb = hawb.historico.LastOrDefault();
                                var currentColetaHawb = hawb.historico.Where(a => a.evento.ToUpper() == "POSTADO - LOGISTICA INICIADA").FirstOrDefault();
                                var currentOrder = orders.FirstOrDefault(a => a.Number == hawb.codigoCartao);

                                //if (!String.IsNullOrEmpty(hawb.dtSla))
                                //    await _flashCourierRepository.UpdateRealDeliveryForecastDate(hawb.dtSla, hawb.codigoCartao);

                                //if (currentStatusHawb.ToUpper().Contains("ENTREGUE") || currentStatusHawb.ToUpper() == "COMPROVANTE REGISTRADO")
                                //    await _flashCourierRepository.UpdateDeliveryMadeDate(currentHistoricoHawb.ocorrencia, hawb.codigoCartao);

                                //if (currentColetaHawb is not null && currentColetaHawb.evento.ToUpper() == "POSTADO - LOGISTICA INICIADA")
                                //    await _flashCourierRepository.UpdateCollectionDate(currentColetaHawb.ocorrencia, hawb.codigoCartao);

                                //if (!String.IsNullOrEmpty(currentHistoricoHawb.evento))
                                //    await _flashCourierRepository.UpdateLastStatusDate(currentHistoricoHawb.ocorrencia, currentHistoricoHawb.eventoId, currentHistoricoHawb.evento, hawb.codigoCartao);
                            }
                        }
                    }
                    continue;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }

        private JArray BuildJObject(Domain.FlashCourier.Entities.Order order, Parameters parameters)
        {
            return new JArray(
                            new JObject {
                            { "dna_hawb", 7 },
                            { "ccusto_id", parameters.ccusto_id },
                            { "tipo_enc_id", parameters.tipo_enc_id },
                            { "prod_flash_id", 152 },
                            { "frq_rec_id", "DSP" },
                            { "id_local_rem", parameters.id_local_rem },
                            { "cliente_id", parameters.cliente_id },
                            { "ctt_id", parameters.ctt_id },
                            //registro no banco
                            { "num_enc_cli", order.Number }, //codigo de rastreio alfanumerico com ate 30 digitos, pode ser um sequencial nosso
                            { "num_cliente", order.invoice.Number },
                            { "nome_rem", "OPEN ERA" },
                            { "endHawbs", new JObject {
                                    { "nome_des", order.client.Reason },
                                    { "logr_dest", order.client.Street },
                                    { "bairro_des", order.client.Neighborhood },
                                    { "num_des", order.client.StreetNumber },
                                    { "fone1_des", order.client.Phone },
                                    { "cid_dest", order.client.City },
                                    { "uf_dest", order.client.State },
                                    { "cep_dest", order.client.ZipCode },
                                    { "compl_end_dest", order.client.ComplementAddress }
                                }
                            },
                            { "cod_lote", "1234567" },
                            { "peso_declarado", order.weight },
                            { "qtde_itens", order.quantity },
                            { "valor", Convert.ToDouble(order.invoice.Amount) },
                            { "cpf_des", order.client.Doc },
                            { "email_des", order.client.Email },
                            { "chave_nf", order.invoice.KeyNfe },
                            { "endHawbs2", new JObject {
                                    { "bairro_des", "" },
                                    { "cid_dest", "" },
                                    { "compl_end_dest", "" },
                                    { "fone1_des", "" },
                                    { "fone2_des", "" },
                                    { "fone3_des", "" },
                                    { "logr_dest", "" },
                                    { "nome_des", "" },
                                    { "num_des", "" },
                                    { "uf_dest", "" },
                                }
                            }
                            }
                        );
        }
    }
}
