using Hangfire.Dashboard;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Scheduling
{
    /// <summary>
    /// autenticação do dashboard do Hangfire por Basic Authentication (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §3.2),
    /// independente da autenticação da API. Sem credencial ou com credencial errada, o dashboard devolve 401 e o navegador pede
    /// usuário e senha. A senha vai em cada requisição: fora do localhost, só por HTTPS
    /// </summary>
    public class DashboardBasicAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly byte[] _userName;
        private readonly byte[] _password;

        public DashboardBasicAuthorizationFilter(string userName, string password) =>
            (_userName, _password) = (Encoding.UTF8.GetBytes(userName), Encoding.UTF8.GetBytes(password));

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (IsAuthorized(httpContext.Request.Headers.Authorization.ToString()))
                return true;

            // o Hangfire devolve 401 para quem não está autenticado; o cabeçalho faz o navegador pedir usuário e senha
            httpContext.Response.Headers.WWWAuthenticate = "Basic realm=\"Hangfire\", charset=\"UTF-8\"";

            return false;
        }

        private bool IsAuthorized(string header)
        {
            if (!AuthenticationHeaderValue.TryParse(header, out var value)
                || !"Basic".Equals(value.Scheme, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrEmpty(value.Parameter))
                return false;

            string decoded;

            try
            {
                decoded = Encoding.UTF8.GetString(Convert.FromBase64String(value.Parameter));
            }
            catch (FormatException)
            {
                return false;
            }

            var separator = decoded.IndexOf(':');

            if (separator < 0)
                return false;

            // comparação em tempo constante, das duas partes, para o tempo de resposta não dizer qual delas errou
            var userNameMatches = CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(decoded[..separator]), _userName);
            var passwordMatches = CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(decoded[(separator + 1)..]), _password);

            return userNameMatches & passwordMatches;
        }
    }
}
