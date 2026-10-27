using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixFiscalDocumentsService: documentos fiscais. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxXMLDocumentos: o XML dos documentos.
        /// </summary>
        [HttpPost("LinxXMLDocumentos")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxXMLDocumentos([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxXMLDocumentos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxNFeEvento: os dados eventos da NFe.
        /// </summary>
        [HttpPost("LinxNFeEvento")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxNFeEvento([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxNFeEvento();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxNfse: nota Fiscal de Serviços Eletrônica
        /// </summary>
        [HttpPost("LinxNfse")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxNfse([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxNfse();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxEntradaNFe: dados de NFes de entrada.
        /// </summary>
        [HttpPost("LinxEntradaNFe")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxEntradaNFe([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxEntradaNFe();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxEntradaNFeProdutos: dados de itens de NFes de entrada.
        /// </summary>
        [HttpPost("LinxEntradaNFeProdutos")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxEntradaNFeProdutos([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxEntradaNFeProdutos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxConhecimentoFretes: dados de conhecimento de fretes.
        /// </summary>
        [HttpPost("LinxConhecimentoFretes")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxConhecimentoFretes([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxConhecimentoFretes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxFreteTipos: tipos de frete.
        /// </summary>
        [HttpPost("LinxFreteTipos")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxFreteTipos([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxFreteTipos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxReducoesZ: as ReduçõesZ de acordo com a loja pesquisada, somente para clientes que ainda utilizam a impressora fiscal
        /// </summary>
        [HttpPost("LinxReducoesZ")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxReducoesZ([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxReducoesZ();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxECF: as ECFs cadastradas para a empresa.
        /// </summary>
        [HttpPost("LinxECF")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxECF([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxECF();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxECFFormasPagamento: as formas de pagamento da ECF.
        /// </summary>
        [HttpPost("LinxECFFormasPagamento")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxECFFormasPagamento([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxECFFormasPagamento();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxNfceEstacao: cadastro de estações.
        /// </summary>
        [HttpPost("LinxNfceEstacao")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxNfceEstacao([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxNfceEstacao();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxSeries: as séries cadastradas no portal.
        /// </summary>
        [HttpPost("LinxSeries")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxSeries([FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService)
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxSeries();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxXMLDocumentos por documento e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxXMLDocumentos/{docCompany}/{cnpjEmp}/{documento}")]
        [Tags("LinxMicrovixFiscalDocuments")]
        public async Task<ActionResult> LinxXMLDocumentosByDocumento(
            [FromServices] ILinxMicrovixFiscalDocumentsService linxMicrovixFiscalDocumentsService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string documento
        )
        {
            var result = await linxMicrovixFiscalDocumentsService.LinxXMLDocumentosByDocumento(userName, docCompany, cnpjEmp, documento);

            return result switch
            {
                true => Ok($"LinxXMLDocumentos {documento} integrated successfully."),
                false => NotFound($"LinxXMLDocumentos {documento} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
