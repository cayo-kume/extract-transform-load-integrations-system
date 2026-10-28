namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Tipo de documento. (DocumentInputType): vai como número (T3)
    /// </summary>
    public enum B2CPaymentDocumentType
    {
        InvoiceMarketplace = 1,   // fatura/marketplace
        Cash = 2,   // dinheiro
        Billet = 3,   // boleto
        CreditCard = 4,   // cartão de crédito
        DebitCard = 5,   // cartão de débito
        RecebimentoPdv = 13,   // recebimento PDV
        Paypal = 16,   // Paypal
        ReceiptCheck = 18,   // cheque presente
        Credev = 20,   // Credev
        Invoice = 23,   // fatura
        Advance = 24,   // adiantamento
        Voucher = 25,   // voucher
        Pix = 26,   // Pix
        PicPay = 27   // PicPay
    }
}
