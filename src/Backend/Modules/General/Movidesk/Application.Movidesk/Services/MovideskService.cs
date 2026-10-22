using Application.Movidesk.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Enums;

namespace Application.Movidesk.Services
{
    /// <summary>
    /// métodos em comum usados pelos services do módulo. O ILoggerService é Scoped: dentro de uma requisição
    /// é a mesma instância do service que chamou, então o GetExecutionGuid() é a execução aberta por ele.
    /// </summary>
    public class MovideskService : IMovideskService
    {
        private readonly ILoggerService _logger;
        private readonly IMovideskRepository _movideskRepository;

        public MovideskService(ILoggerService logger, IMovideskRepository movideskRepository) =>
            (_logger, _movideskRepository) = (logger, movideskRepository);

        public DateTime GetBatchTime() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        public void SaveRequest(EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess, string? requestBody = null) =>
            _movideskRepository.InsertIntoMovideskRequests(
                new Domain.Movidesk.Entities.Writes.Requests.Request(
                    execution: _logger.GetExecutionGuid() ?? Guid.Empty,
                    job: job,
                    user_name: user_name,
                    http_method: httpMethod,
                    route: route,
                    doc_company: doc_company,
                    field_key_value: field_key_value,
                    request_body: requestBody,
                    response_body: responseBody,
                    is_success: isSuccess
                )
            );
    }
}
