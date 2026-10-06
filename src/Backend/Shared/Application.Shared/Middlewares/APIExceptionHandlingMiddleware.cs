using Application.Shared.Handlers.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using Application.Shared.Abstractions.Handlers.Exceptions;

namespace Application.Shared.Middlewares
{
    public class APIExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public APIExceptionHandlingMiddleware(RequestDelegate next) =>
            _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var exceptionHandlers = context.RequestServices.GetServices<IExceptionHandler>();

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var handler = exceptionHandlers
                    .OrderBy(h => h.GetType() == typeof(ExceptionHandler) ? 1 : 0)
                    .FirstOrDefault(h => h.CanHandle(ex));

                if (handler != null)
                {
                    await handler.HandleAsync(context, ex);
                }
                else
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var errorDetails = new
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Title = "An unexpected error occurred.",
                        ErrorCode = "UNHANDLED_ERROR"
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails));
                }
            }
        }
    }
}
