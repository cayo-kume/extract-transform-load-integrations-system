namespace WebApp.Components.Cards
{
    /// <summary>
    /// Iniciais da aplicação, no card que não tem imagem (a imagem só vem do banco, enviada pela engrenagem do card).
    /// </summary>
    public static class AppLogos
    {
        public static string Initials(string? title) =>
            string.IsNullOrWhiteSpace(title) ? "?" : new string(title.Where(char.IsUpper).Take(2).DefaultIfEmpty(title[0]).ToArray());
    }
}
