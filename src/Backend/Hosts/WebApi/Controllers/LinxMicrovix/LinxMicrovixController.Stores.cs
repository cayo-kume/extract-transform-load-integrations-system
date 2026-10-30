using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixStoresService: lojas, usuários e catálogo do ERP. O service é injetado na ação ([FromServices]),
    /// porque cada arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza as lojas do grupo (LinxGrupoLojas); roda antes dos outros, que consultam as lojas gravadas aqui
        /// </summary>
        [HttpPost("LinxGrupoLojas")]
        [Tags("LinxMicrovixStores")]
        public async Task<ActionResult> LinxGrupoLojas([FromServices] ILinxMicrovixStoresService linxMicrovixStoresService)
        {
            var result = await linxMicrovixStoresService.LinxGrupoLojas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o cadastro de cada loja do grupo (LinxLojas)
        /// </summary>
        [HttpPost("LinxLojas")]
        [Tags("LinxMicrovixStores")]
        public async Task<ActionResult> LinxLojas([FromServices] ILinxMicrovixStoresService linxMicrovixStoresService)
        {
            var result = await linxMicrovixStoresService.LinxLojas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os parâmetros do portal por loja (LinxLojasParametros)
        /// </summary>
        [HttpPost("LinxLojasParametros")]
        [Tags("LinxMicrovixStores")]
        public async Task<ActionResult> LinxLojasParametros([FromServices] ILinxMicrovixStoresService linxMicrovixStoresService)
        {
            var result = await linxMicrovixStoresService.LinxLojasParametros();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os usuários do portal (LinxUsuarios)
        /// </summary>
        [HttpPost("LinxUsuarios")]
        [Tags("LinxMicrovixStores")]
        public async Task<ActionResult> LinxUsuarios([FromServices] ILinxMicrovixStoresService linxMicrovixStoresService)
        {
            var result = await linxMicrovixStoresService.LinxUsuarios();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de métodos da API (LinxMetodos)
        /// </summary>
        [HttpPost("LinxMetodos")]
        [Tags("LinxMicrovixStores")]
        public async Task<ActionResult> LinxMetodos([FromServices] ILinxMicrovixStoresService linxMicrovixStoresService)
        {
            var result = await linxMicrovixStoresService.LinxMetodos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os entrantes por loja dos últimos 2 anos (LinxEntrantes)
        /// </summary>
        [HttpPost("LinxEntrantes")]
        [Tags("LinxMicrovixStores")]
        public async Task<ActionResult> LinxEntrantes([FromServices] ILinxMicrovixStoresService linxMicrovixStoresService)
        {
            var result = await linxMicrovixStoresService.LinxEntrantes();

            return Ok($"Records integrated successfully.");
        }
    }
}
