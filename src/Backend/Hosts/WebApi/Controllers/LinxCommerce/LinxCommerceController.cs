using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.LinxCommerce
{
    /// <summary>
    /// rotas que disparam as leituras do Linx Commerce. A classe é partial: cada service tem um arquivo próprio
    /// (LinxCommerceController.Orders.cs...), com uma rota POST por leitura recorrente e o nome do método como rota
    /// (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §6.4 e §9). As rotas sob demanda (fase 3) são GET por chave, com o cabeçalho
    /// X-User-Name, e ficam registradas em linxcommerce.LinxCommerceRequests. Só leituras: as escritas do código antigo não
    /// foram migradas (4.6)
    /// </summary>
    [ApiController]
    //[Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("LinxCommerceJobs/LinxCommerce")]
    public partial class LinxCommerceController : Controller
    {
        /// <summary>
        /// resposta das rotas sob demanda: true gravado (200), false não existe no Linx Commerce (404), null empresa não cadastrada
        /// ou resposta inválida (422, com o detalhe no log da execução), como no TotvsController
        /// </summary>
        private ActionResult OnDemandResponse(bool? result, string record) =>
            result switch
            {
                true => Ok($"{record} integrated successfully."),
                false => NotFound($"{record} not found in Linx Commerce."),
                null => UnprocessableEntity("Company not registered or invalid response from Linx Commerce. See the execution log.")
            };
    }
}
