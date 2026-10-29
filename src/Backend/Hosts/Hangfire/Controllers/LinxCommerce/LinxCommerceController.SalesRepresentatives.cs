using Application.LinxCommerce.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxCommerce
{
    /// <summary>
    /// rotas do LinxCommerceSalesRepresentativesService: vendedores, só leitura. O service é injetado na ação ([FromServices]),
    /// porque cada arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxCommerceController
    {
        /// <summary>
        /// sincroniza os vendedores (Sales SearchSalesRepresentative + GetSalesRepresentative)
        /// </summary>
        [HttpPost("SearchSalesRepresentative")]
        [Tags("LinxCommerceSalesRepresentatives")]
        public async Task<ActionResult> SearchSalesRepresentative([FromServices] ILinxCommerceSalesRepresentativesService linxCommerceSalesRepresentativesService)
        {
            var result = await linxCommerceSalesRepresentativesService.SearchSalesRepresentative();

            return Ok($"Records integrated successfully.");
        }
    }
}
