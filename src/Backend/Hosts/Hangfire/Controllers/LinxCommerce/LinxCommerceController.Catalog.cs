using Application.LinxCommerce.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// busca um produto pelo ProductID (Catalog GetProduct) e grava; a chamada fica em linxcommerce.LinxCommerceRequests
        /// </summary>
        [HttpGet("Products/{docCompany}/{productId:int:min(1)}")]
        [Tags("LinxCommerceCatalog")]
        public async Task<ActionResult> Product(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int productId,
            [FromServices] ILinxCommerceCatalogService linxCommerceCatalogService
        ) =>
            OnDemandResponse(await linxCommerceCatalogService.GetProduct(userName, docCompany, productId), $"Product {productId}");

        /// <summary>
        /// busca um SKU pelo ProductID (Catalog GetSKU) e grava; a chamada fica em linxcommerce.LinxCommerceRequests
        /// </summary>
        [HttpGet("Skus/{docCompany}/{productId:int:min(1)}")]
        [Tags("LinxCommerceCatalog")]
        public async Task<ActionResult> Sku(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int productId,
            [FromServices] ILinxCommerceCatalogService linxCommerceCatalogService
        ) =>
            OnDemandResponse(await linxCommerceCatalogService.GetSKU(userName, docCompany, productId), $"SKU {productId}");
    }
}
