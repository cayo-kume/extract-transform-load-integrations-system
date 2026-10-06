using Application.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;
using Application.Shared.Abstractions.Handlers.Exceptions;

namespace Application.Shared.Handlers.Exceptions
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly ILoggerService _logger;
        private readonly bool _isDevelopment;

        public ExceptionHandler(ILoggerService logger, IHostEnvironment environment)
        {
            _logger = logger;
            _isDevelopment = environment.IsDevelopment();
        }

        // Este é o "catch-all" handler, então ele sempre pode lidar, mas deve ser o último a ser verificado.
        public bool CanHandle(Exception exception) => true;

        public async Task HandleAsync(HttpContext context, Exception exception)
        {
            _logger.AddMessage(
                message: exception.Message,
                sqlCommand: exception.Message.Contains("|") ? exception.Message.Split('|')[1] : exception.Message,
                exceptionMessage: exception.InnerException.Message,
                stackTraceMessage: exception.InnerException.StackTrace
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            object errorDetails = _isDevelopment
                ? new
                {
                    context.Response.StatusCode,
                    Title = "Unable to integrate the records.",
                    ErrorCode = "UNEXPECTED_ERROR",
                    exception.Message,
                    ExceptionMessage = exception.StackTrace
                }
                : new
                {
                    context.Response.StatusCode,
                    Title = "Unable to integrate the records.",
                    ErrorCode = "UNEXPECTED_ERROR"
                };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails));
        }
    }
}
