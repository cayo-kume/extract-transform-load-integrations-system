using Application.LinxCommerce.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxCommerce
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
    }
}
