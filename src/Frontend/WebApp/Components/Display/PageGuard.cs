using MudBlazor;

namespace WebApp.Components.Display
{
    /// <summary>
    /// Erro do repositório vira Snackbar e a tela continua de pé com os dados que já tinha (plano, passo 11.2.5), em vez de
    /// derrubar o circuito. Cancelamento (sair da tela, trocar o filtro) passa direto.
    /// </summary>
    public static class PageGuard
    {
        public static async Task<bool> TryAsync(this ISnackbar snackbar, ILogger logger, Func<Task> action, string failureMessage)
        {
            try
            {
                await action();
                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "{Message}", failureMessage);
                snackbar.Add(failureMessage, Severity.Error);
                return false;
            }
        }
    }
}
