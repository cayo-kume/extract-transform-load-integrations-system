namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// ItemSequentialsInDto (ItemSequentialsInDto)
    /// </summary>
    public class B2COrderItemSequential
    {
        // Tamanho máximo: 100 caracteres.
        public string? code { get; init; }
        // Tamanho máximo: 14 números inteiros com 2 casas decimais.
        public decimal? freightValue { get; init; }
        // Tamanho máximo: 14 números inteiros com 2 casas decimais.
        public decimal? discountValue { get; init; }
    }
}
