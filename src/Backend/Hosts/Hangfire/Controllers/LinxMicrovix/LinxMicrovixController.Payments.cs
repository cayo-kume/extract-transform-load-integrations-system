using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixPaymentsService: pagamentos e caixa. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxMovimentoCartoes: informações dos cartões de acordo com a loja pesquisada
        /// </summary>
        [HttpPost("LinxMovimentoCartoes")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxMovimentoCartoes([FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService)
        {
            var result = await linxMicrovixPaymentsService.LinxMovimentoCartoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoPlanos: os Planos de Pagamentos realizados nas movimentações
        /// </summary>
        [HttpPost("LinxMovimentoPlanos")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxMovimentoPlanos([FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService)
        {
            var result = await linxMicrovixPaymentsService.LinxMovimentoPlanos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPlanos: o cadastro dos Planos de Pagamentos do portal
        /// </summary>
        [HttpPost("LinxPlanos")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxPlanos([FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService)
        {
            var result = await linxMicrovixPaymentsService.LinxPlanos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPlanosParcelas
        /// </summary>
        [HttpPost("LinxPlanosParcelas")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxPlanosParcelas([FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService)
        {
            var result = await linxMicrovixPaymentsService.LinxPlanosParcelas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPlanosBandeiras: o cadastro de bandeiras de cartões vinculadas aos Planos de Pagamento do Portal
        /// </summary>
        [HttpPost("LinxPlanosBandeiras")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxPlanosBandeiras([FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService)
        {
            var result = await linxMicrovixPaymentsService.LinxPlanosBandeiras();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoGiftCard: os registro de giftcard
        /// </summary>
        [HttpPost("LinxMovimentoGiftCard")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxMovimentoGiftCard([FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService)
        {
            var result = await linxMicrovixPaymentsService.LinxMovimentoGiftCard();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxValesComprasEnviadosAPI: vale Compras importado via API.
        /// </summary>
        [HttpPost("LinxValesComprasEnviadosAPI")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxValesComprasEnviadosAPI([FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService)
        {
            var result = await linxMicrovixPaymentsService.LinxValesComprasEnviadosAPI();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxSangriaSuprimentos: as informações das Sangrias ou Suprimentos feitos no caixa
        /// </summary>
        [HttpPost("LinxSangriaSuprimentos")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxSangriaSuprimentos([FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService)
        {
            var result = await linxMicrovixPaymentsService.LinxSangriaSuprimentos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxFechamentoCaixa
        /// </summary>
        [HttpPost("LinxFechamentoCaixa")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxFechamentoCaixa([FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService)
        {
            var result = await linxMicrovixPaymentsService.LinxFechamentoCaixa();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxMovimentoCartoes por identificador e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxMovimentoCartoes/{docCompany}/{cnpjEmp}/{identificador}")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxMovimentoCartoesByIdentificador(
            [FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string identificador
        )
        {
            var result = await linxMicrovixPaymentsService.LinxMovimentoCartoesByIdentificador(userName, docCompany, cnpjEmp, identificador);

            return result switch
            {
                true => Ok($"LinxMovimentoCartoes {identificador} integrated successfully."),
                false => NotFound($"LinxMovimentoCartoes {identificador} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }

        /// <summary>
        /// sob demanda: busca LinxMovimentoPlanos por identificador e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxMovimentoPlanos/{docCompany}/{cnpjEmp}/{identificador}")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxMovimentoPlanosByIdentificador(
            [FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string identificador
        )
        {
            var result = await linxMicrovixPaymentsService.LinxMovimentoPlanosByIdentificador(userName, docCompany, cnpjEmp, identificador);

            return result switch
            {
                true => Ok($"LinxMovimentoPlanos {identificador} integrated successfully."),
                false => NotFound($"LinxMovimentoPlanos {identificador} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }

        /// <summary>
        /// sob demanda: busca LinxPlanos por plano e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxPlanos/{docCompany}/{plano}")]
        [Tags("LinxMicrovixPayments")]
        public async Task<ActionResult> LinxPlanosByPlano(
            [FromServices] ILinxMicrovixPaymentsService linxMicrovixPaymentsService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string plano
        )
        {
            var result = await linxMicrovixPaymentsService.LinxPlanosByPlano(userName, docCompany, plano);

            return result switch
            {
                true => Ok($"LinxPlanos {plano} integrated successfully."),
                false => NotFound($"LinxPlanos {plano} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
