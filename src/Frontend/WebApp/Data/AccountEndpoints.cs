using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Claims do cookie de login: só quem é o usuário. Empresa, holding, perfil e dono o MainLayout relê do banco a cada
    /// sessão (IUserRepository.GetUserAsync), então uma troca de perfil vale na hora e um cookie de outro banco não abre nada.
    /// </summary>
    public static class UserClaims
    {
        public const string UserName = "etl:username";
        public const string Company = "etl:company";
        public const string Holder = "etl:holder";
    }

    /// <summary>
    /// A tela inicial é interativa (circuito) e não grava cookie: ela confere usuário e senha, pede aqui um ticket de uso único,
    /// válido por um minuto, e recarrega o navegador em /account/signin?ticket=..., onde o endpoint grava o cookie.
    /// </summary>
    public class SignInTickets
    {
        private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(1);

        private readonly ConcurrentDictionary<string, (UserModel User, DateTime ExpiresAt)> _tickets = new();

        public string Issue(UserModel user)
        {
            var now = DateTime.UtcNow;
            foreach (var expired in _tickets.Where(x => x.Value.ExpiresAt < now).Select(x => x.Key))
                _tickets.TryRemove(expired, out _);

            var ticket = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace('+', '-').Replace('/', '_').TrimEnd('=');
            _tickets[ticket] = (user, now + Lifetime);
            return ticket;
        }

        public UserModel? Redeem(string? ticket) =>
            ticket is not null && _tickets.TryRemove(ticket, out var entry) && entry.ExpiresAt >= DateTime.UtcNow ? entry.User : null;
    }

    public static class AccountEndpoints
    {
        /// <summary>
        /// /account/signin grava o cookie com o usuário do ticket e abre o painel; /account/signout (POST, com antiforgery)
        /// apaga o cookie e volta para a tela inicial.
        /// </summary>
        public static void MapAccountEndpoints(this WebApplication app)
        {
            app.MapGet("/account/signin", async (string? ticket, SignInTickets tickets, HttpContext context) =>
            {
                if (tickets.Redeem(ticket) is not { } user)
                    return Results.Redirect("/");

                var identity = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(UserClaims.UserName, user.UserName),
                    new Claim(UserClaims.Company, user.Company),
                    new Claim(UserClaims.Holder, user.Holder)
                ], CookieAuthenticationDefaults.AuthenticationScheme);

                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                return Results.Redirect("/home");
            });

            app.MapPost("/account/signout", async (HttpContext context) =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/");
            }).WithMetadata(new RequireAntiforgeryTokenAttribute());
        }
    }
}
