using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixMovementsService: movimento. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxMovimento: as movimentações de entradas e saídas de acordo com a loja e data pesquisada
        /// </summary>
        [HttpPost("LinxMovimento")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxMovimento([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxMovimento();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoPrincipal: dados da movimento principal.
        /// </summary>
        [HttpPost("LinxMovimentoPrincipal")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxMovimentoPrincipal([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxMovimentoPrincipal();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoExtensao: as informações adicionais da venda
        /// </summary>
        [HttpPost("LinxMovimentoExtensao")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxMovimentoExtensao([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxMovimentoExtensao();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoCamposAdicionais: os campos adicionais da movimentação
        /// </summary>
        [HttpPost("LinxMovimentoCamposAdicionais")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxMovimentoCamposAdicionais([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxMovimentoCamposAdicionais();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoObservacoes: o conteúdo da tabela movimento_observacoes
        /// </summary>
        [HttpPost("LinxMovimentoObservacoes")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxMovimentoObservacoes([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxMovimentoObservacoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoObservacaoCF: as observações do cupom fiscal.
        /// </summary>
        [HttpPost("LinxMovimentoObservacaoCF")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxMovimentoObservacaoCF([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxMovimentoObservacaoCF();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoIndicacoes: o cliente indicador da venda.
        /// </summary>
        [HttpPost("LinxMovimentoIndicacoes")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxMovimentoIndicacoes([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxMovimentoIndicacoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoSerial: os Código de Seriais realizadas nas movimentações
        /// </summary>
        [HttpPost("LinxMovimentoSerial")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxMovimentoSerial([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxMovimentoSerial();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxSerialVenda: os registros da tabela serial_venda
        /// </summary>
        [HttpPost("LinxSerialVenda")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxSerialVenda([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxSerialVenda();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoVendaConjugada: movimento da venda conjugada
        /// </summary>
        [HttpPost("LinxMovimentoVendaConjugada")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxMovimentoVendaConjugada([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxMovimentoVendaConjugada();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPerguntaVenda: as perguntas realizadas no final da venda cadastradas no portal.
        /// </summary>
        [HttpPost("LinxPerguntaVenda")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxPerguntaVenda([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxPerguntaVenda();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxRespostaVenda: o cadastro das perguntas realizadas no final da venda no portal.
        /// </summary>
        [HttpPost("LinxRespostaVenda")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxRespostaVenda([FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService)
        {
            var result = await linxMicrovixMovementsService.LinxRespostaVenda();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxMovimento por documento e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxMovimento/{docCompany}/{cnpjEmp}/{documento}")]
        [Tags("LinxMicrovixMovements")]
        public async Task<ActionResult> LinxMovimentoByDocumento(
            [FromServices] ILinxMicrovixMovementsService linxMicrovixMovementsService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string documento
        )
        {
            var result = await linxMicrovixMovementsService.LinxMovimentoByDocumento(userName, docCompany, cnpjEmp, documento);

            return result switch
            {
                true => Ok($"LinxMovimento {documento} integrated successfully."),
                false => NotFound($"LinxMovimento {documento} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
