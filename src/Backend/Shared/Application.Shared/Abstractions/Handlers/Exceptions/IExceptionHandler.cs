using Microsoft.AspNetCore.Http;

namespace Application.Shared.Abstractions.Handlers.Exceptions
{
    public interface IExceptionHandler
    {
        /// <summary>
        /// Verifica se este manipulador pode lidar com o tipo de exceção
        /// </summary>
        /// <param name="exception"></param>
        bool CanHandle(Exception exception);

        /// <summary>
        /// Lida com a exceção e retorna a resposta de erro
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        Task HandleAsync(HttpContext context, Exception exception);
    }
}
