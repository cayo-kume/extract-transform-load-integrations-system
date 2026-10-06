using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderPaymentMethod
    {
        public DateTime lastupdateon { get; set; }
        public Int32? OrderPaymentMethodID { get; set; }
        public Guid? OrderID { get; set; }
        public string? PaymentNumber { get; set; }
        public Int32? PaymentMethodID { get; set; }
        public Guid? TransactionID { get; set; }
        public string? ReconciliationNumber { get; set; }
        public string? Status { get; set; }
        public string? IntegrationID { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountNoInterest { get; set; }
        public decimal? InterestValue { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? RefundAmount { get; set; }
        public Int32? Installments { get; set; }
        public decimal? InstallmentAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public DateTimeOffset? PaymentDate { get; set; }
        public DateTimeOffset? CaptureDate { get; set; }
        public DateTimeOffset? AcquiredDate { get; set; }
        public DateTimeOffset? PaymentCancelledDate { get; set; }

        [SkipProperty]
        public OrderPaymentInfo PaymentInfo { get; set; } = new OrderPaymentInfo();

        public OrderPaymentMethod() { }

        public OrderPaymentMethod(OrderPaymentMethod orderPaymentMethod)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.OrderPaymentMethodID = orderPaymentMethod.OrderPaymentMethodID;
            this.OrderID = orderPaymentMethod.OrderID;
            this.PaymentNumber = orderPaymentMethod.PaymentNumber;
            this.PaymentMethodID = orderPaymentMethod.PaymentMethodID;
            this.TransactionID = orderPaymentMethod.TransactionID;
            this.ReconciliationNumber = orderPaymentMethod.ReconciliationNumber;
            this.Status = orderPaymentMethod.Status;
            this.IntegrationID = orderPaymentMethod.IntegrationID;
            this.Amount = orderPaymentMethod.Amount;
            this.AmountNoInterest = orderPaymentMethod.AmountNoInterest;
            this.InterestValue = orderPaymentMethod.InterestValue;
            this.PaidAmount = orderPaymentMethod.PaidAmount;
            this.RefundAmount = orderPaymentMethod.RefundAmount;
            this.Installments = orderPaymentMethod.Installments;
            this.InstallmentAmount = orderPaymentMethod.InstallmentAmount;
            this.TaxAmount = orderPaymentMethod.TaxAmount;
            this.PaymentDate = orderPaymentMethod.PaymentDate;
            this.CaptureDate = orderPaymentMethod.CaptureDate;
            this.AcquiredDate = orderPaymentMethod.AcquiredDate;
            this.PaymentCancelledDate = orderPaymentMethod.PaymentCancelledDate;

            this.PaymentInfo = new OrderPaymentInfo(orderPaymentMethod.PaymentInfo, orderPaymentMethod.OrderPaymentMethodID);
        }
    }
}
