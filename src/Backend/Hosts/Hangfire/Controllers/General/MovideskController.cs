using Application.Movidesk.Abstractions.Services;
using Application.Movidesk.Validations.Writes.People;
using Application.Movidesk.Validations.Writes.Services;
using Application.Movidesk.Validations.Writes.Tickets;
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
    [Route("MovideskJobs/Movidesk")]
    public class MovideskController : Controller
    {
        private readonly IMovideskTicketsService _movideskTicketsService;
        private readonly IMovideskPeopleService _movideskPeopleService;
        private readonly IMovideskServicesService _movideskServicesService;
        private readonly PostTicketValidator _postTicketValidator;
        private readonly PatchTicketValidator _patchTicketValidator;
        private readonly PostPersonValidator _postPersonValidator;
        private readonly PatchPersonValidator _patchPersonValidator;
        private readonly RelationshipsValidator _relationshipsValidator;
        private readonly PostServiceValidator _postServiceValidator;
        private readonly PatchServiceValidator _patchServiceValidator;

        // limite da Movidesk por anexo (dúvidas frequentes sobre tickets); a requisição aceita vários anexos de uma vez
        private const long MaxAttachmentBytes = 25L * 1024 * 1024;
        private const long MaxUploadRequestBytes = 4 * MaxAttachmentBytes;

        public MovideskController(
            IMovideskTicketsService movideskTicketsService,
            IMovideskPeopleService movideskPeopleService,
            IMovideskServicesService movideskServicesService,
            PostTicketValidator postTicketValidator,
            PatchTicketValidator patchTicketValidator,
            PostPersonValidator postPersonValidator,
            PatchPersonValidator patchPersonValidator,
            RelationshipsValidator relationshipsValidator,
            PostServiceValidator postServiceValidator,
            PatchServiceValidator patchServiceValidator
        ) =>
            (_movideskTicketsService, _movideskPeopleService, _movideskServicesService, _postTicketValidator, _patchTicketValidator, _postPersonValidator, _patchPersonValidator, _relationshipsValidator, _postServiceValidator, _patchServiceValidator) =
            (movideskTicketsService, movideskPeopleService, movideskServicesService, postTicketValidator, patchTicketValidator, postPersonValidator, patchPersonValidator, relationshipsValidator, postServiceValidator, patchServiceValidator);

        [HttpGet("Tickets")]
        [Tags("MovideskTickets")]
        public async Task<ActionResult> Tickets()
        {
            var result = await _movideskTicketsService.GetTickets();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// get one ticket by number (GET tickets?id={id}) and save it; the call is logged in movidesk.MovideskRequests
        /// </summary>
        [HttpGet("Tickets/{docCompany}/{id:long:min(1)}")]
        [Tags("MovideskTickets")]
        public async Task<ActionResult> Ticket(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id
        )
        {
            var result = await _movideskTicketsService.GetTicketById(userName, docCompany, id);

            return result switch
            {
                true => Ok($"Ticket {id} integrated successfully."),
                false => NotFound($"Ticket {id} not found in Movidesk."),
                null => UnprocessableEntity("Company not registered or invalid response from Movidesk. See the execution log.")
            };
        }

        /// <summary>
        /// cria um ticket (POST tickets). O corpo é validado antes de chamar a API: 3 erros seguidos bloqueiam a API de tickets por 60 s
        /// </summary>
        [HttpPost("Tickets/{docCompany}")]
        [Tags("MovideskTickets")]
        public async Task<ActionResult> PostTicket(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Movidesk.Dtos.Writes.Tickets.Ticket ticket
        )
        {
            var validation = await _postTicketValidator.ValidateAsync(ticket);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _movideskTicketsService.PostTicket(userName, docCompany, ticket));
        }

        /// <summary>
        /// altera um ticket (PATCH tickets?id={id}): só os campos enviados mudam, e listas enviadas (tags, clientes,
        /// campos adicionais, ativos, tickets pai/filho) substituem a lista inteira na Movidesk
        /// </summary>
        [HttpPatch("Tickets/{docCompany}/{id:long:min(1)}")]
        [Tags("MovideskTickets")]
        public async Task<ActionResult> PatchTicket(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            [FromBody] Domain.Movidesk.Dtos.Writes.Tickets.Ticket ticket
        )
        {
            var validation = await _patchTicketValidator.ValidateAsync(ticket);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _movideskTicketsService.PatchTicketById(userName, docCompany, id, ticket));
        }

        /// <summary>
        /// envia anexos para uma ação do ticket (POST ticketFileUpload), todos numa chamada; até 25 MB por anexo
        /// </summary>
        [HttpPost("Tickets/{docCompany}/{id:long:min(1)}/Actions/{actionId:int:min(1)}/Files")]
        [Tags("MovideskTickets")]
        [RequestSizeLimit(MaxUploadRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadRequestBytes)]
        public async Task<ActionResult> PostTicketFiles(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id,
            int actionId,
            [Required] List<IFormFile> files
        )
        {
            if (files.Count == 0)
                return BadRequest(new[] { new { field = "files", message = "Envie ao menos um arquivo." } });

            var invalid = files
                .Where(f => f.Length == 0 || f.Length > MaxAttachmentBytes)
                .Select(f => new { field = f.FileName, message = f.Length == 0 ? "Arquivo vazio." : "Arquivo acima de 25 MB, o limite da Movidesk por anexo." })
                .ToList();

            if (invalid.Count > 0)
                return BadRequest(invalid);

            var contents = new List<(string fileName, string contentType, byte[] content)>();

            foreach (var file in files)
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                contents.Add((file.FileName, file.ContentType, stream.ToArray()));
            }

            return WriteResponse(await _movideskTicketsService.PostTicketFileUploadById(userName, docCompany, id, actionId, contents));
        }

        /// <summary>
        /// 200 na escrita aceita; 429 (com Retry-After) quando a Movidesk bloqueou por excesso de erros;
        /// 422 quando a empresa não está cadastrada ou a Movidesk recusou
        /// </summary>
        private ActionResult WriteResponse(Domain.Movidesk.Dtos.Writes.Result result)
        {
            if (result.success)
                return Ok(result);

            if (result.statusCode == StatusCodes.Status429TooManyRequests)
            {
                if (result.retryAfterSeconds is int seconds)
                    Response.Headers.RetryAfter = seconds.ToString();

                return StatusCode(StatusCodes.Status429TooManyRequests, result);
            }

            return UnprocessableEntity(result);
        }

        [HttpGet("People")]
        [Tags("MovideskPeople")]
        public async Task<ActionResult> People()
        {
            var result = await _movideskPeopleService.GetPeople();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// get one person by Cód. Ref. (GET persons?id={id}) and save it; the call is logged in movidesk.MovideskRequests
        /// </summary>
        [HttpGet("People/{docCompany}/{id}")]
        [Tags("MovideskPeople")]
        public async Task<ActionResult> Person(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(64, MinimumLength = 1), RegularExpression(@"^[A-Za-z0-9]+$")] string id
        )
        {
            var result = await _movideskPeopleService.GetPersonById(userName, docCompany, id);

            return result switch
            {
                true => Ok($"Person {id} integrated successfully."),
                false => NotFound($"Person {id} not found in Movidesk."),
                null => UnprocessableEntity("Company not registered or invalid response from Movidesk. See the execution log.")
            };
        }

        /// <summary>
        /// cria uma pessoa (POST persons). O corpo é validado antes de chamar a API; senha não é aceita pela integração
        /// </summary>
        [HttpPost("People/{docCompany}")]
        [Tags("MovideskPeople")]
        public async Task<ActionResult> PostPerson(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Movidesk.Dtos.Writes.People.Person person
        )
        {
            var validation = await _postPersonValidator.ValidateAsync(person);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _movideskPeopleService.PostPerson(userName, docCompany, person));
        }

        /// <summary>
        /// altera uma pessoa (PATCH persons?id={id}): só os campos enviados mudam, e listas enviadas (endereços, contatos,
        /// e-mails, equipes, relacionamentos, campos adicionais, ativos) substituem a lista inteira na Movidesk
        /// </summary>
        [HttpPatch("People/{docCompany}/{id}")]
        [Tags("MovideskPeople")]
        public async Task<ActionResult> PatchPerson(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(64, MinimumLength = 1), RegularExpression(@"^[A-Za-z0-9]+$")] string id,
            [FromBody] Domain.Movidesk.Dtos.Writes.People.Person person
        )
        {
            var validation = await _patchPersonValidator.ValidateAsync(person);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _movideskPeopleService.PatchPersonById(userName, docCompany, id, person));
        }

        /// <summary>
        /// inclui organizações numa pessoa (POST persons/relationships) sem mexer nas que ela já tem
        /// </summary>
        [HttpPost("People/{docCompany}/{id}/Relationships")]
        [Tags("MovideskPeople")]
        public async Task<ActionResult> PostPersonRelationships(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(64, MinimumLength = 1), RegularExpression(@"^[A-Za-z0-9]+$")] string id,
            [FromBody] Domain.Movidesk.Dtos.Writes.People.Relationships relationships
        )
        {
            var validation = await _relationshipsValidator.ValidateAsync(relationships);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _movideskPeopleService.PostPersonRelationshipsById(userName, docCompany, id, relationships));
        }

        /// <summary>
        /// exclui uma pessoa (DELETE persons?id={id}). Irreversível; com sucesso, a pessoa e as filhas saem também do banco local
        /// </summary>
        [HttpDelete("People/{docCompany}/{id}")]
        [Tags("MovideskPeople")]
        public async Task<ActionResult> DeletePerson(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(64, MinimumLength = 1), RegularExpression(@"^[A-Za-z0-9]+$")] string id
        ) =>
            WriteResponse(await _movideskPeopleService.DeletePersonById(userName, docCompany, id));

        [HttpGet("Services")]
        [Tags("MovideskServices")]
        public async Task<ActionResult> Services()
        {
            var result = await _movideskServicesService.GetServices();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// get one service by id (GET services?id={id}) and save it; the call is logged in movidesk.MovideskRequests
        /// </summary>
        [HttpGet("Services/{docCompany}/{id:int:min(1)}")]
        [Tags("MovideskServices")]
        public async Task<ActionResult> Service(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int id
        )
        {
            var result = await _movideskServicesService.GetServiceById(userName, docCompany, id);

            return result switch
            {
                true => Ok($"Service {id} integrated successfully."),
                false => NotFound($"Service {id} not found in Movidesk."),
                null => UnprocessableEntity("Company not registered or invalid response from Movidesk. See the execution log.")
            };
        }

        /// <summary>
        /// cria um serviço (POST services). O corpo é validado antes de chamar a API; o id é gerado pela Movidesk
        /// </summary>
        [HttpPost("Services/{docCompany}")]
        [Tags("MovideskServices")]
        public async Task<ActionResult> PostService(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Movidesk.Dtos.Writes.Services.Service service
        )
        {
            var validation = await _postServiceValidator.ValidateAsync(service);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _movideskServicesService.PostService(userName, docCompany, service));
        }

        /// <summary>
        /// altera um serviço (PATCH services?id={id}): só os campos enviados mudam, e categories substitui a lista inteira
        /// </summary>
        [HttpPatch("Services/{docCompany}/{id:int:min(1)}")]
        [Tags("MovideskServices")]
        public async Task<ActionResult> PatchService(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int id,
            [FromBody] Domain.Movidesk.Dtos.Writes.Services.Service service
        )
        {
            var validation = await _patchServiceValidator.ValidateAsync(service);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _movideskServicesService.PatchServiceById(userName, docCompany, id, service));
        }

        /// <summary>
        /// exclui um serviço do catálogo (DELETE services?id={id}). Irreversível; o serviço continua no banco local
        /// </summary>
        [HttpDelete("Services/{docCompany}/{id:int:min(1)}")]
        [Tags("MovideskServices")]
        public async Task<ActionResult> DeleteService(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int id
        ) =>
            WriteResponse(await _movideskServicesService.DeleteServiceById(userName, docCompany, id));
    }
}
