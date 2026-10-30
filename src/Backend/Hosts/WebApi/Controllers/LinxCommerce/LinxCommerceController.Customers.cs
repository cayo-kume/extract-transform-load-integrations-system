using Application.LinxCommerce.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.LinxCommerce
{
    /// <summary>
    /// rotas do LinxCommerceCustomersService: clientes. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxCommerceController
    {
        /// <summary>
        /// sincroniza os clientes alterados desde a última leitura (Profile SearchCustomer + GetPerson ou GetCompany)
        /// </summary>
        [HttpPost("SearchCustomer")]
        [Tags("LinxCommerceCustomers")]
        public async Task<ActionResult> SearchCustomer([FromServices] ILinxCommerceCustomersService linxCommerceCustomersService)
        {
            var result = await linxCommerceCustomersService.SearchCustomer();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// busca um cliente pelo CustomerID (Profile SearchCustomer + GetPerson ou GetCompany) e grava; as chamadas ficam em
        /// linxcommerce.LinxCommerceRequests
        /// </summary>
        [HttpGet("Customers/{docCompany}/{customerId:int:min(1)}")]
        [Tags("LinxCommerceCustomers")]
        public async Task<ActionResult> Customer(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int customerId,
            [FromServices] ILinxCommerceCustomersService linxCommerceCustomersService
        ) =>
            OnDemandResponse(await linxCommerceCustomersService.GetCustomer(userName, docCompany, customerId), $"Customer {customerId}");
    }
}
