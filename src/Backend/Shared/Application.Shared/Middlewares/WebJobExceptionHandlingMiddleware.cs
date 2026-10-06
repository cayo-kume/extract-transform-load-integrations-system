using Application.Shared.Handlers.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Application.Shared.Abstractions.Handlers.Exceptions;

namespace Application.Shared.Middlewares
{
    public class WebJobExceptionHandlingMiddleware
    {
        private readonly IServiceProvider _services;

        public WebJobExceptionHandlingMiddleware(IServiceProvider services) =>
            _services = services;

        public async Task ExecuteAsync(Func<Task> func)
        {
            var context = new DefaultHttpContext { RequestServices = _services };

            try
            {
                await func();
            }
            catch (Exception ex)
            {
                var handlers = context.RequestServices.GetServices<IExceptionHandler>();

                var handler = handlers
                    .OrderBy(h => h.GetType() == typeof(ExceptionHandler) ? 1 : 0)
                    .FirstOrDefault(h => h.CanHandle(ex));

                if (handler != null)
                {
                    await handler.HandleAsync(context, ex);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"No exception handler registered for exception type '{ex.GetType().Name}'.", ex);
                }
            }
        }
    }
}
