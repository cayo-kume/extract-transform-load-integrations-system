using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixCustomersService: clientes e fornecedores. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxClientesFornec: os Clientes/Fornecedores cadastrados na loja e data pesquisada
        /// </summary>
        [HttpPost("LinxClientesFornec")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxClientesFornec([FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService)
        {
            var result = await linxMicrovixCustomersService.LinxClientesFornec();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxClientesFornecCamposAdicionais: os campos adicionais preenchidos dos Clientes/Fornecedores cadastrados no portal
        /// </summary>
        [HttpPost("LinxClientesFornecCamposAdicionais")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxClientesFornecCamposAdicionais([FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService)
        {
            var result = await linxMicrovixCustomersService.LinxClientesFornecCamposAdicionais();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxClientesFornecClasses: todas as classes vinculadas a um cliente/fornecedor
        /// </summary>
        [HttpPost("LinxClientesFornecClasses")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxClientesFornecClasses([FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService)
        {
            var result = await linxMicrovixCustomersService.LinxClientesFornecClasses();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxClientesFornecContatos: os campos referentes ao contato do cadastro dos Clientes/Fornecedores cadastrados no portal
        /// </summary>
        [HttpPost("LinxClientesFornecContatos")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxClientesFornecContatos([FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService)
        {
            var result = await linxMicrovixCustomersService.LinxClientesFornecContatos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxClientesFornecContatosParentesco: os parentescos cadastrados
        /// </summary>
        [HttpPost("LinxClientesFornecContatosParentesco")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxClientesFornecContatosParentesco([FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService)
        {
            var result = await linxMicrovixCustomersService.LinxClientesFornecContatosParentesco();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxClientesFornecCreditoAvulso: os dados de crédito avulso dos clientes e fornecedores.
        /// </summary>
        [HttpPost("LinxClientesFornecCreditoAvulso")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxClientesFornecCreditoAvulso([FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService)
        {
            var result = await linxMicrovixCustomersService.LinxClientesFornecCreditoAvulso();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxClientesFornecVendedores: o código do vendedor associado ao cadastro do cliente, conforme a tabela clientes_fornec_vendedores.
        /// </summary>
        [HttpPost("LinxClientesFornecVendedores")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxClientesFornecVendedores([FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService)
        {
            var result = await linxMicrovixCustomersService.LinxClientesFornecVendedores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxClientesEnderecosEntrega: endereço de entrega do cliente.
        /// </summary>
        [HttpPost("LinxClientesEnderecosEntrega")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxClientesEnderecosEntrega([FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService)
        {
            var result = await linxMicrovixCustomersService.LinxClientesEnderecosEntrega();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxClientesRede: os registros da tabela clientes_rede no portal centralizador.
        /// </summary>
        [HttpPost("LinxClientesRede")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxClientesRede([FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService)
        {
            var result = await linxMicrovixCustomersService.LinxClientesRede();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxRamosAtividade: os ramos de atividades cadastrados.
        /// </summary>
        [HttpPost("LinxRamosAtividade")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxRamosAtividade([FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService)
        {
            var result = await linxMicrovixCustomersService.LinxRamosAtividade();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxClientesFornec por docCliente e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxClientesFornec/{docCompany}/{docCliente}")]
        [Tags("LinxMicrovixCustomers")]
        public async Task<ActionResult> LinxClientesFornecByDocCliente(
            [FromServices] ILinxMicrovixCustomersService linxMicrovixCustomersService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string docCliente
        )
        {
            var result = await linxMicrovixCustomersService.LinxClientesFornecByDocCliente(userName, docCompany, docCliente);

            return result switch
            {
                true => Ok($"LinxClientesFornec {docCliente} integrated successfully."),
                false => NotFound($"LinxClientesFornec {docCliente} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
