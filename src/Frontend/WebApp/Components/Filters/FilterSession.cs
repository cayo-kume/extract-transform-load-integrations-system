using WebApp.Models;

namespace WebApp.Components.Filters
{
    /// <summary>
    /// Último período escolhido, enquanto a aba está aberta (scoped = um por circuito). Quando a URL não traz ?period=,
    /// a tela usa este: escolher "7 dias" na Visão geral e ir para Logs de Erros mantém os 7 dias.
    /// </summary>
    public class FilterSession
    {
        public Period Period { get; set; } = Period.Last24Hours;
    }
}
