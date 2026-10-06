namespace Domain.LinxCommerce.Entities.Responses
{
    public class GetOrderPayments
    {
        public class Root
        {
            public List<Result> Result { get; set; }
        }

        public class Result
        {
            public string? OrderID { get; set; }
            public string? OrderPaymentMethodID { get; set; }
            public string? PaymentMethodID { get; set; }
            public string? TransactionID { get; set; }
            public string? Status { get; set; }
            public string? PaymentType { get; set; }
            public string? Alias { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? ImagePath { get; set; }
            public string? ProviderDocumentNumbe { get; set; }
        }
    }
}
