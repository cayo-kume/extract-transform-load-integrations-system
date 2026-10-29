using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxCommerce
{
    /// <summary>
    /// rotas que disparam as leituras do Linx Commerce. A classe é partial: cada service tem um arquivo próprio
    /// (LinxCommerceController.Orders.cs...), com uma rota POST por método e o nome do método como rota
    /// (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §6.4 e §9). Só leituras: as escritas do código antigo não foram migradas (4.6)
    /// </summary>
    [ApiController]
    //[Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("LinxCommerceJobs/LinxCommerce")]
    public partial class LinxCommerceController : Controller
    {
    }
}
