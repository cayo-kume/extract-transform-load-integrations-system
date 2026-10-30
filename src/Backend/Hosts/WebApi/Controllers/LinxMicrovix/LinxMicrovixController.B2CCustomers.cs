using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixB2CCustomersService: clientes do b2c. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza B2CConsultaClientes
        /// </summary>
        [HttpPost("B2CConsultaClientes")]
        [Tags("LinxMicrovixB2CCustomers")]
        public async Task<ActionResult> B2CConsultaClientes([FromServices] ILinxMicrovixB2CCustomersService linxMicrovixB2CCustomersService)
        {
            var result = await linxMicrovixB2CCustomersService.B2CConsultaClientes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaClientesContatos
        /// </summary>
        [HttpPost("B2CConsultaClientesContatos")]
        [Tags("LinxMicrovixB2CCustomers")]
        public async Task<ActionResult> B2CConsultaClientesContatos([FromServices] ILinxMicrovixB2CCustomersService linxMicrovixB2CCustomersService)
        {
            var result = await linxMicrovixB2CCustomersService.B2CConsultaClientesContatos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaClientesContatosParentesco
        /// </summary>
        [HttpPost("B2CConsultaClientesContatosParentesco")]
        [Tags("LinxMicrovixB2CCustomers")]
        public async Task<ActionResult> B2CConsultaClientesContatosParentesco([FromServices] ILinxMicrovixB2CCustomersService linxMicrovixB2CCustomersService)
        {
            var result = await linxMicrovixB2CCustomersService.B2CConsultaClientesContatosParentesco();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaClientesEnderecosEntrega
        /// </summary>
        [HttpPost("B2CConsultaClientesEnderecosEntrega")]
        [Tags("LinxMicrovixB2CCustomers")]
        public async Task<ActionResult> B2CConsultaClientesEnderecosEntrega([FromServices] ILinxMicrovixB2CCustomersService linxMicrovixB2CCustomersService)
        {
            var result = await linxMicrovixB2CCustomersService.B2CConsultaClientesEnderecosEntrega();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaClientesEstadoCivil
        /// </summary>
        [HttpPost("B2CConsultaClientesEstadoCivil")]
        [Tags("LinxMicrovixB2CCustomers")]
        public async Task<ActionResult> B2CConsultaClientesEstadoCivil([FromServices] ILinxMicrovixB2CCustomersService linxMicrovixB2CCustomersService)
        {
            var result = await linxMicrovixB2CCustomersService.B2CConsultaClientesEstadoCivil();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaClientesSaldo
        /// </summary>
        [HttpPost("B2CConsultaClientesSaldo")]
        [Tags("LinxMicrovixB2CCustomers")]
        public async Task<ActionResult> B2CConsultaClientesSaldo([FromServices] ILinxMicrovixB2CCustomersService linxMicrovixB2CCustomersService)
        {
            var result = await linxMicrovixB2CCustomersService.B2CConsultaClientesSaldo();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaClientesSaldoLinx
        /// </summary>
        [HttpPost("B2CConsultaClientesSaldoLinx")]
        [Tags("LinxMicrovixB2CCustomers")]
        public async Task<ActionResult> B2CConsultaClientesSaldoLinx([FromServices] ILinxMicrovixB2CCustomersService linxMicrovixB2CCustomersService)
        {
            var result = await linxMicrovixB2CCustomersService.B2CConsultaClientesSaldoLinx();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca B2CConsultaClientes por docCliente e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("B2CConsultaClientes/{docCompany}/{docCliente}")]
        [Tags("LinxMicrovixB2CCustomers")]
        public async Task<ActionResult> B2CConsultaClientesByDocCliente(
            [FromServices] ILinxMicrovixB2CCustomersService linxMicrovixB2CCustomersService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string docCliente
        )
        {
            var result = await linxMicrovixB2CCustomersService.B2CConsultaClientesByDocCliente(userName, docCompany, docCliente);

            return result switch
            {
                true => Ok($"B2CConsultaClientes {docCliente} integrated successfully."),
                false => NotFound($"B2CConsultaClientes {docCliente} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
