using Application.LinxCommerce.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxCommerce
{
    /// <summary>
    /// rotas do LinxCommerceCatalogService: produtos e SKUs. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxCommerceController
    {
        /// <summary>
        /// sincroniza os produtos alterados desde a última leitura (Catalog SearchProduct + GetProduct)
        /// </summary>
        [HttpPost("SearchProduct")]
        [Tags("LinxCommerceCatalog")]
        public async Task<ActionResult> SearchProduct([FromServices] ILinxCommerceCatalogService linxCommerceCatalogService)
        {
            var result = await linxCommerceCatalogService.SearchProduct();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os SKUs alterados desde a última leitura (Catalog SearchSKU + GetSKU)
        /// </summary>
        [HttpPost("SearchSKU")]
        [Tags("LinxCommerceCatalog")]
        public async Task<ActionResult> SearchSKU([FromServices] ILinxCommerceCatalogService linxCommerceCatalogService)
        {
            var result = await linxCommerceCatalogService.SearchSKU();

            return Ok($"Records integrated successfully.");
        }
    }
}
