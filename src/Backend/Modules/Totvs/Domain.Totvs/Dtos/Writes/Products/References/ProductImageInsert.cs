namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// imagem da referência ou do SKU (InsertProductImageModel)
    /// </summary>
    public class ProductImageInsert
    {
        public int? code { get; init; }
        public int? imageTypeCode { get; init; }
    }
}
