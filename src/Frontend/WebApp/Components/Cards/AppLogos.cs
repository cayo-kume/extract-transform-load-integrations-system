namespace WebApp.Components.Cards
{
    /// <summary>
    /// Logo de cada aplicação, pelo IdParent de auditing.Apps. Aplicação sem logo aqui mostra as iniciais no card.
    /// </summary>
    public static class AppLogos
    {
        private static readonly Dictionary<int, string> Logos = new()
        {
            [50] = "images/logos/aftersale.png"
        };

        public static string? For(int idApp) => Logos.GetValueOrDefault(idApp);

        public static string Initials(string? title) =>
            string.IsNullOrWhiteSpace(title) ? "?" : new string(title.Where(char.IsUpper).Take(2).DefaultIfEmpty(title[0]).ToArray());
    }
}
