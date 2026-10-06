namespace Domain.Totvs.Dtos.Fiscal
{
    public class Payment
    {
        public string documentNumber { get; set; }
        public string expirationDate { get; set; }
        public string paymentValue { get; set; }
        public string documentTypeCode { get; set; }
        public string documentType { get; set; }
        public string installment { get; set; }
        public string bearerCode { get; set; }
        public string bearerName { get; set; }
        public CardInformation cardInformation { get; set; }
        public DigitalWalletInformation digitalWalletInformation { get; set; }
    }
}
