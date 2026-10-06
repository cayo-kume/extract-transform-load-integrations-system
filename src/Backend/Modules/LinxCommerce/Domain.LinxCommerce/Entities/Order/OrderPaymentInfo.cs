namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderPaymentInfo
    {
        public DateTime lastupdateon { get; set; }
        public string? Identifier { get; set; }
        public string? Alias { get; set; }
        public DateTimeOffset? PaymentDate { get; set; }
        public DateTimeOffset? ExpirationDate { get; set; }
        public Int32? Month { get; set; }
        public Int32? Year { get; set; }
        public string? Holder { get; set; }
        public string? NumberHint { get; set; }
        public string? SecurityCodeHint { get; set; }
        public string? TransactionNumber { get; set; }
        public string? AuthorizationCode { get; set; }
        public string? ReceiptCode { get; set; }
        public string? ReconciliationNumber { get; set; }
        public string? ConfirmationNumber { get; set; }
        public Int32? PaymentType { get; set; }
        public string? Provider { get; set; }
        public string? ProviderDocumentNumber { get; set; }
        public string? PixQRCode { get; set; }
        public string? PixKey { get; set; }
        public Int32? OrderPaymentMethodID { get; set; }

        public OrderPaymentInfo() { }

        public OrderPaymentInfo(OrderPaymentInfo orderPaymentInfo, Int32? orderPaymentMethodID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.Identifier = orderPaymentInfo.Identifier;
            this.Alias = orderPaymentInfo.Alias;
            this.PaymentDate = orderPaymentInfo.PaymentDate;
            this.ExpirationDate = orderPaymentInfo.ExpirationDate;
            this.Month = orderPaymentInfo.Month;
            this.Year = orderPaymentInfo.Year;
            this.Holder = orderPaymentInfo.Holder;
            this.NumberHint = orderPaymentInfo.NumberHint;
            this.SecurityCodeHint = orderPaymentInfo.SecurityCodeHint;
            this.TransactionNumber = orderPaymentInfo.TransactionNumber;
            this.AuthorizationCode = orderPaymentInfo.AuthorizationCode;
            this.ReceiptCode = orderPaymentInfo.ReceiptCode;
            this.ReconciliationNumber = orderPaymentInfo.ReconciliationNumber;
            this.ConfirmationNumber = orderPaymentInfo.ConfirmationNumber;
            this.PaymentType = orderPaymentInfo.PaymentType;
            this.Provider = orderPaymentInfo.Provider;
            this.ProviderDocumentNumber = orderPaymentInfo.ProviderDocumentNumber;
            this.PixQRCode = orderPaymentInfo.PixQRCode;
            this.PixKey = orderPaymentInfo.PixKey;
            this.OrderPaymentMethodID = orderPaymentMethodID;
        }
    }
}
