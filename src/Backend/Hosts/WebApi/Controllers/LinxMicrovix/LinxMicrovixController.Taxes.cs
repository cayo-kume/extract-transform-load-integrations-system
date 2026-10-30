using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixTaxesService: tabelas fiscais. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxNaturezaOperacao: as naturezas de operação.
        /// </summary>
        [HttpPost("LinxNaturezaOperacao")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxNaturezaOperacao([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxNaturezaOperacao();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCfopFiscal: o cadastro da CFOP Fiscal
        /// </summary>
        [HttpPost("LinxCfopFiscal")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxCfopFiscal([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxCfopFiscal();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCstIcmsFiscal: o cadastro de cst icms fiscal
        /// </summary>
        [HttpPost("LinxCstIcmsFiscal")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxCstIcmsFiscal([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxCstIcmsFiscal();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCstIpiFiscal: o conteúdo da tabela cst_ipi_fiscal
        /// </summary>
        [HttpPost("LinxCstIpiFiscal")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxCstIpiFiscal([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxCstIpiFiscal();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCstPisFiscal: o conteúdo da tabela cst_pis_fiscal
        /// </summary>
        [HttpPost("LinxCstPisFiscal")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxCstPisFiscal([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxCstPisFiscal();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCstCofinsFiscal: o conteúdo da tabela cst_cofins_fiscal
        /// </summary>
        [HttpPost("LinxCstCofinsFiscal")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxCstCofinsFiscal([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxCstCofinsFiscal();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCsosnFiscal: o cadastro de csosn fiscal
        /// </summary>
        [HttpPost("LinxCsosnFiscal")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxCsosnFiscal([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxCsosnFiscal();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxClasseFiscal: o cadastro da classe fiscal.
        /// </summary>
        [HttpPost("LinxClasseFiscal")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxClasseFiscal([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxClasseFiscal();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCest: o conteúdo da tabela classificacao_cest_produto
        /// </summary>
        [HttpPost("LinxCest")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxCest([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxCest();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxNcm: o conteúdo da tabela classificacao_ncm_produto.
        /// </summary>
        [HttpPost("LinxNcm")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxNcm([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxNcm();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxEnquadramentoIpi: o conteúdo da tabela enquadramento_ipi
        /// </summary>
        [HttpPost("LinxEnquadramentoIpi")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxEnquadramentoIpi([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxEnquadramentoIpi();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMotivosDesoneracaoIcms: o conteúdo da tabela motivos_desoneracao_icms
        /// </summary>
        [HttpPost("LinxMotivosDesoneracaoIcms")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxMotivosDesoneracaoIcms([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxMotivosDesoneracaoIcms();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxSpedTipoItem: dados do Sped tipo item.
        /// </summary>
        [HttpPost("LinxSpedTipoItem")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxSpedTipoItem([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxSpedTipoItem();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxSpedTipoBaseCredito: o conteúdo da tabela sped_tipo_base_credito.
        /// </summary>
        [HttpPost("LinxSpedTipoBaseCredito")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxSpedTipoBaseCredito([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxSpedTipoBaseCredito();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxRotinaOrigem: retornas as rotinas origem cadastradas.
        /// </summary>
        [HttpPost("LinxRotinaOrigem")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxRotinaOrigem([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxRotinaOrigem();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxNfeIbgeMunicipio: dados do IBGE sobre municípios.
        /// </summary>
        [HttpPost("LinxNfeIbgeMunicipio")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxNfeIbgeMunicipio([FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService)
        {
            var result = await linxMicrovixTaxesService.LinxNfeIbgeMunicipio();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxNaturezaOperacao por codNaturezaOperacao e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxNaturezaOperacao/{docCompany}/{codNaturezaOperacao}")]
        [Tags("LinxMicrovixTaxes")]
        public async Task<ActionResult> LinxNaturezaOperacaoByCodNaturezaOperacao(
            [FromServices] ILinxMicrovixTaxesService linxMicrovixTaxesService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string codNaturezaOperacao
        )
        {
            var result = await linxMicrovixTaxesService.LinxNaturezaOperacaoByCodNaturezaOperacao(userName, docCompany, codNaturezaOperacao);

            return result switch
            {
                true => Ok($"LinxNaturezaOperacao {codNaturezaOperacao} integrated successfully."),
                false => NotFound($"LinxNaturezaOperacao {codNaturezaOperacao} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
