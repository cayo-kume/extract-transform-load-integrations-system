namespace Domain.Totvs.Dtos.Writes.Products.Colors
{
    /// <summary>
    /// corpo do POST product/v2/colors (UpsertColorCommand): inclui ou altera uma cor (PRDFL025). Um code que já existe é alterado, e o
    /// nome novo vale em todas as referências que usam a cor. Recebido pela rota do controller e enviado à Totvs como está; nulos não vão
    /// </summary>
    public class UpsertColor
    {
        // até 10 caracteres
        public string? code { get; init; }

        // até 40 caracteres
        public string? name { get; init; }

        // dados da cor por loja (PRDFM074)
        public List<ColorInfoAdditional>? colorInfoAdditionals { get; init; }
    }
}
