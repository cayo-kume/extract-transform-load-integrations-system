namespace Domain.Totvs.Dtos.Writes.Products.Values
{
    /// <summary>
    /// corpo do POST product/v2/values/create e /update (InsertProductValueCommand e UpdateProductValueCommand): preço ou custo dos
    /// produtos por empresa. Vale na hora para os PDVs e o e-commerce: só com os códigos e lojas permitidos em configuração e a trava de variação (PR7)
    /// </summary>
    public class ProductValues
    {
        public List<ProductValuesItem>? products { get; init; }
    }
}
