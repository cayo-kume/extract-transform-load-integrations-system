namespace Domain.Totvs.Dtos.Writes.Products.Grids
{
    /// <summary>
    /// corpo do POST product/v2/grids (InsertGridCommand): inclui uma grade de tamanhos (PRDFM008). Só inclusão: a API não altera nem exclui
    /// grade. Recebido pela rota do controller e enviado à Totvs como está; nulos não vão
    /// </summary>
    public class InsertGrid
    {
        // até 6 dígitos; sem ele, a Totvs numera (a resposta traz o gridCode)
        public int? gridCode { get; init; }

        // até 40 caracteres
        public string? name { get; init; }
        public string? type { get; init; }

        public List<GridSize>? gridSizes { get; init; }
    }
}
