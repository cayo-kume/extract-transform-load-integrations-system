namespace Domain.AfterSale.Dtos.Writes.Refunds.Pay.Request
{
    /// <summary>
    /// text fields of POST v3/api/refunds/{id}/pay (multipart/form-data). the payment attachment is sent apart, as a required file.
    /// </summary>
    public class Root
    {
        public string? voucher_redemption_code { get; init; }
        public string? observation { get; init; }
    }
}
