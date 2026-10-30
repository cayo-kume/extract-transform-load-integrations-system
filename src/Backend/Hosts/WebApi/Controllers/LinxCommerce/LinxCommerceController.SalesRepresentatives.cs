using Application.LinxCommerce.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.LinxCommerce
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

        /// <summary>
        /// busca um vendedor pelo SalesRepresentativeID (Sales GetSalesRepresentative) e grava; a chamada fica em
        /// linxcommerce.LinxCommerceRequests
        /// </summary>
        [HttpGet("SalesRepresentatives/{docCompany}/{salesRepresentativeId:int:min(1)}")]
        [Tags("LinxCommerceSalesRepresentatives")]
        public async Task<ActionResult> SalesRepresentative(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int salesRepresentativeId,
            [FromServices] ILinxCommerceSalesRepresentativesService linxCommerceSalesRepresentativesService
        ) =>
            OnDemandResponse(await linxCommerceSalesRepresentativesService.GetSalesRepresentative(userName, docCompany, salesRepresentativeId), $"Sales representative {salesRepresentativeId}");
    }
}
