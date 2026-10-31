using Microsoft.Extensions.Localization;
using MudBlazor;

namespace WebApp.Components.Display
{
    /// <summary>
    /// Traduções dos textos internos do MudBlazor que aparecem no painel (botões do paginador e do diálogo). Chave sem
    /// tradução aqui cai no inglês do próprio MudBlazor.
    /// </summary>
    public class PortugueseMudLocalizer : MudLocalizer
    {
        private static readonly Dictionary<string, string> Translations = new()
        {
            ["MudTablePager_FirstPage"] = "Primeira página",
            ["MudTablePager_PreviousPage"] = "Página anterior",
            ["MudTablePager_NextPage"] = "Próxima página",
            ["MudTablePager_LastPage"] = "Última página",
            ["MudAlert_Close"] = "Fechar",
            ["MudChip_Close"] = "Remover",
            ["MudDialog_Close"] = "Fechar",
            ["MudInput_Clear"] = "Limpar",
            ["MudInput_Increment"] = "Aumentar",
            ["MudInput_Decrement"] = "Diminuir",
            ["MudSnackbar_Close"] = "Fechar"
        };

        public override LocalizedString this[string key] =>
            Translations.TryGetValue(key, out var value)
                ? new LocalizedString(key, value)
                : new LocalizedString(key, key, resourceNotFound: true);
    }
}
