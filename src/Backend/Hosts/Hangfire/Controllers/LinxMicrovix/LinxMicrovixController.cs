using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas que disparam as leituras do Linx Microvix (ERP e B2C). A classe é partial: cada service entra, um por vez, num
    /// arquivo próprio (LinxMicrovixController.Stores.cs...), com uma rota POST por método da API e o nome do método como rota
    /// (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §6.4 e §9)
    /// </summary>
    [ApiController]
    //[Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("LinxMicrovixJobs/LinxMicrovix")]
    public partial class LinxMicrovixController : Controller
    {
    }
}
