namespace Domain.Totvs.Entities.Writes.Products
{
    /// <summary>
    /// item de uma promoção gravado em totvs.TotvsPromotions: a situação da campanha (A aberta, B bloqueada, E encerrada) e o valor anterior
    /// </summary>
    public class PromotionItemState
    {
        public int productCode { get; set; }
        public decimal? previousValue { get; set; }
        public string? status { get; set; }
    }
}
