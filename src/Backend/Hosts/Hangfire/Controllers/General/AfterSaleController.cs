using Application.AfterSale.Abstractions.Services;
using Application.AfterSale.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.General
{
    [ApiController]
    //[Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("AfterSaleJobs/AfterSale")]
    public class AfterSaleController : Controller
    {
        private readonly IAfterSaleMeService _afterSaleMeService;
        private readonly IAfterSaleReversesService _afterSaleReversesService;
        private readonly IAfterSaleRefundsService _afterSaleRefundsService;

        public AfterSaleController(
            IAfterSaleMeService afterSaleService,
            IAfterSaleReversesService afterSaleReversesService,
            IAfterSaleRefundsService afterSaleRefundsService
        ) =>
        (
            _afterSaleMeService,
            _afterSaleReversesService,
            _afterSaleRefundsService
        )
        =
        (
            afterSaleService,
            afterSaleReversesService,
            afterSaleRefundsService
        );

        [HttpGet("Me")]
        [Tags("AfterSaleEcommerces")]
        public async Task<ActionResult> Me()
        {
            var result = await _afterSaleMeService.GetMe();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Reverses")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult> Reverses()
        {
            var result = await _afterSaleReversesService.GetReverses();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Reverse")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult> ReversesById()
        {
            var result = await _afterSaleReversesService.GetReverse();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Status")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult> ReversesStatus()
        {
            var result = await _afterSaleReversesService.GetStatus();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Transportations")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult> Transportations()
        {
            var result = await _afterSaleReversesService.GetTransportations();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("CourierAttributes")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult> CourierAttributes()
        {
            var result = await _afterSaleReversesService.GetCourierAttributes();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Refunds")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> Refunds()
        {
            var result = await _afterSaleRefundsService.GetRefunds();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Refunds/Status")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> RefundsStatus()
        {
            var result = await _afterSaleRefundsService.GetStatus();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Refunds/Actions")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> RefundsActions()
        {
            var result = await _afterSaleRefundsService.GetActions();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Refunds/Banks")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> RefundsBanks()
        {
            var result = await _afterSaleRefundsService.GetBanks();

            return Ok($"Records integrated successfully.");
        }

        [HttpGet("Refunds/Types")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult> RefundsTypes()
        {
            var result = await _afterSaleRefundsService.GetTypes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// approve products by reverse ID (POST v3/api/reverses/{id}/approve)
        /// </summary>
        [HttpPost("Reverses/{docCompany}/{id:long}/Approve")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Reverses.Approve.Response.Root>> ApproveReverse(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [FromBody] Domain.AfterSale.Dtos.Writes.Reverses.Approve.Request.Root request
        )
        {
            var result = await _afterSaleReversesService.PostApproveReverseById(userName, docCompany, id, request);

            return ToActionResult(result);
        }

        /// <summary>
        /// receive products by reverse ID (PUT v3/api/reverses/{id}/receive-products)
        /// </summary>
        [HttpPut("Reverses/{docCompany}/{id:long}/ReceiveProducts")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Reverses.ReceiveProducts.Response.Root>> ReceiveProducts(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [FromBody] Domain.AfterSale.Dtos.Writes.Reverses.ReceiveProducts.Request.Root request
        )
        {
            var result = await _afterSaleReversesService.PutReciveProductsById(userName, docCompany, id, request);

            return ToActionResult(result);
        }

        /// <summary>
        /// update the returned invoice of reverse products (PUT v3/api/reverses/{id}/products/returned-invoice)
        /// </summary>
        [HttpPut("Reverses/{docCompany}/{id:long}/ReturnedInvoice")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Reverses.ReturnedInvoice.Response.Root>> ReturnedInvoice(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [FromBody] Domain.AfterSale.Dtos.Writes.Reverses.ReturnedInvoice.Request.Root request
        )
        {
            var result = await _afterSaleReversesService.PutReturnedInvoiceById(userName, docCompany, id, request);

            return ToActionResult(result);
        }

        /// <summary>
        /// set reverse courier data with the invoice fields and generate the posting code (POST v3/api/reverses/{id}/invoice, json)
        /// </summary>
        [HttpPost("Reverses/{docCompany}/{id:long}/Invoice")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Response.Root>> ReverseInvoice(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [FromBody] Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Request.Root request
        )
        {
            var result = await _afterSaleReversesService.PostReverseInvoiceById(userName, docCompany, id, request);

            return ToActionResult(result);
        }

        /// <summary>
        /// set reverse courier data with the invoice XML file and generate the posting code (POST v3/api/reverses/{id}/invoice, multipart)
        /// </summary>
        [HttpPost("Reverses/{docCompany}/{id:long}/InvoiceXml")]
        [Tags("AfterSaleReverses")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Response.Root>> ReverseInvoiceXml(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [Required] IFormFile xml
        )
        {
            using var stream = new MemoryStream();
            await xml.CopyToAsync(stream);

            var result = await _afterSaleReversesService.PostReverseInvoiceXmlById(userName, docCompany, id, stream.ToArray(), xml.FileName);

            return ToActionResult(result);
        }

        /// <summary>
        /// create an administrative record (note) in the reverse (POST v3/api/reverses/{id}/administrative-records)
        /// </summary>
        [HttpPost("Reverses/{docCompany}/{id:long}/AdministrativeRecords")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Reverses.AdministrativeRecords.Response.Root>> AdministrativeRecord(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [FromBody] Domain.AfterSale.Dtos.Writes.Reverses.AdministrativeRecords.Request.Root request
        )
        {
            var result = await _afterSaleReversesService.PostAdministrativeRecordById(userName, docCompany, id, request);

            return ToActionResult(result);
        }

        /// <summary>
        /// approve refund by ID (POST v3/api/refunds/{id}/approve)
        /// </summary>
        [HttpPost("Refunds/{docCompany}/{id:long}/Approve")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Refunds.Approve.Response.Root>> ApproveRefund(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id
        )
        {
            var result = await _afterSaleRefundsService.PostApproveRefundById(userName, docCompany, id);

            return ToActionResult(result);
        }

        /// <summary>
        /// cancel refund by ID (POST v3/api/refunds/{id}/cancel)
        /// </summary>
        [HttpPost("Refunds/{docCompany}/{id:long}/Cancel")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Refunds.Cancel.Response.Root>> CancelRefund(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id
        )
        {
            var result = await _afterSaleRefundsService.PostCancelRefundById(userName, docCompany, id);

            return ToActionResult(result);
        }

        /// <summary>
        /// pay refund by ID, with the payment attachment (POST v3/api/refunds/{id}/pay, multipart). the file is required here.
        /// </summary>
        [HttpPost("Refunds/{docCompany}/{id:long}/Pay")]
        [Tags("AfterSaleRefunds")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Refunds.Pay.Response.Root>> PayRefund(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [FromForm] Domain.AfterSale.Dtos.Writes.Refunds.Pay.Request.Root request,
            [Required] IFormFile file
        )
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            var result = await _afterSaleRefundsService.PostPayRefundById(userName, docCompany, id, request, stream.ToArray(), file.FileName, file.ContentType);

            return ToActionResult(result);
        }

        /// <summary>
        /// change total amount of refund by ID (POST v3/api/refunds/{id}/change-total-amount)
        /// </summary>
        [HttpPost("Refunds/{docCompany}/{id:long}/ChangeTotalAmount")]
        [Tags("AfterSaleRefunds")]
        public async Task<ActionResult<Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Response.Root>> ChangeTotalAmountRefund(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [FromBody] Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Request.Root request
        )
        {
            var result = await _afterSaleRefundsService.PostChangeTotalAmountRefundById(userName, docCompany, id, request);

            return ToActionResult(result);
        }

        private ActionResult<TResponse> ToActionResult<TResponse>(TResponse result) where TResponse : Domain.AfterSale.Dtos.Writes.Base.ResponseRoot =>
            result.success ? Ok(result) : UnprocessableEntity(result);
    }
}
